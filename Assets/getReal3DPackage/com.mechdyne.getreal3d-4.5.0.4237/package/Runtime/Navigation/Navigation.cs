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
    /// Script used to navigate (internally use NavigationHelper class).
    /// </summary>
    [AddComponentMenu("getReal3D/Generic Navigation")]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterMotorC))]
    [RequireComponent(typeof(PlayerInputs))]
    public class Navigation : MonoBehaviour, NavigationScriptInterface
    {
        /// The navigation helper used by this script. Keep it public so NavigationHelperEditor can
        /// access it.
        public NavigationHelper m_navigationHelper = new NavigationHelper();
        NavigationHelper NavigationScriptInterface.navigationHelper { get { return m_navigationHelper; } }

        void Awake()
        {
            m_navigationHelper.playerInputs = GetComponent<PlayerInputs>();
        }

        void Start()
        {
            m_navigationHelper.Start();
        }

        void Update()
        {
            m_navigationHelper.Update(transform);
        }

        void FixedUpdate()
        {
            m_navigationHelper.FixedUpdate(transform);
        }
    }
}
