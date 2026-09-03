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
using UnityEngine.UI;

/// <summary>
/// This example script shows a basic RPC example.
/// </summary>
public class RpcExample : getReal3D.MonoBehaviourWithRpc
{
    [Tooltip("The renderer which is changed when the RPC is called.")]
    public Renderer targetRenderer;

    private void Start()
    {
        // Only show the 2D UI on the master.
        GetComponent<Canvas>().enabled = getReal3D.Cluster.isMaster;

        // Call the onButtonClick method when the button is clicked.
        GetComponentInChildren<Button>().onClick.AddListener(onButtonClick);
    }

    private void onButtonClick()
    {
        // Call the ChangeColor RPC with a random color as parameter.
        CallRpc(nameof(ChangeColor), Random.ColorHSV(.2f, .8f));
    }

    [getReal3D.RPC]
    private void ChangeColor(Color newColor)
    {
        // Change the color of the target.
        targetRenderer.material.color = newColor;
    }
}
