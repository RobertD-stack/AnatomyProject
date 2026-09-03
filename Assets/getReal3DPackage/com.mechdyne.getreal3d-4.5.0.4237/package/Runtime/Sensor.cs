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
    /// A representation similar to the UnityEngine.Transform used by trackd sensors in getReal3D for Unity.
    /// Consists of a position, rotation, and fixed scale.
    /// </summary>
    public struct Sensor
    {
        /// <summary>
        /// The sensor scale. Always Vector3.one
        /// </summary>
        public Vector3 scale { get { return Vector3.one; } }
        /// <summary>
        /// The sensor position in Unity units.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// The sensor orientation.
        /// </summary>
        public Quaternion rotation;
    }
}
