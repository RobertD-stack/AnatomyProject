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
    /// A utilty class for dealing with OnGUI setup across a getReal3D cluster.
    /// </summary>
    public static class GUI
    {
        /// <summary>
        /// Use to override the default GUI width (800 pixels or the specified width of the master instance)
        /// </summary>
        static public int width { get; internal set; }

        /// <summary>
        /// Use to override the default GUI height (600 pixels or the specified height of the master instance)
        /// </summary>
        static public int height { get; internal set; }

        static private bool resizeRequested = false;
        static private bool resized = false;
        static private bool matrixSaved = false;
        static private bool initialized = false;
        static private bool isEnabled = true;
        static private int preferredWidth = -1;
        static private int preferredHeight = -1;

        /// <summary>
        /// Should this Unity instance show its GUI
        /// </summary>
        static public bool enabled
        {
            get { return isEnabled; }
        }

        /// <summary>
        /// Are the getReal3D GUI Utility initialized and the Unity instance resized
        /// </summary>
        static public bool isReady
        {
            get { return initialized && resized; }
        }

        static private Matrix4x4 saved = Matrix4x4.identity;
        static private System.Reflection.PropertyInfo cameraHasEventMask = null;

        static internal void Init()
        {
            if (!getReal3D.Plugin.initialized() || isReady) return;

            if (!initialized)
            {
                isEnabled = Application.isEditor || getReal3D.Plugin.getCameraShowsUI(0);
                Plugin.debug("getReal3D.GUI on: " + isEnabled.ToString());

                // get window size for master (nodes[0]->display[0]) if available, and set H/W accordingly
                if (Application.isEditor || !getReal3D.Plugin.isDistrib())
                {
                    Plugin.debug("Running in editor or standalone ...");
                    width = Screen.width;
                    height = Screen.height;
                }

                if (!isEnabled)
                {
                    cameraHasEventMask = typeof(UnityEngine.Camera).GetProperty("eventMask", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                }

                initialized = true;
            }

            if (initialized && preferredWidth < 0)
            {
                int w = getReal3D.Plugin.getAppWindowWidth();
                int h = getReal3D.Plugin.getAppWindowHeight();

                if (w <= 0) w = Screen.width;
                if (h <= 0) h = Screen.height;

                if (Application.isEditor)
                {
                    Plugin.debug("Running in editor");
                    if (w < width) w = width;
                    if (h < height) h = height;
                }

                preferredWidth = w;
                preferredHeight = h;
            }

            if (Event.current != null && Event.current.type == EventType.Layout && !resizeRequested)
            {
                if (!matrixSaved)
                {
                    saved = UnityEngine.GUI.matrix;
                    matrixSaved = true;
                }

                if (Screen.width != preferredWidth || Screen.height != preferredHeight)
                {
                    Plugin.info("Unity Screen resolution: " + Screen.width.ToString() + "x" + Screen.height.ToString());
                    Plugin.info("Attempting to change resolution: " + preferredWidth.ToString() + "x" + preferredHeight.ToString());
                    Screen.SetResolution(preferredWidth, preferredHeight, false);
                }
                resizeRequested = true; // or not needed, which ever
            }

            if (initialized && resizeRequested && !resized)
            {
                Plugin.debug(string.Format("Resize complete? now {0}x{1}", Screen.width, Screen.height));
                resized = Application.isEditor || (Screen.width == preferredWidth && Screen.height == preferredHeight);
            }

            if (initialized && resized && !isEnabled && cameraHasEventMask != null && Camera.current != null)
            {
                cameraHasEventMask.SetValue(Camera.current, 0, null);
            }

            // height and width need to be == to master instance height and width
        }

        /// <summary>
        /// For cluster use.
        /// Call at the start of OnGUI callbacks to ensure GUI elements are hidden where configured.
        /// Also keeps OnGUI layout consistent, enabling hidden GUIs to receive input in the cluster.
        /// </summary>
        static public bool BeginGUI()
        {
            Init();
            if (!isEnabled && Event.current != null && Event.current.type == EventType.Repaint)
                return false;
            UnityEngine.GUI.matrix = saved;
            return true;
        }

        /// <summary>
        /// For cluster use.
        /// Call at the end of OnGUI callbacks to ensure GUI elements are hidden where configured.
        /// </summary>
        static public void EndGUI()
        {
            Init();
            UnityEngine.GUI.matrix = saved;
        }

        static GUI()
        {
            if (!getReal3D.Plugin.initialized())
                getReal3D.Plugin.initVR();

            height = getReal3D.Plugin.getMasterWindowHeight();
            width = getReal3D.Plugin.getMasterWindowWidth();

            width = width < 0 ? 800 : width;
            height = height < 0 ? 600 : height;

            string appConfig = getReal3D.Plugin.getApplicationConfig();
            Plugin.debug("AppConfigXML: " + appConfig);
            if (appConfig.Length == 0)
                Plugin.warning("APP_CONFIG does not contain UNITY element");

            Init();
        }
    }
}
