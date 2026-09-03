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

using System.Linq;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// This component creates chaperone grid children objects that are shown when
    /// the distance between the wand or head and any of the screen is less than a
    /// given distance.
    /// </summary>
    /// @see getReal3D.Chaperone
    [RequireComponent(typeof(CreateScreens))]
    public class ChaperoneManager : MonoBehaviour
    {
        private CreateScreens m_createScreens = null;
        private Transform m_head = null;
        private Transform m_wand = null;
        private Chaperone[] m_chaperones = new Chaperone[0];

        [Tooltip("Chaperones are displayed when the head or wand is closer than this distance from a screen")]
        public float triggerDistance = 0.2f;

        [Tooltip("Time to fade in or out the chaperone")]
        public float fadeTime = 1.0f;

        [Tooltip("Set to false in order to disable wand distance check")]
        public bool checkWand = true;

        [Tooltip("Set to false in order to disable head distance check")]
        public bool checkHead = true;

        [Tooltip("Chaperone grid color")]
        public Color color = new Color(0.15f, 0.39f, 0.59f);

        private void Awake()
        {
            var headUpdater = GetComponentInParent<getReal3D.GenericHeadUpdater>();
            Debug.Assert(headUpdater != null, "ChaperoneManager is expected to be added to GenericPlayer");
            m_head = headUpdater.target;
            var wandUpdater = GetComponentInParent<getReal3D.GenericWandUpdater>();
            Debug.Assert(wandUpdater != null, "ChaperoneManager is expected to be added to GenericPlayer");
            m_wand = wandUpdater.target;
            Debug.Assert(m_head != null, "GenericPlayer Head not found");
            Debug.Assert(m_wand != null, "GenericPlayer Wand not found");
            m_createScreens = GetComponent<CreateScreens>();
            Debug.Assert(m_createScreens != null, "GenericPlayer CreateScreens not found");
            m_chaperones = m_createScreens.screens.Select(s => s.GetComponent<Chaperone>()).ToArray();
            foreach (var chaperone in m_chaperones)
            {
                chaperone.gameObject.GetComponent<MeshRenderer>().enabled = false;
                chaperone.SetColor(color);
            }
        }

        private float getDist(Transform target)
        {
            float res = float.MaxValue;
            foreach(var chaperone in m_chaperones)
            {
                var plane = new Plane(chaperone.transform.forward, chaperone.transform.position);
                var dist = Mathf.Abs(plane.GetDistanceToPoint(target.position));
                res = Mathf.Min(res, dist);
            }
            return res;
        }

        private void Update()
        {
            float dist = float.MaxValue;

            if (checkWand)
            {
                dist = Mathf.Min(dist, getDist(m_wand));
            }

            if (checkHead)
            {
                dist = Mathf.Min(dist, getDist(m_head));
            }

            if (dist < triggerDistance)
            {
                foreach (var chaperone in m_chaperones)
                {
                    chaperone.FadeIn(fadeTime);
                }
            }
            else
            {
                foreach (var chaperone in m_chaperones)
                {
                    chaperone.FadeOut(fadeTime);
                }
            }
        }
    }
}
