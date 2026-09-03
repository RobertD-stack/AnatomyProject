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
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{
    /// <summary>
    /// A couple of utils.
    /// </summary>
    static public class Utils
    {

        /// <summary>
        /// Fixes the getReal3D scripts priorities.
        /// </summary>
        /// [MenuItem("getReal3D/Advanced/getReal3D Script Execution Order", false, 105)] // Not working here in 4.3.3 ... dunno why
        static public void FixScriptExecutionOrder(Dictionary<string, int> userPriorities = null)
        {
            MonoScript[] monoScripts = UnityEditor.MonoImporter.GetAllRuntimeMonoScripts();
            Dictionary<string, int> priorities = userPriorities;
            if (priorities == null)
            {
                Dictionary<string, int> defaultPriorities = new Dictionary<string, int> {
                        { "ClusterManager", -32000},
                        { "getRealCameraUpdater", -31999},
                        { "getRealHeadUpdater", -31998},
                        { "getRealWandUpdater", -31997},
                        { "getRealSkybox", 31999},
                        { "ClusterManagerCaller", 32000}
                    };
                priorities = defaultPriorities;
            }

            foreach (MonoScript script in monoScripts)
            {
                if (script != null && script.GetClass() != null)
                {
                    if (priorities.ContainsKey(script.GetClass().Name))
                    {
                        int currentPriority = UnityEditor.MonoImporter.GetExecutionOrder(script);
                        int desiredPriority = priorities[script.GetClass().Name];
                        if (currentPriority != desiredPriority)
                        {
                            UnityEditor.MonoImporter.SetExecutionOrder(script, desiredPriority);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Script priority error data
        /// </summary>
        public class ScriptPriorityError
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="script1"></param>
            /// <param name="script2"></param>
            public ScriptPriorityError(MonoScript script1, MonoScript script2)
            {
                this.script1 = script1;
                this.script2 = script2;
            }

            /// <summary>
            /// First script.
            /// </summary>
            public MonoScript script1;

            /// <summary>
            /// Second script.
            /// </summary>
            public MonoScript script2;
        }

        /// <summary>
        /// Ensure that the priority of T1 is higher than the one of T2, i.e. that T1 executes
        /// before T2. This functions also checks all subtypes of T1 and T2
        /// </summary>
        /// <typeparam name="T1">Script type number 1</typeparam>
        /// <typeparam name="T2">Script type number 2</typeparam>
        public static ScriptPriorityError[] EnsureRelativePriority<T1, T2>()
        {
            List<ScriptPriorityError> errors = new List<ScriptPriorityError>();
            foreach (MonoScript script1 in GetScriptsInheriting<T1>())
            {
                foreach (MonoScript script2 in GetScriptsInheriting<T2>())
                {
                    int priority1 = MonoImporter.GetExecutionOrder(script1);
                    int priority2 = MonoImporter.GetExecutionOrder(script2);
                    if (priority1 >= priority2)
                    {
                        errors.Add(new ScriptPriorityError(script1, script2));
                    }
                }
            }
            return errors.ToArray();
        }

        private static IEnumerable<MonoScript> GetScriptsInheriting<T>()
        {
            foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var classType = script.GetClass();
                if (classType != null &&
                    (classType.IsSubclassOf(typeof(T)) || typeof(T) == classType))
                {
                    yield return script;
                }
            }
        }

        /// <summary>
        /// Returns the list of all children of the given GameObject.
        /// </summary>
        public static List<GameObject> GetAllChildrenRecursive(GameObject obj)
        {
            List<GameObject> go = new List<GameObject>();
            if (obj != null)
            {
                go.Add(obj);
                Transform[] trans = obj.GetComponentsInChildren<Transform>(true) as Transform[];
                foreach (Transform t in trans)
                    if (t != null && !go.Contains(t.gameObject))
                        go.AddRange(GetAllChildrenRecursive(t.gameObject));
            }
            return go;
        }

        /// <summary>
        /// Returns the list of all selected objects.
        /// </summary>
        /// <param name="recursive">If set, also returns children of the selected objects.</param>
        public static List<GameObject> GetAllObjectsInSelection(bool recursive = false)
        {
            List<GameObject> pReturn = new List<GameObject>();

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go != null)
                {
                    List<GameObject> subList = null;
                    if (recursive)
                        subList = GetAllChildrenRecursive(go);
                    else
                    {
                        subList = new List<GameObject>();
                        subList.Add(go);
                    }

                    foreach (GameObject pObject in subList)
                    {
                        if (pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
                        {
                            continue;
                        }

                        if (Application.isEditor)
                        {
                            string sAssetPath = AssetDatabase.GetAssetPath(pObject.transform.root.gameObject);
                            if (!string.IsNullOrEmpty(sAssetPath))
                            {
                                continue;
                            }
                        }

                        pReturn.Add(pObject);
                    }
                }
            }

            return pReturn;
        }

        /// <summary>
        /// Enable / disable Unity cluster.
        /// </summary>
        [System.Obsolete]
        public static bool ChangeClusterMode(bool clusterMode)
        {
            string propertyName = "cluster";
            System.Reflection.PropertyInfo propertyInfo = typeof(EditorUserBuildSettings).GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(null, clusterMode, null);
                return (bool)propertyInfo.GetValue(null, null);
            }
            else if (clusterMode)
            {
                throw new System.Exception("Unable to find EditorUserBuildSettings." + propertyName);
            }
            else
            {
                return false;
            }
        }
    }
}

#endif
