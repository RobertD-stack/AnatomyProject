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
    /// This script updates the attached game object transform to match a screen position, rotation and
    /// size.
    /// </summary>
    public class getRealScreenUpdater : MonoBehaviour
    {
        private string m_screenName;
        private bool m_screenDirty = true;
        private Vector3[] m_corners = new Vector3[4];

        /// Update the screen at each frame. Might be needed if the screen is in head space.
        public bool alwaysUpdate = false;

        /// <summary>
        /// Set the screen name. This is how a screen is identified within the display system.
        /// </summary>
        public string screenName
        {
            get
            {
                return m_screenName;
            }
            set
            {
                m_screenName = value;
                m_screenDirty = true;
            }
        }

        private void Update()
        {
            if (m_screenDirty || alwaysUpdate)
            {
                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            getReal3D.Plugin.getScreenCoordinatesByName(m_screenName, m_corners);
            var center = 0.5f * (m_corners[0] + m_corners[2]);
            var u = m_corners[1] - m_corners[0];
            var v = m_corners[3] - m_corners[0];
            var n = Vector3.Cross(u, v);

            transform.localPosition = center;
            transform.localScale = new Vector3(u.magnitude, v.magnitude, 1);
            transform.localRotation = Quaternion.LookRotation(n, v);

            m_screenDirty = false;
        }
    }
}
