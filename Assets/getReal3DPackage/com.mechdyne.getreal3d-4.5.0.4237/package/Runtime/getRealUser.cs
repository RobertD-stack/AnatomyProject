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
    /// This class is used to identify which of getReal3D users the GenericPlayer scripts should
    /// be using for their inputs and cameras. See getRealUserScript.
    public class getRealUser : MonoBehaviour
    {
        /// <summary>
        /// User identifier
        /// </summary>
        /// Must be within 0 and the number of user - 1. The number of users is defined in the
        /// getReal3D configuration file.
        public uint userId = 0;

        private static List<getRealUser> s_users = new List<getRealUser>();

        /// Retrieve the list of users in the scene.
        public static List<getRealUser> users
        {
            get { return s_users; }
        }

        void OnEnable()
        {
            s_users.Add(this);
        }

        void OnDisable()
        {
            s_users.Remove(this);
        }
    }
}
