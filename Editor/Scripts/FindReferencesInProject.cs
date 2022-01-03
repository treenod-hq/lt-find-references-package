﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#endif
public class FindReferencesInProject
{
    private const string MenuItemText = "Assets/Find References In Project";
#if UNITY_EDITOR
    [MenuItem(MenuItemText, false, 25)]
    public static void Find()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        var referenceCache = new Dictionary<string, List<string>>();

        string[] guids = AssetDatabase.FindAssets("");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

            foreach (var dependency in dependencies)
            {
                if (referenceCache.ContainsKey(dependency))
                {
                    if (!referenceCache[dependency].Contains(assetPath))
                    {
                        referenceCache[dependency].Add(assetPath);
                    }
                }
                else
                {
                    referenceCache[dependency] = new List<string>(){ assetPath };
                }
            }
        }

        Debug.Log("Build index takes " + sw.ElapsedMilliseconds + " milliseconds");

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log("Find: " + path, Selection.activeObject);
        int referenceCount = 0;
        if (referenceCache.ContainsKey(path))
        {
            foreach (var reference in referenceCache[path])
            {
                Debug.Log(reference, AssetDatabase.LoadMainAssetAtPath(reference));
                referenceCount++;
            }
        }

        string popupMessage = "No references";
        if (referenceCount > 0)
        {
            popupMessage = "Find references count : " + referenceCount;
        }
            
        EditorUtility.DisplayDialog("Find references", popupMessage, "OK");
        
        referenceCache.Clear();
    }

    [MenuItem(MenuItemText, true)]
    public static bool Validate()
    {
        if (Selection.activeObject)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return !AssetDatabase.IsValidFolder(path);
        }

        return false;
    }
#endif
}