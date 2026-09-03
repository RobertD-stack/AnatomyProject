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

using System.Collections.Generic;
using UnityEngine;

namespace getReal3D
{
    /// Callback interface for when a wand click 
    internal interface MenuClickedOustideCallback
    {
        void clickOutside();
    }

    /// <summary>
    /// Script used to drag a 3D menu by clicking on some part of it (see draggable field).
    /// </summary>
    ///
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Canvas))]
    public class GenericMenuDrag : MonoBehaviour
    {

        private Canvas m_canvas;
        private RectTransform m_rectTransform;
        internal MenuClickedOustideCallback m_showMenu;
        internal PlayerInputs m_inputs;

        [Tooltip("Parts of the menu that can be used to drag it.")]
        public List<GameObject> m_draggable = new List<GameObject>();

        [Tooltip("Object that is set as the new menu parent while it is being dragged.")]
        public Transform m_hand;

        private Transform m_originalParent;
        private GenericWandEventModule m_wandEventModule;

        void Start()
        {
            m_canvas = GetComponent<Canvas>() as Canvas;
            m_rectTransform = gameObject.transform as RectTransform;
            m_originalParent = transform.parent;
            m_wandEventModule = Plugin.FindAnyObjectByType<GenericWandEventModule>();
        }

        void Update()
        {
            if (m_inputs == null)
            {
                return;
            }

            bool pointerOnRect = RectTransformUtility.RectangleContainsScreenPoint
                (m_rectTransform, m_canvas.worldCamera.pixelRect.center, m_canvas.worldCamera);



            if (m_inputs.WandButtonDown)
            {
                if (!pointerOnRect)
                {
                    m_showMenu.clickOutside();
                }
            }

            if (m_inputs.WandButtonDown && pointerOnRect & isDraggable())
            {
                Grab();
            }

            if (m_inputs.WandButtonUp)
            {
                Release();
            }
        }

        private bool isDraggable()
        {
            if (m_wandEventModule == null)
            {
                return false;
            }
            var currentLook = m_wandEventModule.currentLook;
            if (currentLook == null)
            {
                return false;
            }
            foreach (var obj in m_draggable)
            {
                if (currentLook.transform.IsChildOf(obj.transform))
                {
                    return true;
                }
            }
            return false;
        }

        void Grab()
        {
            transform.SetParent(m_hand.transform, true);
        }

        void Release()
        {
            transform.SetParent(m_originalParent, true);
        }
    }
}
