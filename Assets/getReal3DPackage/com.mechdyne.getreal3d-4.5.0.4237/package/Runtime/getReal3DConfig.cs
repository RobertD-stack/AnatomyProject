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
    /// <summary>
    /// This class represents a getReal3D configuration file within Unity assets.
    /// </summary>
    public class getReal3DConfig : ScriptableObject
    {
        /// <summary>
        /// Stores information about the loaded configuration.
        /// </summary>
        [System.Serializable]
        public struct ConfigInfo
        {
            /// <summary>
            /// Name of the configuration.
            /// </summary>
            public string name;
        }

        /// <summary>
        /// Information about that configuration file.
        /// </summary>
        public ConfigInfo info;

        internal string content
        {
            get { return m_content; }
            set { m_content = value; }
        }

        ///  We store this thing as a char[] otherwise Untiy goes mad when trying to display it.
        ///  (Text mesh generation is limited to 64K vertices!)
        [SerializeField, HideInInspector]
        private string m_content;
    }
}
