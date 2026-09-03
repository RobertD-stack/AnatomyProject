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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace getReal3D.Editor
{

    [InitializeOnLoad]
    internal class getReal3D_Setup : EditorWindow
    {
#pragma warning disable 169
        static getReal3D_Setup window;
#pragma warning restore 169

        static getReal3D_Setup()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            checkForDuplicatedPlugin();
            checkForMovedFiles();
            RequiredSettingsWindow.ShowIfNeeded();
            EditorApplication.update -= Update;
        }

        private static IEnumerable<string> GetFiles(string path, string regexString)
        {
            try
            {
                var regex = new Regex(regexString);
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).
                        Where(f => regex.IsMatch(f));
            }
            catch (DirectoryNotFoundException)
            {
                return Enumerable.Empty<string>();
            }
        }

        private static bool IsDirectoryEmpty(string path)
        {
            try
            {
                return Directory.GetFiles(path).Count() == 0;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        private static IEnumerable<string> TraverseDirectoriesLRN(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] directories = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
            foreach (var dir in directories)
            {
                foreach (var subdir in TraverseDirectoriesLRN(dir.FullName))
                {
                    yield return subdir;
                }
            }
            yield return di.FullName;
        }

        private static void checkForDuplicatedPlugin()
        {
            try
            {
                string pluginPath = Application.dataPath + "/Plugins";
                if (!Directory.Exists(pluginPath))
                {
                    return;
                }
                foreach (var f in GetFiles(pluginPath, @"gr_plugin(64|32)?d?.dll(.meta)?$"))
                {
                    try
                    {
                        Debug.Log(string.Format("Deleting old plugin {0}.", f));
                        File.Delete(f);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(string.Format("Failed to delete old plugin {0}. {1}",
                            f, e.Message));
                    }
                }
                foreach (var path in TraverseDirectoriesLRN(pluginPath))
                {
                    DeleteDirectoryIfEmpty(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to check for duplicated plugins. " + e.Message);
            }
        }

        private static void DeleteDirectoryIfEmpty(string path)
        {
            string meta = path + ".meta";
            if (Directory.Exists(path) && IsDirectoryEmpty(path))
            {
                try
                {
                    Debug.Log(string.Format("Deleting empty directory {0}.", path));
                    Directory.Delete(path);
                    if (File.Exists(meta))
                    {
                        Debug.Log(string.Format("Deleting meta file {0}.", meta));
                        File.Delete(meta);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("Failed to delete empty directory {0}. {1}",
                        path, e.Message));
                }
            }
        }

        private static string ComputeMD5(string filename)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    byte[] hash = md5.ComputeHash((Stream)fileStream);
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int index = 0; index < hash.Length; ++index)
                        stringBuilder.Append(hash[index].ToString("x2"));
                    return stringBuilder.ToString();
                }
            }
        }

        private static void checkForMovedFiles()
        {
            var filenames = new[] {
            new { Filename = "Apartment Scene/UI/LightsMenu.cs", Hash = "133dbce2fbeac7a4cc6bb00871caba54" },
            new { Filename = "Apartment Scene/UI/LightToggle.prefab", Hash = "9764259aadaae6e165a0d82856f7b15f" },
            new { Filename = "_Shaders/Highlight.mat", Hash = "d25385811c54b165b4dcfbe0da35b0a6" },
            new { Filename = "_Shaders/Highlight.shader", Hash = "6fe094e7151fde17c86a301126243822" },
            new { Filename = "Standard Assets/Character Controllers/Sources/Scripts/CharacterMotor.js", Hash = "7e264895818a2b900b8fee5a8ca98c15" },
            new { Filename = "getReal3D/Resources/cave_config.xml", Hash = "a44679b62646cf921fd84048f603bd90" },
            new { Filename = "getReal3D/Resources/editor_config.xml", Hash = "9c53ea7b07311bfd02260dd7f8ed00dc" },
            new { Filename = "getReal3D/Resources/two_users_config.xml", Hash = "a90a5e56d5a32742957735ad88a8a662" },
        };
            foreach (var data in filenames)
            {
                string filename = Application.dataPath + "/" + data.Filename;
                if (File.Exists(filename))
                {
                    if (ComputeMD5(filename) == data.Hash)
                    {
                        Debug.Log("Deleting file at deprecated location: " + data.Filename + ".");
                        try
                        {
                            File.Delete(filename);
                            File.Delete(filename + ".meta");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Failed to remove duplicated file " + data.Filename + ". " + e.ToString());
                        }
                    }
                }
                else
                {
                    //Debug.Log((object)("The " + data.Filename + " file doesn't exist."));
                }
            }
        }
    }
}
#endif
