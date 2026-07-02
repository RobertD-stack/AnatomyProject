#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
using Unity.CodeEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CursorProjectFileSync
{
    static CursorProjectFileSync()
    {
        EditorApplication.delayCall += EnsureProjectFilesExist;
    }

    [MenuItem("Tools/Cursor/Regenerate Project Files")]
    public static void RegenerateProjectFiles()
    {
        if (TrySyncAll())
            Debug.Log("Regenerated .sln and .csproj files for Cursor.");
        else
            Debug.LogWarning(
                "Could not regenerate project files. In Unity go to Edit > Preferences > External Tools, " +
                "set External Script Editor to Cursor, then click Regenerate project files.");
    }

    static void EnsureProjectFilesExist()
    {
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrEmpty(projectRoot))
            return;

        var solutionPath = Path.Combine(projectRoot, "Skeleton Puzzle.sln");
        if (!File.Exists(solutionPath))
            RegenerateProjectFiles();
    }

    static bool TrySyncAll()
    {
        var editor = CodeEditor.CurrentEditor;
        if (editor == null)
            return false;

        var syncAll = editor.GetType().GetMethod(
            "SyncAll",
            BindingFlags.Instance | BindingFlags.Public);
        if (syncAll == null)
            return false;

        syncAll.Invoke(editor, null);
        return true;
    }
}
#endif
