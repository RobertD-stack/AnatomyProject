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

namespace getReal3D.Examples
{
    /// <summary>
    /// Example 2D GUI that can be used to toggle lights on a scene.
    /// </summary>
    public class ShadowUI
    : getReal3D.MonoBehaviourWithRpc
    {
        private Light[] m_lights;

        /// <summary>
        /// Constructor
        /// </summary>
        /// Make sure RPC are not automatically registered so that we can disable the
        /// behavior before registration.
        public ShadowUI() : base(false)
        {

        }

        void Start()
        {
            registerRpc();
            m_lights = Plugin.FindObjectsByTypeUnsorted<Light>();
        }

        void OnGUI()
        {
            if (!getReal3D.GUI.BeginGUI()) return;
            GUILayout.BeginArea(new Rect(0, 0, 150, getReal3D.GUI.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical("Lights", UnityEngine.GUI.skin.window);
            foreach (Light l in m_lights)
            {
                bool res = GUILayout.Toggle(l.enabled, l.name);
                if (res != l.enabled)
                {
                    CallRpc("enableLight", l.name, res);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
            getReal3D.GUI.EndGUI();
        }

        [getReal3D.RPC]
        void enableLight(string name, bool enabled)
        {
            foreach (Light l in m_lights)
            {
                if (l.name == name)
                {
                    l.enabled = enabled;
                }
            }
        }

        void LateUpdate()
        {
            if (getReal3D.Input.GetKeyDown(getReal3D.VirtualKeyCode.VK_ESCAPE))
                Application.Quit();
        }
    }
}
