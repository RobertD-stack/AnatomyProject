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
using UnityEngine;

/// <summary>
/// This script shows how a whole scene could be resized within
/// a getReal3D project.
/// </summary>
[RequireComponent(typeof(PlayerInputsProxy))]
public class SceneScaleExample : MonoBehaviour
{
    private float m_scaleStep = 1.02f;
    private PlayerInputsProxy m_playerInputProxy;

    private void Awake()
    {
        m_playerInputProxy = GetComponent<PlayerInputsProxy>();
    }

    void Update()
    {
        if (m_playerInputProxy.WandButtonDown)
        {
            transform.localScale = transform.localScale * m_scaleStep;
        }
        else if (m_playerInputProxy.ChangeWandButtonDown)
        {
            transform.localScale = transform.localScale / m_scaleStep;
        }
    }
}
