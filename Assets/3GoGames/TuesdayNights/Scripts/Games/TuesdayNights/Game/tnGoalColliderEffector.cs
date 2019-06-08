using UnityEngine;

using TrueSync;

public class tnGoalColliderEffector : TSColliderEffector2D
{
    [Header("Custom Logic")]

    [SerializeField]
    private LayerMask m_ExcludeLayerMask = 0;

    // TSColliderEffector2D's interface

    protected override bool OnValidateGameObject(GameObject i_GameObject)
    {
        bool valid = !Layer.IsGameObjectInLayerMask(i_GameObject, m_ExcludeLayerMask);

        tnCharacterController characterController = i_GameObject.GetComponent<tnCharacterController>();
        if (characterController != null)
        {
            int currentLayer = characterController.currentLayer;
            valid = !Layer.IsLayerInMask(currentLayer, m_ExcludeLayerMask);
        }

        return valid;
    }
}