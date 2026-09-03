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

using System.Collections;
using UnityEngine;

namespace getReal3D
{
    /// The PageSlider class is used to go to the previous or next page of the menu using
    /// the buttons on top of the menu.
    public class PageSlider : MonoBehaviour
    {
        /// If set, that Text will reflect the current page name.
        public UnityEngine.UI.Text m_title = null;

        /// Animation speed for changing the page.
        public float m_speed = 0.0f;

        private int m_currentIndex;
        private bool m_sliding = false;
        private Transform m_current = null;
        private Transform[] m_pages;

        void Start()
        {
            m_pages = getAllChildren().ToArray();
            m_currentIndex = 0;
            if (m_pages.Length != 0)
            {
                m_current = m_pages[0];
                m_current.gameObject.SetActive(true);
            }
            disableAllButCurrent();
            updateTitle();
        }

        /// Show the next page if it exists. Called via UI bindings from the corresponding button.
        public void nextPage()
        {
            if (m_sliding)
            {
                return;
            }
            if (m_currentIndex == m_pages.Length - 1)
            {
                return;
            }
            StartCoroutine(changePage(m_currentIndex + 1, 1));
        }

        /// Show the previous page if it exists. Called via UI bindings from the corresponding button.
        public void previousPage()
        {
            if (m_sliding)
            {
                return;
            }
            if (m_currentIndex == 0)
            {
                return;
            }
            StartCoroutine(changePage(m_currentIndex - 1, -1));
        }

        private void updateTitle()
        {
            if (m_title && m_current)
            {
                m_title.text = m_current.name;
            }
        }

        private void disableAllButCurrent()
        {
            foreach (Transform child in m_pages)
            {
                if (child != m_current)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        IEnumerator changePage(int nextIndex, float slideDirection)
        {
            Transform next = m_pages[nextIndex];
            m_currentIndex = nextIndex;

            m_sliding = true;
            Transform old = m_current;
            m_current = next;
            m_current.gameObject.SetActive(true);

            updateTitle();

            Vector3 oldPosition = old.transform.localPosition;
            Vector3 destPosition = m_current.transform.localPosition;

            float width = 400.0f;
            RectTransform rt = transform as RectTransform;
            if (rt)
            {
                width = rt.rect.width;
            }

            float startTime = Time.time;
            float endTime = startTime + m_speed;
            while (Time.time < endTime)
            {
                float elapsed = Time.time - startTime;
                float t = elapsed / m_speed;

                old.transform.localPosition = oldPosition + slideDirection * t * Vector3.left * width;
                m_current.transform.localPosition = destPosition - slideDirection * (1 - t) * Vector3.left * width;

                yield return null;
            }
            old.gameObject.SetActive(false);
            old.transform.localPosition = oldPosition;
            m_current.transform.localPosition = destPosition;
            m_sliding = false;
        }

        private System.Collections.Generic.List<Transform> getAllChildren()
        {
            System.Collections.Generic.List<Transform> res = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in transform)
            {
                res.Add(child);
            }
            return res;
        }
    }
}
