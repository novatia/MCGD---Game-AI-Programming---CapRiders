using UnityEngine;
using UnityEditor;

using FullInspector;

// [CustomPropertyEditor(typeof(UserStatConditionDescriptor))]
public class UserStatConditionDescriptorPropertyEditor : PropertyEditor<UserStatConditionDescriptor>
{
    public override UserStatConditionDescriptor Edit(Rect region, GUIContent label, UserStatConditionDescriptor element, fiGraphMetadata metadata)
    {
        return base.Edit(region, label, element, metadata);
    }

    public override float GetElementHeight(GUIContent label, UserStatConditionDescriptor element, fiGraphMetadata metadata)
    {
        return base.GetElementHeight(label, element, metadata);
    }
}
