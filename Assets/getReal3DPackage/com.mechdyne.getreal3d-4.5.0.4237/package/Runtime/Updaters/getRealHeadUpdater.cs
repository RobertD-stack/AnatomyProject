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
    /// This class is deprecated. GenericHeadUpdater should be used instead.
    public class getRealHeadUpdater
    : DeprecatedGetRealUserScript
    {
        private Transform m_transform;

        void Awake()
        {
            m_transform = transform;
        }

        void Update()
        {
            getReal3D.Sensor headSensor = getHead();
            m_transform.localPosition = headSensor.position;
            m_transform.localRotation = headSensor.rotation;
        }

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(0.1f, 0.2f, 0.1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(0, 0, 0), 1);
        }
    }
}
