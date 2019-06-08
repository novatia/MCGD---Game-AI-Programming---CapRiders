using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(tnStatEntry))]
    public class Generated_tnStatEntry_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_tnStatEntry_MonoBehaviourStorage, tnStatEntry> {
        public override bool CanEdit(Type type) {
            return typeof(tnStatEntry).IsAssignableFrom(type);
        }
    }
}
