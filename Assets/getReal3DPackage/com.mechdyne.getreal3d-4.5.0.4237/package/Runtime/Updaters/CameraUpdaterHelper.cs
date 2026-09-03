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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace getReal3D
{

    /// <summary>
    /// Structure used to store preview camera matrix.
    /// </summary>
    [System.Serializable]
    public struct CameraPreviewData
    {
        /// <summary>
        /// True if the projection matrix is valid.
        /// </summary>
        public bool hasProjMatrix;
        /// <summary>
        /// Projection matrix of the camera.
        /// </summary>
        public Matrix4x4 projMatrix;
    }

    /// <summary>
    /// Interface that a camera updater script must implement in order to use the
    /// CameraUpdaterHelper class.
    /// </summary>
    public interface CameraUpdaterInterface
    {

        /// <summary>
        /// Return the CameraUpdaterHelper instance.
        /// </summary>
        CameraUpdaterHelper updater
        {
            get;
            set;
        }

        /// <summary>
        /// Return the behaviour this script is attached to.
        /// </summary>
        MonoBehaviour behaviour
        {
            get;
        }

        /// <summary>
        /// Return the camera preview data.
        /// </summary>
        CameraPreviewData cameraPreviewData
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Helper class used to create and update getReal3D cameras.
    /// </summary>
    public class CameraUpdaterHelper
    {
        private Camera m_camera;
        private int m_cameraIndex;
        private uint m_userId;
        private bool m_useRenderToTexture;
        private bool m_OnRenderImageCalled = false;
        private uint m_PostRenderCallCount = 0;
        private bool m_PostRenderCallMissingErrorIssued = false;
        private CameraUpdaterInterface m_cameraInterface;
        private static List<CameraUpdaterInterface> s_cameraUpdaters = null;
        private enum PipelineType
        {
            HDRP, URP, BuiltIn, Unknown
        };
        private PipelineType m_pipelineType = PipelineType.Unknown;
        private bool Invalid { get { return m_cameraIndex < 0; } }

        /// <summary>
        /// Eye enumeration.
        /// </summary>
        public enum Eye
        {
            /// <summary>
            /// Left eye
            /// </summary>
            Left,

            /// <summary>
            /// Right eye
            /// </summary>
            Right
        };

        /// <summary>
        /// The eye this camera is rendering.
        /// </summary>
        public Eye eye
        {
            get;
            private set;
        }

        /// <summary>
        /// Duplicate the camera as needed for rendering of the given user.
        /// </summary>
        /// <param name="baseCamera">Base camera to duplicate.</param>
        /// <param name="userId">User ID of the camera</param>
        public static void CreateCamerasForUserIfNeeded(Camera baseCamera, uint userId)
        {
            if (!Application.isPlaying)
            {
                var cameraUpdaterInterface = baseCamera.GetComponent<CameraUpdaterInterface>();
                if (cameraUpdaterInterface != null)
                {
                    cameraUpdaterInterface.updater = new CameraUpdaterHelper(baseCamera, 0, 0);
                }
                return;
            }

            if (userHandled(userId))
            {
                return;
            }

            List<int> needCameras = ComputeNeededCameras(userId);

            if (s_cameraUpdaters == null)
            {
                s_cameraUpdaters = new List<CameraUpdaterInterface>();
            }

            Plugin.debug("User " + userId.ToString() + " requires cameras: {" +
                string.Join(", ", needCameras.ConvertAll(i => i.ToString()).ToArray()));

            if (needCameras.Count == 0)
            {
                var cameraUpdater = baseCamera.GetComponent<CameraUpdaterInterface>();
                if (cameraUpdater != null)
                {
                    cameraUpdater.updater = new CameraUpdaterHelper(baseCamera, -1, userId);
                    s_cameraUpdaters.Add(cameraUpdater);
                }
                baseCamera.gameObject.SetActive(false);
                return;
            }

            var thisUserCameras = new List<CameraUpdaterInterface>();

            // find cameras, see which we can remove from needCameras
            foreach (Camera cam in baseCamera.transform.parent.GetComponentsInChildren<Camera>())
            {
                if (cam.GetComponent<CameraUpdaterInterface>() != null
                    && (cam.name == baseCamera.name || cam.name == baseCamera.name + "(Clone)"))
                {
                    if (needCameras.Count == 0)
                    {
                        Plugin.error("Too many camera are already spawned!");
                    }
                    else
                    {
                        var cameraInterface = cam.GetComponent<CameraUpdaterInterface>();
                        cameraInterface.updater = new CameraUpdaterHelper(cam, needCameras[0], userId);
                        s_cameraUpdaters.Add(cameraInterface);
                        thisUserCameras.Add(cameraInterface);
                        needCameras.RemoveAt(0);
                    }
                }
            }

            // make missing cameras
            foreach (int idx in needCameras)
            {

                Plugin.debug("Creating camera " + idx.ToString() + ".");

                GameObject newCamObject = MonoBehaviour.Instantiate(baseCamera.gameObject, baseCamera.transform.parent);

                foreach (AudioListener listener in newCamObject.GetComponents<AudioListener>())
                {
                    MonoBehaviour.Destroy(listener);
                }

                newCamObject.tag = baseCamera.gameObject.tag;
                newCamObject.layer = baseCamera.gameObject.layer;

                newCamObject.GetComponent<Camera>().CopyFrom(baseCamera);
                newCamObject.GetComponent<Camera>().targetTexture = null;

                var camUpdater = newCamObject.GetComponent<CameraUpdaterInterface>();
                camUpdater.updater =
                    new CameraUpdaterHelper(newCamObject.GetComponent<Camera>(), idx, userId);
                s_cameraUpdaters.Add(camUpdater);
                thisUserCameras.Add(camUpdater);
            }

            foreach (var cameraUpdater in thisUserCameras)
            {
                cameraUpdater.updater.Setup();
            }

            Plugin.debug("Camera creation done.");
        }

        /// <summary>
        /// Resets the handled users. This can be used if the corresponding cameras have been
        /// deleted and needs to be recreated later.
        /// </summary>
        public static void ResetHandledUsers()
        {
            s_cameraUpdaters = null;
        }

        private static bool userHandled(uint userId)
        {
            return
                s_cameraUpdaters != null &&
                s_cameraUpdaters.Any(c => c.updater.m_userId == userId);
        }

        private CameraUpdaterHelper(Camera camera, int cameraIndex, uint userId)
        {
            m_camera = camera;
            m_cameraInterface = camera.GetComponent<CameraUpdaterInterface>();
            m_cameraIndex = cameraIndex;
            m_userId = userId;
        }

        private void Setup()
        {
            Plugin.debug("Setup for camera " + m_cameraIndex + ".");
            m_camera.enabled = true;

            Rect viewport = new Rect(0f, 0f, 1f, 1f);
            Plugin.getCameraViewport((uint)m_cameraIndex, ref viewport);

            Plugin.debug("Viewport: " + viewport.ToString());

            m_camera.rect = viewport;
            if (Config.renderingPathSet)
            {
                Plugin.debug("Set rendering path to: " + Config.renderingPath.ToString());
                m_camera.renderingPath = Config.renderingPath;
            }


            m_useRenderToTexture = Plugin.getCameraUseRTT((uint)m_cameraIndex);
            if (m_useRenderToTexture)
            {
                Plugin.debug("Setup RTT.");
                EnsureRenderToTexture();
            }
            else
            {
                Plugin.debug("Rendering directly to front buffer.");
            }
            if (m_camera.stereoEnabled)
            {
                Plugin.debug("Camera has stereo enabled.");
                switch (Plugin.getScreenBuffer((uint)m_cameraIndex))
                {
                    case 0:
                        Plugin.debug("Setting camera to render to left buffer.");
                        m_camera.stereoTargetEye = StereoTargetEyeMask.Left;
                        break;
                    case 1:
                        Plugin.debug("Setting camera to render to right buffer.");
                        m_camera.stereoTargetEye = StereoTargetEyeMask.Right;
                        break;
                }

            }

            int buffer = Plugin.getScreenBuffer((uint)m_cameraIndex);
            eye = buffer == 1 ? Eye.Right : Eye.Left;
            Plugin.debug("Rendering eye " + eye.ToString());
        }

        private static List<int> ComputeNeededCameras(uint userId)
        {
            List<int> needCameras = new List<int>();
            for (int i = 0; i < Input.cameras.Count; ++i)
            {
                if (Plugin.getUserFromCamera((uint)i) == userId)
                {
                    needCameras.Add(i);
                }
            }
            return needCameras;
        }

        private void EnsureRenderToTexture()
        {
            if (Invalid)
            {
                return;
            }
            int w = Plugin.getCameraWidth((uint)m_cameraIndex);
            int h = Plugin.getCameraHeight((uint)m_cameraIndex);
            int textureAntiAliasing = QualitySettings.antiAliasing == 0 ? 1 :
                QualitySettings.antiAliasing;
            if (!m_camera.targetTexture ||
                m_camera.targetTexture.width != w ||
                m_camera.targetTexture.height != h ||
                m_camera.targetTexture.antiAliasing != textureAntiAliasing)
            {
                try
                {
                    Plugin.debug("Creating new RenderTexture of size " +
                        w.ToString() + "x" + h.ToString() + " AA: " + textureAntiAliasing
                        + ".");
                    var rtt = new RenderTexture(w, h, 24);
                    rtt.antiAliasing = textureAntiAliasing;
                    m_camera.targetTexture = rtt;
                }
                catch (System.Exception e)
                {
                    Plugin.error("Error while creating render to texture. " + e);
                }
            }
        }

        /// <summary>
        /// Must be called by the owning script during the OnPreCull callback.
        /// </summary>
        public void PreCull()
        {
            checkPipeline(PipelineType.BuiltIn);
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            if (Invalid)
            {
                return;
            }
            if (!Application.isPlaying)
            {

                if (m_cameraInterface.cameraPreviewData.hasProjMatrix)
                {
                    m_camera.projectionMatrix = m_cameraInterface.cameraPreviewData.projMatrix;
                }
                else
                {
                    m_camera.ResetProjectionMatrix();
                }

                return;
            }
            if (m_useRenderToTexture)
            {
                EnsureRenderToTexture();
            }

            m_camera.transform.localPosition = Input.GetCameraSensor((uint)m_cameraIndex).position;
            m_camera.transform.localRotation = Input.GetCameraSensor((uint)m_cameraIndex).rotation;
            var matrix = Input.GetCameraProjection((uint)m_cameraIndex,
                m_camera.farClipPlane, m_camera.nearClipPlane);
            m_camera.projectionMatrix = matrix;
            m_camera.stereoSeparation = 0;
            m_camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matrix);
            m_camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, matrix);
        }

        /// <summary>
        /// Must be called by the owning script during the OnPreRender callback.
        /// </summary>
        public void PreRender()
        {
            checkPipeline(PipelineType.BuiltIn);
            if (Invalid)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            GL.IssuePluginEvent(Plugin.getCameraProfilingCallback(), m_cameraIndex);
        }

        private void IssuePostRenderEvent()
        {
            if (m_camera.targetTexture)
            {
                Plugin.setTextureFromUnity(m_cameraIndex,
                    m_camera.targetTexture.GetNativeTexturePtr());
                GL.IssuePluginEvent(Plugin.getCameraEventCallback(), m_cameraIndex);
                m_cameraInterface.behaviour.StartCoroutine(CallPluginAtEndOfFrame(true));
            }
            else
            {
                m_cameraInterface.behaviour.StartCoroutine(CallPluginAtEndOfFrame(false));
            }
        }

        /// <summary>
        /// Must be called by the owning script during the OnPostRender callback.
        /// </summary>
        public void PostRender()
        {
            checkPipeline(PipelineType.BuiltIn);
            if (Invalid)
            {
                return;
            }
            if (!Application.isPlaying)
            {
                return;
            }
            ++m_PostRenderCallCount;
            if (m_PostRenderCallCount >= 2 && !m_OnRenderImageCalled)
            {
                if (!m_PostRenderCallMissingErrorIssued)
                {
                    m_PostRenderCallMissingErrorIssued = true;
                    Plugin.error("Detected that CameraUpdater doesn't call OnRenderImage.");
                }
                IssuePostRenderEvent();
            }
        }

        /// <summary>
        /// Must be called by the owning script during the OnRenderImage callback.
        /// </summary>
        public void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            checkPipeline(PipelineType.BuiltIn);
            if (Invalid)
            {
                return;
            }
            Graphics.Blit(src, dst);
            if (!Application.isPlaying)
            {
                return;
            }
            m_OnRenderImageCalled = true;
            IssuePostRenderEvent();
        }

        /// <summary>
        /// Must be called by owning script during the OnDestroy callback.
        /// </summary>
        public void Destroyed()
        {

        }

        /// <summary>
        /// Must be called by owning script during the OnDisabled callback.
        /// </summary>
        public void Disabled()
        {
            if (s_cameraUpdaters == null)
            {
                return;
            }
            s_cameraUpdaters.Remove(m_cameraInterface);
        }

        private IEnumerator CallPluginAtEndOfFrame(bool eofOnly)
        {
            yield return new WaitForEndOfFrame();
            if (!eofOnly)
            {
                GL.IssuePluginEvent(Plugin.getCameraEventCallback(), m_cameraIndex);
            }
            GL.IssuePluginEvent(Plugin.getEndOfFrameEventFunc(), m_cameraIndex);
        }

        /// HDRP callback
        public void hdrpBeginCameraRendering(object src)
        {
            checkPipeline(PipelineType.HDRP);
            UpdateCamera();
        }

        /// HDRP callback
        public void hdrpBeginFrameRendering(object src, Camera[] cameras)
        {
            checkPipeline(PipelineType.HDRP);
        }

        /// HDRP callback
        public void hdrpEndCameraRendering(object src)
        {
            checkPipeline(PipelineType.HDRP);
            if (m_camera.targetTexture)
            {
                Plugin.setTextureFromUnity(m_cameraIndex,
                    m_camera.targetTexture.GetNativeTexturePtr());

            }
            GL.IssuePluginEvent(Plugin.getCameraEventCallback(), m_cameraIndex);
        }

        /// HDRP callback
        public void hdrpEndFrameRendering(object src, Camera[] cameras)
        {
            checkPipeline(PipelineType.HDRP);
            if (System.Array.IndexOf(cameras, m_camera) >= 0)
            {
                GL.IssuePluginEvent(Plugin.getEndOfFrameEventFunc(), m_cameraIndex);
            }
        }

        /// URP callback
        public void urpBeginCameraRendering(object src)
        {
            checkPipeline(PipelineType.URP);
            UpdateCamera();
        }

        /// URP callback
        public void urpBeginFrameRendering(object src, Camera[] cameras)
        {
            checkPipeline(PipelineType.URP);
        }

        /// URP callback
        public void urpEndCameraRendering(object src)
        {
            checkPipeline(PipelineType.URP);
            if (m_camera.targetTexture)
            {
                Plugin.setTextureFromUnity(m_cameraIndex,
                    m_camera.targetTexture.GetNativeTexturePtr());
            }
            GL.IssuePluginEvent(Plugin.getCameraEventCallback(), m_cameraIndex);
        }

        /// URP callback
        public void urpEndFrameRendering(object src, Camera[] cameras)
        {
            checkPipeline(PipelineType.URP);
            if (System.Array.IndexOf(cameras, m_camera) >= 0)
            {
                GL.IssuePluginEvent(Plugin.getEndOfFrameEventFunc(), m_cameraIndex);
            }
        }

        private void checkPipeline(PipelineType pipelineType)
        {
            if (m_pipelineType != PipelineType.Unknown &&
               m_pipelineType != pipelineType)
            {
                Debug.LogError("Pipeline type changed from " + m_pipelineType.ToString() +
                    " to " + pipelineType.ToString());
            }
            m_pipelineType = pipelineType;
        }
    }
}
