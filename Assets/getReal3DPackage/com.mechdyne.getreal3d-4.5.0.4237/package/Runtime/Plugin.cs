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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEditor;
using UnityEngine;

// The getReal3D Namespace contains all the public interface to the getReal3D Assembly.
namespace getReal3D
{
    internal interface IUpdateCallback
    {
        void preUpdate();
        void postUpdate();
    }
    /// <summary>
    /// The base wrapper class for the getReal3D plugin. Many of the functions here are low-level and need not be called directly by end-users.
    /// </summary>
    ///
    public static class Plugin
    {
        private const string assetConfigFile = "getReal3D_config";

        private static int m_numButtons;
        private static int m_numValuators;
        private static int m_numSensors;
        private static int m_numCameras;

        private static NativePlugin grPlugin = null;
        private static System.Object lockObject = new System.Object();
        private static uint m_clusterId = 0;
        private static uint m_nodeCount = 0;
        private static XmlElement s_appConfigXml;
        private static HashSet<IUpdateCallback> s_callbacks = new HashSet<IUpdateCallback>();

        /// <summary>
        /// Returns the ID of the node in the cluster.
        /// </summary>
        static public uint ClusterId
        {
            get { return m_clusterId; }
        }

        /// <summary>
        /// Returns the number of the nodes (i.e. Unity instances) in the cluster.
        /// </summary>
        static public uint NodeCount
        {
            get { return m_nodeCount; }
        }

        [DllImport("user32.dll")]
        static extern int MessageBox(int hWnd, string text,
                                     string caption, int type);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        /// Static initializer.
        /// </summary>
        static Plugin()
        {
            lock (lockObject)
            {
                if (Time.frameCount != 0 && Application.isPlaying)
                {
                    if (!Application.isEditor)
                    {
                        UnityEngine.Debug.LogError("getReal3D.initVR() must be called during the first frame.");
                        MessageBox(0, "getReal3D.initVR() must be called during the first frame.", "getReal3D error", 0);
                    }
                }

                grPlugin = NativePluginFactory.Create();

                string codebase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                if (codebase.StartsWith("file:///"))
                {
                    codebase = codebase.Substring(8);
                }

                string path = System.IO.Path.GetDirectoryName(codebase);
                string pluginsDir = path + "\\..\\Plugins";
                if (IntPtr.Size == 8)
                {
                    string plugin64FullName = pluginsDir + "/x86_64/" + grPlugin.pluginFileName() + ".dll";
                    if (File.Exists(plugin64FullName))
                    {
                        pluginsDir += "\\x86_64";
                    }
                }

                SetDllDirectory(pluginsDir);
                grPlugin.initialized();
                SetDllDirectory(null);
            }
            m_numButtons = 0;
            m_numValuators = 0;
            m_numSensors = 0;
            m_numCameras = 0;
            Plugin.debug("getReal3D.Plugin init ...");
        }

        /// <summary>
        /// Initialize VR configuration. Reads config file and initializes components.
        /// </summary>
        /// <returns>True if properly initialized.</returns>
        public static bool initVR()
        {
# if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += deinit;
# endif
            return InternalInitVR(true);
        }

        internal static bool InternalInitVR(bool autoInit)
        {
            if (initialized()) return true;

            getReal3D.Plugin.debug("InitVR");

            int error;
            string path = Environment.GetEnvironmentVariable("GETREAL3D_CONFIG");
            string content = null;
            if (path == null || path.Length == 0)
            {
                if (Application.isEditor)
                {
                    getReal3D.Plugin.info("Running getReal3D from Unity editor using configuration file from assets.\n");
                }
                else
                {
                    getReal3D.Plugin.warning("getReal3D.Plugin::initVR() : Could not retrieve getReal3D config from environment variable GETREAL3D_CONFIG.\n");
                }
                UnityEngine.Object res = Resources.Load(assetConfigFile);
                if (res)
                {
                    getReal3D.Plugin.debug("Loading config: " + assetConfigFile);
                }
                else
                {
                    getReal3D.Plugin.debug("Loading config: " + Settings.get().config);
                    res = Resources.Load(Settings.get().config);
                    if (!res)
                    {
                        getReal3D.Plugin.error("getReal3D.Plugin::initVR() : Unable to load default configuration from resources.\n");
                        return false;
                    }
                }
                if (res is getReal3DConfig)
                {
                    getReal3DConfig config = res as getReal3DConfig;
                    debug("Retrieved config from getReal3DConfig asset.");
                    content = config.content;
                }
                else if (res is TextAsset)
                {
                    TextAsset config = res as TextAsset;
                    debug("Retrieved config from TextAsset.");
                    content = config.text;
                }
                else
                {
                    Plugin.error("getReal3D.Plugin::initVR() : Could not retrieve data from default configuration: data type is not valid.");
                    return false;
                }
            }
            else
            {
                getReal3D.Plugin.debug("getReal3D.Plugin::initVR() : Retrieved getReal3D config from environment \"" + path + "\"");
            }

            string parameterConfigFile = GetParam("gr3d-config");

            if (path != null && !File.Exists(path))
            {
                getReal3D.Plugin.error("Unable to load getReal3D configuration file: " + Path.GetFullPath(path) + ".");
                return false;
            }
            else if (path != null)
            {
                getReal3D.Plugin.debug("getReal3D.Plugin::initVR() : Attempting to load getReal3D config file \"" + Path.GetFullPath(path) + "\"");
                error = grPlugin.init(path, false);
            }
            else if (parameterConfigFile != null && !File.Exists(parameterConfigFile))
            {
                getReal3D.Plugin.error("Unable to load getReal3D configuration file: " + Path.GetFullPath(parameterConfigFile) + ".");
                return false;
            }
            else if (parameterConfigFile != null)
            {
                getReal3D.Plugin.debug("getReal3D.Plugin::initVR() : Attempting to load getReal3D config in parameter \"" + Path.GetFullPath(parameterConfigFile) + "\"");
                error = grPlugin.init(parameterConfigFile, true);
            }
            else if (content != null && content.Length != 0)
            {
                getReal3D.Plugin.debug("Loading from string");
                error = grPlugin.initWithString(content, true);
            }
            else
            {
                getReal3D.Plugin.error("Unable to find config file.");
                return false;
            }

            if (!Application.isEditor)
            {
                getReal3D.Plugin.debug("Unity version: " + Application.unityVersion);
            }

            if (error != 1)
            {
                string errorMessage = "Problem occurred when initializing getReal3D: ";
                switch (error)
                {
                    case -1: errorMessage += " trackd error"; break;
                    case -2: errorMessage += " license check failed."; break;
                    case -3: errorMessage += " configuration file is missing."; break;
                    case -4: errorMessage += " configuration file is invalid."; break;
                    case -5: errorMessage += " cluster error."; break;
                    case -255: errorMessage += " unexpected error (" + error + ")."; break;
                }
                getReal3D.Plugin.error(errorMessage);
                return false;
            }

            Plugin.debug("gr_plugin Initialized: " + initialized().ToString());
            s_appConfigXml = null;
            m_numButtons = grPlugin.getNumButtons();
            m_numValuators = grPlugin.getNumValuators();
            m_numSensors = grPlugin.getNumSensors();
            m_numCameras = grPlugin.getNumberOfCameras();
            Plugin.debug("Found " + m_numButtons + " buttons " + m_numValuators + " valuators " + m_numSensors + " sensors.");

            m_clusterId = grPlugin.getClusterID();
            m_nodeCount = (uint)grPlugin.getConfigCount();
            UnityEngine.Random.InitState(grPlugin.getRandomSeed());
            Plugin.debug("Initialized random seed to " + grPlugin.getRandomSeed().ToString() + ".");

            //plugin successfully initialized
            setupSpeakers();
            setupWindow();
            if (autoInit)
            {
                getReal3D.InputManager.Init();
                getReal3D.Input.Init();
                getReal3D.Scale.Init();
                getReal3D.GUI.Init();
                getReal3D.RpcManager.Init();
#if ENABLE_INPUT_SYSTEM
                GetReal3DDevice.configChanged();
#endif
            }
            return true;
        }

        private static string GetParam(string paramName)
        {
            string[] possibleNames = {
                "-" + paramName,
                "--" + paramName
            };
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (possibleNames.Contains(args[i]) && i != args.Length - 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Cleanup plugin when ready to exit the game. Call this if using the Plugin directly instead of Mechdyne supplied scripts.
        /// </summary>
        public static void deinit()
        {
            Plugin.debug("deinit ...");
            m_numButtons = 0;
            m_numValuators = 0;
            m_numSensors = 0;
            m_numCameras = 0;
            grPlugin.deinit();
        }

        /// <summary>
        /// Shutdown the whole cluster application via the getReal3D daemon.
        /// </summary>
        public static void clusterShutdown()
        {
            grPlugin.clusterShutdown();
        }

        /// <summary>
        /// Check if the plugin has been successfully initialized.
        /// </summary>
        /// <returns>True if the plugin is initialized.</returns>
        public static bool initialized()
        {
            if (grPlugin == null) return false;
            return grPlugin.initialized();
        }

        /// <summary>
        /// Update trackd inputs. Synchronize trackd inputs across the cluster, if appropriate.
        /// </summary>
        /// <returns>Returns false if a node in the cluster has exited.</returns>
        public static bool updateData()
        {
            foreach (var callback in s_callbacks)
            {
                callback.preUpdate();
            }
            bool ret = grPlugin.update();
            if (!ret && isDistrib())
            {
                Plugin.debug("Cluster shutting down ...");
                stopDistrib();
            }
            foreach (var callback in s_callbacks)
            {
                callback.postUpdate();
            }

            return ret;
        }

        /// <summary>
        /// Check if the configuration file requests that the main Unity window be hidden. Standalone only.
        /// </summary>
        /// <returns>The value of the confiuration option.</returns>
        public static bool getAppWindowHidden()
        {
            if (!initialized()) return false;
            return grPlugin.getAppWindowHidden();
        }

        /// <summary>
        /// Get the height of this Unity instance window.
        /// </summary>
        /// <returns>The height in pixels</returns>
        public static int getAppWindowHeight()
        {
            if (!initialized()) return 0;
            return grPlugin.getAppWindowHeight();
        }

        /// <summary>
        /// Get the width of this Unity instance window.
        /// </summary>
        /// <returns>The width in pixels</returns>
        public static int getAppWindowWidth()
        {
            if (!initialized()) return 0;
            return grPlugin.getAppWindowWidth();
        }

        /// <summary>
        /// Get the height of the master Unity instance window.
        /// </summary>
        /// <returns>The height in pixels</returns>
        public static int getMasterWindowHeight()
        {
            if (!initialized()) return 0;
            return grPlugin.getMasterWindowHeight();
        }

        /// <summary>
        /// Get the width of the master Unity instance window.
        /// </summary>
        /// <returns>The width in pixels</returns>
        public static int getMasterWindowWidth()
        {
            if (!initialized()) return 0;
            return grPlugin.getMasterWindowWidth();
        }

        /// <summary>
        /// Query the number of cameras configured in this Unity instance.
        /// </summary>
        /// <returns>The number of cameras</returns>
        public static int getNumberOfCameras()
        {
            if (!initialized()) return 1;
            return grPlugin.getNumberOfCameras();
        }

        /// <summary>
        /// Query if a given camera should show the game UI.
        /// </summary>
        /// <param name="_cameraIndex">The index of the camrea for this Unity instance.</param>
        /// <returns>True if UI should be shown</returns>
        public static bool getCameraShowsUI(uint _cameraIndex)
        {
            if (!initialized()) return true;
            return grPlugin.getCameraShowsUI(_cameraIndex);
        }

        /// <summary>
        /// Query if a given camera should use Render To Texture.
        /// </summary>
        /// <param name="_cameraIndex">The index of the camrea for this Unity instance.</param>
        /// <returns>True if camera should use Render To Texture</returns>
        public static bool getCameraUseRTT(uint _cameraIndex)
        {
            if (!initialized()) return false;
            return grPlugin.getCameraUseRTT(_cameraIndex);
        }

        /// <summary>
        /// Query the width in pixels of a given camera
        /// </summary>
        /// <param name="_cameraIndex">The index of the camrea for this Unity instance.</param>
        /// <returns>Width in pixels. -1 if cameraIndex is not valid.</returns>
        public static int getCameraWidth(uint _cameraIndex)
        {
            if (!initialized()) return Screen.width;
            int retVal = grPlugin.getCameraWidth(_cameraIndex);
            if (retVal == 0)
            {
                Plugin.debug("getReal3D.Plugin:getCameraWidth failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Query the height in pixels of a given camera
        /// </summary>
        /// <param name="_cameraIndex">The index of the camrea for this Unity instance.</param>
        /// <returns>Height in pixels. -1 if cameraIndex is not valid.</returns>
        public static int getCameraHeight(uint _cameraIndex)
        {
            if (!initialized()) return Screen.height;
            int retVal = grPlugin.getCameraHeight(_cameraIndex);
            if (retVal == 0)
            {
                Plugin.debug("getReal3D.Plugin:getCameraHeight failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the normalized viewport Rect for a given camera
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <param name="_viewport">A reference to the Rect to hold the viewport.</param>
        /// <returns>1 if the camera index is valid, 0 for an invalid index</returns>
        public static int getCameraViewport(uint _cameraIndex, ref Rect _viewport)
        {
            float[] tmpFloatArray = new float[4];
            int retVal = grPlugin.getCameraViewport(_cameraIndex, tmpFloatArray);

            if (retVal == 1)
            {
                _viewport.x = tmpFloatArray[0];
                _viewport.y = tmpFloatArray[1];
                _viewport.width = tmpFloatArray[2];
                _viewport.height = tmpFloatArray[3];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin:getCameraViewport failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the aspect ratio for a given camera.
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <returns>Returns the aspect ratio.</returns>
        public static float getCameraAspectRatio(uint _cameraIndex)
        {
            float retVal = grPlugin.getCameraAspectRatio(_cameraIndex);

            if (retVal == 0)
            {
                Plugin.debug("getReal3D.Plugin:getCameraProjectionMatrix failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the field of view for a given camera.
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <returns>Returns the field of view.</returns>
        public static float getCameraFieldOfView(uint _cameraIndex)
        {
            float retVal = grPlugin.getCameraFieldOfView(_cameraIndex);

            if (retVal == 0)
            {
                Plugin.debug("getReal3D.Plugin:getCameraFieldOfView failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        private static float[] s_tmpFloatMatrix = new float[16];

        /// <summary>
        /// Get the user-centric projection matrix for a given camera (does not consider navigation).
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <param name="near">The near plane distance in meters.</param>
        /// <param name="far">The far plane distance in meters.</param>
        /// <param name="_matrix">A reference to the Matrix4x4 to hold the projection.</param>
        /// <returns>Returns 1 on success, 0 on failure (invalid cameraIndex).</returns>
        public static int getCameraProjectionMatrix(uint _cameraIndex, float near, float far, ref Matrix4x4 _matrix)
        {
            int retVal = grPlugin.getCameraProjectionMatrix(_cameraIndex, near, far, s_tmpFloatMatrix);
            if (retVal == 1)
            {
                //Unity uses row major matrices so we transpose the matrix when we write the data (i,j) -> (j,i)
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        _matrix[j, i] = s_tmpFloatMatrix[i * 4 + j];
                    }
                }
            }
            else
            {
                Plugin.debug("getReal3D.Plugin:getProjectionMatrixForCamera failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the rotation of the given camera that aligns it with its configured screen geometry.
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <param name="_quat">A reference to a Quaternion that will hold the camera rotation.</param>
        /// <returns>Returns 1 on success, 0 on failure (invalid cameraIndex).</returns>
        public static int getCameraRotation(uint _cameraIndex, ref Quaternion _quat)
        {
            int retVal = grPlugin.getCameraRotation(_cameraIndex, s_tmpRotFloatArray);

            if (retVal == 1)
            {
                _quat[0] = -s_tmpRotFloatArray[0];
                _quat[1] = -s_tmpRotFloatArray[1];
                _quat[2] = s_tmpRotFloatArray[2];
                _quat[3] = s_tmpRotFloatArray[3];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin:getCameraRotation failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the position of the given camera (does not consider navigation).
        /// </summary>
        /// <param name="_cameraIndex">The index of the camera for this Unity instance.</param>
        /// <param name="_vector">A reference to a Vector3 that will hold the camera position.</param>
        /// <returns>Returns 1 on success, 0 on failure (invalid cameraIndex).</returns>
        public static int getCameraPosition(uint _cameraIndex, ref Vector3 _vector)
        {
            int retVal = grPlugin.getCameraPosition(_cameraIndex, s_tmpPosFloatArray);

            if (retVal == 1)
            {
                _vector[0] = s_tmpPosFloatArray[0];
                _vector[1] = s_tmpPosFloatArray[1];
                _vector[2] = -s_tmpPosFloatArray[2];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin:getCameraPosition failed for camera index:" + _cameraIndex);
            }
            return retVal;
        }

        /// <summary>
        /// Get the transforms for all cameras (managed by getReal3D) in this instance (does not consider navigation).
        /// </summary>
        /// <returns>A list of getReal3D Sensors (similar to a UnityEngine.Transform with scale 1).</returns>
        public static List<getReal3D.Sensor> getCameraSensors()
        {
            var res = new List<getReal3D.Sensor>(m_numCameras);
            getCameraSensors(res);
            return res;
        }

        /// Get the transforms for all cameras (managed by getReal3D) in this instance (does not consider navigation).
        public static void getCameraSensors(List<getReal3D.Sensor> sensors)
        {
            ResizeList(sensors, m_numCameras);
            getReal3D.Sensor tempCamera = new getReal3D.Sensor();
            for (uint i = 0; i < m_numCameras; i++)
            {
                getCameraPosition(i, ref tempCamera.position);
                getCameraRotation(i, ref tempCamera.rotation);
                sensors[(int)i] = tempCamera;
            }
        }

        /// <summary>
        /// Get the position and rotation of the trackd sensor set as the Head in the getReal3D configuration file.
        /// </summary>
        /// <param name="_headPosition">A reference to a Vector3 which will hold the head position</param>
        /// <param name="_headRotation">A reference to a Quaternion which will hold the head rotation</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public static int getHeadPositionRotation(ref Vector3 _headPosition, ref Quaternion _headRotation)
        {
            float[] tmpPosFloatArray = new float[3];
            float[] tmpRotFloatArray = new float[4];

            int retVal = grPlugin.getHeadPositionRotation(tmpPosFloatArray, tmpRotFloatArray);

            if (retVal == 1)
            {
                //trackd uses a RH system, we negate z when returning the position to be compatible with Unity's LH system
                _headPosition[0] = tmpPosFloatArray[0];
                _headPosition[1] = tmpPosFloatArray[1];
                _headPosition[2] = -tmpPosFloatArray[2];

                //trackd uses a RH system, we negate the z component of the quaternion when returning the rotation to be compatible with Unity's LH system
                _headRotation[0] = -tmpRotFloatArray[0];
                _headRotation[1] = -tmpRotFloatArray[1];
                _headRotation[2] = tmpRotFloatArray[2];
                _headRotation[3] = tmpRotFloatArray[3];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin: getHeadPositionRotation failed");
            }
            return retVal;
        }

        /// <summary>
        /// Get the position and rotation of the trackd sensor set as the Wand in the getReal3D configuration file.
        /// </summary>
        /// <param name="_wandPosition">A reference to a Vector3 which will hold the wand position</param>
        /// <param name="_wandRotation">A reference to a Quaternion which will hold the wand rotation</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public static int getWandPositionRotation(ref Vector3 _wandPosition, ref Quaternion _wandRotation)
        {
            float[] tmpPosFloatArray = new float[3];
            float[] tmpRotFloatArray = new float[4];

            int retVal = grPlugin.getWandPositionRotation(tmpPosFloatArray, tmpRotFloatArray);

            if (retVal == 1)
            {
                _wandPosition[0] = tmpPosFloatArray[0];
                _wandPosition[1] = tmpPosFloatArray[1];
                _wandPosition[2] = -tmpPosFloatArray[2];

                _wandRotation[0] = -tmpRotFloatArray[0];
                _wandRotation[1] = -tmpRotFloatArray[1];
                _wandRotation[2] = tmpRotFloatArray[2];
                _wandRotation[3] = tmpRotFloatArray[3];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin: getWandPositionAndRotation failed");
            }
            return retVal;
        }

        /// <summary>
        /// Get the number of tracked sensors reported by trackd.
        /// </summary>
        /// <returns>Returns the number of sensors.</returns>
        public static int getNumSensors()
        {
            return m_numSensors;
        }

        /// <summary>
        /// Get the transforms for all sensors reported by trackd.
        /// </summary>
        /// <returns>A list of getReal3D Sensors (similar to a UnityEngine.Transform with scale 1).</returns>
        public static List<getReal3D.Sensor> getSensors()
        {
            var sensors = new List<getReal3D.Sensor>(m_numSensors);
            getSensors(sensors);
            return sensors;
        }

        static float[] s_tmpPosFloatArray = new float[3];
        static float[] s_tmpRotFloatArray = new float[4];
        static Sensor s_tmpSensor = new Sensor();

        /// Get the transforms for all sensors reported by trackd.
        public static void getSensors(List<Sensor> sensors)
        {
            ResizeList(sensors, m_numSensors);

            for (int i = 0; i < m_numSensors; i++)
            {
                int retVal = grPlugin.getSensorPositionRotation(i, s_tmpPosFloatArray, s_tmpRotFloatArray);
                if (retVal == 1)
                {
                    s_tmpSensor.position[0] = s_tmpPosFloatArray[0];
                    s_tmpSensor.position[1] = s_tmpPosFloatArray[1];
                    s_tmpSensor.position[2] = -s_tmpPosFloatArray[2];

                    s_tmpSensor.rotation[0] = -s_tmpRotFloatArray[0];
                    s_tmpSensor.rotation[1] = -s_tmpRotFloatArray[1];
                    s_tmpSensor.rotation[2] = s_tmpRotFloatArray[2];
                    s_tmpSensor.rotation[3] = s_tmpRotFloatArray[3];

                    sensors[i] = s_tmpSensor;
                }
                else
                {
                    Plugin.debug("getReal3D.Plugin: getSensorPositionRotation failed for sensor index: " + i);
                }
            }
        }

        /// <summary>
        /// Get the position and rotation of the given trackd sensor.
        /// </summary>
        /// <param name="_sensorIdx">The sensor index</param>
        /// <param name="_sensorPosition">A reference to a Vector3 which will hold the position</param>
        /// <param name="_sensorRotation">A reference to a Quaternion which will hold the rotation</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public static int getSensorPositionRotation(int _sensorIdx, ref Vector3 _sensorPosition, ref Quaternion _sensorRotation)
        {
            float[] tmpPosFloatArray = new float[3];
            float[] tmpRotFloatArray = new float[4];

            int retVal = grPlugin.getSensorPositionRotation(_sensorIdx, tmpPosFloatArray, tmpRotFloatArray);

            if (retVal == 1)
            {
                _sensorPosition[0] = tmpPosFloatArray[0];
                _sensorPosition[1] = tmpPosFloatArray[1];
                _sensorPosition[2] = -tmpPosFloatArray[2];

                _sensorRotation[0] = -tmpRotFloatArray[0];
                _sensorRotation[1] = -tmpRotFloatArray[1];
                _sensorRotation[2] = tmpRotFloatArray[2];
                _sensorRotation[3] = tmpRotFloatArray[3];
            }
            else
            {
                Plugin.debug("getReal3D.Plugin: getSensorPositionRotation failed for sensor index: " + _sensorIdx);
            }
            return retVal;
        }

        static int[] m_buttons = null;

        /// <summary>
        /// Get a list of controller button states reported by trackd.
        /// </summary>
        /// <returns>The list of states. 1 is button down.</returns>
        public static List<int> getControllerButtons()
        {
            var res = new List<int>();
            getControllerButtons(res);
            return res;
        }

        /// Get a list of controller button states reported by trackd.
        public static void getControllerButtons(List<int> res)
        {
            ResizeList(res, m_numButtons);
            if (m_numButtons > 0)
            {
                ResizeArray(ref m_buttons, m_numButtons);
                grPlugin.getButtons(m_buttons);
                for (int i = 0; i < m_numButtons; ++i)
                {
                    res[i] = m_buttons[i];
                }
            }
        }

        static float[] m_valuators = null;

        /// <summary>
        /// Get a list of controller valuator states (e.g. joystick axes) reported by trackd.
        /// </summary>
        /// <returns>A list of float values. Values typically range -1 to 1.</returns>
        public static List<float> getControllerValuators()
        {
            var res = new List<float>();
            getControllerValuators(res);
            return res;
        }

        /// Get a list of controller valuator states (e.g. joystick axes) reported by trackd.
        public static void getControllerValuators(List<float> res)
        {
            ResizeList(res, m_numValuators);
            if (m_numValuators > 0)
            {
                ResizeArray(ref m_valuators, m_numValuators);
                grPlugin.getValuators(m_valuators);
                for (int i = 0; i < m_numValuators; ++i)
                {
                    res[i] = m_valuators[i];
                }
            }
        }

        /// <summary>
        /// Get the scale value (VR_WORLD_SCALE) from the getReal3D configuration (or last value set) for this Unity instance.
        /// </summary>
        /// <returns>The VR_WORLD_SCALE value.</returns>
        public static float GetScale()
        {
            return grPlugin.getWorldScale();
        }

        /// <summary>
        /// Override the scale value (VR_WORLD_SCALE) from the getReal3D configuration for this Unity instance.
        /// </summary>
        /// <param name="scale">The override value.</param>
        public static void SetScale(float scale)
        {
            grPlugin.setWorldScale(scale);
        }

        /// <summary>
        /// Get the stereo eye-separation from the getReal3D configuration (or last value set) for this Unity instance.
        /// </summary>
        public static float GetEyeSeparation()
        {
            return grPlugin.getEyeSeparation();
        }

        /// <summary>
        /// Change the stereo eye-separation.
        /// </summary>
        /// <param name="separation">The override value.</param>
        public static void SetEyeSeparation(float separation)
        {
            grPlugin.setEyeSeparation(separation);
        }

        /// <summary>
        /// Is the Unity game running in a cluster.
        /// </summary>
        /// <returns>True if clustered</returns>
        public static bool isDistrib()
        {
            return grPlugin.isDistrib();
        }

        /// <summary>
        /// Get the hostname of the machine this Unity instance is running on.
        /// </summary>
        /// <returns>A string containing the hostname</returns>
        public static string getHostName()
        {
            return Marshal.PtrToStringAnsi(grPlugin.getHostName());
        }

        /// <summary>
        /// Get the hostname of the machine running the master instance of the Unity cluster.
        /// </summary>
        /// <returns>Returns the server hostname if running in a cluster, or this hostname if not.</returns>
        public static string getServerHostName()
        {
            return Marshal.PtrToStringAnsi(grPlugin.getAppServerHostName());
        }

        /// <summary>
        /// This Unity instance leaves the cluster, causing all other Unity instances to exit.
        /// </summary>
        internal static void stopDistrib()
        {
            grPlugin.stopDistrib();
        }

        /// <summary>
        /// Get the Unity-specific portion of the getReal3D configuration for this session.
        /// </summary>
        /// <returns>A string containing the &lt;unity&gt; element of the config.</returns>
        public static string getApplicationConfig()
        {
            return Marshal.PtrToStringAnsi(grPlugin.getApplicationConfig());
        }

        /// <summary>
        /// Get XML content of the Unity-specific portion of the getReal3D configuration for this session.
        /// </summary>
        /// <returns>A string containing the &lt;unity&gt; element of the config.</returns>
        public static XmlElement getApplicationConfigXml()
        {
            if (s_appConfigXml == null)
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(getApplicationConfig());
                }
                catch (XmlException)
                {
                    Plugin.warning("Failed to load XML from application config " + getApplicationConfig());
                    return null;
                }
                if (!doc.HasChildNodes) return null;

                XmlNode unityNode = doc.FirstChild;
                if (unityNode == null)
                {
                    Plugin.warning("app_config does not contain unity element");
                    return null;
                }

                s_appConfigXml = (XmlElement)unityNode;
                if (s_appConfigXml == null || s_appConfigXml.Name != "unity")
                {
                    Plugin.warning("app_config does not contain unity element");
                    s_appConfigXml = null;
                    return null;
                }
            }
            return s_appConfigXml;
        }

        /// <summary>
        /// Check if compositor is running on this node.
        /// </summary>
        /// <returns>True if a compositor is running on this node.</returns>
        internal static bool getNodeUsesCompositor()
        {
            return grPlugin.getMgxWindowId() > -1;
        }

        /// <summary>
        /// A unique Id of this Unity instance within the cluster.
        /// </summary>
        /// <returns>The Id</returns>
        public static uint getClusterID()
        {
            return m_clusterId;
        }

        /// <summary>
        /// Sends bytestream from this instance to all other instances. Used in Mechdyne Cluster.
        /// </summary>
        /// <param name="stream">The bytestream</param>
        /// <returns>True if successful.</returns>
        internal static bool sendBytes(MemoryStream stream)
        {
            bool ret = false;
            //Plugin.debug("Plugin.sendBytes sending " + stream.Position.ToString() + " bytes");
            ret = grPlugin.sendDistributedData(stream.GetBuffer(), (uint)stream.Position);
            stream.Position = 0;
            stream.SetLength(0);
            return ret;
        }

        private static int bufferChunkSize = 64 * 1024;
        /// <summary>
        /// Receives bytestream from the sender, if any. Used in Mechdyne Cluster.
        /// </summary>
        /// <param name="stream">The bytestream</param>
        /// <param name="waitForData">If True, blocks until data is received.</param>
        /// <returns>True if no errors.</returns>
        public static bool recvBytes(MemoryStream stream, bool waitForData)
        {
            // TODO: rewrite this to get the byte count for the next available message, then get a byte array for that, the add the byte array to the stream, then check again, etc
            // this way i don't need to worry about hitting the end of the stream (C++-side cant expand the stream, but c# can if i just write to it)
            if (stream.Length == stream.Position) // we've read all this data, it's okay to clobber it
            {
                stream.Position = 0;
                stream.SetLength(0);
            }
            //Plugin.debug("Get size of buffer");
            uint needed = grPlugin.getNextDistributedDataSize(waitForData);
            //Plugin.debug("Got size of buffer: " + needed.ToString());
            if (needed == 0) return true;
            long position = stream.Position;
            while (needed > stream.Capacity)
            {
                stream.Capacity += bufferChunkSize;
                Plugin.debug("recvBytes: growing buffer to " + stream.Capacity.ToString() + " bytes");
            }
            long length = stream.Length;
            long available = stream.Capacity - stream.Length;
            long added = 0;
            stream.SetLength(stream.Capacity);
            while (needed > 0 && needed <= available)
            {
                //Plugin.debug("Getting data, have " + available.ToString() + " bytes available");
                int count = (int)grPlugin.recvDistributedData(stream.GetBuffer(), (uint)stream.Capacity, (uint)(length + added), false);
                added += count;
                available -= count;
                needed = grPlugin.getNextDistributedDataSize(false);
                //Plugin.debug("Got size of next buffer: " + needed.ToString());
            }
            //Plugin.debug("Adding " + added.ToString() + " bytes");
            //if (needed > 0) Plugin.debug("Out of room: " + length.ToString());
            stream.SetLength(length + added);
            stream.Position = position;
            return true;
        }

        /// <summary>
        /// The number of machines used in the cluster.
        /// </summary>
        /// <returns>The node count</returns>
        public static int getNodeCount()
        {
            return grPlugin.getNodeCount();
        }

        /// <summary>
        /// The number of Unity instances in the cluster.
        /// </summary>
        /// <returns>The instance count</returns>
        public static int getConfigCount()
        {
            return grPlugin.getConfigCount();
        }

        /// <summary>
        /// The number of Unity instances on the given node
        /// </summary>
        /// <param name="nodeId">The node Id</param>
        /// <returns>The instance count</returns>
        public static int getNodeConfigCount(int nodeId)
        {
            return grPlugin.getNodeConfigCount(nodeId);
        }

        /// <summary>
        /// Get the number of screens (cameras) in the configuration of each Unity instance.
        /// </summary>
        /// <param name="nodeId">The node (machine) Id.</param>
        /// <param name="displayId">The display (instance) Id.</param>
        /// <returns>The number of screens.</returns>
        public static int getScreenCount(int nodeId, int displayId)
        {
            return grPlugin.getScreenCount(nodeId, displayId);
        }

        /// <summary>
        /// Get the coordinates of a given screen.
        /// </summary>
        /// <param name="nodeId">The node (machine) Id.</param>
        /// <param name="displayId">The display (instance) Id.</param>
        /// <param name="screenId">The screen (camera) Id.</param>
        /// <returns>The 4 corners of the screen (in the same space as head/wand/sensor cooridnates).</returns>
        public static UnityEngine.Vector3[] getScreenCoordinates(uint nodeId, uint displayId, uint screenId)
        {
            float[] positions = new float[12];
            if (grPlugin.getScreenCoordinates(nodeId, displayId, screenId, positions) != 1)
            {
                return null;
            }
            UnityEngine.Vector3[] res = new UnityEngine.Vector3[4];

            for (int i = 0, idx = 0; i < 4; ++i, idx += 3)
            {
                res[i].Set(positions[idx + 0], positions[idx + 1], -positions[idx + 2]);
            }

            return res;
        }

        /// <summary>
        /// Write messages to the getReal3D log file.
        /// </summary>
        /// <param name="logLevel">The level of this message.</param>
        /// <param name="msg">The message.</param>
        public static void Log(LogLevel logLevel, string msg)
        {
            Log(logLevel, msg, default(IntPtr));
        }

        /// <summary>
        /// Write messages to the getReal3D log file.
        /// </summary>
        /// <param name="logLevel">The level of this message.</param>
        /// <param name="msg">The message.</param>
        /// <param name="logger">The optional logger.</param>
        public static void Log(LogLevel logLevel, string msg, IntPtr logger)
        {
            if (logLevel == LogLevel.Discarded)
            {
                return;
            }
            bool logInUnity =
                (Debug.isDebugBuild || Application.isEditor || logLevel == LogLevel.Error)
                && logLevel >= Settings.get().logLevel;
            if (logInUnity)
            {
                if (logLevel == LogLevel.Warning)
                {
                    Debug.LogWarning(msg);
                }
                else if (logLevel == LogLevel.Error)
                {
                    Debug.LogError(msg);
                }
                else if (logLevel != LogLevel.Discarded)
                {
                    Debug.Log(msg);
                }
            }
            if (initialized())
            {
                if (logger == default(IntPtr))
                {
                    grPlugin.logMsg(logLevel, Time.frameCount.ToString("0000.##") + " " + msg);
                }
                else
                {
                    grPlugin.log(logLevel, logger, Time.frameCount.ToString("0000.##") + " " + msg);
                }
            }
        }

        /// <summary>
        /// A convenience function for debug logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        public static void debug(string msg)
        {
            Log(LogLevel.Debug, msg);
        }

        /// <summary>
        /// A convenience function for debug logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="logger">The optional logger.</param>
        public static void debug(string msg, IntPtr logger)
        {
            Log(LogLevel.Debug, msg, logger);
        }

        /// <summary>
        /// A convenience function for info logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        public static void info(string msg)
        {
            Log(LogLevel.Info, msg);
        }

        /// <summary>
        /// A convenience function for info logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="logger">The optional logger.</param>
        public static void info(string msg, IntPtr logger)
        {
            Log(LogLevel.Info, msg, logger);
        }

        /// <summary>
        /// A convenience function for warning logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        public static void warning(string msg)
        {
            Log(LogLevel.Warning, msg);
        }

        /// <summary>
        /// A convenience function for warning logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="logger">The optional logger.</param>
        public static void warning(string msg, IntPtr logger)
        {
            Log(LogLevel.Warning, msg, logger);
        }

        /// <summary>
        /// A convenience function for error logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        public static void error(string msg)
        {
            Log(LogLevel.Error, msg);
        }

        /// <summary>
        /// A convenience function for error logging.
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="logger">The optional logger.</param>
        public static void error(string msg, IntPtr logger)
        {
            Log(LogLevel.Error, msg, logger);
        }

        /// <summary>
        /// The sensor index of the Head as set in the getReal3D configuration.
        /// </summary>
        /// <returns>The Head index</returns>
        public static int getHeadIndex()
        {
            return grPlugin.getHeadIndex();
        }

        /// <summary>
        /// The sensor index of the Wand as set in the getReal3D configuration.
        /// </summary>
        /// <returns>The Wand index</returns>
        public static int getWandIndex()
        {
            return grPlugin.getWandIndex();
        }

        /// <summary>
        /// The getReal3D frame count on the master node in the cluster. This may not be the same as UnityEngine.Time.frameCount
        /// </summary>
        /// <returns>The frame count</returns>
        public static int getFrameCount()
        {
            return grPlugin.getFrameCount();
        }

        /// <summary>
        /// Is this Unity instance configured to provide audio playback.
        /// </summary>
        /// <returns>True if this instance should provide sound</returns>
        public static bool isAudioPlayer()
        {
            return grPlugin.isAudioPlayer();
        }

        /// <summary>
        /// Return the user which should produce audio playback
        /// </summary>
        /// <returns>User ID</returns>
        public static int getAudioUser()
        {
            return grPlugin.getAudioUser();
        }

        /// <summary>
        /// Return the user which should produce audio playback
        /// </summary>
        /// <returns>User ID</returns>
        internal static int getSwapInterval()
        {
            return grPlugin.getSwapInterval();
        }

        /// <summary>
        /// Return the camera event callback from the getReal3D plugin
        /// </summary>
        public static IntPtr getCameraEventCallback()
        {
            return grPlugin.getCameraEventCallback();
        }

        internal static IntPtr getEndOfFrameEventFunc()
        {
            return grPlugin.getEndOfFrameEventFunc();
        }

        /// <summary>
        /// Return the camera event callback from the getReal3D plugin
        /// </summary>
        public static IntPtr getCameraProfilingCallback()
        {
            return grPlugin.getCameraProfilingCallback();
        }

        /// <summary>
        /// Return the number of user(s)
        /// </summary>
        /// <returns>Number of users</returns>
        public static uint getUserCount()
        {
            return grPlugin.getUserCount();
        }

        /// <summary>
        /// Return user name
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Name of user</returns>
        public static string getUserName(uint userId)
        {
            return grPlugin.getUserName(userId);
        }

        /// <summary>
        /// Return the user associated to a camera
        /// </summary>
        /// <param name="cameraId">The camera ID</param>
        /// <returns>User ID</returns>
        public static int getUserFromCamera(uint cameraId)
        {
            return grPlugin.getUserFromCamera(cameraId);
        }

        /// <summary>
        /// Return the wand index for the given user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Wand index</returns>
        public static int getWandIndexForUser(uint userId)
        {
            return grPlugin.getWandIndexForUser(userId);
        }

        /// <summary>
        /// Return the head index for the given user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Head index</returns>
        public static int getHeadIndexForUser(uint userId)
        {
            return grPlugin.getHeadIndexForUser(userId);
        }

        internal static NativePlugin getNativePlugin()
        {
            return grPlugin;
        }
        /// <summary>
        /// Sets the texture used to render a given source.
        /// </summary>
        /// <param name="id">Source ID</param>
        /// <param name="texture">Low level texture pointer.</param>
        static public void setTextureFromUnity(int id, System.IntPtr texture)
        {
            grPlugin.setTextureFromUnity(id, texture);
        }

        private static AudioSpeakerMode getCurrentSpeakerMode()
        {
            // Unity >= 5
            //AudioConfiguration config = AudioSettings.GetConfiguration();
            //return config.speakerMode;

            // Unity < 5
            //return AudioSettings.speakerMode;


            var method = typeof(AudioSettings).GetMethod("GetConfiguration");
            if (method != null)
            {
                object audioConfig = method.Invoke(null, null);
                var field = audioConfig.GetType().GetField("speakerMode");
                var value = field.GetValue(audioConfig);
                if (value == null)
                {
                    warning("No value for AudioSpeakerMode.speakerMode!");
                    return AudioSpeakerMode.Stereo;
                }
                return (AudioSpeakerMode)value;
            }
            else
            {
                var prop = typeof(AudioSettings).GetProperty("speakerMode");
                var value = prop.GetValue(null, null);
                if (value == null)
                {
                    warning("No value for AudioSpeakerMode.speakerMode!");
                    return AudioSpeakerMode.Stereo;
                }
                return (AudioSpeakerMode)value;
            }
        }

        private static void setSpeakerMode(AudioSpeakerMode speakerMode)
        {
            // Unity >= 5
            //AudioConfiguration config = AudioSettings.GetConfiguration();
            //config.speakerMode = speakerMode;
            //AudioSettings.Reset(config);

            // Unity < 5
            //AudioSettings.speakerMode = speakerMode;

            var method = typeof(AudioSettings).GetMethod("GetConfiguration");
            if (method != null)
            {
                object audioConfig = method.Invoke(null, null);
                var field = audioConfig.GetType().GetField("speakerMode");
                field.SetValue(audioConfig, speakerMode);
                var resetMethod = typeof(AudioSettings).GetMethod("Reset");
                resetMethod.Invoke(null, new object[] { audioConfig });
            }
            else
            {
                var prop = typeof(AudioSettings).GetProperty("speakerMode");
                prop.SetValue(null, speakerMode, null);
            }
        }

        private static void setupSpeakers()
        {
            debug("Sound setup...");

            XmlElement unity = getApplicationConfigXml();
            AudioSpeakerMode speakerMode;
            if (unity != null && unity.HasAttribute("speaker_mode"))
            {
                debug("Found speaker mode override: " + unity.GetAttribute("speaker_mode") + ".");
                try
                {
                    speakerMode = (AudioSpeakerMode)Enum.Parse(typeof(AudioSpeakerMode), unity.GetAttribute("speaker_mode"));
                }
                catch (System.Exception e)
                {
                    warning("Bad speaker mode name: '" + unity.GetAttribute("speaker_mode") + "': " + e.ToString());
                    return;
                }
            }
            else
            {
                return;
            }

            AudioSpeakerMode currentSpeakerMode = getCurrentSpeakerMode();
            debug("Unity current speaker mode: " + currentSpeakerMode.ToString() + ".");

            if (currentSpeakerMode != speakerMode)
            {
                debug("Setting speaker mode to " + speakerMode.ToString() + ".");
                setSpeakerMode(speakerMode);
            }
            else
            {
                debug("No need to change speaker mode.");
            }

        }

        internal static void getMasterInputs(byte[] keysArray, byte[] buttonsArray, float[] axisArray)
        {
            grPlugin.getMasterInputs(keysArray, buttonsArray, axisArray);
        }

        internal static int getMasterKeyCount()
        {
            return grPlugin.getMasterKeyCount();
        }

        internal static int getMasterAxisCount()
        {
            return grPlugin.getMasterAxisCount();
        }

        internal static int getMasterButtonCount()
        {
            return grPlugin.getMasterButtonCount();
        }

        internal static void setNextFramePayload(WriteBuffer buffer)
        {
            grPlugin.setPayload(buffer.buffer, (uint)buffer.size);
        }

        internal static void getPayload(ReadBuffer buffer)
        {
            uint needed = grPlugin.getPayload(null, 0);
            buffer.Resize((int)needed);

            if (needed == 0)
            {
                return;
            }

            grPlugin.getPayload(buffer.buffer, needed);
        }

        internal static void getClientPayload(uint clientId, ReadBuffer buffer)
        {
            uint needed = grPlugin.getClientPayload(clientId, null, 0);
            buffer.Resize((int)needed);

            if (needed == 0)
            {
                return;
            }

            grPlugin.getClientPayload(clientId, buffer.buffer, needed);
        }

        internal static void addCallback(IUpdateCallback callback)
        {
            s_callbacks.Add(callback);
        }
        internal static void removeCallback(IUpdateCallback callback)
        {
            s_callbacks.Remove(callback);
        }

        internal static int getScreenBuffer(uint cameraId)
        {
            uint nodeId = grPlugin.getNodeID();
            uint displayId = grPlugin.getDisplayID();
            return grPlugin.getScreenBuffer(nodeId, displayId, cameraId);
        }

        private static void setupWindow()
        {
            if (Application.isEditor)
            {
                return;
            }
            try
            {
                WindowSettings windowSettings = grPlugin.getWindowSettings();
                Cursor.visible = !windowSettings.hideMouse;
            }
            catch (Exception e)
            {
                error("Error while retrieving window settings: " + e);
            }
        }

        private static float[] s_screenPositions = new float[12];

        /// <summary>
        /// Return the coordinates of a screen.
        /// </summary>
        /// <param name="name">Name of the screen</param>
        /// <returns>LL, LR, UR and UL corners</returns>
        public static void getScreenCoordinatesByName(string name, Vector3[] corners)
        {
            if (grPlugin.getScreenCoordinatesByName(name, s_screenPositions) != 1)
            {
                return;
            }
            for (int i = 0, idx = 0; i < 4; ++i, idx += 3)
            {
                corners[i].Set(s_screenPositions[idx + 0], s_screenPositions[idx + 1], -s_screenPositions[idx + 2]);
            }
        }

        /// <summary>
        /// Return the screen space of the given screen.
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        public static ScreenSpace getConfigScreenSpace(int screenId)
        {
            return grPlugin.getConfigScreenSpace(screenId);
        }

        /// <summary>
        /// Return the number of screens in the configuration.
        /// </summary>
        public static int getConfigScreenCount()
        {
            return grPlugin.getConfigScreenCount();
        }

        /// <summary>
        /// Return the name of a screen.
        /// </summary>
        public static string getScreenName(int screenId)
        {
            return grPlugin.getScreenName(screenId);
        }

        private static void ResizeList<T>(List<T> list, int sz)
        {
            ResizeList(list, sz, default(T));
        }

        private static void ResizeList<T>(List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if (sz < cur)
            {
                list.RemoveRange(sz, cur - sz);
            }
            else if (sz > cur)
            {
                if (sz > list.Capacity)
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }

        private static void ResizeArray<T>(ref T[] ary, int sz)
        {
            if (ary == null || ary.Length != sz)
            {
                ary = new T[sz];
            }
        }

        internal static T FindAnyObjectByType<T>() where T : UnityEngine.Object
        {
#if UNITY_6000_0_OR_NEWER
            return UnityEngine.Object.FindAnyObjectByType<T>();
#else
            return UnityEngine.Object.FindObjectOfType<T>();
#endif
        }

        internal static T[] FindObjectsByTypeUnsorted<T>() where T : UnityEngine.Object
        {
#if UNITY_6000_0_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<T>();
#endif
        }
    }
}
