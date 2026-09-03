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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace getReal3D.Examples
{
    /// <summary>
    /// Creates and update buttons/sliders to show the getReal3D inputs on a UI.
    /// </summary>
    public class ShowInputs : MonoBehaviour
    {
        /// Prefab for button UI
        public GameObject buttonPrefab;

        /// Prefab for an axis slider UI
        public GameObject axisSliderPrefab;

        private List<GameObject> m_buttons = new List<GameObject>();
        private List<GameObject> m_axes = new List<GameObject>();

        void Start()
        {
            // Setup buttons
            var buttons = transform.Find("Canvas/Buttons");
            foreach (var buttonName in Input.buttonsName())
            {
                m_buttons.Add(Instantiate(buttonPrefab, buttons));
                m_buttons.Last().name = buttonName;
                m_buttons.Last().transform.Find("Text").GetComponent<Text>().text = buttonName;
            }

            // Setup axes
            var axes = transform.Find("Canvas/Axes");
            foreach (var axisName in Input.valuatorsName())
            {
                m_axes.Add(Instantiate(axisSliderPrefab, axes));
                m_axes.Last().name = axisName;
                m_axes.Last().transform.Find("Text").GetComponent<Text>().text = axisName;
            }
        }

        void Update()
        {
            // Update the buttons image depending if the button is pushed or released
            foreach (var button in m_buttons)
            {
                if (Input.GetButtonDown(button.name))
                {
                    button.transform.Find("ImageUp").gameObject.SetActive(false);
                    button.transform.Find("ImageDown").gameObject.SetActive(true);
                }
                else if (Input.GetButtonUp(button.name))
                {
                    button.transform.Find("ImageUp").gameObject.SetActive(true);
                    button.transform.Find("ImageDown").gameObject.SetActive(false);
                }
            }

            // Update the slider knobs according to each axis value
            foreach (var axis in m_axes)
            {
                MoveSliderKnob(axis, Input.GetAxis(axis.name));
            }
        }

        /// Move the slider knob to the given position
        private void MoveSliderKnob(GameObject slider, float pos)
        {
            var baseObj = slider.transform.Find("Slider/Base");
            var baseTransform = baseObj.transform as RectTransform;
            var knobObj = slider.transform.Find("Slider/Knob");
            var knobTransform = knobObj.transform as RectTransform;
            knobTransform.anchoredPosition = new Vector2(
                knobTransform.anchoredPosition.x,
                baseTransform.anchoredPosition.y + 0.9f * baseTransform.rect.height * pos / 2
            );
        }
    }

}
