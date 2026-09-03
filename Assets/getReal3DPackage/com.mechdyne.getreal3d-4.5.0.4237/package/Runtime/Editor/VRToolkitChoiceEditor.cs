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
using UnityEngine;
using UnityEditor;
#if (UNITY_2017_2_OR_NEWER)
using VRSettings = UnityEngine.XR.XRSettings;
#else
using VRSettings = UnityEngine.VR.VRSettings;
#endif

namespace getReal3D.Editor
{

    [CustomEditor(typeof(VRToolkitChoice))]
    public class VRToolkitChoiceEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("List of VR toolkit that can be used.", MessageType.Info);

            var toolkitChoice = target as VRToolkitChoice;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add toolkit", GUILayout.Width(80)))
            {
                toolkitChoice.toolkits.Add(new VRToolkitChoice.VRToolkit());
            }
            GUILayout.EndHorizontal();

            int toDelete = -1;
            for (int i = 0; i < toolkitChoice.toolkits.Count; ++i)
            {
                var toolkit = toolkitChoice.toolkits[i];
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Target:");
                toolkit.target = EditorGUILayout.ObjectField(toolkit.target, typeof(GameObject),
                    true) as GameObject;
                GUILayout.Label("XR Device:");
                toolkit.deviceName = EditorGUILayout.TextField(toolkit.deviceName);
                GUILayout.Label("Force ");
                bool wasDefault = i == toolkitChoice.forcedToolkitIndex;
                bool isDefault = EditorGUILayout.Toggle(wasDefault);
                if (wasDefault && !isDefault)
                {
                    toolkitChoice.forcedToolkitIndex = -1;
                }
                else if (!wasDefault && isDefault)
                {
                    toolkitChoice.forcedToolkitIndex = i;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(toolkitChoice);
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    toDelete = i;
                }
                GUILayout.EndHorizontal();
            }

            if (toDelete >= 0)
            {
                toolkitChoice.toolkits.RemoveAt(toDelete);
                EditorUtility.SetDirty(toolkitChoice);
                if (toolkitChoice.forcedToolkitIndex == toDelete)
                {
                    toolkitChoice.forcedToolkitIndex = -1;
                }
                else if (toolkitChoice.forcedToolkitIndex > toDelete)
                {
                    --toolkitChoice.forcedToolkitIndex;
                }
            }

            if (toolkitChoice.toolkits.Count > 1)
            {
                EditorGUI.BeginChangeCheck();
                toolkitChoice.autoSelectFromLoadedDeviceName = EditorGUILayout.Toggle(
                    "Select toolkit from loaded XR device name.",
                    toolkitChoice.autoSelectFromLoadedDeviceName);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(toolkitChoice);
                }
                if (!string.IsNullOrEmpty(VRSettings.loadedDeviceName))
                {
                    EditorGUILayout.HelpBox("Loaded device: " + VRSettings.loadedDeviceName,
                        MessageType.Info);
                }

            }

            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif
