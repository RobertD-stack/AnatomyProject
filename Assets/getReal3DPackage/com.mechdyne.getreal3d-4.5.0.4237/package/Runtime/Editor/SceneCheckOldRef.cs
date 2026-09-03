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
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{
    public class SceneCheckOldRef : SceneCheck
    {
        private static List<string> s_brokenFiles = new List<string>();
        private static string m_guid = "6aebc42a318181047bb12fdb4b28b5c8";
        private static Dictionary<long, string> m_changes = new Dictionary<long, string>
    {
        { -1501968154, "6706c1c1709804643ba88ab00d361236" }, // NetworkTransform
        { -2054039809, "67ab6e03f51ff1241a6d8db56944a685" }, // NetworkIdentity
        { -1855698357, "c4fda767a16463b4eb54e23961be691b" }, // NetworkTransformChild
        { -1759138922, "a5498d16343b28f48b0d9bfa9c80d624" }, // NetworkManager
    };

        // m_Script: {fileID: -1501968154, guid: 6aebc42a318181047bb12fdb4b28b5c8, type: 3}
        // ->
        // m_Script: {fileID:  11500000, guid: 67ab6e03f51ff1241a6d8db56944a685, type: 3}

        private static Dictionary<string, string> replacements
        {
            get
            {
                var replacements = new Dictionary<string, string>();
                foreach (var item in m_changes)
                {
                    string from = string.Format("m_Script: {{fileID: {0}, guid: 6aebc42a318181047bb12fdb4b28b5c8, type: 3}}", item.Key);
                    string to = string.Format("m_Script: {{fileID:  11500000, guid: {0}, type: 3}}", item.Value);
                    replacements.Add(from, to);
                }
                return replacements;
            }
        }

        bool SceneCheck.showError()
        {
            bool needsUpdate = false;
            if (s_brokenFiles.Count > 0)
            {
                EditorGUILayout.HelpBox("Found missing old getReal3D component in some files." +
                    "/!\\ Make sure to do a backup before attempting to fix. /!\\", MessageType.Error);
                foreach (var brokenFile in s_brokenFiles)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(brokenFile);
                    if (GUILayout.Button("Try to fix", GUILayout.Width(150)))
                    {
                        if (tryFix(brokenFile))
                        {
                            needsUpdate = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            if (needsUpdate)
            {
                AssetDatabase.Refresh();
            }
            return needsUpdate;
        }

        private bool tryFix(string filename)
        {
            bool changed = false;
            string content = File.ReadAllText(filename);
            foreach (var replacement in replacements)
            {
                var newContent = content.Replace(replacement.Key, replacement.Value);
                changed |= newContent != content;
                content = newContent;
            }
            if (changed)
            {
                Debug.Log("Modified file " + filename);
                File.WriteAllText(filename, content);
            }
            return changed;
        }

        private bool verifyFile(string filename)
        {
            string content = File.ReadAllText(filename);
            return content.Contains(m_guid);
        }

        void SceneCheck.update()
        {
            s_brokenFiles.Clear();
            var searchPatterns = new string[] { "*.prefab", "*.unity" };
            foreach (string searchPattern in searchPatterns)
            {
                foreach (string file in Directory.EnumerateFiles(Application.dataPath,
                    searchPattern, SearchOption.AllDirectories))
                {
                    if (verifyFile(file))
                    {
                        s_brokenFiles.Add(file);
                    }
                }
            }
        }
    }

    [InitializeOnLoad]
    public class SceneCheckOldRefRegister
    {
        static SceneCheckOldRefRegister()
        {
            SceneChecker.RegisterSceneCheck(new SceneCheckOldRef());
        }
    }
}
#endif
