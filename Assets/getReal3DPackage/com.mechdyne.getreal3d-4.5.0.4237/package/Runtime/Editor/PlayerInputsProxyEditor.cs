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
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{
    [CustomEditor(typeof(PlayerInputsProxy))]
    [CanEditMultipleObjects]
    class PlayerInputsProxyEditor : UnityEditor.Editor
    {

        SerializedProperty m_initializationMode;
        SerializedProperty m_searchInThisObject;

        public void OnEnable()
        {
            m_initializationMode = serializedObject.FindProperty("initializationMode");
            m_searchInThisObject = serializedObject.FindProperty("searchInThisObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var target = (PlayerInputsProxy)serializedObject.targetObject;

            PropertyField("Initialization", m_initializationMode);

            if (target.initializationMode == PlayerInputsProxy.InitializationMode.SearchInObject)
            {
                PropertyField("Target object", m_searchInThisObject);
                if (target.searchInThisObject == null)
                {
                    EditorGUILayout.HelpBox("Target object not set.", MessageType.Warning, true);
                }
            }

            if (EditorApplication.isPlaying && target.target == null)
            {
                EditorGUILayout.HelpBox("Target not found.", MessageType.Warning, true);
            }

            if (target.target != null)
            {
                EditorGUILayout.ObjectField("Target", target.target.behaviour, typeof(GameObject), true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void PropertyField(string label, SerializedProperty property)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property, new GUIContent(label));
            GUILayout.EndHorizontal();
        }
    }
}
#endif
