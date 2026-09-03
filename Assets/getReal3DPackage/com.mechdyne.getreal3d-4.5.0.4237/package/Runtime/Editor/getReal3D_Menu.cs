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
    public class getReal3D_Menu
    {

        [UnityEditor.Callbacks.PostProcessScene(101)]
        [MenuItem("getReal3D/Advanced/getReal3D Script Execution Order", false, 105)]
        static public void FixExecutionOrder()
        {
            getReal3D.Editor.Utils.FixScriptExecutionOrder();
        }

        [UnityEditor.Callbacks.PostProcessScene(102)]
        static public void CheckForMultisampling()
        {
            if (Application.isPlaying)
            {
                return;
            }

            System.IO.StringWriter errors = new System.IO.StringWriter();
            errors.WriteLine("This build might fail when running with getReal3D for Unity:\n");

            bool hasWarning = false;
            int currentLevel = UnityEngine.QualitySettings.GetQualityLevel();
            int levelCount = UnityEngine.QualitySettings.names.Length;
            for (int i = 0; i < levelCount; ++i)
            {
                UnityEngine.QualitySettings.SetQualityLevel(i);
                if (UnityEngine.QualitySettings.vSyncCount != 0)
                {
                    string err = "VSync is activated for quality settings " +
                        UnityEngine.QualitySettings.names[i] + ".";
                    errors.WriteLine(err);
                    hasWarning = true;
                }
            }

            UnityEngine.QualitySettings.SetQualityLevel(currentLevel);

            if (hasWarning)
            {
                ShowBuildError(errors.ToString());
            }
        }

        static internal void ShowBuildError(string message)
        {
            if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
            {
                Debug.LogError(message);
            }
            else
            {
                EditorUtility.DisplayDialog("getReal3D", message, "Ok");
            }
        }

        public static void BuildPlayerImpl(string[] levels, string output, bool arch64 = false)
        {
            BuildTarget currentTarget = EditorUserBuildSettings.activeBuildTarget;
            SwitchActiveBuildStandaloneTarget(BuildTarget.StandaloneWindows);

            BuildTarget buildTarget = arch64 ? BuildTarget.StandaloneWindows64 :
                BuildTarget.StandaloneWindows;
            AddGraphicApi(buildTarget, UnityEngine.Rendering.GraphicsDeviceType.Direct3D11);
#if !UNITY_2017_2_OR_NEWER
        AddGraphicApi(buildTarget, UnityEngine.Rendering.GraphicsDeviceType.Direct3D9);
#endif
            UnityEditor.BuildOptions options = BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, output, buildTarget, options);
            SwitchActiveBuildStandaloneTarget(currentTarget);
        }

        public static void BuildPlayerImpl(string output, bool arch64 = false)
        {
            BuildPlayerImpl(getEnabledScenes(), output, arch64);
        }

        public static string[] getEnabledScenes()
        {
            List<string> temp = new List<string>();
            foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
            {
                if (S.enabled)
                {
                    temp.Add(S.path);
                }
            }
            return temp.ToArray();
        }

        private static void AddGraphicApi(BuildTarget target,
            UnityEngine.Rendering.GraphicsDeviceType type)
        {
            List<UnityEngine.Rendering.GraphicsDeviceType> deviceTypes = PlayerSettings.GetGraphicsAPIs
                (BuildTarget.StandaloneWindows).ToList<UnityEngine.Rendering.GraphicsDeviceType>();
            if (!deviceTypes.Contains(type))
            {
                deviceTypes.Add(type);
                PlayerSettings.SetGraphicsAPIs(target, deviceTypes.ToArray());
            }
        }

        public static void SwitchActiveBuildStandaloneTarget(BuildTarget target)
        {
#if UNITY_5_6_OR_NEWER
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, target);
#else
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
#endif
        }
    }
}

#endif
