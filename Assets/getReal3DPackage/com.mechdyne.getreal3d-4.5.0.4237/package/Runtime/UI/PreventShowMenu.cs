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
    /// This script, when attached to an object with a canvas prevents the GenericShowMenu to
    /// pop up the getReal3D menu when the wand is pointing at the canvas.
    /// </summary>
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Canvas))]
    public class PreventShowMenu : MonoBehaviour, PreventShowMenuCallback
    {
        private Canvas m_canvas;
        private RectTransform m_rectTransform;

        void Start()
        {
            m_canvas = GetComponent<Canvas>();
            m_rectTransform = gameObject.transform as RectTransform;
            var genericShowMenu = Plugin.FindObjectsByTypeUnsorted<GenericShowMenu>();
            foreach (var gsm in genericShowMenu)
            {
                gsm.Register(this);
            }
        }

        void OnDestroy()
        {
            var genericShowMenu = Plugin.FindObjectsByTypeUnsorted<GenericShowMenu>();
            foreach (var gsm in genericShowMenu)
            {
                gsm.Deregister(this);
            }
        }

        public bool preventsShowMenu(Transform wandObject)
        {
            return RectTransformUtility.RectangleContainsScreenPoint
                (m_rectTransform, m_canvas.worldCamera.pixelRect.center, m_canvas.worldCamera);
        }

    }
}
