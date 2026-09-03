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

using getReal3D;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// This example script shows an RPC triggered by a slave node and called on the master.
/// </summary>
public class MasterRpcExample : MonoBehaviourWithRpc
{
    /// Tracks which nodes are done loading
    private bool[] DoneLoading;

    private void Start()
    {
        // Per node loading status
        DoneLoading = new bool[Plugin.NodeCount];

        // Start loading
        StartCoroutine(LoadLocalFile());
    }

    private IEnumerator LoadLocalFile()
    {
        Debug.LogError("Starts loading...");

        // Simulate a random loading time per node
        float[] PerNodeLoadingTime = new float[] { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };
        var loadingTime = PerNodeLoadingTime[Plugin.ClusterId % PerNodeLoadingTime.Length];
        yield return new WaitForSeconds(loadingTime);

        Debug.LogError("Done loading...");

        // Signal master that the node is done loading
        if (Cluster.isMaster)
        {
            LoadingDone(0);
        }
        else
        {
            CallMasterRpc(nameof(LoadingDone), Plugin.ClusterId);
        }
    }

    [getReal3D.RPC]
    private void LoadingDone(uint nodeId)
    {
        DoneLoading[nodeId] = true;
        var doneCount = DoneLoading.Count(dl => dl);
        Debug.LogError($"Node {nodeId} done loading ({doneCount}/{DoneLoading.Length}).");

        if (doneCount == DoneLoading.Length)
        {
            // All nodes are done
            Debug.LogError($"All nodes are done loading!");
        }
    }
}
