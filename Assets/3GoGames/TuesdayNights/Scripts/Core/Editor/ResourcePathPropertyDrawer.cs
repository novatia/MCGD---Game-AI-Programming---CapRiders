using UnityEngine;
using UnityEditor;

using System.IO;

[CustomPropertyDrawer(typeof(ResourcePath))]
public class ResourcePathPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty pathProperty = property.FindPropertyRelative("m_Path");

        bool applyChanges = false;

        string path = pathProperty.stringValue;
        GameObject obj = null;

        if (path != "")
        {
            obj = Resources.Load<GameObject>(path);
            if (obj == null)
            {
                pathProperty.stringValue = "";
                applyChanges = true;
            }
        }

        obj = (GameObject)EditorGUI.ObjectField(position, label, obj, typeof(GameObject), false);
        
        if (obj != null)
        {
            string itemPath = AssetDatabase.GetAssetPath(obj);
            string[] itemPathSplit = itemPath.Split(new string[1] { "Resources/" }, System.StringSplitOptions.None);
            string prefabPath = itemPathSplit[itemPathSplit.Length - 1];
            string extension = Path.GetExtension(prefabPath);
            int extensionLenght = extension.Length;

            prefabPath = prefabPath.Substring(0, prefabPath.Length - extensionLenght);

            pathProperty.stringValue = prefabPath;
            applyChanges = true;
        }

        if (applyChanges)
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
