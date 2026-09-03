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

    internal class NavigationHelperEditor
    {

        SerializedProperty navigationMethod;
        SerializedProperty UseFixedUpdate;
        SerializedProperty TranslationSpeed;
        SerializedProperty RotationSpeed;
        SerializedProperty WandLookDeadZone;
        SerializedProperty navFollows;
        SerializedProperty navReference;
        SerializedProperty rotationAround;
        SerializedProperty rotationAroundReference;
        SerializedProperty rotationFollows;
        SerializedProperty rotationFollowsReference;
        SerializedProperty joylookRotationAxes;
        SerializedProperty wandlookRotation;
        SerializedProperty wandLookContinuousDrive;
        SerializedProperty capsuleFollowsHeadPosition;
        SerializedProperty orbitNavDataCenter;
        SerializedProperty orbitNavDataRadius;

        public void OnEnable(SerializedObject serializedObject)
        {
            navigationMethod =
                serializedObject.FindProperty("m_navigationHelper.m_navigationMethod");
            UseFixedUpdate = serializedObject.FindProperty("m_navigationHelper.UseFixedUpdate");
            TranslationSpeed = serializedObject.FindProperty("m_navigationHelper.TranslationSpeed");
            RotationSpeed = serializedObject.FindProperty("m_navigationHelper.RotationSpeed");
            WandLookDeadZone = serializedObject.FindProperty("m_navigationHelper.WandLookDeadZone");
            navFollows = serializedObject.FindProperty("m_navigationHelper.navFollows");
            navReference = serializedObject.FindProperty("m_navigationHelper.navReference");
            rotationAround = serializedObject.FindProperty("m_navigationHelper.rotationAround");
            rotationAroundReference = serializedObject.FindProperty
                ("m_navigationHelper.rotationAroundReference");
            rotationFollows = serializedObject.FindProperty("m_navigationHelper.rotationFollows");
            rotationFollowsReference = serializedObject.FindProperty
                ("m_navigationHelper.rotationFollowsReference");
            joylookRotationAxes = serializedObject.FindProperty
                ("m_navigationHelper.joylookRotationAxes");
            wandlookRotation = serializedObject.FindProperty("m_navigationHelper.wandlookRotation");
            wandLookContinuousDrive = serializedObject.FindProperty
                ("m_navigationHelper.wandLookContinuousDrive");
            capsuleFollowsHeadPosition = serializedObject.FindProperty
                ("m_navigationHelper.capsuleFollowsHeadPosition");
            orbitNavDataCenter = serializedObject.FindProperty
                ("m_navigationHelper.orbitNavData.center");
            orbitNavDataRadius = serializedObject.FindProperty
                ("m_navigationHelper.orbitNavData.initialRadius");
        }
        public void OnInspectorGUI(SerializedObject serializedObject, NavigationHelper navigationHelper)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Navigation Settings", EditorStyles.boldLabel);
            PropertyField("Type", navigationMethod);
            PropertyField("Use fixed update?", UseFixedUpdate);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Navigation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(TranslationSpeed);
            EditorGUILayout.PropertyField(navFollows);
            if (navFollows.enumValueIndex == (int)NavigationHelper.NavFollows.Reference)
            {
                EditorGUILayout.PropertyField(navReference);
            }
            EditorGUILayout.PropertyField(capsuleFollowsHeadPosition);
            if (navigationHelper.m_navigationMethod == NavigationHelper.NavigationMethod.Orbit)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Orbit Navigation Settings", EditorStyles.boldLabel);
                PropertyField("Center", orbitNavDataCenter);
                PropertyField("Radius", orbitNavDataRadius);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Look Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(RotationSpeed);
            EditorGUILayout.PropertyField(WandLookDeadZone);
            EditorGUILayout.PropertyField(rotationAround);
            if (rotationAround.enumValueIndex == (int)NavigationHelper.NavFollows.Reference)
            {
                EditorGUILayout.PropertyField(rotationAroundReference);
            }
            EditorGUILayout.PropertyField(rotationFollows);
            if (rotationFollows.enumValueIndex == (int)NavigationHelper.NavFollows.Reference)
            {
                EditorGUILayout.PropertyField(rotationFollowsReference);
            }
            EditorGUILayout.PropertyField(joylookRotationAxes);
            EditorGUILayout.PropertyField(wandlookRotation);
            EditorGUILayout.PropertyField(wandLookContinuousDrive);
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
