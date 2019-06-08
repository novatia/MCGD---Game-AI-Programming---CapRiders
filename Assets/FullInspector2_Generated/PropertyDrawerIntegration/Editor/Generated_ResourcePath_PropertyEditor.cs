using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(ResourcePath))]
    public class Generated_ResourcePath_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_ResourcePath_MonoBehaviourStorage, ResourcePath> {
        public override bool CanEdit(Type type) {
            return typeof(ResourcePath).IsAssignableFrom(type);
        }
    }
}
