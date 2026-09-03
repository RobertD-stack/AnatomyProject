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
    /// This script allows to dynamically toggle stereo rendering.
    /// </summary>
    /// <remarks>
    /// The rendering context remains in stereo, but the eye separation is set to zero.
    /// </remarks>
    public class ToggleStereo : getReal3D.MonoBehaviourWithRpc
    {

        private bool m_stereoEnabled;
        private float m_eyeSeparationBackup;

        void Start()
        {
            m_stereoEnabled = true;
        }

        void OnGUI()
        {
            if (!getReal3D.GUI.BeginGUI()) return;
            GUILayout.BeginArea(new Rect(150, 0, 150, getReal3D.GUI.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical("Stereo", UnityEngine.GUI.skin.window);
            bool res = GUILayout.Toggle(m_stereoEnabled, "Stereo");
            if (res != m_stereoEnabled)
            {
                CallRpc("enableStereo", res);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
            getReal3D.GUI.EndGUI();
        }

        [getReal3D.RPC]
        private void enableStereo(bool enabled)
        {
            if (enabled)
            {
                getReal3D.Plugin.SetEyeSeparation(m_eyeSeparationBackup);
            }
            else
            {
                m_eyeSeparationBackup = getReal3D.Plugin.GetEyeSeparation();
                getReal3D.Plugin.SetEyeSeparation(0.0f);
            }
            m_stereoEnabled = enabled;
        }
    }
}
