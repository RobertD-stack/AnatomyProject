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
using System.Runtime.InteropServices;
using System.Security;

namespace getReal3D
{
    /// <summary>
    /// Levels of log message importance. Used to choose the amount run-time logging or denote the importance of a log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Discarded log-level denotes a discarded message.
        /// </summary>
        Discarded = -1,
        /// <summary>
        /// Debug log-level denotes detailed messages used in tracking the application flow and state. These messages are often so frequent as to impact performance.
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Info log-level denotes messages about occasional flow and state changes. These messages are safely ignored in typical use.
        /// </summary>
        Info,
        /// <summary>
        /// Warning log-level denotes messages about unexpected conditions that were caught and are not considered detremental to application performance or execution.
        /// </summary>
        Warning,
        /// <summary>
        /// Error log-level denotes messages about unexpected conditions that were caught but may impede application performance or execution.
        /// </summary>
        Error,
        /// <summary>
        /// None log-level denotes disabled logging.
        /// </summary>
        None
    }

    /// <summary>
    /// Screen space
    /// </summary>
    public enum ScreenSpace
    {
        /// <summary>
        /// Tracker space
        /// </summary>
        TrackerSpace = 0,
        /// <summary>
        /// Eye space
        /// </summary>
        EyeSpace,
        /// <summary>
        /// Head space
        /// </summary>
        HeadSpace,
        /// <summary>
        /// Wand space
        /// </summary>
        WandSpace,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowSettings
    {
        ///<summary>horizontal position of the window.</summary>
        public int x;
        ///<summary>vertical position of the window.</summary>
        public int y;
        ///<summary>width of the window.</summary>
        public uint width;
        ///<summary>height of the window.</summary>
        public uint height;
        ///<summary>if the window should use GPU affinity.</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool useAffinity;
        ///<summary>if the window should be topmost</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool isTopmost;
        ///<summary>if the window should display a border</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool border;
        ///<summary>if the window should be hidden</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool hidden;
        ///<summary>if the mouse should be hidden</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool hideMouse;
    };

    /// <summary>
    /// Native plugin access. See gr_plugin.h for more documentation.
    /// </summary>
    internal interface NativePlugin
    {
        string pluginFileName();

        int init(string configPath, bool noCluster);

        int initWithString(string configPath, bool noCluster);

        void deinit();

        void clusterShutdown();

        bool initialized();

        bool update();

        bool getAppWindowHidden();

        int getAppWindowHeight();

        int getAppWindowWidth();

        int getMasterWindowHeight();

        int getMasterWindowWidth();

        int getNumberOfCameras();

        bool getCameraShowsUI(uint _cameraIndex);

        bool getCameraUseRTT(uint _cameraIndex);

        int getCameraWidth(uint _cameraIndex);

        int getCameraHeight(uint _cameraIndex);

        //bool resizeCamera(uint _cameraIndex, int w, int h);

        int getCameraViewport(uint _cameraIndex, float[] _viewport);

        float getCameraAspectRatio(uint _cameraIndex);

        float getCameraFieldOfView(uint _cameraIndex);

        int getCameraProjectionMatrix(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);

        int getCameraProjectionMatrixD3D(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);

        int getCameraRotation(uint _cameraIndex, float[] _quat);

        int getCameraPosition(uint _cameraIndex, float[] _eyePosition);

        int getHeadPositionRotation(float[] _headPosition, float[] _headRotation);

        int getWandPositionRotation(float[] _wandPosition, float[] _wandRotation);

        int getNumSensors();

        int getSensorPositionRotation(int _sensorIndex, float[] _sensorPosition, float[] _sensorRotation);

        int getNumButtons();

        int getButton(uint _buttonNumber);

        int getButtons(int[] _buttonArray);

        int getNumValuators();

        float getValuator(uint _valuatorNumber);

        int getValuators(float[] _valuatorArray);

        float getWorldScale();

        void setWorldScale(float _worldScale);

        float getEyeSeparation();

        void setEyeSeparation(float _eyeSeparation);

        bool isDistrib();

        void stopDistrib();

        uint getNodeID();

        uint getDisplayID();

        bool isMaster();

        uint getClusterID();

        int getMgxWindowId();

        IntPtr getAppServerHostName();

        IntPtr getHostName();

        IntPtr getApplicationConfig();

        bool sendDistributedData(byte[] data, uint length);

        uint recvDistributedData(byte[] data, uint maxLength, uint offset, bool wait = false);

        uint getNextDistributedDataSize(bool wait = false);

        int getConfigCount();

        int getNodeCount();

        int getNodeConfigCount(int nodeId);

        bool isAudioPlayer();

        int getScreenCount(int nodeId, int displayId);

        int getScreenCoordinates(uint nodeId, uint displayId, uint screenId, float[] coords);

        int getHeadIndex();

        int getWandIndex();

        int getFrameCount();

        void logMsg(LogLevel logLevel, string msg);

        WindowSettings getWindowSettings();

        void setTextureFromUnity(int id, System.IntPtr texture);

        int getMasterInputs(byte[] _keysArray, byte[] _buttonArray, float[] _axisArray);

        int getMasterKeyCount();

        int getMasterAxisCount();

        int getMasterButtonCount();

        int getWandIndexForUser(uint userId);

        int getHeadIndexForUser(uint userId);

        int getHeadPositionRotationForUser(uint userId, float[] headPosition, float[] headRotation);

        int getWandPositionRotationForUser(uint userId, float[] wandPosition, float[] wandRotation);

        uint getUserCount();

        string getUserName(uint id);

        int getUserFromCamera(uint cameraId);

        int getAudioUser();

        IntPtr getCameraEventCallback();

        IntPtr getEndOfFrameEventFunc();

        IntPtr getCameraProfilingCallback();

        void setPayload(byte[] data, uint length);

        uint getPayload(byte[] data, uint bufferLength);

        uint getClientPayload(uint clientId, byte[] data, uint bufferLength);

        int getRandomSeed();

        #region AutogenInterface
        IntPtr createLogger(string a0);
        void releaseLogger(IntPtr a0);
        void log(LogLevel a0, IntPtr a1, string a2);
        int getSwapInterval();
        int getScreenBuffer(uint a0, uint a1, uint a2);
        int getScreenCoordinatesByName(string a0, float[] a1);
        ScreenSpace getConfigScreenSpace(int a0);
        int getConfigScreenCount();
        string getScreenName(int a0);
        void mcStartServer();
        void mcStopServer();
        void mcOnClientConnected(Action<uint, string, string> a0);
        void mcOnClientDisconnected(Action<uint> a0);
        void mcOnClientMessage(Action<uint, IntPtr, uint> a0);
        void mcOnServerError(Action<int, int> a0);
        void mcOnClientError(Action<int> a0);
        void mcSendToClient(uint a0, IntPtr a1, uint a2);
        void mcSendToClients(IntPtr a0, uint a1);
        bool mcIsClient();
        void mcConnectMaster(string a0);
        void mcDisconnectMaster();
        void mcOnMasterConnected(Action<string, string, uint> a0);
        void mcOnMasterDisconnected(Action a0);
        void mcOnMasterMessage(Action<IntPtr, uint> a0);
        void mcSendToMaster(IntPtr a0, uint a1);
        bool mcIsMaster();
        #endregion
    }

    [SuppressUnmanagedCodeSecurity]
    internal class NativePlugin32 : NativePlugin
    {
#if GETREAL3D_LOAD_DEBUG_PLUGIN
        private const string grPluginFileName = "gr_plugind";
#else
        private const string grPluginFileName = "gr_plugin";
#endif

        public string pluginFileName() { return grPluginFileName; }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_init(string configPath, bool noCluster);
        public int init(string configPath, bool noCluster) { return grPlugin_init(configPath, noCluster); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_initWithString(string configPath, bool noCluster);
        public int initWithString(string configPath, bool noCluster) { return grPlugin_initWithString(configPath, noCluster); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_deinit();
        public void deinit() { grPlugin_deinit(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_clusterShutdown();
        public void clusterShutdown() { grPlugin_clusterShutdown(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_initialized();
        public bool initialized() { return grPlugin_initialized(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPluginUnity_update();
        public bool update() { return grPluginUnity_update(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getAppWindowHidden();
        public bool getAppWindowHidden() { return grPlugin_getAppWindowHidden(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAppWindowHeight();
        public int getAppWindowHeight() { return grPlugin_getAppWindowHeight(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAppWindowWidth();
        public int getAppWindowWidth() { return grPlugin_getAppWindowWidth(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAppWindowWidth();
        public int getMasterWindowWidth() { return grPlugin_getMasterAppWindowWidth(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAppWindowHeight();
        public int getMasterWindowHeight() { return grPlugin_getMasterAppWindowHeight(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumberOfCameras();
        public int getNumberOfCameras() { return grPlugin_getNumberOfCameras(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getCameraShowsUI(uint _cameraIndex);
        public bool getCameraShowsUI(uint _cameraIndex) { return grPlugin_getCameraShowsUI(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getCameraUseRTT(uint _cameraIndex);
        public bool getCameraUseRTT(uint _cameraIndex) { return grPlugin_getCameraUseRTT(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraWidth(uint _cameraIndex);
        public int getCameraWidth(uint _cameraIndex) { return grPlugin_getCameraWidth(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraHeight(uint _cameraIndex);
        public int getCameraHeight(uint _cameraIndex) { return grPlugin_getCameraHeight(_cameraIndex); }

        //[DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //private static extern bool grPlugin_resizeCamera(uint _cameraIndex, int w, int h);
        //public bool resizeCamera(uint _cameraIndex, int w, int h) { return grPlugin_resizeCamera(_cameraIndex, w, h); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraViewport(uint _cameraIndex, float[] _viewport);
        public int getCameraViewport(uint _cameraIndex, float[] _viewport) { return grPlugin_getCameraViewport(_cameraIndex, _viewport); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getCameraAspectRatio(uint _cameraIndex);
        public float getCameraAspectRatio(uint _cameraIndex) { return grPlugin_getCameraAspectRatio(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getCameraFieldOfView(uint _cameraIndex);
        public float getCameraFieldOfView(uint _cameraIndex) { return grPlugin_getCameraFieldOfView(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraProjectionMatrix(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);
        public int getCameraProjectionMatrix(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix) { return grPlugin_getCameraProjectionMatrix(_cameraIndex, _cameraNear, _cameraFar, _matrix); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraProjectionMatrixD3D(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);
        public int getCameraProjectionMatrixD3D(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix) { return grPlugin_getCameraProjectionMatrixD3D(_cameraIndex, _cameraNear, _cameraFar, _matrix); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraRotation(uint _cameraIndex, float[] _quat);
        public int getCameraRotation(uint _cameraIndex, float[] _quat) { return grPlugin_getCameraRotation(_cameraIndex, _quat); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraPosition(uint _cameraIndex, float[] _eyePosition);
        public int getCameraPosition(uint _cameraIndex, float[] _eyePosition) { return grPlugin_getCameraPosition(_cameraIndex, _eyePosition); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadPositionRotation(float[] _headPosition, float[] _headRotation);
        public int getHeadPositionRotation(float[] _headPosition, float[] _headRotation) { return grPlugin_getHeadPositionRotation(_headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandPositionRotation(float[] _wandPosition, float[] _wandRotation);
        public int getWandPositionRotation(float[] _wandPosition, float[] _wandRotation) { return grPlugin_getWandPositionRotation(_wandPosition, _wandRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumSensors();
        public int getNumSensors() { return grPlugin_getNumSensors(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getSensorPositionRotation(int _sensorIndex, float[] _sensorPosition, float[] _sensorRotation);
        public int getSensorPositionRotation(int _sensorIndex, float[] _sensorPosition, float[] _sensorRotation) { return grPlugin_getSensorPositionRotation(_sensorIndex, _sensorPosition, _sensorRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumButtons();
        public int getNumButtons() { return grPlugin_getNumButtons(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getButton(uint _buttonNumber);
        public int getButton(uint _buttonNumber) { return grPlugin_getButton(_buttonNumber); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getButtons(int[] _buttonArray);
        public int getButtons(int[] _buttonArray) { return grPlugin_getButtons(_buttonArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumValuators();
        public int getNumValuators() { return grPlugin_getNumValuators(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getValuator(uint _valuatorNumber);
        public float getValuator(uint _valuatorNumber) { return grPlugin_getValuator(_valuatorNumber); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getValuators(float[] _valuatorArray);
        public int getValuators(float[] _valuatorArray) { return grPlugin_getValuators(_valuatorArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getWorldScale();
        public float getWorldScale() { return grPlugin_getWorldScale(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setWorldScale(float _worldScale);
        public void setWorldScale(float _worldScale) { grPlugin_setWorldScale(_worldScale); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getEyeSeparation();
        public float getEyeSeparation() { return grPlugin_getEyeSeparation(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setEyeSeparation(float _eyeSeparation);
        public void setEyeSeparation(float _eyeSeparation) { grPlugin_setEyeSeparation(_eyeSeparation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_isDistrib();
        public bool isDistrib() { return grPlugin_isDistrib(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_stopDistrib();
        public void stopDistrib() { grPlugin_stopDistrib(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getNodeID();
        public uint getNodeID() { return grPlugin_getNodeID(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getDisplayID();
        public uint getDisplayID() { return grPlugin_getDisplayID(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getClusterID();
        public uint getClusterID() { return grPlugin_getClusterID(); }

        public bool isMaster() { return (getNodeID() == 0) && (getDisplayID() == 0); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMgxWindowId();
        public int getMgxWindowId() { return grPlugin_getMgxWindowId(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getAppServerHostName();
        public IntPtr getAppServerHostName() { return grPlugin_getAppServerHostName(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getHostName();
        public IntPtr getHostName() { return grPlugin_getHostName(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getApplicationConfig();
        public IntPtr getApplicationConfig() { return grPlugin_getApplicationConfig(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_sendDistributedData(byte[] data, uint length);
        public bool sendDistributedData(byte[] data, uint length) { return grPlugin_sendDistributedData(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_recvDistributedData(byte[] data, uint maxLength, uint offset, bool wait);
        public uint recvDistributedData(byte[] data, uint maxLength, uint offset, bool wait = false) { return grPlugin_recvDistributedData(data, maxLength, offset, wait); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getNextDistributedDataSize(bool wait);
        public uint getNextDistributedDataSize(bool wait = false) { return grPlugin_getNextDistributedDataSize(wait); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getConfigCount();
        public int getConfigCount() { return grPlugin_getConfigCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNodeCount();
        public int getNodeCount() { return grPlugin_getNodeCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNodeConfigCount(int nodeId);
        public int getNodeConfigCount(int nodeId) { return grPlugin_getNodeConfigCount(nodeId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_isAudioPlayer();
        public bool isAudioPlayer() { return grPlugin_isAudioPlayer(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCount(int nodeId, int displayId);
        public int getScreenCount(int nodeId, int displayId) { return grPlugin_getScreenCount(nodeId, displayId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCoordinates(uint nodeId, uint displayId, uint screenId, float[] coords);
        public int getScreenCoordinates(uint nodeId, uint displayId, uint screenId, float[] coords) { return grPlugin_getScreenCoordinates(nodeId, displayId, screenId, coords); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadIndex();
        public int getHeadIndex() { return grPlugin_getHeadIndex(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandIndex();
        public int getWandIndex() { return grPlugin_getWandIndex(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getFrameCount();
        public int getFrameCount() { return grPlugin_getFrameCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_logMsg(LogLevel logLevel, string msg);
        public void logMsg(LogLevel logLevel, string msg) { grPlugin_logMsg(logLevel, msg); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWindowSettings(IntPtr win, uint structSize);
        public WindowSettings getWindowSettings()
        {
            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WindowSettings)));
            int res = grPlugin_getWindowSettings(pnt, (uint)Marshal.SizeOf(typeof(WindowSettings)));
            if (res != 1)
            {
                throw new Exception("Error during grPlugin_getWindowSettings call.");
            }
            return (WindowSettings)Marshal.PtrToStructure(pnt, typeof(WindowSettings));
        }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setTextureFromUnity(int id, System.IntPtr texture);
        public void setTextureFromUnity(int id, System.IntPtr texture) { grPlugin_setTextureFromUnity(id, texture); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getCameraEventCallback();
        public IntPtr getCameraEventCallback() { return grPlugin_getCameraEventCallback(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getEndOfFrameEventFunc();
        public IntPtr getEndOfFrameEventFunc() { return grPlugin_getEndOfFrameEventFunc(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getCameraProfilingCallback();
        public IntPtr getCameraProfilingCallback() { return grPlugin_getCameraProfilingCallback(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterInputs(byte[] _keysArray, byte[] _buttonsArray, float[] _axisArray);
        public int getMasterInputs(byte[] _keysArray, byte[] _buttonsArray, float[] _axisArray) { return grPlugin_getMasterInputs(_keysArray, _buttonsArray, _axisArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterKeyCount();
        public int getMasterKeyCount() { return grPlugin_getMasterKeyCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAxisCount();
        public int getMasterAxisCount() { return grPlugin_getMasterAxisCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterButtonCount();
        public int getMasterButtonCount() { return grPlugin_getMasterButtonCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandIndexForUser(uint id);
        public int getWandIndexForUser(uint id) { return grPlugin_getWandIndexForUser(id); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadIndexForUser(uint id);
        public int getHeadIndexForUser(uint id) { return grPlugin_getHeadIndexForUser(id); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation);
        public int getHeadPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation) { return grPlugin_getHeadPositionRotationForUser(userId, _headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation);
        public int getWandPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation) { return grPlugin_getWandPositionRotationForUser(userId, _headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getUserCount();
        public uint getUserCount() { return grPlugin_getUserCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getUserName(uint id);
        public string getUserName(uint id)
        {
            IntPtr name = grPlugin_getUserName(id);
            if (name != IntPtr.Zero) { return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(name); }
            else { throw new System.ArgumentOutOfRangeException("Invalid user ID in getUserName"); }
        }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getUserFromCamera(uint cameraId);
        public int getUserFromCamera(uint cameraId) { return grPlugin_getUserFromCamera(cameraId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAudioUser();
        public int getAudioUser() { return grPlugin_getAudioUser(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPluginUnity_setPayload(byte[] data, uint length);
        public void setPayload(byte[] data, uint length) { grPluginUnity_setPayload(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPluginUnity_getPayload(byte[] data, uint length);
        public uint getPayload(byte[] data, uint length) { return grPluginUnity_getPayload(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPluginUnity_getClientPayload(uint clientId, byte[] data, uint length);
        public uint getClientPayload(uint id, byte[] data, uint length) { return grPluginUnity_getClientPayload(id, data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getRandomSeed();
        public int getRandomSeed() { return grPlugin_getRandomSeed(); }

        #region AutogenImplementation32
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_createLogger(string a0);
        public IntPtr createLogger(string a0) { return grPlugin_createLogger(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_releaseLogger(IntPtr a0);
        public void releaseLogger(IntPtr a0) { grPlugin_releaseLogger(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_log(LogLevel a0, IntPtr a1, string a2);
        public void log(LogLevel a0, IntPtr a1, string a2) { grPlugin_log(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getSwapInterval();
        public int getSwapInterval() { return grPlugin_getSwapInterval(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenBuffer(uint a0, uint a1, uint a2);
        public int getScreenBuffer(uint a0, uint a1, uint a2) { return grPlugin_getScreenBuffer(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCoordinatesByName(string a0, float[] a1);
        public int getScreenCoordinatesByName(string a0, float[] a1) { return grPlugin_getScreenCoordinatesByName(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern ScreenSpace grPlugin_getConfigScreenSpace(int a0);
        public ScreenSpace getConfigScreenSpace(int a0) { return (ScreenSpace)grPlugin_getConfigScreenSpace(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getConfigScreenCount();
        public int getConfigScreenCount() { return grPlugin_getConfigScreenCount(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getScreenName(int a0);
        public string getScreenName(int a0) { IntPtr res = grPlugin_getScreenName(a0); return (res != IntPtr.Zero) ? System.Runtime.InteropServices.Marshal.PtrToStringAnsi(res) : ""; }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcStartServer();
        public void mcStartServer() { grPlugin_mcStartServer(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcStopServer();
        public void mcStopServer() { grPlugin_mcStopServer(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientConnected(Action<uint, string, string> a0);
        public void mcOnClientConnected(Action<uint, string, string> a0) { grPlugin_mcOnClientConnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientDisconnected(Action<uint> a0);
        public void mcOnClientDisconnected(Action<uint> a0) { grPlugin_mcOnClientDisconnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientMessage(Action<uint, IntPtr, uint> a0);
        public void mcOnClientMessage(Action<uint, IntPtr, uint> a0) { grPlugin_mcOnClientMessage(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnServerError(Action<int, int> a0);
        public void mcOnServerError(Action<int, int> a0) { grPlugin_mcOnServerError(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientError(Action<int> a0);
        public void mcOnClientError(Action<int> a0) { grPlugin_mcOnClientError(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToClient(uint a0, IntPtr a1, uint a2);
        public void mcSendToClient(uint a0, IntPtr a1, uint a2) { grPlugin_mcSendToClient(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToClients(IntPtr a0, uint a1);
        public void mcSendToClients(IntPtr a0, uint a1) { grPlugin_mcSendToClients(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_mcIsClient();
        public bool mcIsClient() { return grPlugin_mcIsClient(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcConnectMaster(string a0);
        public void mcConnectMaster(string a0) { grPlugin_mcConnectMaster(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcDisconnectMaster();
        public void mcDisconnectMaster() { grPlugin_mcDisconnectMaster(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterConnected(Action<string, string, uint> a0);
        public void mcOnMasterConnected(Action<string, string, uint> a0) { grPlugin_mcOnMasterConnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterDisconnected(Action a0);
        public void mcOnMasterDisconnected(Action a0) { grPlugin_mcOnMasterDisconnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterMessage(Action<IntPtr, uint> a0);
        public void mcOnMasterMessage(Action<IntPtr, uint> a0) { grPlugin_mcOnMasterMessage(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToMaster(IntPtr a0, uint a1);
        public void mcSendToMaster(IntPtr a0, uint a1) { grPlugin_mcSendToMaster(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_mcIsMaster();
        public bool mcIsMaster() { return grPlugin_mcIsMaster(); }
        #endregion
    }

    [SuppressUnmanagedCodeSecurity]
    internal class NativePlugin64 : NativePlugin
    {
#if GETREAL3D_LOAD_DEBUG_PLUGIN
        private const string grPluginFileName = "gr_plugin64d";
#else
        private const string grPluginFileName = "gr_plugin64";
#endif

        public string pluginFileName() { return grPluginFileName; }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_init(string configPath, bool noCluster);
        public int init(string configPath, bool noCluster) { return grPlugin_init(configPath, noCluster); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_initWithString(string configPath, bool noCluster);
        public int initWithString(string configPath, bool noCluster) { return grPlugin_initWithString(configPath, noCluster); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_deinit();
        public void deinit() { grPlugin_deinit(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_clusterShutdown();
        public void clusterShutdown() { grPlugin_clusterShutdown(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_initialized();
        public bool initialized() { return grPlugin_initialized(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPluginUnity_update();
        public bool update() { return grPluginUnity_update(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getAppWindowHidden();
        public bool getAppWindowHidden() { return grPlugin_getAppWindowHidden(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAppWindowHeight();
        public int getAppWindowHeight() { return grPlugin_getAppWindowHeight(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAppWindowWidth();
        public int getAppWindowWidth() { return grPlugin_getAppWindowWidth(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAppWindowWidth();
        public int getMasterWindowWidth() { return grPlugin_getMasterAppWindowWidth(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAppWindowHeight();
        public int getMasterWindowHeight() { return grPlugin_getMasterAppWindowHeight(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumberOfCameras();
        public int getNumberOfCameras() { return grPlugin_getNumberOfCameras(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getCameraShowsUI(uint _cameraIndex);
        public bool getCameraShowsUI(uint _cameraIndex) { return grPlugin_getCameraShowsUI(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_getCameraUseRTT(uint _cameraIndex);
        public bool getCameraUseRTT(uint _cameraIndex) { return grPlugin_getCameraUseRTT(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraWidth(uint _cameraIndex);
        public int getCameraWidth(uint _cameraIndex) { return grPlugin_getCameraWidth(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraHeight(uint _cameraIndex);
        public int getCameraHeight(uint _cameraIndex) { return grPlugin_getCameraHeight(_cameraIndex); }

        //[DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //private static extern bool grPlugin_resizeCamera(uint _cameraIndex, int w, int h);
        //public bool resizeCamera(uint _cameraIndex, int w, int h) { return grPlugin_resizeCamera(_cameraIndex, w, h); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraViewport(uint _cameraIndex, float[] _viewport);
        public int getCameraViewport(uint _cameraIndex, float[] _viewport) { return grPlugin_getCameraViewport(_cameraIndex, _viewport); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getCameraAspectRatio(uint _cameraIndex);
        public float getCameraAspectRatio(uint _cameraIndex) { return grPlugin_getCameraAspectRatio(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getCameraFieldOfView(uint _cameraIndex);
        public float getCameraFieldOfView(uint _cameraIndex) { return grPlugin_getCameraFieldOfView(_cameraIndex); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraProjectionMatrix(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);
        public int getCameraProjectionMatrix(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix) { return grPlugin_getCameraProjectionMatrix(_cameraIndex, _cameraNear, _cameraFar, _matrix); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraProjectionMatrixD3D(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix);
        public int getCameraProjectionMatrixD3D(uint _cameraIndex, float _cameraNear, float _cameraFar, float[] _matrix) { return grPlugin_getCameraProjectionMatrixD3D(_cameraIndex, _cameraNear, _cameraFar, _matrix); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraRotation(uint _cameraIndex, float[] _quat);
        public int getCameraRotation(uint _cameraIndex, float[] _quat) { return grPlugin_getCameraRotation(_cameraIndex, _quat); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getCameraPosition(uint _cameraIndex, float[] _eyePosition);
        public int getCameraPosition(uint _cameraIndex, float[] _eyePosition) { return grPlugin_getCameraPosition(_cameraIndex, _eyePosition); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadPositionRotation(float[] _headPosition, float[] _headRotation);
        public int getHeadPositionRotation(float[] _headPosition, float[] _headRotation) { return grPlugin_getHeadPositionRotation(_headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandPositionRotation(float[] _wandPosition, float[] _wandRotation);
        public int getWandPositionRotation(float[] _wandPosition, float[] _wandRotation) { return grPlugin_getWandPositionRotation(_wandPosition, _wandRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumSensors();
        public int getNumSensors() { return grPlugin_getNumSensors(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getSensorPositionRotation(int _sensorIndex, float[] _sensorPosition, float[] _sensorRotation);
        public int getSensorPositionRotation(int _sensorIndex, float[] _sensorPosition, float[] _sensorRotation) { return grPlugin_getSensorPositionRotation(_sensorIndex, _sensorPosition, _sensorRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumButtons();
        public int getNumButtons() { return grPlugin_getNumButtons(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getButton(uint _buttonNumber);
        public int getButton(uint _buttonNumber) { return grPlugin_getButton(_buttonNumber); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getButtons(int[] _buttonArray);
        public int getButtons(int[] _buttonArray) { return grPlugin_getButtons(_buttonArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNumValuators();
        public int getNumValuators() { return grPlugin_getNumValuators(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getValuator(uint _valuatorNumber);
        public float getValuator(uint _valuatorNumber) { return grPlugin_getValuator(_valuatorNumber); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getValuators(float[] _valuatorArray);
        public int getValuators(float[] _valuatorArray) { return grPlugin_getValuators(_valuatorArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getWorldScale();
        public float getWorldScale() { return grPlugin_getWorldScale(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setWorldScale(float _worldScale);
        public void setWorldScale(float _worldScale) { grPlugin_setWorldScale(_worldScale); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern float grPlugin_getEyeSeparation();
        public float getEyeSeparation() { return grPlugin_getEyeSeparation(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setEyeSeparation(float _eyeSeparation);
        public void setEyeSeparation(float _eyeSeparation) { grPlugin_setEyeSeparation(_eyeSeparation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_isDistrib();
        public bool isDistrib() { return grPlugin_isDistrib(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_stopDistrib();
        public void stopDistrib() { grPlugin_stopDistrib(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getNodeID();
        public uint getNodeID() { return grPlugin_getNodeID(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getDisplayID();
        public uint getDisplayID() { return grPlugin_getDisplayID(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getClusterID();
        public uint getClusterID() { return grPlugin_getClusterID(); }

        public bool isMaster() { return (getNodeID() == 0) && (getDisplayID() == 0); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMgxWindowId();
        public int getMgxWindowId() { return grPlugin_getMgxWindowId(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getAppServerHostName();
        public IntPtr getAppServerHostName() { return grPlugin_getAppServerHostName(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getHostName();
        public IntPtr getHostName() { return grPlugin_getHostName(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        private static extern IntPtr grPlugin_getApplicationConfig();
        public IntPtr getApplicationConfig() { return grPlugin_getApplicationConfig(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_sendDistributedData(byte[] data, uint length);
        public bool sendDistributedData(byte[] data, uint length) { return grPlugin_sendDistributedData(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_recvDistributedData(byte[] data, uint maxLength, uint offset, bool wait);
        public uint recvDistributedData(byte[] data, uint maxLength, uint offset, bool wait = false) { return grPlugin_recvDistributedData(data, maxLength, offset, wait); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getNextDistributedDataSize(bool wait);
        public uint getNextDistributedDataSize(bool wait = false) { return grPlugin_getNextDistributedDataSize(wait); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getConfigCount();
        public int getConfigCount() { return grPlugin_getConfigCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNodeCount();
        public int getNodeCount() { return grPlugin_getNodeCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getNodeConfigCount(int nodeId);
        public int getNodeConfigCount(int nodeId) { return grPlugin_getNodeConfigCount(nodeId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_isAudioPlayer();
        public bool isAudioPlayer() { return grPlugin_isAudioPlayer(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCount(int nodeId, int displayId);
        public int getScreenCount(int nodeId, int displayId) { return grPlugin_getScreenCount(nodeId, displayId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCoordinates(uint nodeId, uint displayId, uint screenId, float[] coords);
        public int getScreenCoordinates(uint nodeId, uint displayId, uint screenId, float[] coords) { return grPlugin_getScreenCoordinates(nodeId, displayId, screenId, coords); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadIndex();
        public int getHeadIndex() { return grPlugin_getHeadIndex(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandIndex();
        public int getWandIndex() { return grPlugin_getWandIndex(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getFrameCount();
        public int getFrameCount() { return grPlugin_getFrameCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_logMsg(LogLevel logLevel, string msg);
        public void logMsg(LogLevel logLevel, string msg) { grPlugin_logMsg(logLevel, msg); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWindowSettings(IntPtr win, uint structSize);
        public WindowSettings getWindowSettings()
        {
            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WindowSettings)));
            int res = grPlugin_getWindowSettings(pnt, (uint)Marshal.SizeOf(typeof(WindowSettings)));
            if (res != 1)
            {
                throw new Exception("Error during grPlugin_getWindowSettings call.");
            }
            return (WindowSettings)Marshal.PtrToStructure(pnt, typeof(WindowSettings));
        }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_setTextureFromUnity(int id, System.IntPtr texture);
        public void setTextureFromUnity(int id, System.IntPtr texture) { grPlugin_setTextureFromUnity(id, texture); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getCameraEventCallback();
        public IntPtr getCameraEventCallback() { return grPlugin_getCameraEventCallback(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getEndOfFrameEventFunc();
        public IntPtr getEndOfFrameEventFunc() { return grPlugin_getEndOfFrameEventFunc(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getCameraProfilingCallback();
        public IntPtr getCameraProfilingCallback() { return grPlugin_getCameraProfilingCallback(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterInputs(byte[] _keysArray, byte[] _buttonsArray, float[] _axisArray);
        public int getMasterInputs(byte[] _keysArray, byte[] _buttonsArray, float[] _axisArray) { return grPlugin_getMasterInputs(_keysArray, _buttonsArray, _axisArray); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterKeyCount();
        public int getMasterKeyCount() { return grPlugin_getMasterKeyCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterAxisCount();
        public int getMasterAxisCount() { return grPlugin_getMasterAxisCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getMasterButtonCount();
        public int getMasterButtonCount() { return grPlugin_getMasterButtonCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandIndexForUser(uint id);
        public int getWandIndexForUser(uint id) { return grPlugin_getWandIndexForUser(id); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadIndexForUser(uint id);
        public int getHeadIndexForUser(uint id) { return grPlugin_getHeadIndexForUser(id); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getHeadPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation);
        public int getHeadPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation) { return grPlugin_getHeadPositionRotationForUser(userId, _headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getWandPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation);
        public int getWandPositionRotationForUser(uint userId, float[] _headPosition, float[] _headRotation) { return grPlugin_getWandPositionRotationForUser(userId, _headPosition, _headRotation); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPlugin_getUserCount();
        public uint getUserCount() { return grPlugin_getUserCount(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getUserName(uint id);
        public string getUserName(uint id)
        {
            IntPtr name = grPlugin_getUserName(id);
            if (name != IntPtr.Zero) { return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(name); }
            else { throw new System.ArgumentOutOfRangeException("Invalid user ID in getUserName"); }
        }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getUserFromCamera(uint cameraId);
        public int getUserFromCamera(uint cameraId) { return grPlugin_getUserFromCamera(cameraId); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getAudioUser();
        public int getAudioUser() { return grPlugin_getAudioUser(); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPluginUnity_setPayload(byte[] data, uint length);
        public void setPayload(byte[] data, uint length) { grPluginUnity_setPayload(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPluginUnity_getPayload(byte[] data, uint length);
        public uint getPayload(byte[] data, uint length) { return grPluginUnity_getPayload(data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint grPluginUnity_getClientPayload(uint clientId, byte[] data, uint length);
        public uint getClientPayload(uint id, byte[] data, uint length) { return grPluginUnity_getClientPayload(id, data, length); }

        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getRandomSeed();
        public int getRandomSeed() { return grPlugin_getRandomSeed(); }

        #region AutogenImplementation64
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_createLogger(string a0);
        public IntPtr createLogger(string a0) { return grPlugin_createLogger(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_releaseLogger(IntPtr a0);
        public void releaseLogger(IntPtr a0) { grPlugin_releaseLogger(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_log(LogLevel a0, IntPtr a1, string a2);
        public void log(LogLevel a0, IntPtr a1, string a2) { grPlugin_log(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getSwapInterval();
        public int getSwapInterval() { return grPlugin_getSwapInterval(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenBuffer(uint a0, uint a1, uint a2);
        public int getScreenBuffer(uint a0, uint a1, uint a2) { return grPlugin_getScreenBuffer(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getScreenCoordinatesByName(string a0, float[] a1);
        public int getScreenCoordinatesByName(string a0, float[] a1) { return grPlugin_getScreenCoordinatesByName(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern ScreenSpace grPlugin_getConfigScreenSpace(int a0);
        public ScreenSpace getConfigScreenSpace(int a0) { return (ScreenSpace)grPlugin_getConfigScreenSpace(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int grPlugin_getConfigScreenCount();
        public int getConfigScreenCount() { return grPlugin_getConfigScreenCount(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr grPlugin_getScreenName(int a0);
        public string getScreenName(int a0) { IntPtr res = grPlugin_getScreenName(a0); return (res != IntPtr.Zero) ? System.Runtime.InteropServices.Marshal.PtrToStringAnsi(res) : ""; }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcStartServer();
        public void mcStartServer() { grPlugin_mcStartServer(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcStopServer();
        public void mcStopServer() { grPlugin_mcStopServer(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientConnected(Action<uint, string, string> a0);
        public void mcOnClientConnected(Action<uint, string, string> a0) { grPlugin_mcOnClientConnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientDisconnected(Action<uint> a0);
        public void mcOnClientDisconnected(Action<uint> a0) { grPlugin_mcOnClientDisconnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientMessage(Action<uint, IntPtr, uint> a0);
        public void mcOnClientMessage(Action<uint, IntPtr, uint> a0) { grPlugin_mcOnClientMessage(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnServerError(Action<int, int> a0);
        public void mcOnServerError(Action<int, int> a0) { grPlugin_mcOnServerError(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnClientError(Action<int> a0);
        public void mcOnClientError(Action<int> a0) { grPlugin_mcOnClientError(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToClient(uint a0, IntPtr a1, uint a2);
        public void mcSendToClient(uint a0, IntPtr a1, uint a2) { grPlugin_mcSendToClient(a0, a1, a2); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToClients(IntPtr a0, uint a1);
        public void mcSendToClients(IntPtr a0, uint a1) { grPlugin_mcSendToClients(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_mcIsClient();
        public bool mcIsClient() { return grPlugin_mcIsClient(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcConnectMaster(string a0);
        public void mcConnectMaster(string a0) { grPlugin_mcConnectMaster(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcDisconnectMaster();
        public void mcDisconnectMaster() { grPlugin_mcDisconnectMaster(); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterConnected(Action<string, string, uint> a0);
        public void mcOnMasterConnected(Action<string, string, uint> a0) { grPlugin_mcOnMasterConnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterDisconnected(Action a0);
        public void mcOnMasterDisconnected(Action a0) { grPlugin_mcOnMasterDisconnected(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcOnMasterMessage(Action<IntPtr, uint> a0);
        public void mcOnMasterMessage(Action<IntPtr, uint> a0) { grPlugin_mcOnMasterMessage(a0); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void grPlugin_mcSendToMaster(IntPtr a0, uint a1);
        public void mcSendToMaster(IntPtr a0, uint a1) { grPlugin_mcSendToMaster(a0, a1); }
        [DllImport(grPluginFileName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool grPlugin_mcIsMaster();
        public bool mcIsMaster() { return grPlugin_mcIsMaster(); }
        #endregion

    }

    internal static class NativePluginFactory
    {
        static public NativePlugin Create()
        {
            return (IntPtr.Size == 8) ? (NativePlugin)(new NativePlugin64()) : (NativePlugin)(new NativePlugin32());
        }
    }
}
