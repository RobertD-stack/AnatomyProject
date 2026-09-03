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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace getReal3D
{

    /** getRealVRSettingsUI script
     *
     * Part of: getReal3D for Unity 2.1, (C) Mechdyne, 2013
     *
     **/

    public class getRealVRSettingsUI : getReal3D.MonoBehaviourWithRpc
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// Make sure RPC are not automatically registered so that we can disable the
        /// behavior before registration.
        public getRealVRSettingsUI() : base(false)
        {

        }

        /// <summary>
        /// A helper class for handling a text field as float data.
        /// </summary>
        private class TextFieldAsFloatData
        {
            public TextFieldAsFloatData(string l, float fv, string tf = "##0.000",
                                        float minLimit = float.MinValue,
                                        float maxLimit = float.MaxValue)
            {
                label = l;
                floatValue = fv;
                textFormat = tf;
                this.minLimit = minLimit;
                this.maxLimit = maxLimit;

                if (validStyle == null)
                {
                    GUISkin skin = (GUISkin)Instantiate(Resources.Load("gr2DUISkin"));
                    validStyle = new GUIStyle(skin.GetStyle("TextField"));
                    invalidStyle = new GUIStyle(skin.GetStyle("TextField"));

                    invalidStyle.normal.textColor = Color.red;
                    invalidStyle.active.textColor = Color.red;
                    invalidStyle.hover.textColor = Color.red;
                    invalidStyle.focused.textColor = Color.red;
                }

                style = validStyle;
            }

            public bool OnGUIUpdate()
            {
                bool retVal = false;
                GUILayout.BeginHorizontal();
                GUILayout.Label(label);

                textValue = (style == validStyle) ? floatValue.ToString(textFormat) : textValue;
                textValue = GUILayout.TextField(textValue, 6, style);

                double tempVal = floatValue;
                if (UnityEngine.GUI.changed)
                {
                    if (System.Double.TryParse(textValue, out tempVal) && tempVal >= minLimit && tempVal <= maxLimit)
                    {
                        style = validStyle;
                        retVal = (float)tempVal != floatValue;
                        floatValue = (float)tempVal;
                    }
                    else
                    {
                        style = invalidStyle;
                    }
                }
                GUILayout.EndHorizontal();
                return retVal;
            }

            public string label { get; set; }
            public float floatValue { get; set; }
            public string textFormat { get; set; }

            public float minLimit { get; set; }
            public float maxLimit { get; set; }

            string textValue { get; set; }
            GUIStyle style { get; set; }

            static GUIStyle validStyle = null;
            static GUIStyle invalidStyle = null;
        }

        private TextFieldAsFloatData m_worldScale = null;
        private TextFieldAsFloatData m_eyeSeparation = null;
        private TextFieldAsFloatData m_translationSpeed = null;
        private TextFieldAsFloatData m_rotationSpeed = null;

        private TextFieldAsFloatData worldScale
        {
            get
            {
                if (m_worldScale == null)
                {
                    m_worldScale = new TextFieldAsFloatData("World Scale", getReal3D.Scale.userScale, "##0.0000", minLimit: 1e-3f, maxLimit: 1e3f);
                }
                return m_worldScale;
            }
        }

        private TextFieldAsFloatData eyeSeparation
        {
            get
            {
                if (m_eyeSeparation == null)
                {
                    m_eyeSeparation = new TextFieldAsFloatData("Eye Separation (unknown)", getReal3D.Scale.eyeSeparation, minLimit: 1e-3f, maxLimit: 1e3f);
                }
                return m_eyeSeparation;
            }
        }

        private TextFieldAsFloatData translationSpeed
        {
            get
            {
                if (m_translationSpeed == null)
                {
                    m_translationSpeed = new TextFieldAsFloatData("Translation Speed", 1.0f, minLimit: 1e-3f, maxLimit: 1e3f);
                }
                return m_translationSpeed;
            }
        }

        private TextFieldAsFloatData rotationSpeed
        {
            get
            {
                if (m_rotationSpeed == null)
                {
                    m_rotationSpeed = new TextFieldAsFloatData("Rotation Speed", 1.0f, minLimit: 1e-3f, maxLimit: 1e3f);
                }
                return m_rotationSpeed;
            }
        }

        private float m_eventTime = 0.0f;
        private int m_eventFrame = 0;
        private bool m_drawGUI = true;
        private bool m_keepOpen = false;
        private bool m_showAvatars = false;

        private const string None = "None";
        private string m_currentScreenName = None;
        private List<string> m_screenNames = null;
        private float m_defaultDistanceBackup = 0;
        private Transform m_menuSpawnReferenceBackup;
        private Vector2 m_scrollPosition = Vector2.zero;

        /// Prefix used to build the avatar layer
        public string m_avatarLayerPrefix = "Avatar_";

        /// <summary>
        /// Late initialization (give dependencies a chance to run their Awake).
        /// </summary>
        void Start()
        {
            registerRpc();
        }

        void LateUpdate()
        {
            translationSpeed.floatValue = Input.NavOptions.TranslationSpeed;
            rotationSpeed.floatValue = Input.NavOptions.RotationSpeed;
            worldScale.floatValue = getReal3D.Scale.userScale;
            eyeSeparation.floatValue = getReal3D.Scale.eyeSeparation;
            eyeSeparation.label = "Eye Separation (" + getReal3D.Scale.eyeSeparationUnitString + ")";
        }

        [getReal3D.RPC]
        void ChangeWorldScale(float val)
        {
            getReal3D.Scale.userScale = val;
        }

        [getReal3D.RPC]
        void ChangeEyeSeparation(float val)
        {
            getReal3D.Scale.eyeSeparation = val;
        }

        [getReal3D.RPC]
        void ChangeTranslationSpeed(float val)
        {
            Input.NavOptions.TranslationSpeed = val;
        }

        [getReal3D.RPC]
        void ChangeRotationSpeed(float val)
        {
            Input.NavOptions.RotationSpeed = val;
        }

        [getReal3D.RPC]
        void ShowAvatars(bool visible)
        {
            m_showAvatars = visible;
            foreach (getRealUser user in getRealUser.users)
            {
                string avatarLayer = m_avatarLayerPrefix + (user.userId + 1).ToString();
                int avatarLayerId = LayerMask.NameToLayer(avatarLayer);
                if (avatarLayerId >= 0)
                {
                    foreach (GameObject obj in GetObjectsInLayer(user.transform.root.gameObject, avatarLayerId))
                    {
                        obj.SetActive(visible);
                    }
                }
            }
        }

        [getReal3D.RPC]
        void SetMenuScreen(string screenName)
        {
            if (screenName == m_currentScreenName)
            {
                return;
            }
            var genericShowMenu = GetComponent<GenericShowMenu>();
            if (screenName == None)
            {
                genericShowMenu.m_defaultDistance = m_defaultDistanceBackup;
                genericShowMenu.m_menuSpawnReference = m_menuSpawnReferenceBackup;
                genericShowMenu.setDefaultPosition();
            }
            else
            {
                if (m_currentScreenName == None)
                {
                    m_defaultDistanceBackup = genericShowMenu.m_defaultDistance;
                    m_menuSpawnReferenceBackup = genericShowMenu.m_menuSpawnReference;
                }
                var createScreens = GetComponent<CreateScreens>();
                genericShowMenu.m_defaultDistance = 0;
                genericShowMenu.m_menuSpawnReference = createScreens.screens.Where(go =>
                    go.GetComponent<getRealScreenUpdater>().screenName == screenName).
                    FirstOrDefault().transform;
                genericShowMenu.setDefaultPosition();
            }
            m_currentScreenName = screenName;
        }

        private static List<GameObject> GetObjectsInLayer(GameObject root, int layer)
        {
            var ret = new List<GameObject>();
            if (root.layer == layer)
            {
                ret.Add(root);
            }
            foreach (Transform t in root.transform)
            {
                ret.AddRange(GetObjectsInLayer(t.gameObject, layer));
            }
            return ret;
        }

        void OnGUI()
        {
            var createScreens = GetComponent<CreateScreens>();
            if (m_screenNames == null && createScreens != null && createScreens.isActiveAndEnabled)
            {
                m_screenNames = createScreens.screens.Select(go =>
                    go.GetComponent<getRealScreenUpdater>().screenName).ToList();
            }
            if (Event.current.isKey || Event.current.isMouse)
            {
                m_eventTime = Time.time;
            }
            if (Event.current.type == EventType.Layout && Time.frameCount != m_eventFrame)
            {
                m_eventFrame = Time.frameCount;
                m_drawGUI = Time.time < m_eventTime + 5.0f || m_keepOpen;
            }
            if (!m_drawGUI) return;
            if (!getReal3D.GUI.BeginGUI()) return;

            GUILayout.BeginArea(new Rect(0, 0, 220, getReal3D.GUI.height));
            GUILayout.BeginVertical("VR Settings", UnityEngine.GUI.skin.window);

            if (worldScale.OnGUIUpdate())
            {
                CallRpc("ChangeWorldScale", worldScale.floatValue);
            }

            if (eyeSeparation.OnGUIUpdate())
            {
                CallRpc("ChangeEyeSeparation", eyeSeparation.floatValue);
            }

            if (translationSpeed.OnGUIUpdate())
            {
                CallRpc("ChangeTranslationSpeed", translationSpeed.floatValue);
            }

            if (rotationSpeed.OnGUIUpdate())
            {
                CallRpc("ChangeRotationSpeed", rotationSpeed.floatValue);
            }

            bool showAvatars = GUILayout.Toggle(m_showAvatars, "Avatars");
            if (showAvatars != m_showAvatars)
            {
                CallRpc("ShowAvatars", showAvatars);
            }

            if (m_screenNames != null)
            {
                GUILayout.Label("Snap menu to screen:");
                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
                if (GUILayout.Toggle(m_currentScreenName == None, None) && m_currentScreenName != None)
                {
                    CallRpc("SetMenuScreen", None);
                }
                foreach (var screenName in m_screenNames)
                {
                    if (GUILayout.Toggle(m_currentScreenName == screenName, screenName) &&
                        m_currentScreenName != screenName)
                    {
                        CallRpc("SetMenuScreen", screenName);
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("FPS: " + (1.0f / Time.smoothDeltaTime).ToString("##0.00"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("SPF: " + Time.smoothDeltaTime.ToString("##0.000"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Frame: " + Time.frameCount);
            if (getReal3D.Plugin.isDistrib())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Cluster frame: " + getReal3D.Cluster.frameCount);
            }
            GUILayout.EndHorizontal();

            m_keepOpen = GUILayout.Toggle(m_keepOpen, "Keep open");

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
            getReal3D.GUI.EndGUI();
        }
    }
}
