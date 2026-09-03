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
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This example script shows an RPC example with custom serialization.
/// </summary>
public class CustomRpcExample : MonoBehaviourWithRpc
{
    // Some custom data which we want to use as an RPC parameter.
    public class CustomData1
    {
        public Color32 color;
        public float scale;
    }

    // Serializer for the custom data parameter.
    public class CustomDataSerializer : IRPCSerializer<CustomData1>
    {
        public CustomData1 deserialize(byte[] bytes)
        {
            Debug.Assert(bytes.Length == 8);
            var res = new CustomData1();
            res.color.r = bytes[0];
            res.color.g = bytes[1];
            res.color.b = bytes[2];
            res.color.a = bytes[3];
            res.scale = BitConverter.ToSingle(bytes, 4);
            return res;
        }

        public byte[] serialize(CustomData1 obj)
        {
            var bytes = new byte[8];
            bytes[0] = obj.color.r;
            bytes[1] = obj.color.g;
            bytes[2] = obj.color.b;
            bytes[3] = obj.color.a;
            BitConverter.GetBytes(obj.scale).CopyTo(bytes, 4);
            return bytes;
        }
    }

    // Some custom data which we want to use as an RPC parameter.
    [Serializable()]
    public class CustomData2
    {
        public float scaleX;
        public float scaleY;
    }

    [Tooltip("The renderer which is changed when the RPC is called.")]
    public Renderer targetRenderer;

    private void Start()
    {
        // Only show the 2D UI on the master.
        GetComponent<Canvas>().enabled = getReal3D.Cluster.isMaster;

        // Call the onButtonClick methods when the buttons are clicked.
        transform.Find("ChangeColor1").GetComponentInChildren<Button>().onClick.
            AddListener(onButtonClick1);
        transform.Find("ChangeColor2").GetComponentInChildren<Button>().onClick.
            AddListener(onButtonClick2);
    }

    private void onButtonClick1()
    {
        // Call the ChangeColor RPC with a CustomData1 as parameter. CustomData1 is serialized
        // using an instance of CustomDataSerializer.
        var param = new CustomData1
        {
            color = UnityEngine.Random.ColorHSV(.2f, .8f),
            scale = UnityEngine.Random.Range(.5f, 1.5f)
        };
        CallRpc(nameof(ChangeColor1), param);
    }

    private void onButtonClick2()
    {
        // Call the ChangeColor RPC with a random color as parameter.
        var param = new CustomData2
        {
            scaleX = UnityEngine.Random.Range(.5f, 1.5f),
            scaleY = UnityEngine.Random.Range(.5f, 1.5f)
        };
        CallRpc(nameof(ChangeColor2), param);
    }

    // Define an RPC. We specify that in order to serialize CustomData1, CustomDataSerializer
    // should be used.
    [getReal3D.RPC]
    private void ChangeColor1([RPCSerializer(typeof(CustomDataSerializer))]
                              CustomData1 newSettings)
    {
        // Change the color of the target.
        targetRenderer.material.color = newSettings.color;
        targetRenderer.gameObject.transform.localScale = Vector3.one * newSettings.scale;
    }

    // Define an RPC. It is assumed that CustomData2 has the serialization methods defined.
    [getReal3D.RPC]
    private void ChangeColor2(CustomData2 newSettings)
    {
        // Change the color of the target.
        var newScale = new Vector3(newSettings.scaleX, newSettings.scaleY, 1);
        targetRenderer.gameObject.transform.localScale = newScale;
    }
}
