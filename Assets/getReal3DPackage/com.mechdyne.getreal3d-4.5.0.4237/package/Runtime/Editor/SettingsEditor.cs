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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{

    [UnityEditor.CustomEditor(typeof(getReal3D.Settings))]
    internal class SettingsEditor : UnityEditor.Editor
    {
        private string[] logLevelNames;
        private string[] rpcLogLevelNames;
        private KeyValuePair<string, string>[] configNames;
        private List<FileSystemWatcher> fileSystemWatchers = null;
        private readonly object configNamesLock = new object();

        [MenuItem("getReal3D/Advanced/Settings", false, 100)]
        static public void CreateWindow()
        {
            Selection.objects = new Object[] { Settings.get() };
        }


        private void ConfigDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            lock (configNamesLock)
            {
                configNames = null;
            }
        }

        public override void OnInspectorGUI()
        {
            if (fileSystemWatchers == null)
            {
                fileSystemWatchers = new List<FileSystemWatcher>();
                foreach (var dir in Settings.ConfigurationDirectories)
                {
                    var directoryInfo = new DirectoryInfo(dir);
                    if (directoryInfo.Exists)
                    {
                        var fileSystemWatcher = new FileSystemWatcher(directoryInfo.FullName);
                        fileSystemWatcher.Changed += ConfigDirectoryChanged;
                        fileSystemWatcher.Created += ConfigDirectoryChanged;
                        fileSystemWatcher.Renamed += ConfigDirectoryChanged;
                        fileSystemWatcher.Deleted += ConfigDirectoryChanged;
                        fileSystemWatcher.EnableRaisingEvents = true;
                        fileSystemWatchers.Add(fileSystemWatcher);
                    }
                }
            }
            if (logLevelNames == null)
            {
                List<string> names = new List<string>(System.Enum.GetNames(typeof(LogLevel)));
                names.Remove(LogLevel.Discarded.ToString());
                names.Remove(LogLevel.None.ToString());
                logLevelNames = names.ToArray();
            }
            if (rpcLogLevelNames == null)
            {
                List<string> names = new List<string>(System.Enum.GetNames(typeof(LogLevel)));
                names.Remove(LogLevel.None.ToString());
                rpcLogLevelNames = names.ToArray();
            }
            getReal3D.Settings props = target as getReal3D.Settings;
            EditorGUILayout.HelpBox("getReal3D settings", MessageType.Info, true);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();

            props.autoInitVR = EditorGUILayout.Toggle(new GUIContent("Automatically initialize getReal3D", "Automatically call getReal3D.Plugin.initVR function."), props.autoInitVR);

#if ENABLE_INPUT_SYSTEM
            props.inputSystemController = EditorGUILayout.Toggle(new GUIContent("Input System device", "If set to true, getReal3D will emulate an Input System Controller"), props.inputSystemController);
#endif

            {
                int current = System.Array.IndexOf(logLevelNames, props.logLevel.ToString());
                GUIContent[] content = logLevelNames.Select(s => new GUIContent(s)).ToArray();
                string tooltip = "If a log event sent to getReal3D.Plugin is below this" +
                    " level, then it will be sent to Unity internal log.";
                int selected = EditorGUILayout.Popup(new GUIContent("Log level redirection", tooltip), current, content);
                props.logLevel = (LogLevel)System.Enum.Parse(typeof(LogLevel), logLevelNames[selected]);
            }

            {
                int current = System.Array.IndexOf(rpcLogLevelNames, props.rpcLogLevel.ToString());
                GUIContent[] content = rpcLogLevelNames.Select(s => new GUIContent(s)).ToArray();
                string tooltip = "Log level for RPC related log events.";
                int selected = EditorGUILayout.Popup(new GUIContent("RPC log level", tooltip), current, content);
                props.rpcLogLevel = (LogLevel)System.Enum.Parse(typeof(LogLevel), rpcLogLevelNames[selected]);
            }

            lock (configNamesLock)
            {
                if (configNames == null)
                {
                    configNames = Settings.AvailableConfigs.ToArray();
                }
                int current = System.Array.IndexOf(configNames.Select(s => s.Key).ToArray(), props.config);
                GUIContent[] content = configNames.Select(s => new GUIContent(s.Value)).ToArray();
                string tooltip = "Configuration file to load.";
                int selected = EditorGUILayout.Popup(new GUIContent("Configuration", tooltip), current, content);
                if (selected >= 0)
                {
                    props.config = configNames[selected].Key;
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Restore Defaults"))
            {
                props.Reset();
            }

            if (props.IgnoredErrorCount() != 0)
            {
                string msg = string.Format("{0} recommended settings are ignored.",
                    props.IgnoredErrorCount());
                EditorGUILayout.HelpBox(msg, MessageType.Warning, true);
                if (GUILayout.Button("Restore warnings"))
                {
                    props.ResetIgnoredErrors();
                    RequiredSettingsWindow.ShowIfNeeded();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}

#endif
