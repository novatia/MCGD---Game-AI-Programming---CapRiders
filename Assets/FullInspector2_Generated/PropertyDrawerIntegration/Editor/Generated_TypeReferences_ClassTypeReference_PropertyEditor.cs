using System;
using FullInspector.Internal;

namespace FullInspector.Generated {
    [CustomPropertyEditor(typeof(TypeReferences.ClassTypeReference))]
    public class Generated_TypeReferences_ClassTypeReference_PropertyEditor : fiGenericPropertyDrawerPropertyEditor<Generated_TypeReferences_ClassTypeReference_MonoBehaviourStorage, TypeReferences.ClassTypeReference> {
        public override bool CanEdit(Type type) {
            return typeof(TypeReferences.ClassTypeReference).IsAssignableFrom(type);
        }
    }
}
