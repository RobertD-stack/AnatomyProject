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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// Stores button, sensor and valuator mappings.
    /// </summary>
    [Serializable]
    class InputMappingsBase<T> : ISerializationCallbackReceiver where T : InputMapping, new()
    {
        private Dictionary<string, T> m_buttonMap;
        private Dictionary<string, T> m_sensorMap;
        private Dictionary<string, T> m_valuatorMap;
        private Dictionary<string, T> m_stickMap;

        /// Returns the button mappings as a dictionary with the button name as key
        public Dictionary<string, T> buttonMap
        {
            get
            {
                if (m_buttonMap == null)
                {
                    m_buttonMap = buttons.ToDictionary(x => x.name, x => x);
                }
                return m_buttonMap;
            }
        }

        /// Returns the valuator mappings as a dictionary with the button name as key
        public Dictionary<string, T> valuatorMap
        {
            get
            {
                if (m_valuatorMap == null)
                {
                    m_valuatorMap = valuators.ToDictionary(x => x.name, x => x);
                }
                return m_valuatorMap;
            }
        }

        /// Returns the sensor mappings as a dictionary with the button name as key
        public Dictionary<string, T> sensorMap
        {
            get
            {
                if (m_sensorMap == null)
                {
                    m_sensorMap = sensors.ToDictionary(x => x.name, x => x);
                }
                return m_sensorMap;
            }
        }

        /// Returns the stick mappings as a dictionary with the stick name as key
        public Dictionary<string, T> stickMap
        {
            get
            {
                if (m_stickMap == null)
                {
                    m_stickMap = sticks.ToDictionary(x => x.name, x => x);
                }
                return m_stickMap;
            }
        }

        /// Returns the button mappings
        public ObservableCollection<T> buttons { get; private set; } = new ObservableCollection<T>();

        /// Returns the valuator mappings
        public ObservableCollection<T> valuators { get; private set; } = new ObservableCollection<T>();

        /// Returns the sensor mappings
        public ObservableCollection<T> sensors { get; private set; } = new ObservableCollection<T>();

        /// Returns the sticks mappings
        public ObservableCollection<T> sticks { get; private set; } = new ObservableCollection<T>();

        [SerializeField]
        private List<T> _buttons = new List<T>(); //< Used for Unity serialization

        [SerializeField]
        private List<T> _valuators = new List<T>(); //< Used for Unity serialization

        [SerializeField]
        private List<T> _sensors = new List<T>(); //< Used for Unity serialization

        [SerializeField]
        private List<T> _sticks = new List<T>(); //< Used for Unity serialization

        public InputMappingsBase()
        {
            buttons.CollectionChanged += (sender, e) => { m_buttonMap = null; };
            sensors.CollectionChanged += (sender, e) => { m_sensorMap = null; };
            valuators.CollectionChanged += (sender, e) => { m_valuatorMap = null; };
            sticks.CollectionChanged += (sender, e) => { m_stickMap = null; };
        }

        internal void setupMappingFromXML(string XML)
        {
            Plugin.debug("Parsing AppConfigXML");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);
            if (!doc.HasChildNodes) return;
            XmlNode unityNode = doc.FirstChild;
            if (unityNode != null)
            {
                XmlElement unityElem = (XmlElement)unityNode;
                if (unityElem != null && unityElem.Name == "unity")
                {
                    readMappings(unityElem, "map_button", MappingType.Button, buttons);
                    readMappings(unityElem, "map_valuator", MappingType.Axis, valuators);
                    readMappings(unityElem, "map_tracker", MappingType.Tracker, sensors);
                    readMappings(unityElem, "map_stick", MappingType.Stick, sticks);
                }
                else
                {
                    Plugin.warning("app_config does not contain unity element");
                }
            }
            else
            {
                Plugin.warning("app_config does not contain unity element");
            }
        }

        internal string genXml()
        {
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            StringWriter sw = new StringWriter();

            sw.WriteLine("<unity>");
            foreach (var m in buttons)
            {
                sw.Write("  <map_button name=\"{0}\" ", SecurityElement.Escape(m.name));
                if (m.index >= 0)
                {
                    sw.Write("index=\"{0}\" ", m.index + 1);
                }
                if (m.invert)
                {
                    sw.Write("invert=\"true\" ");
                }
                if (m.unityName != null)
                {
                    sw.Write("unity_button=\"{0}\" ", SecurityElement.Escape(m.unityName));
                }
                if (m.positiveKey != VirtualKeyCode.None)
                {
                    sw.Write("key=\"{0}\" ", m.positiveKey);
                }
                if (m.positiveButton != ButtonCode.None)
                {
                    sw.Write("button=\"{0}\" ", m.positiveButton);
                }
                sw.WriteLine("/>");
            }
            foreach (var m in valuators)
            {
                sw.Write("  <map_valuator name=\"{0}\" ", SecurityElement.Escape(m.name));
                if (m.index >= 0)
                {
                    sw.Write("index=\"{0}\" ", m.index + 1);
                }
                if (m.invert)
                {
                    sw.Write("invert=\"true\" ");
                }
                if (m.unityName != null)
                {
                    sw.Write("unity_axis=\"{0}\" ", SecurityElement.Escape(m.unityName));
                }
                if (m.positiveKey != VirtualKeyCode.None)
                {
                    sw.Write("positive_key=\"{0}\" ", m.positiveKey);
                }
                if (m.negativeKey != VirtualKeyCode.None)
                {
                    sw.Write("negative_key=\"{0}\" ", m.negativeKey);
                }
                if (m.positiveButton != ButtonCode.None)
                {
                    sw.Write("positive_button=\"{0}\" ", m.positiveButton);
                }
                if (m.negativeButton != ButtonCode.None)
                {
                    sw.Write("negative_button=\"{0}\" ", m.negativeButton);
                }
                if (m.deadZone != 0)
                {
                    sw.Write("dead_zone=\"{0}\" ", m.deadZone);
                }
                if (m.positiveAxis != AxisCode.None)
                {
                    sw.Write("{0}=\"{1}\" ", m.negativeAxis != AxisCode.None ? "positive_axis" : "axis", m.positiveAxis);
                }
                if (m.negativeAxis != AxisCode.None)
                {
                    sw.Write("negative_axis=\"{0}\" ", m.negativeAxis);
                }
                sw.WriteLine("/>");
            }
            foreach (var m in sensors)
            {
                sw.Write("  <map_tracker name=\"{0}\" ", SecurityElement.Escape(m.name));
                if (m.index >= 0)
                {
                    sw.Write("index=\"{0}\" ", m.index + 1);
                }
                sw.WriteLine("/>");
            }
            foreach (var s in sticks)
            {
                sw.Write("  <map_stick name=\"{0}\" ", SecurityElement.Escape(s.name));
                if (!string.IsNullOrEmpty(s.verticalAxis))
                {
                    sw.Write("vertical_axis=\"{0}\" ", SecurityElement.Escape(s.verticalAxis));
                }
                if (!string.IsNullOrEmpty(s.horizontalAxis))
                {
                    sw.Write("horizontal_axis=\"{0}\" ", SecurityElement.Escape(s.horizontalAxis));
                }
                sw.WriteLine("/>");
            }
            sw.WriteLine("</unity>");
            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            return sw.ToString();
        }
        private static int IndexOf(ObservableCollection<T> mappings, string name)
        {
            try
            {
                return mappings.Select((m, i) => new { mapping = m, index = i }).
                    First(mapping => mapping.mapping.name == name).index;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static void readMappings(XmlElement unityElem,
                                         string tagName,
                                         MappingType type,
                                         ObservableCollection<T> res)
        {
            XmlNodeList bList = unityElem.GetElementsByTagName(tagName);
            IEnumerator ie = bList.GetEnumerator();
            while (ie.MoveNext())
            {
                var el = new T();
                el.type = type;
                el.fromXml((XmlElement)ie.Current);
                for (int dup; (dup = IndexOf(res, el.name)) != -1;)
                {
                    res.RemoveAt(dup);
                }
                res.Add(el);
            }
        }

        /// <summary>
        /// Convert the ObservableCollection into lists so Unity knows how to serialize the mappings
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _buttons = buttons.ToList();
            _valuators = valuators.ToList();
            _sensors = sensors.ToList();
            _sticks = sticks.ToList();
        }

        /// <summary>
        /// Convert the lists back into ObservableCollection
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            buttons = new ObservableCollection<T>(_buttons);
            valuators = new ObservableCollection<T>(_valuators);
            sensors = new ObservableCollection<T>(_sensors);
            sticks = new ObservableCollection<T>(_sticks);
            buttons.CollectionChanged += (sender, e) => { m_buttonMap = null; };
            sensors.CollectionChanged += (sender, e) => { m_sensorMap = null; };
            valuators.CollectionChanged += (sender, e) => { m_valuatorMap = null; };
            sticks.CollectionChanged += (sender, e) => { m_stickMap = null; };
        }

    }
}
