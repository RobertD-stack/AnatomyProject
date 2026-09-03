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

using UnityEngine;

namespace getReal3D
{
    /// This script is used to build the rendering settings menu page.
    public class RenderingSettings : MonoBehaviour
    {
        [Tooltip("Slider for changing the rendering path")]
        public UnityEngine.UI.Slider m_renderingPathSlider;

        [Tooltip("Slider for changing the rendering quality")]
        public UnityEngine.UI.Slider m_qualitySlider;

        [Tooltip("Text for showing the current quality name")]
        public UnityEngine.UI.Text m_qualityName;

        [Tooltip("FPS text")]
        public UnityEngine.UI.Text m_fps;

        [Tooltip("Text for showing the current rendering path name")]
        public UnityEngine.UI.Text m_renderingPathName;

        private GameObject[] m_camerasObjects;

        void Start()
        {
            m_camerasObjects = GameObject.FindGameObjectsWithTag("MainCamera");

            m_qualitySlider.minValue = 0;
            m_qualitySlider.maxValue = QualitySettings.names.Length - 1;
            m_qualitySlider.wholeNumbers = true;
            m_qualitySlider.value = QualitySettings.GetQualityLevel();

            m_renderingPathSlider.minValue = 0;
            m_renderingPathSlider.maxValue = System.Enum.GetNames(typeof(RenderingPath)).Length - 1;
            m_renderingPathSlider.wholeNumbers = true;
            string[] renderingPathNames = System.Enum.GetNames(typeof(RenderingPath));
            RenderingPath currentRenderingPath = getRenderingPath();
            m_renderingPathSlider.value = System.Array.IndexOf(renderingPathNames, currentRenderingPath.ToString());

            updateQualityName();
            updateRenderingPathName();
        }

        void Update()
        {
            m_fps.text = (1.0f / Time.smoothDeltaTime).ToString("##0.00");
        }

        /// Called from UI when quality slider has changed
        public void qualitySliderChanged(float val)
        {
            int qualityLevel = (int)val;
            int currentQualityLevel = QualitySettings.GetQualityLevel();
            if (qualityLevel != currentQualityLevel)
            {
                QualitySettings.SetQualityLevel(qualityLevel, false);
            }
            updateQualityName();
        }

        /// Called from UI when rendering path slider has changed
        public void renderingPathSliderChanged(float val)
        {
            string[] renderingPathNames = System.Enum.GetNames(typeof(RenderingPath));
            string renderingPathName = renderingPathNames[(int)val];
            RenderingPath rp = (RenderingPath)System.Enum.Parse(typeof(RenderingPath), renderingPathName);
            foreach (GameObject cam in m_camerasObjects)
            {
                cam.GetComponent<Camera>().renderingPath = rp;
            }
            updateRenderingPathName();
        }

        private void updateQualityName()
        {
            m_qualityName.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        }

        private void updateRenderingPathName()
        {
            m_renderingPathName.text = getRenderingPath().ToString();
        }

        private RenderingPath getRenderingPath()
        {
            if (m_camerasObjects.Length == 0)
            {
#if UNITY_2022_1_OR_NEWER
                return RenderingPath.Forward;
#else
                return RenderingPath.DeferredLighting;
#endif
            }
            else
            {
                return m_camerasObjects[0].GetComponent<Camera>().renderingPath;
            }
        }

        private float m_eyeSeparationBackup = 0.0f;

        /// Called from UI when stereo is toggled
        public void stereoToggle(bool stereoEnabled)
        {
            if (stereoEnabled)
            {
                getReal3D.Plugin.SetEyeSeparation(m_eyeSeparationBackup);
            }
            else
            {
                m_eyeSeparationBackup = getReal3D.Plugin.GetEyeSeparation();
                getReal3D.Plugin.SetEyeSeparation(0.0f);
            }
        }
    }
}
