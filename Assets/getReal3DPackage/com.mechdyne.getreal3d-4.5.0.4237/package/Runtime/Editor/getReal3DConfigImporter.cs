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

#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Xml.XPath;
using System;

#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace getReal3D.Editor
{
    /// <summary>
    /// Class used to import .gr3d configuration files into Unity assets.
    /// </summary>
    [ScriptedImporter(1, "gr3d")]
    public class getReal3DConfigImporter : ScriptedImporter
    {
        /// <summary>
        ///  This method must by overriden by the derived class and is called by the Asset
        ///  pipeline to import files.
        /// </summary>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var config = ScriptableObject.CreateInstance<getReal3DConfig>();
            config.content = File.ReadAllText(ctx.assetPath);
            config.info = getConfigInfo(ctx.assetPath);
            ctx.AddObjectToAsset("config", config);
            ctx.SetMainObject(config);
        }

        private static getReal3DConfig.ConfigInfo getConfigInfo(string fullName)
        {
            try
            {
                var doc = new XPathDocument(fullName);
                var nav = doc.CreateNavigator();
                getReal3DConfig.ConfigInfo res = new getReal3DConfig.ConfigInfo();
                var path = "/boost_serialization/display/px/DisplaySystem/name";
                res.name = nav.SelectSingleNode(path).ToString();
                return res;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return new getReal3DConfig.ConfigInfo();
            }
        }
    }
}
#endif
