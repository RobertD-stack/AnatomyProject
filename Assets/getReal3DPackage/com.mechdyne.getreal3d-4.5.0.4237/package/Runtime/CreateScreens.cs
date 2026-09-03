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

namespace getReal3D
{
    /// <summary>
    /// This script creates the given prefab object for each screen of the display system. If there is
    /// no prefab, then an empty game object is created instead. Upon creation, a getRealScreenUpdater
    /// behaviour is attached to the new object in order to set the correct screen transform. The
    /// screens are created when the OnEnable event is received, and destroyed in the OnDestroy event.
    /// Alternatively, the Create and DestroyScreens functions can be used.
    /// </summary>
    public class CreateScreens : MonoBehaviour
    {
        /// <summary>
        /// This screen prefab is spawned for every screen of the display system.
        /// </summary>
        public GameObject m_screenPrefab;

        /// <summary>
        /// Retrieve all screens game objects that have been created.
        /// </summary>
        public List<GameObject> screens
        {
            get { return m_screens; }
        }

        private List<GameObject> m_screens = new List<GameObject>();

        private void OnEnable()
        {
            Create();
        }

        /// <summary>
        /// Creates the screens objects.
        /// </summary>
        public void Create()
        {
            DestroyScreens();
            int screenCount = getReal3D.Plugin.getConfigScreenCount();
            for (int i = 0; i < screenCount; ++i)
            {
                var screenName = getReal3D.Plugin.getScreenName(i);
                var screenSpace = getReal3D.Plugin.getConfigScreenSpace(i);
                GameObject screen = null;
                if (m_screenPrefab != null)
                {
                    screen = Instantiate(m_screenPrefab);
                }
                else
                {
                    screen = new GameObject();
                }
                screen.name = screenName;
                var screenUpdate = screen.GetComponent<getRealScreenUpdater>();
                if (screenUpdate == null)
                {
                    screenUpdate = screen.AddComponent<getRealScreenUpdater>();
                }
                screenUpdate.screenName = screenName;
                screenUpdate.alwaysUpdate = screenSpace == getReal3D.ScreenSpace.HeadSpace;
                screen.transform.parent = transform;
                screen.transform.localPosition = Vector3.zero;
                screen.transform.localRotation = Quaternion.identity;

                m_screens.Add(screen);
            }
        }

        private void OnDestroy()
        {
            DestroyScreens();
        }

        private void DestroyScreens()
        {
            foreach (var go in m_screens)
            {
                Destroy(go);
            }
            m_screens.Clear();
        }
    }
}
