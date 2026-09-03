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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace getReal3D
{
    /// <summary>
    /// Script used to update a camera position, rotation and projection.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("getReal3D/Updater/Camera Updater")]
    [ExecuteInEditMode]
    public class getRealCameraUpdater : getRealUserScript, CameraUpdaterInterface
    {

        public CameraUpdaterHelper updater
        {
            get { return m_cameraUpdaterHelper; }
            set { m_cameraUpdaterHelper = value; }
        }

        public MonoBehaviour behaviour
        {
            get { return this; }
        }

        [SerializeField]
        private CameraPreviewData m_cameraPreviewData;

        public CameraPreviewData cameraPreviewData
        {
            get { return m_cameraPreviewData; }
            set { m_cameraPreviewData = value; }
        }

        private CameraUpdaterHelper m_cameraUpdaterHelper;
        private Camera m_camera = null;

        void OnEnable()
        {
            m_camera = GetComponent<Camera>();
            CameraUpdaterHelper.CreateCamerasForUserIfNeeded(GetComponent<Camera>(), userId());
            registerRenderPipeline();
        }

        /// Can be set to false in order to disable head tracking updates
        public bool m_updateTrackingPosition = true;

        void OnPreCull()
        {
            if (m_cameraUpdaterHelper != null && m_updateTrackingPosition)
            {
                m_cameraUpdaterHelper.PreCull();
            }
        }

        void OnPreRender()
        {
            if (m_cameraUpdaterHelper != null)
            {
                m_cameraUpdaterHelper.PreRender();
            }
        }

        void OnPostRender()
        {
            if (m_cameraUpdaterHelper != null)
            {
                m_cameraUpdaterHelper.PostRender();
            }
        }

        void OnDestroy()
        {
            deregisterRenderPipeline();
            if (m_cameraUpdaterHelper != null)
            {
                m_cameraUpdaterHelper.Destroyed();
            }
        }

        void OnDisable()
        {
            if (m_cameraUpdaterHelper != null)
            {
                m_cameraUpdaterHelper.Disabled();
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (m_cameraUpdaterHelper != null)
            {
                m_cameraUpdaterHelper.OnRenderImage(src, dst);
            }
            else
            {
                Graphics.Blit(src, dst);
            }
        }

        #region URP/HDRP
#if UNITY_2019_4_OR_NEWER
        private void registerRenderPipeline()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
#if UNITY_6000_0_OR_NEWER
            RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
            RenderPipelineManager.endContextRendering += OnEndContextRendering;
#else
            RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
            RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
#endif
        }


        private void deregisterRenderPipeline()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
#if UNITY_6000_0_OR_NEWER
            RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
            RenderPipelineManager.endContextRendering -= OnEndContextRendering;
#else
            RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
            RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
#endif
        }

        void OnBeginCameraRendering(ScriptableRenderContext src, Camera camera)
        {
            if (m_cameraUpdaterHelper != null && camera == m_camera)
            {
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                    m_cameraUpdaterHelper.hdrpBeginCameraRendering(src);
                else
                    m_cameraUpdaterHelper.urpBeginCameraRendering(src);
            }
        }

        void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            OnBeginFrameRendering(context, cameras.ToArray());
        }

        void OnBeginFrameRendering(ScriptableRenderContext src, Camera[] cameras)
        {
            if (m_cameraUpdaterHelper != null)
            {
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                    m_cameraUpdaterHelper.hdrpBeginFrameRendering(src, cameras);
                else
                    m_cameraUpdaterHelper.urpBeginFrameRendering(src, cameras);
            }
        }

        void OnEndCameraRendering(ScriptableRenderContext src, Camera camera)
        {
            if (m_cameraUpdaterHelper != null && camera == m_camera)
            {
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                    m_cameraUpdaterHelper.hdrpEndCameraRendering(src);
                else
                    m_cameraUpdaterHelper.urpEndCameraRendering(src);
            }
        }

        void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            OnEndFrameRendering(context, cameras.ToArray());
        }

        void OnEndFrameRendering(ScriptableRenderContext src, Camera[] cameras)
        {
            if (m_cameraUpdaterHelper != null)
            {
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                    m_cameraUpdaterHelper.hdrpEndFrameRendering(src, cameras);
                else
                    m_cameraUpdaterHelper.urpEndFrameRendering(src, cameras);
            }
        }
#else
    private void registerRenderPipeline(){}
    private void deregisterRenderPipeline(){}
#endif
#endregion
        }

}
