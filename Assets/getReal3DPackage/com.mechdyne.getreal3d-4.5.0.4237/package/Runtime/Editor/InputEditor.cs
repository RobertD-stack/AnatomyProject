/*************************************************************************
 *
 * Copyright 2026, Mechdyne Corporation
 * ALL RIGHTS RESERVED
 *
 * UNPUBLISHED -- Rights reserved under the copyright laws of the United
 * States. Use of a copyright notice is precautionary only and does not
 * imply publication or disclosure.
 *
 * THE CONTENT OF THIS WORK CONTAINS CONFIDENTIAL AND PROPRIETARY
 * INFORMATION OF MECHDYNE CORPORATION. ANY DUPLICATION, MODIFICATION,
 * DISTRIBUTION, OR DISCLOSURE IN ANY FORM, IN WHOLE, OR IN PART, IS
 * STRICTLY PROHIBITED WITHOUT THE PRIOR EXPRESS WRITTEN PERMISSION OF
 * MECHDYNE CORPORATION.
 *
 * Version 4.5.0.4237
 *
 ************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{
    internal class SelectionWindow : SearchableEditorWindow
    {
        private string m_searchString = "";
        private string[] m_items;
        private Vector2 m_scrollPosition = Vector2.zero;
        private CultureInfo m_culture = CultureInfo.CurrentCulture;

        internal delegate void OnSelected(string selection);
        internal OnSelected onSelected = null;
        internal string[] items { get { return m_items; } set { m_items = value; } }

        public SelectionWindow()
        {

        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            System.Object[] parameters = new System.Object[2] { m_searchString, new GUILayoutOption[0] };
            m_searchString = (string)DoInvoke(typeof(EditorGUILayout), "ToolbarSearchField", parameters);
            GUILayout.EndHorizontal();

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            foreach (string item in m_items)
            {
                if (m_culture.CompareInfo.IndexOf(item, m_searchString, CompareOptions.IgnoreCase) >= 0)
                {
                    if (GUILayout.Button(item, EditorStyles.whiteLargeLabel))
                    {
                        if (onSelected != null)
                        {
                            onSelected(item);
                        }
                        Close();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }

        private static System.Object DoInvoke(Type type, string methodName, System.Object[] parameters)
        {
            Type[] types = new Type[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }

            MethodInfo method = type.GetMethod(methodName, (BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public), null, types, null);
            return DoInvoke2(type, method, parameters);
        }

        private static System.Object DoInvoke2(Type type, MethodInfo method, System.Object[] parameters)
        {
            if (method.IsStatic)
            {
                return method.Invoke(null, parameters);
            }

            System.Object obj = type.InvokeMember(null,
            BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.CreateInstance, null, null, new System.Object[0]);

            return method.Invoke(obj, parameters);
        }
    }

    internal class InputEditor : EditorWindow
    {
        private const string unityBindingWarning = "Unity binding above only applies when" +
            " running from the editor, or without getReal3D.";
        static private string[] s_trackdIndexes = getTrackdIndexes();
        static private string[] s_keys = getKeys();
        static private string[] s_buttons = Enum.GetNames(typeof(ButtonCode));
        static private string[] s_axis = Enum.GetNames(typeof(AxisCode));
        static private string[] s_buttonsDevices = getButtonsDevices();
        static private string[] s_buttonTypes = getButtonTypes();
        static private string[] s_axisDevices = getAxisDevices();
        static private string[] s_mouseAxisTypes = getMouseAxisTypes();
        static private string[] s_joystickAxisTypes = getJoystickAxisTypes();

        static private int m_pixelPerIdentLevel = 17;
        static private int m_buttonsMaxWidth = 200;
        static private int m_axisMaxWidth = 200;

        [SerializeField]
        private Vector2 m_scrollPosition = Vector2.zero;

        [SerializeField]
        private bool m_changed = false;

        [SerializeField]
        private System.Diagnostics.Stopwatch m_lastTimeChanged = new System.Diagnostics.Stopwatch();

        [SerializeField]
        private bool m_showTrackers = true;

        [SerializeField]
        private bool m_showAxis = true;

        [SerializeField]
        private bool m_showButtons = true;

        [SerializeField]
        private bool m_showSticks = true;

        [Serializable]
        class Mapping : InputMapping
        {
            public Mapping(MappingType type, XmlElement elem)
            {
                this.type = type;
                fromXml(elem);
            }
            public Mapping()
            {
            }
            public bool show = false;
        }

        [Serializable]
        class InputMappings : InputMappingsBase<Mapping>
        {

        }

        [SerializeField]
        private InputMappings m_inputMappings = new InputMappings();

        InputEditor()
        {
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("getReal3D/Input Editor", false, 50)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            InputEditor window = (InputEditor)EditorWindow.GetWindow(typeof(InputEditor));
            window.reload(false);
            window.RemoveNotification();
            window.Show();
        }

        void OnInspectorUpdate()
        {
            checkIfSaveNeeded();
        }

        void OnDestroy()
        {
            checkIfSaveNeeded(true);
        }

        void OnGUI()
        {
            float backup = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to default"))
            {
                resetToDefault();
            }
            if (GUILayout.Button("Copy to clipboard"))
            {
                copy();
            }
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Those settings are persistent. They won't be " +
                    "restored when the game is stopped.", MessageType.Warning);
                EditorGUILayout.HelpBox("Unity forwarded bindings won't show unless the play" +
                    " window has focus.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Run the application in order to see realtime values.",
                    MessageType.Info);
            }

            EditorGUI.BeginChangeCheck();

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            m_showTrackers = makeUi("Trackers", m_inputMappings.sensors, MappingType.Tracker, m_showTrackers);
            m_showButtons = makeUi("Buttons", m_inputMappings.buttons, MappingType.Button, m_showButtons);
            m_showAxis = makeUi("Axis", m_inputMappings.valuators, MappingType.Axis, m_showAxis);
            m_showSticks = makeUi("Sticks", m_inputMappings.sticks, MappingType.Stick, m_showSticks);

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                scheduleForSave();
            }

            if (getReal3D.Plugin.initialized())
            {
                Repaint();
            }

            EditorGUIUtility.labelWidth = backup;

            var count = m_inputMappings.sensorMap.Count;
        }

        bool makeUi(string name, ObservableCollection<Mapping> mapping, MappingType mappingType, bool show)
        {

            GUILayout.BeginHorizontal();
            show = EditorGUILayout.Foldout(show, name);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                mapping.Add(new Mapping { name = "Unknown", type = mappingType, show = true });
                show = true;
            }
            GUILayout.EndHorizontal();

            if (!show)
            {
                return show;
            }

            EditorGUI.indentLevel++;

            int? toDelete = null;

            for (int i = 0; i < mapping.Count; ++i)
            {
                int mappingIndex = i;
                GUILayout.BeginHorizontal();
                mapping[i].show = EditorGUILayout.Foldout(mapping[i].show, mapping[i].name);

                try
                {
                    if (mappingType == MappingType.Axis && getReal3D.Plugin.initialized())
                    {
                        GUILayout.Label("Value: " + getReal3D.Input.GetAxis(mapping[i].name).ToString());
                    }
                    else if (mappingType == MappingType.Button && getReal3D.Plugin.initialized())
                    {
                        GUILayout.Label("Value: " + getReal3D.Input.GetButton(mapping[i].name).ToString());
                    }
                    else if (mappingType == MappingType.Tracker && getReal3D.Plugin.initialized())
                    {
                        GUILayout.Label("Value: " + getReal3D.Input.GetSensor(mapping[i].name).position.ToString());
                    }
                    else if (mappingType == MappingType.Stick && getReal3D.Plugin.initialized())
                    {
                        GUILayout.Label("Value: " + getReal3D.Input.GetStick(mapping[i].name));
                    }
                }
                catch (Exception)
                {
                    GUILayout.Label("Value: Error");
                }

                bool delete = GUILayout.Button("Delete", GUILayout.Width(120));
                if (delete)
                {
                    toDelete = i;
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                if (mapping[i].type == MappingType.Tracker && mapping[i].show)
                {
                    mapping[i].name = EditorGUILayout.TextField("Name:", mapping[i].name);
                    mapping[i].index = trackdIndexPopup(mapping[i].index, "trackd Sensor:");
                }
                else if (mapping[i].type == MappingType.Button && mapping[i].show)
                {
                    mapping[i].name = EditorGUILayout.TextField("Name:", mapping[i].name);
                    mapping[i].index = trackdIndexPopup(mapping[i].index, "trackd Button:");
                    createKeyField("Key:", mapping[i].positiveKey, delegate (VirtualKeyCode key)
                    {
                        mapping[mappingIndex].positiveKey = key;
                        scheduleForSave();
                        Repaint();
                    });
                    createButtonField("Button:", mapping[i].positiveButton, delegate (ButtonCode button)
                    {
                        mapping[mappingIndex].positiveButton = button;
                        scheduleForSave();
                        Repaint();
                    });
                    mapping[i].invert = EditorGUILayout.Toggle("Invert", mapping[i].invert);
#if ENABLE_LEGACY_INPUT_MANAGER
                    mapping[i].unityName = EditorGUILayout.TextField("Standalone Unity binding:", mapping[i].unityName);
                    if (!string.IsNullOrEmpty(mapping[i].unityName))
                    {
                        EditorGUILayout.HelpBox(unityBindingWarning, MessageType.Warning);
                    }
#endif
                }
                else if (mapping[i].type == MappingType.Axis && mapping[i].show)
                {
                    mapping[i].name = EditorGUILayout.TextField("Name:", mapping[i].name);
                    mapping[i].index = trackdIndexPopup(mapping[i].index, "trackd Valuator:");
                    mapping[i].deadZone = Mathf.Clamp(EditorGUILayout.FloatField("Deadzone:", mapping[i].deadZone), 0.0f, 1.0f);

                    createKeyField("Key:", mapping[i].positiveKey, delegate (VirtualKeyCode key)
                    {
                        mapping[mappingIndex].positiveKey = key;
                        scheduleForSave();
                        Repaint();
                    });

                    createKeyField("Negative key:", mapping[i].negativeKey, delegate (VirtualKeyCode key)
                    {
                        mapping[mappingIndex].negativeKey = key;
                        scheduleForSave();
                        Repaint();
                    });

                    createButtonField("Button:", mapping[i].positiveButton, delegate (ButtonCode button)
                    {
                        mapping[mappingIndex].positiveButton = button;
                        scheduleForSave();
                        Repaint();
                    });

                    createButtonField("Negative button:", mapping[i].negativeButton, delegate (ButtonCode button)
                    {
                        mapping[mappingIndex].negativeButton = button;
                        scheduleForSave();
                        Repaint();
                    });

                    createAxisField("Axis:", mapping[i].positiveAxis, delegate (AxisCode axis)
                    {
                        mapping[mappingIndex].positiveAxis = axis;
                        scheduleForSave();
                        Repaint();
                    });

                    createAxisField("Negative axis:", mapping[i].negativeAxis, delegate (AxisCode axis)
                    {
                        mapping[mappingIndex].negativeAxis = axis;
                        scheduleForSave();
                        Repaint();
                    });

                    mapping[i].invert = EditorGUILayout.Toggle("Invert", mapping[i].invert);
#if ENABLE_LEGACY_INPUT_MANAGER
                    mapping[i].unityName = EditorGUILayout.TextField("Standalone Unity binding:", mapping[i].unityName);
                    if (!string.IsNullOrEmpty(mapping[i].unityName))
                    {
                        EditorGUILayout.HelpBox(unityBindingWarning, MessageType.Warning);
                    }
#endif
                }
                else if (mapping[i].type == MappingType.Stick && mapping[i].show)
                {
                    mapping[i].name = EditorGUILayout.TextField("Name:", mapping[i].name);

                    createAxisMappingField("Horizontal Axis:", mapping[i].horizontalAxis, delegate (string axis)
                    {
                        mapping[mappingIndex].horizontalAxis = axis;
                        scheduleForSave();
                        Repaint();
                    });

                    createAxisMappingField("Vertical Axis:", mapping[i].verticalAxis, delegate (string axis)
                    {
                        mapping[mappingIndex].verticalAxis = axis;
                        scheduleForSave();
                        Repaint();
                    });
                }

                EditorGUI.indentLevel--;
            }

            if (toDelete.HasValue)
            {
                mapping.RemoveAt(toDelete.Value);
            }

            EditorGUI.indentLevel--;
            return show;
        }

        private delegate void OnSelected<T>(T key);

        private void createKeyField(string label, VirtualKeyCode key, OnSelected<VirtualKeyCode> onKeySelected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * m_pixelPerIdentLevel);
            GUILayout.Label(label);
            if (GUILayout.Button(keyToString(key), EditorStyles.popup, GUILayout.MaxWidth(m_buttonsMaxWidth)))
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                popup.items = s_keys;
                OnSelected<VirtualKeyCode> onKey = onKeySelected;
                popup.onSelected = delegate (string item)
                {
                    onKey(keyFromName(item));
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            GUILayout.EndHorizontal();
        }

        private void createAxisMappingField(string label, string axis, OnSelected<string> onAxisSelected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * m_pixelPerIdentLevel);
            GUILayout.Label(label);
            if (GUILayout.Button(axis, EditorStyles.popup, GUILayout.MaxWidth(m_axisMaxWidth)))
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                popup.items = m_inputMappings.valuators.Select(v => v.name).ToArray();
                OnSelected<string> onAxis = onAxisSelected;
                popup.onSelected = delegate (string item)
                {
                    onAxis(item);
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            GUILayout.EndHorizontal();
        }

        private void createButtonField(string label, ButtonCode button, OnSelected<ButtonCode> onKeySelected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * m_pixelPerIdentLevel);
            GUILayout.Label(label);
            string deviceName = getDeviceName(button);
            string buttonName = getElementName(button);
            if (GUILayout.Button(deviceName, EditorStyles.popup, GUILayout.MaxWidth(m_buttonsMaxWidth)))
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                popup.items = s_buttonsDevices;
                OnSelected<ButtonCode> onKey = onKeySelected;
                popup.onSelected = delegate (string item)
                {
                    onKey(getButtonCode(item, buttonName));
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            if (GUILayout.Button(buttonName, EditorStyles.popup, GUILayout.MaxWidth(m_buttonsMaxWidth)) && button != ButtonCode.None)
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                popup.items = s_buttonTypes;
                OnSelected<ButtonCode> onKey = onKeySelected;
                popup.onSelected = delegate (string item)
                {
                    onKey(getButtonCode(deviceName, item));
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            GUILayout.EndHorizontal();
        }

        private void createAxisField(string label, AxisCode axis, OnSelected<AxisCode> onAxisSelected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * m_pixelPerIdentLevel);
            GUILayout.Label(label);
            string deviceName = getDeviceName(axis);
            string axisName = getElementName(axis);
            if (GUILayout.Button(deviceName, EditorStyles.popup, GUILayout.MaxWidth(m_buttonsMaxWidth)))
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                popup.items = s_axisDevices;
                OnSelected<AxisCode> onAxis = onAxisSelected;
                popup.onSelected = delegate (string item)
                {
                    onAxis(getAxisCode(item, axisName));
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            if (GUILayout.Button(axisName, EditorStyles.popup, GUILayout.MaxWidth(m_buttonsMaxWidth)) && axis != AxisCode.None)
            {
                SelectionWindow popup = (SelectionWindow)EditorWindow.CreateInstance<SelectionWindow>();
                if (deviceName == "Mouse")
                {
                    popup.items = s_mouseAxisTypes;
                }
                else
                {
                    popup.items = s_joystickAxisTypes;
                }
                OnSelected<AxisCode> onAxis = onAxisSelected;
                popup.onSelected = delegate (string item)
                {
                    onAxis(getAxisCode(deviceName, item));
                };
                Rect rect = new Rect(position.x, position.y, 0, 0);
                popup.ShowAsDropDown(rect, new Vector2(position.width, position.height));
            }
            GUILayout.EndHorizontal();
        }

        private int trackdIndexPopup(int currentValue, string name)
        {
            int currentIndex = currentValue == -1 ? 0 : currentValue + 1;
            int index = EditorGUILayout.Popup(name, currentIndex, s_trackdIndexes);
            return index == 0 ? -1 : index - 1;
        }

        private void resetToDefault()
        {
            loadFromXml(Settings.defaultMappingConfigContent);
            ShowNotification(new GUIContent("Configuration reset to default."));
            scheduleForSave();
        }

        private void save()
        {
            Settings.get().mappingConfigContent = m_inputMappings.genXml();
        }

        private void copy()
        {
            string xml = m_inputMappings.genXml();
            EditorGUIUtility.systemCopyBuffer = xml;
            ShowNotification(new GUIContent("Configuration copied to clipboard."));
        }

        private void reload(bool showNotification = true)
        {

            loadFromXml(Settings.get().mappingConfigContent);
            if (showNotification)
            {
                ShowNotification(new GUIContent("Configuration loaded."));
            }
        }

        private void loadFromXml(string xml)
        {
            try
            {
                m_inputMappings = new InputMappings();
                m_inputMappings.setupMappingFromXML(xml);
                m_showTrackers = true;
                m_showAxis = true;
                m_showButtons = true;
            }
            catch (XmlException e)
            {
                Plugin.error("XML exception caught while reading inputs mappings. "
                    + e.Message + "\n" + xml);
            }
            catch (Exception e)
            {
                Plugin.error("Exception caught while reading inputs mappings. "
                    + e.Message + "\n" + xml);
            }
        }

        private void scheduleForSave()
        {
            m_changed = true;
            m_lastTimeChanged.Reset();
            m_lastTimeChanged.Start();
        }

        private void checkIfSaveNeeded(bool force = false)
        {
            if (m_changed && (force || m_lastTimeChanged.ElapsedMilliseconds > 1000))
            {
                save();
                if (getReal3D.Plugin.initialized())
                {
                    getReal3D.Input.reloadMappings();
                }
                m_changed = false;
            }
        }

        private string keyToString(VirtualKeyCode key)
        {
            string name = key.ToString();
            if (name.StartsWith("VK_"))
            {
                return name.Substring(3);
            }
            else
            {
                return name;
            }

        }

        private VirtualKeyCode keyFromName(string name)
        {
            try
            {
                return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), "VK_" + name, true);
            }
            catch (Exception) { }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), name, true);
        }

        private string getDeviceName(ButtonCode button)
        {
            return getDeviceName(button.ToString());
        }

        private string getDeviceName(AxisCode axis)
        {
            return getDeviceName(axis.ToString());
        }

        private string getDeviceName(string name)
        {
            if (name.StartsWith("JOYSTICK_"))
            {
                return "Any joystick";
            }
            else if (name.StartsWith("JOYSTICK1_"))
            {
                return "Joystick 1";
            }
            else if (name.StartsWith("JOYSTICK2_"))
            {
                return "Joystick 2";
            }
            else if (name.StartsWith("JOYSTICK3_"))
            {
                return "Joystick 3";
            }
            else if (name.StartsWith("JOYSTICK4_"))
            {
                return "Joystick 4";
            }
            else if (name.StartsWith("MOUSE_"))
            {
                return "Mouse";
            }
            else if (name == "None" || name == "Count")
            {
                return "None";
            }
            else
            {
                throw new System.Exception("Unexpected name in getDeviceName: '" + name + "'");
            }
        }

        private string getElementName(AxisCode axis)
        {
            if (axis == AxisCode.None)
            {
                return "None";
            }
            string name = axis.ToString();
            try
            {
                return name.Split(new char[] { '_' }, 2)[1];
            }
            catch (Exception)
            {
                return name;
            }
        }

        private string getElementName(ButtonCode button)
        {
            if (button == ButtonCode.None)
            {
                return "None";
            }
            return s_buttonTypes[((int)button) % s_buttonTypes.Length];
        }

        private ButtonCode getButtonCode(string deviceName, string button)
        {
            int joystickIndex = Array.IndexOf(s_buttonsDevices, deviceName);
            int buttonOffset = Array.IndexOf(s_buttonTypes, button);
            if (buttonOffset == -1)
            {
                buttonOffset = 0;
            }
            if (joystickIndex == 0)
            {
                return (ButtonCode)buttonOffset;
            }
            else if (joystickIndex == 1)
            {
                return (ButtonCode)(ButtonCode.JOYSTICK1_DPAD_UP + buttonOffset);
            }
            else if (joystickIndex == 2)
            {
                return (ButtonCode)(ButtonCode.JOYSTICK2_DPAD_UP + buttonOffset);
            }
            else if (joystickIndex == 3)
            {
                return (ButtonCode)(ButtonCode.JOYSTICK3_DPAD_UP + buttonOffset);
            }
            else if (joystickIndex == 4)
            {
                return (ButtonCode)(ButtonCode.JOYSTICK4_DPAD_UP + buttonOffset);
            }
            else
            {
                return ButtonCode.None;
            }
        }

        private AxisCode getAxisCode(string deviceName, string axis)
        {
            int joystickIndex = Array.IndexOf(s_axisDevices, deviceName);
            int axisOffset = Array.IndexOf(s_joystickAxisTypes, axis);
            int mouseAxisOffset = Array.IndexOf(s_mouseAxisTypes, axis);

            if (axisOffset == -1) { axisOffset = 0; }

            if (joystickIndex == 0)
            {
                return (AxisCode)axisOffset;
            }
            else if (joystickIndex == 1)
            {
                return (AxisCode)(AxisCode.JOYSTICK1_THUMB_LX + axisOffset);
            }
            else if (joystickIndex == 2)
            {
                return (AxisCode)(AxisCode.JOYSTICK2_THUMB_LX + axisOffset);
            }
            else if (joystickIndex == 3)
            {
                return (AxisCode)(AxisCode.JOYSTICK3_THUMB_LX + axisOffset);
            }
            else if (joystickIndex == 4)
            {
                return (AxisCode)(AxisCode.JOYSTICK4_THUMB_LX + axisOffset);
            }
            else if (joystickIndex == 5)
            {
                Plugin.debug(deviceName + " " + axis);
                if (mouseAxisOffset == -1) { return AxisCode.MOUSE_X; }
                else { return (AxisCode)(AxisCode.MOUSE_X + mouseAxisOffset); }
            }
            else
            {
                return AxisCode.None;
            }
        }

        private static string[] getKeys()
        {
            var keys = Enum.GetNames(typeof(VirtualKeyCode));
            for (var i = 0; i < keys.Length; i++)
            {
                if (keys[i].StartsWith("VK_"))
                {
                    keys[i] = keys[i].Substring(3);
                }
            }
            return keys;
        }

        private static string[] getTrackdIndexes()
        {
            var res = new string[33];
            res[0] = "None";
            for (int i = 1; i <= 32; ++i)
            {
                res[i] = i.ToString();
            }
            return res;
        }

        static private string[] getButtonsDevices()
        {
            return new string[] {
                    "Any joystick",
                    "Joystick 1",
                    "Joystick 2",
                    "Joystick 3",
                    "Joystick 4",
                    "None"
                };
        }

        static private string[] getButtonTypes()
        {
            return new string[] {
                    "DPAD_UP",
                    "DPAD_DOWN",
                    "DPAD_LEFT",
                    "DPAD_RIGHT",
                    "START",
                    "BACK",
                    "LEFT_THUMB",
                    "RIGHT_THUMB",
                    "LEFT_SHOULDER",
                    "RIGHT_SHOULDER",
                    "Button 11",
                    "Button 12",
                    "A",
                    "B",
                    "X",
                    "Y",
                    "Button 17",
                    "Button 18",
                    "Button 19",
                    "Button 20",
                    "Button 21",
                    "Button 22",
                    "Button 23",
                    "Button 24",
                    "Button 25",
                    "Button 26",
                    "Button 27",
                    "Button 28",
                    "Button 29",
                    "Button 30",
                    "Button 31",
                    "Button 32",
                };
        }

        private static string[] getAxisDevices()
        {
            return new string[] {
                    "Any joystick",
                    "Joystick 1",
                    "Joystick 2",
                    "Joystick 3",
                    "Joystick 4",
                    "Mouse",
                    "None"
                };
        }

        private static string[] getMouseAxisTypes()
        {
            return new string[] {
                    "MOUSE_X",
                    "MOUSE_Y",
                    "MOUSE_WHEEL"
                };
        }

        private static string[] getJoystickAxisTypes()
        {
            return new string[] {
                    "THUMB_LX",
                    "THUMB_LY",
                    "THUMB_RX",
                    "THUMB_RY",
                    "TRIGGER_L",
                    "TRIGGER_R"
                };
        }

    }
}

#endif
