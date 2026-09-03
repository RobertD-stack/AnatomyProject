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

namespace getReal3D
{
    /// <summary>
    /// This class is used to quickly set the colors on getReal3D UI menu.
    /// </summary>
    public class MenuColors : MonoBehaviour
    {
        [Tooltip("Text color")]
        public Color m_textColor = Color.black;

        [Tooltip("Panel color")]
        public Color m_panelColor = Color.white;

        [Tooltip("Panel outline color")]
        public Color m_panelOutlineColor = Color.white;

        [Tooltip("Button color")]
        public Color m_buttonsColor = Color.white;

        [Tooltip("Button color whn highlighted")]
        public Color m_highlightedButtonsColor = Color.red;

        [Tooltip("Slider fillRect color")]
        public Color m_sliderFillColor = Color.white;

        [Tooltip("Slider color")]
        public Color m_sliderColor = Color.white;

        [Tooltip("Slider handle color")]
        public Color m_sliderHandleColor = Color.white;

        [Tooltip("Toggle color")]
        public Color m_toggleColor = Color.white;

        [Tooltip("Toggle checkmark color")]
        public Color m_toggleCheckmarkColor = Color.black;

        static private string[] panelPages = { "Panel", "Title", "PreviousButton", "NextButton" };

        void OnValidate()
        {
            updateMenu();
        }

        void Start()
        {
            updateMenu();
        }

        void updateMenu()
        {
            handleGameObject(gameObject);
        }

        /// Colorize the UI components in the given @p page game object.
        public void handleGameObject(GameObject page)
        {
            Text text = page.GetComponent<Text>() as Text;
            if (text)
            {
                text.color = m_textColor;
            }

            Image image = page.GetComponent<Image>() as Image;
            if (image)
            {
                if (-1 != System.Array.IndexOf(panelPages, page.name))
                {
                    image.color = m_panelColor;
                }
                if (page.name.EndsWith("Outline"))
                {
                    image.color = m_panelOutlineColor;
                }
            }

            Button button = page.GetComponent<Button>() as Button;
            if (button && image)
            {
                image.color = m_buttonsColor;
            }

            if (button)
            {
                var colors = button.colors;
                colors.highlightedColor = m_highlightedButtonsColor;
                button.colors = colors;
            }

            Slider slider = page.GetComponent<Slider>() as Slider;
            if (slider)
            {
                if (slider.fillRect)
                {
                    Image imageFillRect = slider.fillRect.GetComponent<Image>() as Image;
                    imageFillRect.color = m_sliderFillColor;
                }
                if (slider.handleRect)
                {
                    Image imageHandleRect = slider.handleRect.GetComponent<Image>() as Image;
                    imageHandleRect.color = m_sliderHandleColor;
                }

                Image imageBackground = slider.transform.Find("Background").GetComponent<Image>() as Image;
                imageBackground.color = m_sliderColor;
            }

            Toggle toggle = page.GetComponent<Toggle>() as Toggle;
            if (toggle)
            {
                Graphic imageCheckmark = toggle.graphic;
                imageCheckmark.color = m_toggleCheckmarkColor;
                Image imageBackground = toggle.transform.Find("Background").GetComponent<Image>() as Image;
                imageBackground.color = m_toggleColor;
            }

            foreach (Transform t in page.transform)
            {
                handleGameObject(t.gameObject);
            }
        }
    }
}
