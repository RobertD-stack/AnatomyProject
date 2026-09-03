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
    /// Updates the head position and rotation via the information retrieved from the
    /// attached PlayerInputs interface.
    /// </summary>
    [AddComponentMenu("getReal3D/Generic Head Updater")]
    [RequireComponent(typeof(PlayerInputs))]
    public class GenericHeadUpdater : MonoBehaviour
    {
        private PlayerInputs m_playerInputs;
        private PlayerInputs playerInputs
        {
            get
            {
                if (m_playerInputs == null)
                {
                    m_playerInputs = GetComponent<PlayerInputs>();
                }
                return m_playerInputs;
            }
        }

        /// <summary>
        /// The target object to update.
        /// </summary>
        [Tooltip("Target object to update.")]
        public Transform target;

        void Awake()
        {
            if (!target)
            {
                target = transform;
            }
        }

        void Update()
        {
            getReal3D.Sensor headSensor = playerInputs.Head;
            target.localPosition = headSensor.position;
            target.localRotation = headSensor.rotation;
        }
    }
}
