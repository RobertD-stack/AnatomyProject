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
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace getReal3D
{
    internal enum MappingType
    {
        Tracker,
        Button,
        Axis,
        Stick,
    };

    [Serializable]
    internal class InputMapping
    {
        public int index { get; set; } = -1;
        public float deadZone { get; set; } = 0f;
        public bool invert { get; set; } = false;
        public string unityName { get; set; } = null;
        public VirtualKeyCode positiveKey { get; set; } = VirtualKeyCode.None;
        public VirtualKeyCode negativeKey { get; set; } = VirtualKeyCode.None;
        public ButtonCode positiveButton { get; set; } = ButtonCode.None;
        public ButtonCode negativeButton { get; set; } = ButtonCode.None;
        public AxisCode positiveAxis { get; set; } = AxisCode.None;
        public AxisCode negativeAxis = AxisCode.None;
        public string name;
        public string verticalAxis;
        public string horizontalAxis;
        public MappingType type = MappingType.Tracker;

        internal void fromXml(XmlElement elem)
        {
            if (elem.HasAttribute("name"))
            {
                name = elem.Attributes["name"].Value;
            }
            if (elem.HasAttribute("vertical_axis"))
            {
                verticalAxis = elem.Attributes["vertical_axis"].Value;
            }
            if (elem.HasAttribute("horizontal_axis"))
            {
                horizontalAxis = elem.Attributes["horizontal_axis"].Value;
            }
            unityName = null;
            if (elem.HasAttribute("unity_button"))
            {
                unityName = elem.Attributes["unity_button"].Value;
            }
            else if (elem.HasAttribute("unity_axis"))
            {
                unityName = elem.Attributes["unity_axis"].Value;
            }

            if (elem.HasAttribute("index"))
            {
                index = Convert.ToInt32(elem.Attributes["index"].Value) - 1;
            }
            else
            {
                index = -1;
            }
            if (elem.HasAttribute("invert"))
            {
                string sinvert = elem.Attributes["invert"].Value.ToLower();
                invert = (sinvert == "true" || sinvert == "1" || sinvert == "y");
            }
            else
            {
                invert = false;
            }

            deadZone = ParseSingle(elem, "dead_zone", 0);

            if (elem.HasAttribute("key"))
            {
                positiveKey = parseKey(elem.Attributes["key"].Value);
            }
            else if (elem.HasAttribute("positive_key"))
            {
                positiveKey = parseKey(elem.Attributes["positive_key"].Value);
            }
            else
            {
                positiveKey = VirtualKeyCode.None;
            }
            if (elem.HasAttribute("negative_key"))
            {
                negativeKey = parseKey(elem.Attributes["negative_key"].Value);
            }
            else
            {
                negativeKey = VirtualKeyCode.None;
            }
            if (elem.HasAttribute("button"))
            {
                positiveButton = parseButton(elem.Attributes["button"].Value);
            }
            else if (elem.HasAttribute("positive_button"))
            {
                positiveButton = parseButton(elem.Attributes["positive_button"].Value);
            }
            else
            {
                positiveButton = ButtonCode.None;
            }
            if (elem.HasAttribute("negative_button"))
            {
                negativeButton = parseButton(elem.Attributes["negative_button"].Value);
            }
            else
            {
                negativeButton = ButtonCode.None;
            }
            if (elem.HasAttribute("axis"))
            {
                positiveAxis = parseAxis(elem.Attributes["axis"].Value);
            }
            else if (elem.HasAttribute("positive_axis"))
            {
                positiveAxis = parseAxis(elem.Attributes["positive_axis"].Value);
            }
            else
            {
                positiveAxis = AxisCode.None;
            }
            if (elem.HasAttribute("negative_axis"))
            {
                negativeAxis = parseAxis(elem.Attributes["negative_axis"].Value);
            }
            else
            {
                negativeAxis = AxisCode.None;
            }
        }

        static private float ParseSingle(XmlElement elem, string attributeName, float defaultValue)
        {
            if (elem.HasAttribute(attributeName))
            {
                string attributeValue = elem.Attributes[attributeName].Value;
                try
                {
                    return Convert.ToSingle(attributeValue, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Debug.LogWarning("Invalid value for " + attributeName + ": " +
                        attributeValue + "'");
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public InputMapping()
        {

        }

        public override string ToString()
        {
            string res = "";
            res += name + " ";
            if (index != -1)
            {
                res += "index: " + index.ToString() + " ";
            }
            if (deadZone != 0)
            {
                res += "deadZone: " + deadZone.ToString() + " ";
            }
            if (invert)
            {
                res += "invert: true ";
            }
            if (unityName != null)
            {
                res += "unityName: " + unityName + " ";
            }
            if (positiveKey != VirtualKeyCode.None)
            {
                res += "key: " + positiveKey.ToString() + " ";
            }
            if (negativeKey != VirtualKeyCode.None)
            {
                res += "negativeKey: " + negativeKey.ToString() + " ";
            }
            if (positiveButton != ButtonCode.None)
            {
                res += "button: " + positiveButton.ToString() + " ";
            }
            if (negativeButton != ButtonCode.None)
            {
                res += "negativeButton: " + negativeButton.ToString() + " ";
            }
            if (positiveAxis != AxisCode.None)
            {
                res += "axis: " + positiveAxis.ToString() + " ";
            }
            if (negativeAxis != AxisCode.None)
            {
                res += "negativeAxis: " + negativeAxis.ToString() + " ";
            }
            return res;
        }

        public bool getButtonValue(List<int> trackdButtons, byte[] keyboardState, byte[] buttonsState)
        {
            bool res = false;
            if (index >= 0 && index < trackdButtons.Count)
            {
                res = invert ^ (trackdButtons[index] == 1);
            }
            if (!res && positiveKey >= 0 && (int)positiveKey < keyboardState.Length && positiveKey != VirtualKeyCode.None)
            {
                res = keyboardState[(int)positiveKey] != 0;
            }
            if (!res && positiveButton >= 0 && (int)positiveButton < buttonsState.Length && positiveButton != ButtonCode.None)
            {
                res = buttonsState[(int)positiveButton] != 0;
            }
            return res;
        }

        public float getAxisValue(List<float> trackdAxis, byte[] keyboardState, float[] axisState, byte[] buttonsState, bool raw)
        {
            float val = 0;
            if (index >= 0 && index < trackdAxis.Count)
            {
                val = trackdAxis[index];
            }

            if (val == 0 && positiveAxis >= 0 && (int)positiveAxis < axisState.Length)
            {
                val = axisState[(int)positiveAxis];
            }

            if (val == 0 && negativeAxis >= 0 && (int)negativeAxis < axisState.Length)
            {
                val = -axisState[(int)negativeAxis];
            }

            if (val == 0 && positiveKey >= 0 && (int)positiveKey < keyboardState.Length && positiveKey != VirtualKeyCode.None)
            {
                val = keyboardState[(int)positiveKey] != 0 ? 1 : 0;
            }

            if (val == 0 && negativeKey >= 0 && (int)negativeKey < keyboardState.Length && negativeKey != VirtualKeyCode.None)
            {
                val = keyboardState[(int)negativeKey] != 0 ? -1 : 0;
            }

            if (val == 0 && positiveButton >= 0 && (int)positiveButton < buttonsState.Length && positiveButton != ButtonCode.None)
            {
                val = buttonsState[(int)positiveButton] != 0 ? 1 : 0;
            }

            if (val == 0 && negativeButton >= 0 && (int)negativeButton < buttonsState.Length && negativeButton != ButtonCode.None)
            {
                val = buttonsState[(int)negativeButton] != 0 ? -1 : 0;
            }

            if (!raw)
            {
                float rval = (Math.Abs(val) - deadZone) / (1f - deadZone);
                rval *= rval > 0f ? val : 0f;
                return invert ? -rval : rval;
            }
            else
            {
                return val;
            }
        }

        private static VirtualKeyCode parseKey(string s)
        {
            try
            {
                return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), "VK_" + s, true);
            }
            catch (Exception) { }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), s, true);
        }

        private static ButtonCode parseButton(string s)
        {
            try
            {
                return (ButtonCode)Enum.Parse(typeof(ButtonCode), "JOYSTICK_" + s, true);
            }
            catch (Exception) { }

            return (ButtonCode)Enum.Parse(typeof(ButtonCode), s, true);
        }

        private static AxisCode parseAxis(string s)
        {
            try
            {
                return (AxisCode)Enum.Parse(typeof(AxisCode), "JOYSTICK_" + s, true);
            }
            catch (Exception) { }
            return (AxisCode)Enum.Parse(typeof(AxisCode), s, true);
        }
    }
}
