using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(IntRange))]
    public class Generated_IntRange_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_IntRange_MonoBehaviourStorage, IntRange> {
        public override bool CanEdit(Type type) {
            return typeof(IntRange).IsAssignableFrom(type);
        }
    }
}
