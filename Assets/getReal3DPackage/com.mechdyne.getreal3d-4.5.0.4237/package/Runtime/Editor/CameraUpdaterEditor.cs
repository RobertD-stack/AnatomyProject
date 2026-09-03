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
using UnityEditor.SceneManagement;
using UnityEngine;

namespace getReal3D.Editor
{

    /// <summary>
    /// Editor class used to trigger the camera preview.
    /// </summary>
    public class CameraUpdaterEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Custom inspector function
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            EditorGUILayout.HelpBox("Camera preview setup.", MessageType.None, true);

            serializedObject.Update();
            var target = (CameraUpdaterInterface)serializedObject.targetObject;

            if (GUILayout.Button("Preview camera from configuration file"))
            {
                UpdateCamera(target);
                EditorUtility.SetDirty(target.behaviour);
                EditorSceneManager.MarkAllScenesDirty();
            }

            if (target.cameraPreviewData.hasProjMatrix && GUILayout.Button("Reset to Unity camera"))
            {
                ResetCamera(target);
                EditorUtility.SetDirty(target.behaviour);
                EditorSceneManager.MarkAllScenesDirty();
            }

            if (target.cameraPreviewData.hasProjMatrix)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Projection matrix:");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextArea(target.cameraPreviewData.projMatrix.ToString());
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("No projection matrix.");
            }

        }

        private void ResetCamera(CameraUpdaterInterface target)
        {
            CameraPreviewData cameraPreviewData;
            cameraPreviewData.hasProjMatrix = false;
            cameraPreviewData.projMatrix = Matrix4x4.identity;
            target.cameraPreviewData = cameraPreviewData;
            target.behaviour.transform.localPosition = new Vector3(0, 1.5f, 0);
            target.behaviour.transform.localRotation = Quaternion.identity;
        }

        private void UpdateCamera(CameraUpdaterInterface target)
        {
            try
            {
                CameraPreviewData cameraPreviewData;
                cameraPreviewData.hasProjMatrix = false;
                cameraPreviewData.projMatrix = Matrix4x4.identity;
                Plugin.InternalInitVR(false);
                Plugin.updateData();
                var camera = target.behaviour.GetComponent<Camera>();
                var cameras = Plugin.getCameraSensors();
                target.behaviour.transform.localPosition = cameras[0].position;
                target.behaviour.transform.localRotation = cameras[0].rotation;
                Plugin.getCameraProjectionMatrix(0, camera.nearClipPlane,
                    camera.farClipPlane, ref cameraPreviewData.projMatrix);
                cameraPreviewData.hasProjMatrix = true;
                target.cameraPreviewData = cameraPreviewData;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error while updating camera: " + e.ToString());
                Debug.LogWarning(e.StackTrace);
            }
            Plugin.deinit();
        }
    }
}
#endif
