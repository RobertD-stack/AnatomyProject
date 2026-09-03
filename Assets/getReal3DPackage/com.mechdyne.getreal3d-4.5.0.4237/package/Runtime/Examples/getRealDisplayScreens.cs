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
    /// This script shows how to retrieve the getReal3D screens coordinates.
    /// </summary>
    public class getRealDisplayScreens : MonoBehaviour
    {

        [Tooltip("Color of the lines.")]
        public Color color = Color.white;

        [Tooltip("If depth test is enabled when drawing the screen.")]
        public bool depthTest = true;

        void Update()
        {
            int nodeCount = getReal3D.Plugin.getNodeCount();
            for (int i = 0; i < nodeCount; ++i)
            {
                int instanceCount = getReal3D.Plugin.getNodeConfigCount(i);
                for (int j = 0; j < instanceCount; ++j)
                {
                    int screenCount = getReal3D.Plugin.getScreenCount(i, j);
                    for (int k = 0; k < screenCount; ++k)
                    {
                        Vector3[] corners = getReal3D.Plugin.getScreenCoordinates((uint)i, (uint)j, (uint)k);
                        Debug.DrawLine(transform.TransformPoint(corners[0]), transform.TransformPoint(corners[1]), color, 0.0f, depthTest);
                        Debug.DrawLine(transform.TransformPoint(corners[1]), transform.TransformPoint(corners[2]), color, 0.0f, depthTest);
                        Debug.DrawLine(transform.TransformPoint(corners[2]), transform.TransformPoint(corners[3]), color, 0.0f, depthTest);
                        Debug.DrawLine(transform.TransformPoint(corners[3]), transform.TransformPoint(corners[0]), color, 0.0f, depthTest);
                    }
                }
            }
        }
    }
}
