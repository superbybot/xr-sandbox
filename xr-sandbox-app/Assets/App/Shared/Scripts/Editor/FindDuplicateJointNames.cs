// Assets/App/Shared/Scripts/Editor/FindDuplicateJointNames.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace App.Shared.Scripts.Editor
{
    public class FindDuplicateJointNames : EditorWindow
    {
        // ---------------------------------------------------------------------
        // Selected‑object scan (new behaviour)
        // ---------------------------------------------------------------------
        [MenuItem("Tools/Find Duplicate Joint Names (Selection)")]
        private static void FindInSelection()
        {
            var selected = Selection.activeTransform;
            if (selected == null)
            {
                Debug.LogWarning("No GameObject selected. Please select a root object first.");
                return;
            }
            var transforms = selected.GetComponentsInChildren<Transform>(true);
            var nameMap = new Dictionary<string, List<Transform>>();
            foreach (var t in transforms)
            {
                if (!nameMap.TryGetValue(t.name, out var list))
                {
                    list = new List<Transform>();
                    nameMap[t.name] = list;
                }
                list.Add(t);
            }
            ReportDuplicates(nameMap, $"under \"{selected.name}\"");
        }

        // ---------------------------------------------------------------------
        // Helper that prints the results to the Console
        // ---------------------------------------------------------------------
        private static void ReportDuplicates(Dictionary<string, List<Transform>> nameMap, string context)
        {
            bool any = false;
            foreach (var kvp in nameMap)
            {
                if (kvp.Value.Count > 1)
                {
                    any = true;
                    Debug.Log($"Duplicate name \"{kvp.Key}\" ({kvp.Value.Count} occurrences) {context}:");
                    foreach (var dup in kvp.Value)
                    {
                        // Build a full hierarchy path for easier identification
                        string path = dup.name;
                        Transform parent = dup.parent;
                        while (parent != null)
                        {
                            path = parent.name + "/" + path;
                            parent = parent.parent;
                        }
                        Debug.Log($"    → {path} (instance ID {dup.GetInstanceID()})");
                    }
                }
            }
            if (!any)
                Debug.Log($"No duplicate GameObject names found {context}.");
        }
    }
}
