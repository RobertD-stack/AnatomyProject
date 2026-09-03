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

using System.IO;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// A utility class for getReal3D Mechdyne-Cluster.
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Is this instance the master instance in a getReal3D for Unity Mechdyne-Cluster
        /// </summary>
        static public bool isMaster
        {
            get { return Plugin.ClusterId == 0; }
        }

        /// <summary>
        /// Is the ClusterManager needed in a getReal3D for Unity Mechdyne-Cluster
        /// </summary>
        static public bool isOn
        {
            get { return true; }
        }

        /// <summary>
        /// Is this instance a client in a getReal3D for Unity Mechdyne-Cluster
        /// </summary>
        static public bool isClientAndClusterOn
        {
            get { return Plugin.ClusterId != 0; }
        }

        /// <summary>
        /// UnityEngine.Time.time from the master instance in a getReal3D for Unity Mechdyne-Cluster
        /// </summary>
        static public float time
        {
            get { return Time.time; }
        }

        /// <summary>
        /// UnityEngine.Time.deltaTime from the master instance in a getReal3D for Unity Mechdyne-Cluster
        /// </summary>
        static public float deltaTime
        {
            get { return Time.deltaTime; }
        }

        /// <summary>
        /// Same as getReal3D.Plugin.getFrameCount()
        /// The getReal3D cluster frame count; may not be the same as UnityEngine.Time.frameCount
        /// </summary>
        static public int frameCount
        {
            get { return Time.frameCount; }
        }

    }


}
