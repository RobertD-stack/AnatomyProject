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
    /// MenuScale is used to set the the getReal3D menu scale
    [RequireComponent(typeof(RectTransform))]
    public class MenuScale : MonoBehaviour
    {
        /// Width of the menu. Will be updated at startup via MenuSettings.
        public float m_menuWidth = 1;
        private float m_scalePerMeter = 0.00125f; /// magic number!

        void Start()
        {
            MenuSettings ms = GetComponentInParent<MenuSettings>();
            if (ms && ms.width.HasValue)
            {
                m_menuWidth = ms.width.Value;
            }
            updateMenuSize();
        }

        void OnValidate()
        {
            updateMenuSize();
        }

        void updateMenuSize()
        {
            transform.localScale = Vector3.one * m_menuWidth * m_scalePerMeter;
        }

    }
}
