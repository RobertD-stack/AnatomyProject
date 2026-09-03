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
    /// getRealUserScript is an helper script that can be used to retrieve information about the
    /// getRealUser this script is attached to.
    /// </summary>
    public class getRealUserScript : MonoBehaviour
    {

        private getRealUser m_getRealUser = null;

        /// Get the user index
        public uint userId()
        {
            findGetRealUser();
            return m_getRealUser == null ? 0 : m_getRealUser.userId;
        }

        /// Get the getRealUser component
        public getRealUser user()
        {
            findGetRealUser();
            return m_getRealUser;
        }

        /// Get the head sensor of the user
        public Sensor getHead()
        {
            return getReal3D.Input.headForUser(userId());
        }

        /// Get the wand sensor of the user
        public Sensor getWand()
        {
            return getReal3D.Input.wandForUser(userId());
        }

        private void findGetRealUser()
        {
            if (m_getRealUser) { return; }
            m_getRealUser = GetComponentInParent<getRealUser>();
            if (!m_getRealUser)
            {
                getRealUser anyUser = Plugin.FindAnyObjectByType<getRealUser>();
                if (anyUser)
                {
                    getReal3D.Plugin.error("Unable to find a getRealUser in any of my parents but" +
                        " there are users in the scene. If you are building a multi users game," +
                        " please add a getRealUser script to all users.");
                    m_getRealUser = anyUser;
                }
                else
                {
                    m_getRealUser = transform.root.gameObject.AddComponent<getRealUser>();
                }
            }
        }
    }
}
