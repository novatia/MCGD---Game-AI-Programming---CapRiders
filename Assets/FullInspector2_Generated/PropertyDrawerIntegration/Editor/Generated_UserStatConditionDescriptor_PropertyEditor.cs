using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UserStatConditionDescriptor))]
    public class Generated_UserStatConditionDescriptor_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UserStatConditionDescriptor_MonoBehaviourStorage, UserStatConditionDescriptor> {
        public override bool CanEdit(Type type) {
            return typeof(UserStatConditionDescriptor).IsAssignableFrom(type);
        }
    }
}
