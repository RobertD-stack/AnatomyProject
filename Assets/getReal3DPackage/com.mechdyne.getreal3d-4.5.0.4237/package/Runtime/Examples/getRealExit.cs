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

    /// <summary>
    /// This script is used to exit getReal3D upon a key press.
    /// </summary>
    public class getRealExit : MonoBehaviour
    {

        [Tooltip("Key used to exit.")]
        public getReal3D.VirtualKeyCode m_exitKey = getReal3D.VirtualKeyCode.VK_ESCAPE;

        void Update()
        {
            if (getReal3D.Input.GetKeyDown(m_exitKey))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            getReal3D.Plugin.clusterShutdown();
#endif
            }
        }
    }
}
