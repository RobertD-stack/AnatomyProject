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
using UnityEngine;

namespace getReal3D
{

    /// <summary>
    /// This class loosely emulates the built-in Unity Input class.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// The trackd sensors
        /// </summary>
        public static List<getReal3D.Sensor> sensors { get; internal set; }
        /// <summary>
        /// The cameras configured for this Unity instance
        /// </summary>
        public static List<getReal3D.Sensor> cameras { get; internal set; }
        /// <summary>
        /// The trackd controller valuators
        /// </summary>
        public static List<float> valuators { get; internal set; }
        /// <summary>
        /// The trackd controller button states (1 == down)
        /// </summary>
        public static List<int> buttons { get; internal set; }
        private static List<int> prevButtons;

        /// <summary>
        /// The sensor index of the wand.
        /// </summary>
        public static int wandIndex { get; internal set; }
        /// <summary>
        /// The sensor index of the head.
        /// </summary>
        public static int headIndex { get; internal set; }

        private static int[] userWandIndex = null;
        private static int[] userHeadIndex = null;

        private static getReal3D.Sensor headSensor;
        private static getReal3D.Sensor wandSensor;

        private static InputMappings inputMappings = new InputMappings();

        private static byte[] m_lastKeyboardState = new byte[(int)VirtualKeyCode.Count];
        private static byte[] m_keyboardState = new byte[(int)VirtualKeyCode.Count];
        private static byte[] m_buttonState = new byte[(int)ButtonCode.Count];
        private static byte[] m_lastButtonState = new byte[(int)ButtonCode.Count];
        private static float[] m_axisState = new float[(int)AxisCode.Count];
        private static bool? m_masterInputMatchSize;
        private static bool m_isRunningViaGetReal3D = false;

        /// <summary>
        /// This event is called once a frame, after the inputs are updated.
        /// </summary>
        internal delegate void InputUpdatedDelegate();
        internal static event InputUpdatedDelegate OnInputUpdated;

        /// <summary>
        /// Return buttons name.
        /// </summary>
        public static string[] buttonsName()
        {
            string[] keys = new string[inputMappings.buttonMap.Keys.Count];
            inputMappings.buttonMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Return valuators name.
        /// </summary>
        public static string[] valuatorsName()
        {
            string[] keys = new string[inputMappings.valuatorMap.Keys.Count];
            inputMappings.valuatorMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Return sensors name.
        /// </summary>
        public static string[] sensorsName()
        {
            string[] keys = new string[inputMappings.sensorMap.Keys.Count];
            inputMappings.sensorMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Utility class holding configuration values for controlling navigation, such as TranslationSpeed or RotationSpeed.
        /// </summary>
        public static class NavOptions
        {
            static public float RotationSpeed
            {
                get; set;
            }

            static public float TranslationSpeed
            {
                get; set;
            }

            static public float WandLookDeadZone
            {
                get; set;
            }
        }

        private static InputManager inputManager = null;

        private static int lastUpdateFrame = -1;

        /// <summary>
        /// Initializes getReal3D Input classes.
        /// </summary>
        /// <returns>True if successful</returns>
        public static bool Init()
        {
            if (!getReal3D.Plugin.initialized())
            {
                wandIndex = 1;
                headIndex = 0;
                return getReal3D.Plugin.initVR();
            }
            return true;
        }

        /// <summary>
        /// The current wand transform
        /// </summary>
        public static getReal3D.Sensor wand
        {
            get { Update(); return wandSensor; }
            internal set { wandSensor = value; }
        }

        /// <summary>
        /// The current head transform
        /// </summary>
        public static getReal3D.Sensor head
        {
            get { Update(); return headSensor; }
            internal set { headSensor = value; }
        }

        /// <summary>
        /// The head transform for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        public static getReal3D.Sensor headForUser(uint userId)
        {
            Update();
            System.Diagnostics.Debug.Assert(userHeadIndex != null);
            if (userId >= userHeadIndex.Length)
            {
                throw new IndexOutOfRangeException("User ID is out of range in Input.headForUser.");
            }
            int sensorId = userHeadIndex[userId];
            if (sensorId >= 0 && sensorId < sensors.Count)
            {
                return sensors[sensorId];
            }
            else
            {
                return new getReal3D.Sensor { rotation = Quaternion.identity, position = Vector3.zero };
            }
        }

        /// <summary>
        /// The wand transform for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        public static getReal3D.Sensor wandForUser(uint userId)
        {
            System.Diagnostics.Debug.Assert(userWandIndex != null);
            Update();
            if (userId >= userWandIndex.Length)
            {
                throw new IndexOutOfRangeException("User ID is out of range in Input.wandForUser.");
            }
            int sensorId = userWandIndex[userId];
            if (sensorId >= 0 && sensorId < sensors.Count)
            {
                return sensors[sensorId];
            }
            else
            {
                return new getReal3D.Sensor { rotation = Quaternion.identity, position = Vector3.zero };
            }
        }

        internal static void reloadMappings()
        {
            inputMappings.buttonMap.Clear();
            inputMappings.valuatorMap.Clear();
            inputMappings.sensorMap.Clear();
            inputMappings.setupMappingFromXML(Settings.get().mappingConfigContent);
        }

        static Input()
        {
            getReal3D.InputManager.Init();
            if (inputManager == null)
                inputManager = InputManager.Instance;

            wandIndex = Plugin.getWandIndex();
            headIndex = Plugin.getHeadIndex();

            sensors = new List<getReal3D.Sensor>();
            cameras = new List<getReal3D.Sensor>();
            valuators = new List<float>();
            buttons = new List<int>();
            prevButtons = new List<int>();

            inputMappings = new InputMappings();

            inputMappings.setupMappingFromXML(Settings.get().mappingConfigContent);

            // read xml config here
            string appConfig = getReal3D.Plugin.getApplicationConfig();
            Plugin.debug("AppConfigXML: " + appConfig);
            if (appConfig.Length > 0)
                inputMappings.setupMappingFromXML(appConfig);
            else
                Plugin.warning("APP_CONFIG does not contain UNITY element");

            reportMappings();

            m_isRunningViaGetReal3D = Plugin.isDistrib();

            Update();
        }

        private static void reportMappings()
        {
            foreach (KeyValuePair<string, InputMapping> pair in inputMappings.buttonMap)
                Plugin.debug("getReal3D.Input: button " + pair.Key + " = " + pair.Value.ToString());
            foreach (KeyValuePair<string, InputMapping> pair in inputMappings.valuatorMap)
                Plugin.debug("getReal3D.Input: axis " + pair.Key + " = " + pair.Value.ToString());
            foreach (KeyValuePair<string, InputMapping> pair in inputMappings.sensorMap)
                Plugin.debug("getReal3D.Input: sensor " + pair.Key + " = " + pair.Value.ToString());
        }

        /// <summary>
        /// Update trackd state. Called internally by getReal3D scripts.
        /// </summary>
        public static void Update()
        {
            inputManager.UpdateIfNeeded();
            if (userWandIndex == null || userHeadIndex == null)
            {
                uint userCount = getReal3D.Plugin.getUserCount();
                userWandIndex = new int[userCount];
                userHeadIndex = new int[userCount];
                for (uint i = 0; i < userCount; ++i)
                {
                    userWandIndex[i] = getReal3D.Plugin.getWandIndexForUser(i);
                    userHeadIndex[i] = getReal3D.Plugin.getHeadIndexForUser(i);
                }
            }
            if (lastUpdateFrame < Time.frameCount)
            {
                lastUpdateFrame = Time.frameCount;
                getReal3D.Plugin.getSensors(sensors);
                if (headIndex >= 0 && headIndex < sensors.Count)
                    headSensor = sensors[headIndex];
                else
                    getReal3D.Plugin.getSensorPositionRotation(headIndex, ref headSensor.position, ref headSensor.rotation);
                if (wandIndex >= 0 && wandIndex < sensors.Count)
                    wandSensor = sensors[wandIndex];
                else
                    getReal3D.Plugin.getSensorPositionRotation(wandIndex, ref wandSensor.position, ref wandSensor.rotation);
                getReal3D.Plugin.getControllerValuators(valuators);
                prevButtons.Clear();
                prevButtons.AddRange(buttons);
                getReal3D.Plugin.getControllerButtons(buttons);
                getReal3D.Plugin.getCameraSensors(cameras);

                byte[] temp = m_lastKeyboardState;
                m_lastKeyboardState = m_keyboardState;
                m_keyboardState = temp;

                temp = m_lastButtonState;
                m_lastButtonState = m_buttonState;
                m_buttonState = temp;

                if (!m_masterInputMatchSize.HasValue)
                {
                    m_masterInputMatchSize =
                        Plugin.getMasterAxisCount() == m_axisState.Length &&
                        Plugin.getMasterKeyCount() == m_keyboardState.Length &&
                        Plugin.getMasterButtonCount() == m_buttonState.Length;
                    if (!m_masterInputMatchSize.Value)
                    {
                        getReal3D.Plugin.error("Axis count or key count is different from what expected!");
                        getReal3D.Plugin.error("Got " + Plugin.getMasterAxisCount().ToString() + " axis while we expected " + m_axisState.Length + ".");
                        getReal3D.Plugin.error("Got " + Plugin.getMasterKeyCount().ToString() + " keys while we expected " + m_keyboardState.Length + ".");
                        getReal3D.Plugin.error("Got " + Plugin.getMasterButtonCount().ToString() + " buttons while we expected " + m_buttonState.Length + ".");
                    }
                }
                if (m_masterInputMatchSize.Value)
                {
                    getReal3D.Plugin.getMasterInputs(m_keyboardState, m_buttonState, m_axisState);
                }
                if (OnInputUpdated != null)
                {
                    OnInputUpdated();
                }
            }
        }

        internal unsafe static bool getPackedNamedButtons(byte* buf, int count)
        {
            bool changed = false;
            if (count != inputMappings.buttons.Count)
            {
                throw new Exception($"Expected {inputMappings.buttons.Count} buttons, got {count}.");
            }
            int idx = 0;
            foreach (var button in inputMappings.buttons)
            {
                var dst = button.getButtonValue(buttons, m_keyboardState, m_buttonState) ? (byte)0xff : (byte)0;
                changed |= dst != buf[idx];
                buf[idx] = dst;
                ++idx;
            }
            return changed;
        }

        internal unsafe static bool getPackedNamedValuators(float* buf, int count)
        {
            bool changed = false;
            if (count != inputMappings.valuators.Count)
            {
                throw new Exception($"Expected {inputMappings.valuators.Count} valuators, got {count}.");
            }
            int idx = 0;
            foreach (var axis in inputMappings.valuators)
            {
                var dst = axis.getAxisValue(valuators, m_keyboardState, m_axisState, m_buttonState, false);
                changed |= dst != buf[idx];
                buf[idx] = dst;
                ++idx;
            }
            return changed;
        }

        internal unsafe static bool getPackedNamedSensorsPositions(float* buf, int count)
        {
            bool changed = false;
            if (count != inputMappings.sensors.Count)
            {
                throw new Exception($"Expected {inputMappings.sensors.Count} sensors, got {count}.");
            }
            int idx = 0;
            foreach (var sensor in inputMappings.sensors)
            {
                var sensorValue = sensors[sensor.index];
                changed |= buf[idx] != sensorValue.position.x;
                buf[idx++] = sensorValue.position.x;
                changed |= buf[idx] != sensorValue.position.y;
                buf[idx++] = sensorValue.position.y;
                changed |= buf[idx] != sensorValue.position.z;
                buf[idx++] = sensorValue.position.z;
            }
            return changed;
        }

        internal unsafe static bool getPackedNamedSensorsRotations(float* buf, int count)
        {
            bool changed = false;
            if (count != inputMappings.sensors.Count)
            {
                throw new Exception($"Expected {inputMappings.sensors.Count} sensors, got {count}.");
            }
            int idx = 0;
            foreach (var sensor in inputMappings.sensors)
            {
                var sensorValue = sensors[sensor.index];
                changed |= buf[idx] != sensorValue.rotation.x;
                buf[idx++] = sensorValue.rotation.x;
                changed |= buf[idx] != sensorValue.rotation.y;
                buf[idx++] = sensorValue.rotation.y;
                changed |= buf[idx] != sensorValue.rotation.z;
                buf[idx++] = sensorValue.rotation.z;
                changed |= buf[idx] != sensorValue.rotation.w;
                buf[idx++] = sensorValue.rotation.w;
            }
            return changed;
        }

        internal unsafe static bool getPackedNamedSticks(float* buf, int count)
        {
            bool changed = false;
            if (count != inputMappings.sticks.Count)
            {
                throw new Exception($"Expected {inputMappings.sticks.Count} sticks, got {count}.");
            }
            int idx = 0;
            foreach (var stick in inputMappings.sticks)
            {
                var dst = GetStick(stick.name);
                changed |= dst.x != buf[idx + 0];
                changed |= dst.y != buf[idx + 1];
                buf[idx++] = dst.x;
                buf[idx++] = dst.y;
            }
            return changed;
        }

        /// <summary>
        /// Get a Button by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The button name</param>
        /// <returns>True if down</returns>
        public static bool GetButton(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.buttonMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no button named: " + name);
                return false;
            }
            bool val = data.getButtonValue(buttons, m_keyboardState, m_buttonState);
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!val && !m_isRunningViaGetReal3D && !string.IsNullOrEmpty(data.unityName))
            {
                try
                {
                    val = UnityEngine.Input.GetButton(data.unityName);
                }
                catch (UnityEngine.UnityException) { }
            }
#endif
            return val;
        }

        /// <summary>
        /// Get a Button by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The button name</param>
        /// <returns>True if the button went up this frame</returns>
        public static bool GetButtonUp(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.buttonMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no button named: " + name);
                return false;
            }
            bool val = !data.getButtonValue(buttons, m_keyboardState, m_buttonState) && data.getButtonValue(prevButtons, m_lastKeyboardState, m_lastButtonState);
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!val && !m_isRunningViaGetReal3D && !string.IsNullOrEmpty(data.unityName))
            {
                try
                {
                    val = UnityEngine.Input.GetButtonUp(data.unityName);
                }
                catch (UnityEngine.UnityException) { }
            }
#endif
            return val;
        }

        /// <summary>
        /// Get a Button by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The button name</param>
        /// <returns>True if the button went down this frame</returns>
        public static bool GetButtonDown(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.buttonMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no button named: " + name);
                return false;
            }
            bool val = data.getButtonValue(buttons, m_keyboardState, m_buttonState) && !data.getButtonValue(prevButtons, m_lastKeyboardState, m_lastButtonState);
#if ENABLE_LEGACY_INPUT_MANAGER
            if (!val && !m_isRunningViaGetReal3D && !string.IsNullOrEmpty(data.unityName))
            {
                try
                {
                    val = UnityEngine.Input.GetButtonDown(data.unityName);
                }
                catch (UnityEngine.UnityException) { }
            }
#endif
            return val;
        }

        /// <summary>
        /// Get a Axis (trackd valuator) by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The axis name</param>
        /// <returns>The value</returns>
        public static float GetAxis(string name)
        {
            Update();
            InputMapping data = null;
            try
            {
                if (!inputMappings.valuatorMap.TryGetValue(name, out data))
                {
                    Plugin.warning("getReal3D InputManager has no axis named: " + name);
                    return 0f;
                }
            }
            catch (Exception)
            {
                Plugin.warning("getReal3D InputManager has no axis named: " + name);
                return 0f;
            }
            float rval = data.getAxisValue(valuators, m_keyboardState, m_axisState, m_buttonState, false);
#if ENABLE_LEGACY_INPUT_MANAGER
            if (rval == 0 && !m_isRunningViaGetReal3D && !string.IsNullOrEmpty(data.unityName))
            {
                try
                {
                    rval = UnityEngine.Input.GetAxis(data.unityName);
                }
                catch (UnityEngine.UnityException) { }
            }
#endif
            return rval;
        }

        /// <summary>
        /// Get a Axis (trackd valuator) by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The axis name</param>
        /// <returns>The value without configured deadzone or inversion</returns>
        public static float GetAxisRaw(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.valuatorMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no axis named: " + name);
                return 0f;
            }
            float rval = data.getAxisValue(valuators, m_keyboardState, m_axisState, m_buttonState, true);
#if ENABLE_LEGACY_INPUT_MANAGER
            if (rval == 0 && !m_isRunningViaGetReal3D && !string.IsNullOrEmpty(data.unityName))
            {
                try
                {
                    rval = UnityEngine.Input.GetAxisRaw(data.unityName);
                }
                catch (UnityEngine.UnityException) { }
            }
#endif
            return rval;
        }

        /// <summary>
        /// Return the axis value.
        /// </summary>
        public static float GetAxis(AxisCode axisCode)
        {
            if (axisCode < 0 || axisCode >= AxisCode.Count)
            {
                throw new IndexOutOfRangeException("In getReal3D.Input.GetAxis.");
            }
            return m_axisState[(int)axisCode];
        }

        /// <summary>
        ///  Get the stick value.
        /// </summary>
        public static Vector2 GetStick(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.stickMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no stick named: " + name);
                return Vector2.zero;
            }
            return new Vector2(GetAxis(data.horizontalAxis), GetAxis(data.verticalAxis));
        }

        /// <summary>
        /// Return true if the corresponding key is pushed.
        /// </summary>
        public static bool GetKey(VirtualKeyCode keyCode)
        {
            if (keyCode < 0 || keyCode >= VirtualKeyCode.Count)
            {
                throw new IndexOutOfRangeException("In getReal3D.Input.GetKey.");
            }
            return m_keyboardState[(int)keyCode] != 0;
        }

        /// <summary>
        /// Return true if the corresponding key has been pushed during this frame.
        /// </summary>
        public static bool GetKeyDown(VirtualKeyCode keyCode)
        {
            if (keyCode < 0 || keyCode >= VirtualKeyCode.Count)
            {
                throw new IndexOutOfRangeException("In getReal3D.Input.GetKeyDown.");
            }
            return m_keyboardState[(int)keyCode] != 0 && m_lastKeyboardState[(int)keyCode] == 0;
        }

        /// <summary>
        /// Return true if the corresponding key has been released during this frame.
        /// </summary>
        public static bool GetKeyUp(VirtualKeyCode keyCode)
        {
            if (keyCode < 0 || keyCode >= VirtualKeyCode.Count)
            {
                throw new IndexOutOfRangeException("In getReal3D.Input.GetKeyUp.");
            }
            return m_keyboardState[(int)keyCode] == 0 && m_lastKeyboardState[(int)keyCode] != 0;
        }

        /// <summary>
        /// Get a trackd sensor by name (see getReal3D configuration).
        /// </summary>
        /// <param name="name">The sensor name</param>
        /// <returns>The sensor transform</returns>
        public static getReal3D.Sensor GetSensor(string name)
        {
            Update();
            InputMapping data = null;
            if (!inputMappings.sensorMap.TryGetValue(name, out data))
            {
                Plugin.warning("getReal3D InputManager has no sensor named: " + name);
                return new getReal3D.Sensor();
            }
            if (data.index >= sensors.Count || data.index < 0) return new getReal3D.Sensor();
            return sensors[data.index];
        }

        /// <summary>
        /// Get a camera sensor by index (for this Unity instance, see getReal3D configuration).
        /// </summary>
        /// <param name="i">The camera index</param>
        /// <returns>The camera transform</returns>
        public static getReal3D.Sensor GetCameraSensor(uint i = 0)
        {
            Update();
            if (i < cameras.Count)
            {
                return cameras[(int)i];
            }
            return new getReal3D.Sensor();
        }

        /// <summary>
        /// Get a camera user-centered projection matrix by index (for this Unity instance, see getReal3D configuration).
        /// </summary>
        /// <param name="i">The camera index</param>
        /// <param name="far">The distance to the camera far plane</param>
        /// <param name="near">The distance to the camera new plane</param>
        /// <returns>The camera projection matrix</returns>
        public static Matrix4x4 GetCameraProjection(uint i = 0, float far = 100f, float near = 0.3048f)
        {
            //Plugin.info("GetCameraProjection Time: " + Time.realtimeSinceStartup.ToString());
            Update();
            if (i < cameras.Count)
            {
                Matrix4x4 proj = new Matrix4x4();
                getReal3D.Plugin.getCameraProjectionMatrix(i, near, far, ref proj);
                return proj;
            }
            return Matrix4x4.identity;
        }

        /// <summary>
        /// Get a camera aspect ratio by index (for this Unity instance, see getReal3D configuration).
        /// </summary>
        /// <param name="i">The camera index</param>
        /// <returns>The camera projection aspect ratio</returns>
        public static float GetCameraAspectRatio(uint i = 0)
        {
            Update();
            if (i < cameras.Count)
            {
                return getReal3D.Plugin.getCameraAspectRatio(i);
            }
            return 0f;
        }

        /// <summary>
        /// Get a camera field of view by index (for this Unity instance, see getReal3D configuration).
        /// </summary>
        /// <param name="i">The camera index</param>
        /// <returns>The camera field of view</returns>
        public static float GetCameraFieldOfView(uint i = 0)
        {
            //Plugin.info("GetCameraProjection Time: " + Time.realtimeSinceStartup.ToString());
            Update();
            if (i < cameras.Count)
            {
                return getReal3D.Plugin.getCameraFieldOfView(i);
            }
            return 0f;
        }

    }
}
