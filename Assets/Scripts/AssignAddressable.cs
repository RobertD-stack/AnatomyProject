using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using System.IO;

public enum ReassignAddressables
{
    True,
    False
}

[InitializeOnLoad]
public class AddressableAssignmentClass: MonoBehaviour
{
    public ReassignAddressables reassignAddressables;

    void Awake()
    {
        if (reassignAddressables == ReassignAddressables.True)
        {
            MarkAssetsInFolderAddressable();
        }
    }

    public static void MarkAssetsInFolderAddressable()
    {
        string folderPath = "Assets/Resources_moved/Edited Human Parts/List"; // Change this to your folder
        string groupName = "Default Local Group";    // Optional: Group name

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Make sure Addressables are set up.");
            return;
        }

        // Create group if it doesn't exist
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
        }

        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (asset == null || AssetDatabase.IsValidFolder(assetPath))
                continue;

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = Path.GetFileNameWithoutExtension(assetPath); // Use file name as address
            Debug.Log($"Marked {assetPath} as addressable with address: {entry.address}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}