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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using UnityEngine;

namespace getReal3D
{

    /// <summary>
    /// getReal3D game build time settings.
    /// </summary>
    internal class Settings : ScriptableObject
    {

        const string AssetName = "getReal3D_Settings.asset";
        const string AssetPath = "Assets/Resources/";

        /// <summary>
        /// Delegate type for settings changed event
        /// </summary>
        internal delegate void SettingsChanged();

        /// <summary>
        /// Event called when some settings change
        /// </summary>
        internal event SettingsChanged OnSettingsChanged;

        /// <summary>
        /// If getReal3D.Plugin.InitVR is called automatically at game startup.
        /// </summary>
        [SerializeField]
        private bool m_autoInitVR;

        /// <summary>
        /// If getReal3D.Plugin.InitVR is called automatically at game startup.
        /// </summary>
        public bool autoInitVR
        {
            get { return m_autoInitVR; }
            set
            {
                if (m_autoInitVR != value)
                {
                    m_autoInitVR = value;
                    CallSettingsChanged();
                }
            }
        }

        /// <summary>
        /// The log level of getReal3D for console redirection.
        /// </summary>
        [SerializeField]
        private LogLevel m_logLevel;

        /// <summary>
        /// The log level of getReal3D for console redirection.
        /// </summary>
        public LogLevel logLevel
        {
            get { return m_logLevel; }
            set
            {
                if (m_logLevel != value)
                {
                    m_logLevel = value;
                    CallSettingsChanged();
                }
            }
        }

        /// <summary>
        /// The log level of getReal3D RPCs.
        /// </summary>
        [SerializeField]
        private LogLevel m_rpcLogLevel;

        /// <summary>
        /// The log level of getReal3D RPCs.
        /// </summary>
        public LogLevel rpcLogLevel
        {
            get { return m_rpcLogLevel; }
            set
            {
                if (m_rpcLogLevel != value)
                {
                    m_rpcLogLevel = value;
                    CallSettingsChanged();
                }
            }
        }

        /// <summary>
        /// Ignored errors.
        /// </summary>
        [SerializeField]
        private List<string> m_ignoredErrors = new List<string>();

        /// <summary>
        /// Return all available configuration for editor
        /// </summary>
        [SerializeField]
        private string m_config;

        public string config
        {
            get { return m_config; }
            set
            {
                if (m_config != value)
                {
                    m_config = value;
                    CallSettingsChanged();
                }
            }
        }

        /// <summary>
        /// If getReal3D creates an Input System device
        /// </summary>
        [SerializeField]
        private bool m_inputSystemController;

        /// <summary>
        /// If getReal3D creates an Input System device
        /// </summary>
        public bool inputSystemController
        {
            get { return m_inputSystemController; }
            set
            {
                if (m_inputSystemController != value)
                {
                    m_inputSystemController = value;
                    CallSettingsChanged();
                }
            }
        }

        /// <summary>
        /// Reset the setting to the default values.
        /// </summary>
        public void Reset()
        {
            autoInitVR = true;
            logLevel = LogLevel.Warning;
            rpcLogLevel = LogLevel.Discarded;
            m_ignoredErrors = new List<string>();
            inputSystemController = true;
            config = DefaultConfig;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Settings()
        {
            m_autoInitVR = true;
            m_logLevel = LogLevel.Warning;
            m_rpcLogLevel = LogLevel.Discarded;
            m_ignoredErrors = new List<string>();
            m_inputSystemController = true;
            m_config = DefaultConfig;
        }

        private static Settings s_settings;

        internal static Settings get()
        {
            if (!s_settings)
            {
                s_settings = Resources.Load("getReal3D_Settings") as Settings;
            }
            if (!s_settings)
            {
                s_settings = CreateInstance<Settings>();
                s_settings.Save();
            }
            return s_settings;
        }

        void Save()
        {
#if UNITY_EDITOR
            try
            {
                Directory.CreateDirectory(AssetPath);
                if (!UnityEditor.AssetDatabase.Contains(this))
                {
                    UnityEditor.AssetDatabase.CreateAsset(this, AssetPath + AssetName);
                }
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
#endif
        }

        internal static string DefaultConfig = "editor_config";

        internal static string[] ConfigurationDirectories
        {
            get
            {
                return new string[] {
                    Path.GetFullPath("Packages/com.mechdyne.getreal3d/Resources"),
                    Application.dataPath + "/Resources"
                };
            }
        }

        internal static List<KeyValuePair<string, string>> AvailableConfigs
        {
            get
            {
                var res = new List<KeyValuePair<string, string>>();
                foreach (var directory in ConfigurationDirectories)
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    if (directoryInfo.Exists)
                    {
                        var filesInfo1 = directoryInfo.GetFiles("*_config.xml", SearchOption.TopDirectoryOnly);
                        var filesInfo2 = directoryInfo.GetFiles("*.gr3d", SearchOption.TopDirectoryOnly);
                        var filesInfo = filesInfo1.Union(filesInfo2);
                        foreach (var fileInfo in filesInfo)
                        {
                            res.Add(new KeyValuePair<string, string>(
                                Path.GetFileNameWithoutExtension(fileInfo.Name),
                                getConfigTitle(fileInfo.FullName)));
                        }
                    }
                }
                return res;
            }
        }

        private static string getConfigTitle(string fullName)
        {
            try
            {
                var doc = new XPathDocument(fullName);
                var nav = doc.CreateNavigator();
                return nav.SelectSingleNode("/boost_serialization/display/px/DisplaySystem/name").
                    ToString();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return fullName;
            }
        }

        internal static string defaultMappingConfigContent
        {
            get
            {
                return @"
                    <unity>
                      <map_button name=""WandButton"" index=""1"" unity_button="""" key=""VK_LCONTROL"" button=""JOYSTICK_A"" />
                      <map_button name=""ChangeWand"" index=""2"" unity_button="""" key=""VK_LSHIFT"" button=""JOYSTICK_X"" />
                      <map_button name=""Reset"" index=""3"" unity_button="""" key=""VK_LMENU"" button=""JOYSTICK_B16"" />
                      <map_button name=""Jump"" index=""4"" unity_button="""" key=""VK_SPACE"" button=""JOYSTICK_B"" />
                      <map_button name=""WandLook"" index=""5"" />
                      <map_button name=""NavSpeed"" index=""6"" />
                      <map_button name=""WandDrive"" index=""7"" />
                      <map_valuator name=""Yaw"" index=""1"" positive_key=""VK_E"" negative_key=""VK_Q"" dead_zone=""0.1"" axis=""JOYSTICK_THUMB_RX"" />
                      <map_valuator name=""Forward"" index=""2"" unity_axis="""" positive_key=""VK_W"" negative_key=""VK_S"" dead_zone=""0.1"" axis=""JOYSTICK_THUMB_LY"" />
                      <map_valuator name=""Strafe"" index=""3"" unity_axis="""" positive_key=""VK_D"" negative_key=""VK_A"" dead_zone=""0.1"" axis=""JOYSTICK_THUMB_LX"" />
                      <map_valuator name=""Pitch"" dead_zone=""0.1"" />
                      <map_valuator name=""UpDown"" index=""6"" positive_button=""JOYSTICK_DPAD_UP"" negative_button=""JOYSTICK_B2"" />
                      <map_tracker name=""Head"" index=""1"" />
                      <map_tracker name=""Wand"" index=""2"" />
                    </unity>";
            }
        }

        internal string mappingConfigContent
        {
            get
            {
                UnityEngine.Object res = Resources.Load("mappings");
                if (!res)
                {
                    return defaultMappingConfigContent;
                }
                TextAsset data = res as TextAsset;
                if (data == null)
                {
                    throw new Exception("Mapping asset is not a text asset.");
                }
                return data.text;
            }
#           if UNITY_EDITOR
            set
            {
                string path = "Assets/Resources/mappings.xml";
                System.IO.Directory.CreateDirectory("Assets/Resources");
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(value);
                    }
                }
                UnityEditor.AssetDatabase.Refresh();
                CallSettingsChanged();
            }
#           endif
        }

        /// <summary>
        /// Remove the given error from the ignored errors list.
        /// </summary>
        /// <param name="error"></param>
        internal void RemoveIgnoredErrors(string error)
        {
            m_ignoredErrors.Remove(error);
            CallSettingsChanged();
        }

        /// <summary>
        /// /// Add the given error to the ignored errors list.
        /// </summary>
        /// <param name="error"></param>
        internal void AddIgnoredErrors(string error)
        {
            m_ignoredErrors.Add(error);
            CallSettingsChanged();
        }

        /// <summary>
        /// Reset all ignored errors, i.e. clears the errors list.
        /// </summary>
        /// <param name="error"></param>
        internal void ResetIgnoredErrors()
        {
            m_ignoredErrors.Clear();
            CallSettingsChanged();
        }

        /// <summary>
        /// Return true if the given error exists in the ignored errors list.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        internal bool IsErrorIgnored(string error)
        {
            return m_ignoredErrors.Contains(error);
        }

        /// <summary>
        /// Returns the number of errors in the ignored errors list.
        /// </summary>
        /// <returns></returns>
        internal int IgnoredErrorCount()
        {
            return m_ignoredErrors.Count;
        }

        private void CallSettingsChanged()
        {
            OnSettingsChanged?.Invoke();
            Save();
        }
    }

}
