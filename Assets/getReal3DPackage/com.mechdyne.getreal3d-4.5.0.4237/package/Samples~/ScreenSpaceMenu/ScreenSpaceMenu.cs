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


namespace getReal3D.Examples
{

    /// <summary>
    /// This script demonstrates how to setup a screen space menu in getReal3D. The two buttons
    /// on the menu are used to move the menu from one screen to the other.
    /// </summary>
    [RequireComponent(typeof(CreateScreens))]
    public class ScreenSpaceMenu : MonoBehaviour
    {
        // Script we used to create the screens objects
        private CreateScreens m_createScreens;

        // The menu
        private Transform m_menu;

        // Start is called before the first frame update
        void Start()
        {
            // Retrieve the menu
            m_menu = transform.Find("Menu");

            // Retrieve the CreateScreens
            m_createScreens = GetComponent<CreateScreens>();

            // Set the wand camera to the canvas Event Camera is required to get wand/menu interaction
            var canvas = m_menu.Find("Canvas").GetComponent<Canvas>();
            canvas.worldCamera = transform.root.Find("Hand/WandManager/PickingCamera").GetComponent<Camera>();

            // Setup actions
            m_menu.Find("Canvas/Nav/PreviousButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PreviousScreen);
            m_menu.Find("Canvas/Nav/NextButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(NextScreen);

            // Create the screens manually so that we're sure there is no Start event order issue
            m_createScreens.Create();

            // Move our menu to the first screen
            m_menu.SetParent(m_createScreens.screens[0].transform, false);
        }

        /// Move the menu to the next screen
        private void NextScreen()
        {
            var index = m_createScreens.screens.IndexOf(m_menu.parent.gameObject);
            var newIndex = (index + 1) % m_createScreens.screens.Count;
            m_menu.SetParent(m_createScreens.screens[newIndex].transform, false);
        }

        /// Move the menu to the previous screen
        private void PreviousScreen()
        {
            var index = m_createScreens.screens.IndexOf(m_menu.parent.gameObject);
            var newIndex = (index - 1 >= 0) ? index - 1 : m_createScreens.screens.Count - 1;
            m_menu.SetParent(m_createScreens.screens[newIndex].transform, false);
        }
    }

}
