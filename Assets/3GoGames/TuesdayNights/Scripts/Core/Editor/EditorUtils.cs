using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class EditorUtils
{
    public static IEnumerable<GameObject> SceneRoots()
    {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded))
        {
            yield return prop.pptrValue as GameObject;
        }
    }

    public static IEnumerable<Transform> AllSceneObjects()
    {
        var queue = new Queue<Transform>();

        foreach (var root in SceneRoots())
        {
            var tf = root.transform;
            yield return tf;
            queue.Enqueue(tf);
        }

        while (queue.Count > 0)
        {
            foreach (Transform child in queue.Dequeue())
            {
                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    public static List<Transform> SceneRootsList()
    {
        return EditorUtils.SceneRoots().Select(g => g.transform).ToList();
    }
}
