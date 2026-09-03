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
using UnityEngine.UI;

namespace getReal3D
{
    /// <summary>
    /// LightsMenuUI is used to build the Lights menu.
    /// </summary>
    public class LightsMenuUI : MonoBehaviour
    {
        [Tooltip("Light toggle UI prefab")]
        public GameObject m_lightTogglePrefab;

        [Tooltip("If set, the colors of the menu will be updated according to the color settings")]
        public MenuColors m_changeColors;

        private Light[] m_lights;
        private Toggle[] m_lightsToggle;

        void Start()
        {
            m_lights = Plugin.FindObjectsByTypeUnsorted<Light>();
            m_lightsToggle = new Toggle[m_lights.Length];
            for (int i = 0; i < m_lights.Length; ++i)
            {
                Light light = m_lights[i];
                GameObject lightToggle = GameObject.Instantiate(m_lightTogglePrefab) as GameObject;
                lightToggle.transform.SetParent(transform, false);
                Text label = lightToggle.GetComponentInChildren<Text>() as Text;
                label.text = light.name;
                m_lightsToggle[i] = lightToggle.GetComponent<Toggle>() as Toggle;
                UnityEngine.Events.UnityAction<bool> action = delegate (bool e)
                {
                    light.enabled = e;
                };
                m_lightsToggle[i].onValueChanged.AddListener(action);
            }
            if (m_changeColors)
            {
                m_changeColors.handleGameObject(gameObject);
            }
        }

        void Update()
        {
            for (int i = 0; i < m_lights.Length; ++i)
            {
                if (m_lights[i].enabled != m_lightsToggle[i].isOn)
                {
                    m_lightsToggle[i].isOn = m_lights[i].enabled;
                }
            }
        }

    }
}
