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

#if ENABLE_INPUT_SYSTEM

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace getReal3D
{

    /// <summary>
    /// getReal3D implementation of an InputDevice.
    /// </summary>
    /// When created, a new layout is built depending on the mapping configuration.
    /// The device should show under the Other/getReal3D category.
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal class GetReal3DDevice
    {
        private class DeviceInfo
        {
            public InputDevice device;
            public InputEventPtr eventPtr;
            public NativeArray<byte> buffer;
            public unsafe StateEvent* stateEvent;
        }

        private const string s_interfaceName = "getReal3D Interface";
        private const string s_productName = "getReal3D";
        private const string s_layoutName = "getReal3D Controller";
        private static FourCC s_fourCC = new FourCC("GR3D");
        private static DeviceInfo s_deviceInfo = null;
        private static int s_buttonCount = 0;
        private static int s_axisCount = 0;
        private static uint s_axisOffset = 0;
        private static int s_sensorCount = 0;
        private static uint s_sensorOffset = 0;
        private static uint s_rotationOffset = 0;
        private static int s_stickCount = 0;
        private static uint s_stickOffset = 0;
        private static uint s_endOffset = 0;

#if UNITY_EDITOR
        static GetReal3DDevice()
        {
            Initialize();
            EditorApplication.playModeStateChanged += playModeStateChanged;
            Settings.get().OnSettingsChanged += configChanged;
            AssemblyReloadEvents.beforeAssemblyReload += beforeAssemblyReload;
        }

        private static void playModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                    CreateDevice(); break;
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.ExitingEditMode:
                    RemoveDevice(); break;
            }
        }

        private static void beforeAssemblyReload()
        {
            RemoveDevice();
        }
        private static void afterAssemblyReload()
        {
            CreateDevice();
        }
#endif

        public static void configChanged()
        {
            try
            {
                Plugin.debug("Config changed");
                RemoveDevice();
                if (InputSystem.ListLayouts().Contains(s_layoutName))
                {
                    InputSystem.RemoveLayout(s_layoutName);
                }
                CreateDevice();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            InputSystem.onFindLayoutForDevice += FindLayoutForDevice;
            InputSystem.onDeviceChange += DeviceChange;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            CreateDevice();
        }

        private static uint AlignToMultipleOf(uint number, uint alignment)
        {
            var remainder = number % alignment;
            if (remainder == 0)
                return number;

            return number + alignment - remainder;
        }

        private static string FindLayoutForDevice(ref InputDeviceDescription description,
                                                  string matchedLayout,
                                                  InputDeviceExecuteCommandDelegate executeDeviceCommand)
        {
            if (description.interfaceName != s_interfaceName)
            {
                return null;
            }

            if (matchedLayout == s_layoutName && InputSystem.ListLayouts().Contains(s_layoutName))
            {
                return s_layoutName;
            }
            else if (matchedLayout != s_layoutName && !string.IsNullOrEmpty(matchedLayout))
            {
                return null;
            }

            var matcher = InputDeviceMatcher.FromDeviceDescription(description);

            var product = description.product;

            var inputMappings = new InputMappings();
            inputMappings.setupMappingFromXML(Settings.get().mappingConfigContent);

            if (getReal3D.Plugin.initialized())
            {
                string appConfig = getReal3D.Plugin.getApplicationConfig();
                Plugin.debug("AppConfigXML: " + appConfig);
                if (appConfig.Length > 0)
                {
                    inputMappings.setupMappingFromXML(appConfig);
                }
                else
                {
                    Plugin.warning("APP_CONFIG does not contain UNITY element");
                }
            }

            Plugin.debug("Creating getReal3D device layout.");

            InputSystem.RegisterLayout<AxisWithButtons>();

            InputSystem.RegisterLayoutBuilder(
                () =>
                {
                    var builder = new InputControlLayout.Builder()
                        .WithDisplayName(product)
                        .WithFormat(s_fourCC);

                    uint byteOffset = 0;
                    foreach (var button in inputMappings.buttons)
                    {
                        builder.AddControl(button.name).WithLayout("Button").
                            WithSizeInBits(8).
                            WithByteOffset(byteOffset);
                        byteOffset++;
                    }
                    s_buttonCount = inputMappings.buttons.Count;
                    byteOffset = AlignToMultipleOf(byteOffset, 4);
                    s_axisOffset = byteOffset;

                    foreach (var valuator in inputMappings.valuators)
                    {
                        builder.AddControl(valuator.name).
                            WithLayout("AxisWithButtons").
                            WithByteOffset(byteOffset);
                        byteOffset += 4;
                    }
                    s_axisCount = inputMappings.valuators.Count;
                    s_sensorOffset = byteOffset;

                    foreach (var sensor in inputMappings.sensors)
                    {
                        builder.AddControl(sensor.name + "Position").
                            WithLayout("Vector3").
                            WithByteOffset(byteOffset);
                        byteOffset += 4 * 3;
                    }
                    s_sensorCount = inputMappings.sensors.Count;
                    s_rotationOffset = byteOffset;

                    foreach (var sensor in inputMappings.sensors)
                    {
                        builder.AddControl(sensor.name + "Rotation").
                            WithLayout("Quaternion").
                            WithByteOffset(byteOffset);
                        byteOffset += 4 * 4;
                    }

                    s_stickOffset = byteOffset;
                    s_stickCount = inputMappings.sticks.Count;

                    foreach (var stick in inputMappings.sticks)
                    {
                        builder.AddControl(stick.name).
                            WithLayout("Stick").
                            WithByteOffset(byteOffset);
                        byteOffset += 4 * 2;
                    }

                    s_endOffset = byteOffset;

                    var layout = builder.Build();
                    return layout;
                },
                s_layoutName,
                matches: matcher);
            return s_layoutName;
        }

        private static void CreateDevice()
        {
            RemoveDevice();
            if (Settings.get().inputSystemController)
            {
                Plugin.debug("Adding getReal3D device");
                InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = s_interfaceName,
                    product = s_productName
                });

                // Need to do some cleanup here to avoid duplicated device when we reload the assembly
                RemoveDuplicates();
            }
        }

        private static void RemoveDuplicates()
        {
            Func<InputDevice, bool> isDuplicate = (device) =>
                device.description.product == s_productName && device.name != s_layoutName;
            foreach (var device in InputSystem.devices.Where(isDuplicate).ToArray())
            {
                Plugin.debug("Removing duplicated getReal3D device: " + device.ToString());
                InputSystem.RemoveDevice(device);
            }
        }

        private static void RemoveDevice()
        {
            for (InputDevice device; (device = FindDevice()) != null;)
            {
                Plugin.debug("Removing getReal3D device  " + device.ToString());
                InputSystem.RemoveDevice(device);
            }
        }

        private static bool IsOurDevice(InputDevice device)
        {
            return device.description.product == s_productName;
        }

        private static void DeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (IsOurDevice(device))
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        DeviceAdded(device);
                        break;
                    case InputDeviceChange.Removed:
                        DeviceRemoved(device);
                        break;
                    case InputDeviceChange.HardReset:
                    case InputDeviceChange.SoftReset:
                        DeviceRemoved(device);
                        DeviceAdded(device);
                        break;
                }
            }
        }

        private unsafe static void DeviceAdded(InputDevice device)
        {
            Plugin.debug("getReal3D device added: " + device.ToString());
            s_deviceInfo = new DeviceInfo();
            s_deviceInfo.device = device;
            if (Application.isPlaying)
            {
                s_deviceInfo.buffer = StateEvent.From(device, out s_deviceInfo.eventPtr,
                    Allocator.Persistent);
                s_deviceInfo.stateEvent = StateEvent.From(s_deviceInfo.eventPtr);
                Input.OnInputUpdated += UpdateDevice;
            }
        }

        private static void DeviceRemoved(InputDevice device)
        {
            Plugin.debug("getReal3D device removed: " + device.ToString());
            if (s_deviceInfo != null)
            {
                if (s_deviceInfo.buffer != null && s_deviceInfo.buffer.IsCreated)
                {
                    s_deviceInfo.buffer.Dispose();
                }
            }
            s_deviceInfo = null;
            if (Application.isPlaying)
            {
                Input.OnInputUpdated -= UpdateDevice;
            }
        }

        private static InputDevice FindDevice()
        {
            return InputSystem.devices.FirstOrDefault(x => IsOurDevice(x));
        }

        private static unsafe void UpdateDevice()
        {
            if (s_deviceInfo != null)
            {
                var buttons = (byte*)s_deviceInfo.stateEvent->state;
                if (s_buttonCount > s_axisOffset)
                {
                    throw new Exception("Unexpected StateEvent state size.");
                }
                var axes = (float*)(buttons + s_axisOffset);
                var positions = (float*)(buttons + s_sensorOffset);
                var rotations = (float*)(buttons + s_rotationOffset);
                var sticks = (float*)(buttons + s_stickOffset);
                if (s_endOffset != s_deviceInfo.stateEvent->stateSizeInBytes)
                {
                    throw new Exception("Unexpected StateEvent state size.");
                }
                bool changed = false;
                changed |= Input.getPackedNamedButtons(buttons, s_buttonCount);
                changed |= Input.getPackedNamedValuators(axes, s_axisCount);
                changed |= Input.getPackedNamedSensorsPositions(positions, s_sensorCount);
                changed |= Input.getPackedNamedSensorsRotations(rotations, s_sensorCount);
                changed |= Input.getPackedNamedSticks(sticks, s_stickCount);
                if (changed)
                {
                    s_deviceInfo.eventPtr.time = InputState.currentTime;
                    InputSystem.QueueEvent(s_deviceInfo.eventPtr);
                }

            }
        }

    }
}

#endif
