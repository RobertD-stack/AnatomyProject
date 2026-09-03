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
using System.Xml;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// The getReal3D Config class provides runtime access to Unity specific configuration settings.
    /// </summary>
    static public class Config
    {
        /// <summary>
        /// Get the desired UnityEngine.RenderingPath for getReal3D cameras.
        /// </summary>
        static public RenderingPath renderingPath { get; internal set; }
        /// <summary>
        /// Set to true if the rendering path was specified in the configuration file.
        /// </summary>
        static public bool renderingPathSet { get; internal set; }
        /// <summary>
        /// Get the desired framerate for Unity instances.
        /// </summary>
        static public int targetFrameRate { get; internal set; }
        /// <summary>
        /// Get the desired UnityEngine.QualityLevel for Unity instances.
        /// </summary>
        static public int qualityLevel { get; internal set; }

        static public bool UseSmoothDeltaTime { get; internal set; } = true;

        static Config()
        {
            Init();
        }

        internal static void setVSync()
        {
            string overrideVSync = System.Environment.GetEnvironmentVariable("GETREAL3D_OVERRIDE_VSYNC");
            if (string.IsNullOrEmpty(overrideVSync))
            {
                getReal3D.Plugin.debug("No GETREAL3D_OVERRIDE_VSYNC set ... using getReal3D defaults");
                int vsync = getReal3D.Plugin.getSwapInterval();
                getReal3D.Plugin.debug("Using configuration setting: " + vsync.ToString());
                QualitySettings.vSyncCount = vsync;
            }
            else
            {
                getReal3D.Plugin.debug("Found GETREAL3D_OVERRIDE_VSYNC = " + overrideVSync);
                int vSyncCount = 0;
                if (!int.TryParse(overrideVSync, out vSyncCount))
                {
                    string lower = overrideVSync.ToLower();
                    vSyncCount = (lower == "yes" || lower == "y" || lower == "on" || lower == "true") ? 1 : 0;
                }
                QualitySettings.vSyncCount = vSyncCount;
            }
            getReal3D.Plugin.info("QualitySettings.vSyncCount = " + QualitySettings.vSyncCount.ToString());
        }

        private static void Init()
        {
            string appConfig = getReal3D.Plugin.getApplicationConfig();
            if (appConfig.Length > 0)
                setRenderingOptions(appConfig);
            else
                getReal3D.Plugin.warning("APP_CONFIG does not contain UNITY element");
        }

        private static void setRenderingOptions(string appConfig)
        {
            // defaults
            RenderingPath rp = RenderingPath.UsePlayerSettings;
            int tfr = -1;
            int ql = QualitySettings.GetQualityLevel();
            renderingPathSet = false;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(appConfig);
            if (doc.HasChildNodes)
            {
                //Plugin.debug("Looking for UNITY element");
                XmlNode unityNode = doc.FirstChild;
                if (unityNode != null)
                {
                    XmlElement unityElem = (XmlElement)unityNode;
                    if (unityElem != null && unityElem.Name == "unity")
                    {
                        readLogLevelSettings(unityElem);
                        XmlNode rendering = unityElem.SelectSingleNode("rendering");
                        if (rendering != null)
                        {
                            if (((XmlElement)rendering).HasAttribute("rendering_path"))
                            {
                                Plugin.debug("Get RenderingPath");
                                string rpStr = rendering.Attributes["rendering_path"].Value;
                                if (!string.IsNullOrEmpty(rpStr))
                                {
                                    rp = (RenderingPath)Enum.Parse(typeof(RenderingPath), rpStr);
                                    renderingPathSet = true;
                                }
                            }

                            if (((XmlElement)rendering).HasAttribute("target_framerate"))
                            {
                                Plugin.debug("Get TargetFrameRate");
                                string tfrStr = rendering.Attributes["target_framerate"].Value;
                                Int32.TryParse(tfrStr, out tfr);
                            }

                            var usdtEntryName = "use_smooth_delta_time";
                            if (((XmlElement)rendering).HasAttribute(usdtEntryName))
                            {
                                string str = rendering.Attributes[usdtEntryName].Value;
                                if (str == "1" || str.Equals("true", StringComparison.OrdinalIgnoreCase))
                                {
                                    UseSmoothDeltaTime = true;
                                }
                                else if (str == "0" || str.Equals("false", StringComparison.OrdinalIgnoreCase))
                                {
                                    UseSmoothDeltaTime = false;
                                }
                                else
                                {
                                    Plugin.warning($"Invalid boolean value: '{str}'");
                                }
                                Plugin.debug($"UseSmoothDeltaTime={UseSmoothDeltaTime}");
                            }

                            if (((XmlElement)rendering).HasAttribute("quality_level"))
                            {
                                Plugin.debug("Get QualityLevel");
                                string qlStr = rendering.Attributes["quality_level"].Value;
                                if (!string.IsNullOrEmpty(qlStr))
                                {
                                    Int32.TryParse(qlStr, out ql);
                                    bool found = false;
                                    for (int i = 0; i < QualitySettings.names.Length && !found; ++i)
                                    {
                                        if (string.Compare(QualitySettings.names[i], qlStr, true) == 0)
                                        {
                                            ql = i;
                                            found = true;
                                        }
                                    }
                                    ql = ql < 0 ? 0 : (ql >= QualitySettings.names.Length ? QualitySettings.names.Length - 1 : ql);
                                    if (!found)
                                    {
                                        Plugin.warning("Could not find QualityLevel named " + qlStr + ". Setting to " + QualitySettings.names[ql]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            renderingPath = rp;
            targetFrameRate = tfr;
            qualityLevel = ql;
        }

        private static void readLogLevelSettings(XmlElement unityElem)
        {
            if (Application.isEditor)
            {
                return;
            }
            if (unityElem.HasAttribute("rpc_log_level"))
            {
                string attrib = unityElem.GetAttribute("rpc_log_level");
                try
                {
                    Settings.get().rpcLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), attrib);
                }
                catch (Exception)
                {
                    Plugin.warning("Failed to parse RPC log level '" + attrib + "'.");
                }
            }
        }

    }
}
