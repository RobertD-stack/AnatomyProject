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

using System;
using System.Xml;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// MenuSettings holds some settings related to the getReal3D menu.
    /// </summary>
    public class MenuSettings : MonoBehaviour
    {
        /// Width of the menu
        public float? width;

        /// Depth of the menu (i.e. how far from the wand the menu shows up)
        public float? depth;

        /// Maximum rotation speed for the navigation menu slider
        public float? maxRotationSpeed;

        /// Maximum translation speed for the navigation menu slider
        public float? maxTranslationSpeed;

        /// Button used for showing the menu (deprecated)
        [HideInInspector]
        public string menuButton = null;

        /// Button used for draggin the menu (deprecated)
        [HideInInspector]
        public string dragMenuButton = null;

        void Start()
        {
            menuButton = null;
            dragMenuButton = null;

            XmlElement unityElem = getReal3D.Plugin.getApplicationConfigXml();
            if (unityElem == null)
            {
                return;
            }

            XmlNodeList menus = unityElem.GetElementsByTagName("menu");
            if (menus.Count == 0)
            {
                return;
            }

            XmlElement menu = menus.Item(0) as XmlElement;

            width = readFromAttribute(menu, "width");
            depth = readFromAttribute(menu, "depth");
            maxRotationSpeed = readFromAttribute(menu, "max_rotation_speed");
            maxTranslationSpeed = readFromAttribute(menu, "max_translation_speed");
            menuButton = readStringFromAttribute(menu, "button");
            dragMenuButton = readStringFromAttribute(menu, "drag_button");

            WandEventModule wandEventModule = Plugin.FindAnyObjectByType<WandEventModule>();
            if (menuButton != null && wandEventModule)
            {
                wandEventModule.submitButtonName = menuButton;
            }
        }

        private float? readFromAttribute(XmlElement element, String attribute)
        {
            if (element.HasAttribute(attribute))
            {
                string w = element.Attributes[attribute].Value;
                try
                {
                    return Convert.ToSingle(w);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Bad format for menu " + attribute + " '" + w + "': " + e.Message);
                }
            }
            return default(float?);
        }

        private string readStringFromAttribute(XmlElement element, String attribute)
        {
            if (element.HasAttribute(attribute))
            {
                return element.Attributes[attribute].Value;
            }
            return null;
        }

    }
}
