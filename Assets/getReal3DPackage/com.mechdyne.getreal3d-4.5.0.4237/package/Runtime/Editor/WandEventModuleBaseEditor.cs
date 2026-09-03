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
    public class WandEventModuleBaseEditor : UnityEditor.Editor
    {
        SerializedProperty sendWandEvents;
        SerializedProperty cursor;
        SerializedProperty mouseInteractionMode;
        SerializedProperty mouseCamera;
        SerializedProperty inputActionsPerSecond;
        SerializedProperty repeatDelay;
        SerializedProperty sendMoveEvents;

        void OnEnable()
        {
            sendWandEvents = serializedObject.FindProperty("m_sendWandEvents");
            cursor = serializedObject.FindProperty("cursor");
            mouseInteractionMode = serializedObject.FindProperty("m_mouseInteractionMode");
            mouseCamera = serializedObject.FindProperty("m_mouseCamera");
            inputActionsPerSecond = serializedObject.FindProperty("m_inputActionsPerSecond");
            repeatDelay = serializedObject.FindProperty("m_repeatDelay");
            sendMoveEvents = serializedObject.FindProperty("m_sendMoveEvents");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            PropertyField("Send Wand events", sendWandEvents);
            if (sendWandEvents.boolValue)
            {
                PropertyField("Cursor to display", cursor);
            }
            PropertyField("Master mouse interaction", mouseInteractionMode);
            var interactionMode = (WandEventModuleBase.MouseInteractionMode)
                mouseInteractionMode.enumValueIndex;
            if (interactionMode == WandEventModuleBase.MouseInteractionMode.WorldOnly ||
                interactionMode == WandEventModuleBase.MouseInteractionMode.All)
            {
                PropertyField("Master camera", mouseCamera);
            }
            PropertyField("Send move events", sendMoveEvents);
            if (sendMoveEvents.boolValue)
            {
                EditorGUILayout.PropertyField(repeatDelay);
                EditorGUILayout.PropertyField(inputActionsPerSecond);
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

    [CustomEditor(typeof(GenericWandEventModule))]
    public class GenericWandEventModuleEditor : WandEventModuleBaseEditor { }

    [CustomEditor(typeof(WandEventModule))]
    public class WandEventModuleEditor : WandEventModuleBaseEditor { }

}
#endif
