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
    /// The getReal.InputManager takes care of updating the trackd input data on a regular basis.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private static GameObject instance;

        static private int lastUpdateFrame = -1;
        static private bool initialized = false;

        internal static void Init()
        {
            if (instance == null)
            {
                instance = GameObject.Find("GetRealManager");
                if (instance == null)
                    instance = new GameObject("GetRealManager");
                DontDestroyOnLoad(instance);
                instance.AddComponent<InputManager>();
            }
            if (initialized) return;

            Application.runInBackground = true;

            getReal3D.Plugin.info("Application.targetFrameRate = " + Config.targetFrameRate.ToString());
            Application.targetFrameRate = Config.targetFrameRate;

            getReal3D.Plugin.info("QualityLevel = " + QualitySettings.names[Config.qualityLevel].ToString());
            QualitySettings.SetQualityLevel(Config.qualityLevel, false);

            QualitySettings.maxQueuedFrames = 0;
            Config.setVSync();
            initialized = true;
        }

        internal static InputManager Instance
        {
            get
            {
                if (instance == null) Init();
                return instance.GetComponent<InputManager>();
            }
        }

        void Awake()
        {
            if (!getReal3D.Plugin.initialized())
            {
                getReal3D.Plugin.initVR();
#if TOUCH_ENABLED
                getReal3D.TouchProcessor.Init();
#endif
            }
        }

        void OnApplicationQuit()
        {
            Plugin.debug("OnApplicationQuit");
            getReal3D.Plugin.deinit();
#if TOUCH_ENABLED
            getReal3D.TouchProcessor.DeInit();
#endif
            instance = null;
            lastUpdateFrame = -1;
            initialized = false;
        }

        internal bool UpdateIfNeeded()
        {
            if (lastUpdateFrame < Time.frameCount)
            {
                lastUpdateFrame = Time.frameCount;
                //Plugin.info("UpdateIfNeeded Time: " + Time.realtimeSinceStartup.ToString());
                if (getReal3D.Plugin.updateData())
                {
                    getReal3D.Input.Update();
#if TOUCH_ENABLED
                    getReal3D.TouchProcessor.Update();
#endif
                    return true;
                }
                else
                {
                    Plugin.debug("update quit");
                    Application.Quit();
                    return false;
                }
            }
            return false;
        }

        void Update()
        {
            if (!getReal3D.Plugin.initialized()) return;
            UpdateIfNeeded();
            if (!Application.isEditor)
            {
                if (AudioListener.volume > 0f && !getReal3D.Plugin.isAudioPlayer()) // want to make this configurable one day
                {
                    AudioListener.volume = 0f;
                }
            }
        }

        void LateUpdate()
        {
            if (!getReal3D.GUI.isReady)
            {
                getReal3D.GUI.Init();
            }
        }

        void OnGUI()
        {
            if (!Application.isEditor && !getReal3D.GUI.isReady)
            {
                getReal3D.GUI.Init();
            }
        }
    }
}
