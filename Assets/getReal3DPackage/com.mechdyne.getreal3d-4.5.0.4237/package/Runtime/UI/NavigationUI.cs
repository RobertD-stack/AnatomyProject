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

using System;
using UnityEngine;

namespace getReal3D
{
    /// Handles the navigation page UI of the getReal3D menu.
    public class NavigationUI : MonoBehaviourWithRpc
    {
        /// The Slider for changing translation speed
        public UnityEngine.UI.Slider m_translationSpeedSlider = null;

        /// The Slider for changing rotation speed
        public UnityEngine.UI.Slider m_rotationSpeedSlider = null;

        /// The player on which to change those navigation settings
        public GameObject player = null;

        void Start()
        {
            MenuSettings ms = GetComponentInParent<MenuSettings>() as MenuSettings;
            if (ms)
            {
                if (ms.maxRotationSpeed.HasValue)
                {
                    m_rotationSpeedSlider.maxValue = ms.maxRotationSpeed.Value;
                }
                if (ms.maxTranslationSpeed.HasValue)
                {
                    m_translationSpeedSlider.maxValue = ms.maxTranslationSpeed.Value;
                }
            }

            retrieveFromNavOptions();
        }

        /// Called from UI when rotation slider changes
        public void setRotationSpeed(float val)
        {
            if (Cluster.isMaster)
            {
                CallRpc(nameof(setRotationSpeedRpc), val);
            }
        }


        [getReal3D.RPC]
        private void setRotationSpeedRpc(float val)
        {
            Plugin.debug($"Setting rotation speed to {val}");
            Input.NavOptions.RotationSpeed = val;
        }

        /// Called from UI when translation slider changes
        public void setTranslationSpeed(float val)
        {
            if (Cluster.isMaster)
            {
                CallRpc(nameof(setTranslationSpeedRpc), val);
            }
        }

        [getReal3D.RPC]
        private void setTranslationSpeedRpc(float val)
        {
            Plugin.debug($"Setting translation speed to {val}");
            Input.NavOptions.TranslationSpeed = val;
        }

        /// Called from UI when navigation method changes
        public void changeNavigation(int val)
        {
            var navigationScriptInterface = player.GetComponent<NavigationScriptInterface>();
            if (navigationScriptInterface != null)
            {
                if (Enum.IsDefined(typeof(NavigationHelper.NavigationMethod), val))
                {
                    Plugin.debug($"Changed navigation to {(NavigationHelper.NavigationMethod)val}");
                    navigationScriptInterface.navigationHelper.m_navigationMethod =
                        (NavigationHelper.NavigationMethod)val;
                    return;
                }
            }
        }

        private void retrieveFromNavOptions()
        {
            setSliderValue(m_rotationSpeedSlider, Input.NavOptions.RotationSpeed);
            setSliderValue(m_translationSpeedSlider, Input.NavOptions.TranslationSpeed);
        }

        private void setSliderValue(UnityEngine.UI.Slider slider, float val)
        {
            if (val < slider.minValue)
            {
                return;
            }
            if (val > slider.maxValue)
            {
                slider.maxValue = 2 * val;
            }
            if (val != slider.value)
            {
                slider.value = val;
            }
        }

    }
}
