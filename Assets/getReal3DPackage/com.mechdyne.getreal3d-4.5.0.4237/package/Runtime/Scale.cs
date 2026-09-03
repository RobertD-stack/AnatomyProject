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


namespace getReal3D
{
    /// <summary>
    /// A utility class for converting units, and dealing with scaling Unity worlds.
    /// For VR applications, it is best to define Unity worlds in meters, and this is the default assumption for most of getReal3D (eye separation defaults to inches).
    /// However, Unity does not require any particular unit; existing games may be in any unit, and due to source material scale is may be convenient to use other units.
    /// </summary>
    public static class Scale
    {
        internal static Dictionary<Units, float> _unitScale = new Dictionary<Units, float>();
        internal static Dictionary<Units, string> _unitNames = new Dictionary<Units, string>();

        internal static float _vrScale = 1.0f;
        internal static float _userScale = 1.0f;
        internal static float _refScale = 1.0f;

        internal static bool initialized = false;

        static Scale()
        {
            if (!getReal3D.Plugin.initialized())
                getReal3D.Plugin.initVR();

            Init();
        }

        /// <summary>
        /// Supported Units for scale conversion.
        /// </summary>
        public enum Units
        {
            /// <summary>
            ///
            /// </summary>
            meters,
            /// <summary>
            ///
            /// </summary>
            centimeters,
            /// <summary>
            ///
            /// </summary>
            millimeters,
            /// <summary>
            ///
            /// </summary>
            inches,
            /// <summary>
            ///
            /// </summary>
            feet
        };

        /// <summary>
        /// The current Units for eye separation. While most of getReal3D defaults to meters, eye separation defaults to inches.
        /// </summary>
        public static Units eyeSeparationUnits { get; set; }
        /// <summary>
        /// The current Units for eye separation as a (short) string
        /// </summary>
        public static string eyeSeparationUnitString { get { return _unitNames[eyeSeparationUnits]; } }
        /// <summary>
        /// The current eye separation value
        /// </summary>
        public static float eyeSeparation
        {
            get { return getReal3D.Plugin.GetEyeSeparation() / _unitScale[eyeSeparationUnits]; }
            set { getReal3D.Plugin.SetEyeSeparation(value * _unitScale[eyeSeparationUnits]); }
        }

        /// <summary>
        /// The current world scale (Unity units)
        /// </summary>
        static public float worldScale
        {
            get { return _refScale / _userScale; }
        }

        /// <summary>
        /// The Units of the reference object
        /// </summary>
        public static Units referenceUnits { get; set; }
        /// <summary>
        /// The scale factor for the reference Unit
        /// </summary>
        public static float referenceUnitScale { get { return _unitScale[referenceUnits]; } }
        /// <summary>
        /// The short string for the reference Unit
        /// </summary>
        public static string referenceUnitString { get { return _unitNames[referenceUnits]; } }
        /// <summary>
        /// The scale of the reference object
        /// </summary>
        static public float referenceScale
        {
            get { return _refScale / referenceUnitScale; }
            set { _vrScale *= _refScale / (value * referenceUnitScale); _refScale = value * referenceUnitScale; getReal3D.Plugin.SetScale(_vrScale); }
        }

        /// <summary>
        /// The user conrolled scale factor
        /// </summary>
        static public float userScale
        {
            get { return _userScale; }
            set { _userScale = value; }
        }

        /// <summary>
        /// Get the scale factor for a Unit
        /// </summary>
        /// <param name="u">The Unit</param>
        /// <returns>The scale factor</returns>
        static public float GetUnitScale(Units u)
        {
            return _unitScale[u];
        }

        /// <summary>
        /// Get the short string for a Unit
        /// </summary>
        /// <param name="u">The Unit</param>
        /// <returns>The short string</returns>
        static public string GetUnitString(Units u)
        {
            return _unitNames[u];
        }

        internal static void Init()
        {
            if (!initialized)
            {
                if (_unitScale.Count == 0)
                {
                    _unitScale.Add(Units.meters, 1.0f);
                    _unitScale.Add(Units.centimeters, 0.01f);
                    _unitScale.Add(Units.millimeters, 0.001f);
                    _unitScale.Add(Units.feet, 0.3048f);
                    _unitScale.Add(Units.inches, 0.0254f);
                }
                if (_unitNames.Count == 0)
                {
                    _unitNames.Add(Units.meters, "m");
                    _unitNames.Add(Units.centimeters, "cm");
                    _unitNames.Add(Units.millimeters, "mm");
                    _unitNames.Add(Units.feet, "ft");
                    _unitNames.Add(Units.inches, "in");
                }
                _vrScale = getReal3D.Plugin.GetScale();
                _refScale = 1.0f;
                _userScale = 1.0f;
                referenceUnits = Units.meters;
                eyeSeparationUnits = Units.inches;
                initialized = true;
            }
        }
    }
}
