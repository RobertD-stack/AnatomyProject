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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{
    internal class RequiredSettingsWindow : EditorWindow
    {
        private const bool forceShow = false;
        private List<Error> m_errors;
        private Vector2 scrollPosition;

        static internal void ShowIfNeeded()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            bool allErrorsIgnored = GetAllErrors().All(e => isIgnored(e));
            if (forceShow || !allErrorsIgnored)
            {
                var window = GetWindow<RequiredSettingsWindow>(true, "getReal3D Required Settings");
                window.ComputeErrors();
            }
        }

        void OnGUI()
        {
            EditorGUILayout.HelpBox("Recommended project settings for getReal3D:", MessageType.Warning);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            EditorGUI.BeginChangeCheck();

            if (m_errors == null)
            {
                m_errors = GetAllErrors();
            }

            foreach (var error in m_errors.Where(e => !isIgnored(e)))
            {
                GUILayout.Label(error.errorString);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(error.buttonString))
                {
                    error.Accept();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    Ignore(error);
                }

                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear All Ignores"))
            {
                ClearAllIgnore();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (m_errors.Count() > 0)
            {
                if (GUILayout.Button("Accept All"))
                {
                    AcceptAll();
                }
                if (GUILayout.Button("Ignore All"))
                {
                    if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
                    {
                        IgnoreAll();
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Close"))
                {
                    Close();
                }
            }
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                ComputeErrors();
            }
        }

        private void ClearAllIgnore()
        {
            foreach (var error in m_errors)
            {
                ClearIgnore(error);
            }
        }

        private void IgnoreAll()
        {
            foreach (var error in m_errors)
            {
                Ignore(error);
            }
        }

        private static void ClearIgnore(Error error)
        {
            Settings.get().RemoveIgnoredErrors(error.ignoreCode);
        }

        private static void Ignore(Error error)
        {
            if (!isIgnored(error))
            {
                Settings.get().AddIgnoredErrors(error.ignoreCode);
            }
        }

        private static bool isIgnored(Error error)
        {
            return Settings.get().IsErrorIgnored(error.ignoreCode);
        }

        private void AcceptAll()
        {
            foreach (var error in m_errors)
            {
                error.Accept();
            }
        }

        private void ComputeErrors()
        {
            m_errors = GetAllErrors();
        }

        private static List<Error> GetAllErrors()
        {
            var errors = new List<Error>();

            int currentLevel = QualitySettings.GetQualityLevel();
            int levelCount = QualitySettings.names.Length;
            for (int i = 0; i < levelCount; ++i)
            {
                QualitySettings.SetQualityLevel(i);
                int levelToModify = i;
                string name = UnityEngine.QualitySettings.names[i];
                if (QualitySettings.vSyncCount != 0)
                {
                    errors.Add(new GenericError("VSync is activated for quality level " + name + ".",
                        "Disable", name + "_vsync",
                        () =>
                        {
                            QualitySettings.SetQualityLevel(levelToModify);
                            QualitySettings.vSyncCount = 0;
                            QualitySettings.SetQualityLevel(currentLevel);
                        }
                    ));
                }
            }

            QualitySettings.SetQualityLevel(currentLevel);

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
            {
                errors.Add(new GenericError("Current build target is " + EditorUserBuildSettings.activeBuildTarget,
                    "Set StandaloneWindows64 build target.", "build_target",
                    () =>
                    {
                        BuildTarget target = BuildTarget.StandaloneWindows64;
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, target);
                    }
                ));
            }
#if !UNITY_6000_0_OR_NEWER
            if (PlayerSettings.captureSingleScreen)
            {
                errors.Add(new GenericError("Capture Single Screen is enabled.",
                    "Disable", "captureSingleScreen",
                    () => { PlayerSettings.captureSingleScreen = false; }
                ));
            }
#endif
            if (PlayerSettings.fullScreenMode != FullScreenMode.Windowed)
            {
                errors.Add(new GenericError("Default is Full Screen is enabled.",
                    "Disable", "defaultIsFullScreen",
                    () => { PlayerSettings.fullScreenMode = FullScreenMode.Windowed; }
                ));
            }

            if (!PlayerSettings.runInBackground)
            {
                errors.Add(new GenericError("Run in background is disabled.",
                    "Enable", "runInBackground",
                    () => { PlayerSettings.runInBackground = true; }
                ));
            }

            if (PlayerSettings.resizableWindow)
            {
                errors.Add(new GenericError("Resizable windows is enabled.",
                    "Disable", "resizableWindow",
                    () => { PlayerSettings.resizableWindow = false; }
                ));
            }

            if (PlayerSettings.forceSingleInstance)
            {
                errors.Add(new GenericError("Force single instance is enabled.",
                    "Disable", "forceSingleInstance",
                    () => { PlayerSettings.forceSingleInstance = false; }
                ));
            }

            if (!PlayerSettings.usePlayerLog)
            {
                errors.Add(new GenericError("Use player log is disabled.",
                    "Enable", "usePlayerLog",
                    () => { PlayerSettings.usePlayerLog = true; }
                ));
            }

            if (PlayerSettings.SplashScreen.show)
            {
                errors.Add(new GenericError("Splash screen is enabled.", "Disable", "splashScreen",
                    () => { PlayerSettings.SplashScreen.show = false; }
                ));
            }

            return errors;
        }

        private interface Error
        {
            void Accept();
            string errorString { get; }
            string buttonString { get; }
            string ignoreCode { get; }
        }

        public class GenericError : Error
        {
            public delegate void AcceptDelegate();
            private AcceptDelegate m_acceptDelegate;

            public GenericError(string error, string button, string ignore, AcceptDelegate acceptDelegate)
            {
                buttonString = button;
                errorString = error;
                ignoreCode = ignore;
                m_acceptDelegate = acceptDelegate;
            }

            public string buttonString { get; set; }

            public string errorString { get; set; }

            public string ignoreCode { get; set; }

            public void Accept()
            {
                m_acceptDelegate();
            }

        }
    }

}
#endif
