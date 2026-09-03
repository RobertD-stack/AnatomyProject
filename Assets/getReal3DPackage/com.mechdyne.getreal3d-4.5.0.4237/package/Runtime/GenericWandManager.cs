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
    /// <summary>
    /// This scripts allows the player to cycle through the different wands.
    /// </summary>
    [RequireComponent(typeof(PlayerInputs))]
    public class GenericWandManager : MonoBehaviour
    {
        [Tooltip("List of the different wands the player can choose from.")]
        public List<GameObject> m_managedObjects;

        private int m_activeIndex = 0;
        private PlayerInputs m_playerInputs;
        private PlayerInputs playerInputs
        {
            get
            {
                if (m_playerInputs == null)
                {
                    m_playerInputs = GetComponent<PlayerInputs>();
                }
                return m_playerInputs;
            }
        }

        void Start()
        {
            SetWandActive(0);
        }

        void Update()
        {
            if (playerInputs.ChangeWandButtonDown)
            {
                SetWandActive((m_activeIndex + 1) % m_managedObjects.Count);
            }
        }

        private void SetWandActive(int index)
        {
            getReal3D.Plugin.debug(string.Format("{0} SetWandActive({1})", gameObject.name, index));
            m_activeIndex = index;
            for (int i = 0; i < m_managedObjects.Count; ++i)
            {
                m_managedObjects[i].SetActive(i == index);
            }
        }
    }
}
