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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static getReal3D.Editor.Utils;

namespace getReal3D.Editor
{
    internal interface SceneCheck
    {
        bool showError();
        void update();
    }

    internal interface ScriptPriorityCheck
    {
        Editor.Utils.ScriptPriorityError[] GetScriptPriorityErrors();
    }

    public class SceneChecker
        : EditorWindow
    {
        static bool needsUpdate = true;
        static bool foundCameraUpdater = false;
        static bool foundNavigationScript = false;

        static bool s_hideDisabledWarnings = true;
        static List<ErrorInfo> errors = new List<ErrorInfo>();
        static Regex filenameFilter = new Regex(@".*\.(cs|js)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Vector2 m_scrollPosition;
        Vector2 m_scrollPosition2;
        Vector2 m_scrollPosition3;

        internal static void RegisterSceneCheck(SceneCheck sceneCheck)
        {
            s_sceneChecks.Add(sceneCheck);
        }
        static List<SceneCheck> s_sceneChecks = new List<SceneCheck>();
        internal static void RegisterScriptPriorityCheck(ScriptPriorityCheck scriptPriorityCheck)
        {
            s_scriptPriorityChecks.Add(scriptPriorityCheck);
        }
        static List<ScriptPriorityCheck> s_scriptPriorityChecks = new List<ScriptPriorityCheck>();

        public class ErrorInfo
        {
            public int lineNum;
            public string line;
            public string path;
            public string filename;

            public ErrorInfo(string path, string filename, int lineNum, string line)
            {
                this.path = path;
                this.filename = filename;
                this.lineNum = lineNum;
                this.line = line;
            }

            public bool isDisabled()
            {
                return line.EndsWith(disabledPostfix);
            }

            static public readonly string disabledPostfix = "// no gr3d warning";
        }

        static bool foundEventSystem = false;
        static bool foundWandEventModule = false;
        static bool foundVRMenu = false;
        static bool eventCameraMissing = false;
        static bool eventCameraNotOnWand = false;

        static Editor.Utils.ScriptPriorityError[] s_scriptPriorityErrors;

        static Behaviour[] s_culledBehaviours;

        static bool ArrayEmpty(object[] array)
        {
            return array == null || array.Length == 0;
        }

        [MenuItem("getReal3D/Scene Checker", false, 50)]
        static void CreateChecker()
        {
            UpdateSceneStatus();
            EditorWindow.GetWindow(typeof(SceneChecker), false, "Scene Checker");
        }

        static T[] FindObjectsOfTypeInScene<T>() where T : Behaviour
        {
            List<T> res = new List<T>();
            T[] allComponents = Resources.FindObjectsOfTypeAll<T>();
            foreach (var monoBehaviour in allComponents)
            {
                if (monoBehaviour.hideFlags != HideFlags.None)
                {
                    continue;
                }
                res.Add(monoBehaviour);
            }
            return res.ToArray();
        }

        static void UpdateSceneStatus()
        {
            getRealCameraUpdater[] cu = Resources.FindObjectsOfTypeAll(typeof(getRealCameraUpdater)) as getRealCameraUpdater[];
            Navigation[] nv = Resources.FindObjectsOfTypeAll(typeof(Navigation)) as Navigation[];

            foundCameraUpdater = !ArrayEmpty(cu);
            foundNavigationScript = !ArrayEmpty(nv);

            errors = new List<ErrorInfo>();

            string[] sourceFiles = AssetDatabase.FindAssets("t:Script", new[] { "Assets" });
            for (int i = 0; i < sourceFiles.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(sourceFiles[i]);
                string filename = Path.GetFileNameWithoutExtension(path);
                EditorUtility.DisplayProgressBar("Checking scripts...", path,
                                                 i / (float)sourceFiles.Length);
                if (filenameFilter.IsMatch(path))
                {
                    try
                    {
                        StreamReader file = new StreamReader(path);
                        string line;
                        int lineNum = 1;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (!checkLine(line))
                            {
                                errors.Add(new ErrorInfo(path, filename, lineNum, line.Trim()));
                            }
                            ++lineNum;
                        }
                        file.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Failed to parse script " + path + ": " + e.ToString());
                    }
                }
                // UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(@"C:\1.txt", 1);
            }
            EditorUtility.ClearProgressBar();

            foundEventSystem = false;
            foundWandEventModule = false;
            foundVRMenu = false;
            eventCameraMissing = false;
            eventCameraNotOnWand = false;

            UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfTypeInScene<UnityEngine.EventSystems.EventSystem>();
            foundEventSystem = eventSystems.Length != 0;
            foreach (var eventSystem in eventSystems)
            {
                WandEventModuleBase wandEventModule = eventSystem.GetComponent<WandEventModuleBase>();
                foundWandEventModule |= wandEventModule != null;
            }

            UnityEngine.Canvas[] canvases = FindObjectsOfTypeInScene<UnityEngine.Canvas>();
            foreach (var canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    foundVRMenu = true;
                    Camera eventCamera = canvas.worldCamera;
                    if (eventCamera)
                    {
                        eventCameraNotOnWand |= eventCamera.GetComponentInParent<getRealWandUpdater>() != null;
                        GenericWandUpdater wandUpdater = eventCamera.GetComponentInParent<GenericWandUpdater>();
                        eventCameraNotOnWand |= wandUpdater && eventCamera.transform.IsChildOf(wandUpdater.target);
                    }
                    else
                    {
                        eventCameraMissing = true;
                    }
                }
            }

            //m_duplicatedNetworkIdentities = MultiCluster.NetworkIdentity.GetDuplicated();

            //s_scriptPriorityErrors = GetScriptPriorityErrors();
            foreach (var sceneCheck in s_sceneChecks)
            {
                sceneCheck.update();
            }

            {
                var scriptPriorityErrors = new List<ScriptPriorityError>();
                foreach (var scriptPriorityChecks in s_scriptPriorityChecks)
                {
                    var errors = scriptPriorityChecks.GetScriptPriorityErrors();
                    scriptPriorityErrors.AddRange(errors);
                }
                s_scriptPriorityErrors = scriptPriorityErrors.ToArray();
            }

            var culledAnimators = Resources.FindObjectsOfTypeAll<Animator>().
                    Where(animator => animator.cullingMode != AnimatorCullingMode.AlwaysAnimate).
                    Cast<Behaviour>();

            var culledAnimations = Resources.FindObjectsOfTypeAll<Animation>().
                    Where(animation => animation.cullingType != AnimationCullingType.AlwaysAnimate).
                    Cast<Behaviour>();

            s_culledBehaviours = culledAnimators.Concat(culledAnimations).ToArray();

            needsUpdate = false;
        }

        static private bool checkLine(string str)
        {
            bool betweenQuotes = false;
            bool betweenComments = false;
            char lastChar = '\0';
            int len = str.Length;
            for (int i = 0; i < len;)
            {
                char c = str[i];
                bool lastCharWasIdentifierChar =
                    (lastChar >= 'a' && lastChar <= 'z') ||
                    (lastChar >= 'A' && lastChar <= 'Z') ||
                    (lastChar >= '0' && lastChar <= '9') ||
                    (lastChar == '_');
                if (lastChar == '/' && c == '/')
                {
                    return true;
                }
                else if (lastChar != '\\' && c == '"')
                {
                    betweenQuotes = !betweenQuotes;
                }
                else if (lastChar != '/' && c == '*')
                {
                    betweenComments = true;
                }
                else if (lastChar != '*' && c == '/')
                {
                    betweenComments = true;
                }
                else if (!betweenComments && !betweenQuotes)
                {
                    if (0 == string.Compare(str, i, "UnityEngine.Input.", 0, 18, StringComparison.Ordinal))
                    {
                        return false;
                    }
                    else if (0 == string.Compare(str, i, "getReal3D.Input.", 0, 16, StringComparison.Ordinal))
                    {
                        lastChar = '.';
                        i += 16;
                        continue;
                    }
                    else if (!lastCharWasIdentifierChar && 0 == string.Compare(str, i, "Input.", 0, 6, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
                lastChar = c;
                ++i;
            }
            return true;
        }

        void OnGUI()
        {
            if (needsUpdate) UpdateSceneStatus();
            if (!foundCameraUpdater)
                EditorGUILayout.HelpBox("No getRealCameraUpdater script found. You probably want a getRealCameraUpdater attached to a GameObject with a Camera.", MessageType.Warning, true);
            else
                EditorGUILayout.HelpBox("Found getRealCameraUpdater.", MessageType.Info, true);

            if (!foundNavigationScript)
                EditorGUILayout.HelpBox("No getReal3D navigation scripts found. You probably want a navigation script (getRealAimAndGoController, getRealWalkthruController, getRealWandLook, getRealJoyLook, Navigation) attached to a GameObject.", MessageType.None, true);
            else
                EditorGUILayout.HelpBox("Found getReal3D navigation scripts.", MessageType.Info, true);

            if (foundVRMenu)
            {
                if (!foundEventSystem)
                {
                    EditorGUILayout.HelpBox("EventSystem script not found. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if (!foundWandEventModule)
                {
                    EditorGUILayout.HelpBox("EventSystem found, but no WandEventModule associated. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if (eventCameraMissing)
                {
                    EditorGUILayout.HelpBox("World space rendering canvas found, but no camera is associated to it. VR menu won't receive any input.", MessageType.Warning, true);
                }
                else if (!eventCameraNotOnWand)
                {
                    EditorGUILayout.HelpBox("World space rendering canvas found, but attached camera is not a child of the wand. VR menu won't receive any input.", MessageType.Warning, true);
                }
            }



            bool allErrorsDisabled = errors.All(e => e.isDisabled());
            bool hasVisibleErrors = errors.Count != 0 && !(allErrorsDisabled && s_hideDisabledWarnings);
            EditorGUILayout.HelpBox("Prefer getReal3D.Input over UnityEngine.Input. or Input.", hasVisibleErrors ? MessageType.Warning : MessageType.Info, true);

            if (errors.Count != 0)
            {
                s_hideDisabledWarnings = GUILayout.Toggle(s_hideDisabledWarnings, "Don't show warnings for lines ending by '" + ErrorInfo.disabledPostfix + "'.");

                m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
                foreach (var errorInfo in errors)
                {
                    if (!s_hideDisabledWarnings || !errorInfo.isDisabled())
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select", GUILayout.Width(50)))
                        {
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(errorInfo.path);
                        }
                        if (GUILayout.Button("Open", GUILayout.Width(50)))
                        {
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(errorInfo.path, errorInfo.lineNum);
                        }
                        GUILayout.Label(errorInfo.filename + ":" + errorInfo.lineNum + " " + errorInfo.line);
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }

            if (s_scriptPriorityErrors != null && s_scriptPriorityErrors.Count() > 0)
            {
                EditorGUILayout.HelpBox("There are script priority errors. Fix them in the Project" +
                    " Settings.", MessageType.Warning, true);
                m_scrollPosition2 = EditorGUILayout.BeginScrollView(m_scrollPosition2);
                foreach (var scriptPriorityError in s_scriptPriorityErrors)
                {
                    GUILayout.BeginHorizontal();

                    int priority1 = MonoImporter.GetExecutionOrder(scriptPriorityError.script1);
                    int priority2 = MonoImporter.GetExecutionOrder(scriptPriorityError.script2);
                    var msg = string.Format("Script {0} (priority {1}) should " +
                        "run before script {2} (priority {3}).",
                        scriptPriorityError.script1.name, priority1,
                        scriptPriorityError.script2.name, priority2);

                    GUILayout.Label(msg);

                    if (GUILayout.Button("Script 1", GUILayout.Width(70)))
                    {
                        var path = AssetDatabase.GetAssetPath(scriptPriorityError.script1);
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                    }
                    if (GUILayout.Button("Script 2", GUILayout.Width(70)))
                    {
                        var path = AssetDatabase.GetAssetPath(scriptPriorityError.script2);
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                    }

                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

            if (s_culledBehaviours != null && s_culledBehaviours.Count() > 0)
            {
                EditorGUILayout.HelpBox("The following behaviours should not use any culling. " +
                    "Please set the Culling Mode property to Always Animate",
                    MessageType.Warning, true);

                m_scrollPosition3 = EditorGUILayout.BeginScrollView(m_scrollPosition3);
                foreach (var behaviour in s_culledBehaviours)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(behaviour.gameObject.name);
                    if (GUILayout.Button("Select", GUILayout.Width(50)))
                    {
                        Selection.activeObject = behaviour.gameObject;
                    }
                    if (GUILayout.Button("Fix", GUILayout.Width(50)))
                    {
                        if (behaviour is Animator animator)
                        {
                            FixCulling(animator);
                        }
                        else if (behaviour is Animation animation)
                        {
                            FixCulling(animation);
                        }
                        needsUpdate = true;
                    }

                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

            foreach (var sceneCheck in s_sceneChecks)
            {
                needsUpdate |= sceneCheck.showError();
            }

            if (GUILayout.Button("Update"))
            {
                UpdateSceneStatus();
            }
        }

        private static bool isPrefab(GameObject obj)
        {
#           if UNITY_2018_3_OR_NEWER
            return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;
#           else
            return PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab;
#           endif
        }

        private static void FixCulling(Animator animator)
        {
            if (isPrefab(animator.gameObject))
            {
                Undo.RecordObject(animator, "Changed animator culling mode");
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                EditorUtility.SetDirty(animator);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Undo.RecordObject(animator, "Changed animator culling mode");
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        private static void FixCulling(Animation animation)
        {
            if (isPrefab(animation.gameObject))
            {
                Undo.RecordObject(animation, "Changed animation culling type");
                animation.cullingType = AnimationCullingType.AlwaysAnimate;
                EditorUtility.SetDirty(animation);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Undo.RecordObject(animation, "Changed animation culling type");
                animation.cullingType = AnimationCullingType.AlwaysAnimate;
            }
        }
    }
}
#endif
