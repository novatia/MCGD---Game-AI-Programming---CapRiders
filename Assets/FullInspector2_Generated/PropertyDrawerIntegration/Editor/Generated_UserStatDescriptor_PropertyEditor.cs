using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(UserStatDescriptor))]
    public class Generated_UserStatDescriptor_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_UserStatDescriptor_MonoBehaviourStorage, UserStatDescriptor> {
        public override bool CanEdit(Type type) {
            return typeof(UserStatDescriptor).IsAssignableFrom(type);
        }
    }
}
