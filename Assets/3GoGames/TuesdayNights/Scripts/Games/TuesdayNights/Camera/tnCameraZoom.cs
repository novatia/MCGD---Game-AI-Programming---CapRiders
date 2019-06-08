using UnityEngine;

using System;
using System.Collections;

public class tnCameraZoom : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_Offset = Vector2.zero;

    [SerializeField]
    private float m_StepFactorPosition = 0.5f;
    [SerializeField]
    private float m_StepFactorSize = 0.5f;

    [SerializeField]
    private float m_MinSize = 0.5f;

    [SerializeField]
    private float m_Duration = 2.0f;

    private FilteredFloat m_FilterX;
    private FilteredFloat m_FilterY;

    private FilteredFloat m_FilterSize;

    private tnMatchController m_MatchController = null;
    private tnGameCamera m_GameCamera = null;

    void Awake()
    {
        m_MatchController = FindObjectOfType<tnMatchController>();

        if (m_MatchController == null)
        {
            Debug.LogWarning("[tnCameraZoom] Missing MatchController.");
        }

        m_GameCamera = FindObjectOfType<tnGameCamera>();

        if (m_GameCamera == null)
        {
            Debug.LogWarning("[tnCameraZoom] Missing GameCamera.");
        }
    }

    public void SetHighlighted(int i_Index, Action i_OnCompleted = null)
    {
        StartCoroutine(InternalSetHighlighted(i_Index, i_OnCompleted));
    }

    private IEnumerator InternalSetHighlighted(int i_Id, Action i_OnCompleted = null)
    {
        if (m_GameCamera != null && m_MatchController != null)
        {
            GameObject character = m_MatchController.GetCharacterById(i_Id);
            if (character != null)
            {
                // Hide all characters' hud.

                for (int characterIndex = 0; characterIndex < m_MatchController.charactersCount; ++characterIndex)
                {
                    GameObject characterGo = m_MatchController.GetCharacterByIndex(characterIndex);

                    tnCharacterViewController controller = characterGo.GetComponent<tnCharacterViewController>();
                    if (controller != null)
                    {
                        controller.SetPlayerNameVisible(false);
                        controller.SetEnergyBarVisible(false);

                        controller.SetArrowVisible(false);
                        controller.SetChargingForceBarVisible(false);
                    }
                }

                // Animate camera.

                Vector3 originalPosition = m_GameCamera.position;
                float originalSize = m_GameCamera.size;

                m_FilterX = new FilteredFloat(m_StepFactorPosition, m_StepFactorPosition);
                m_FilterY = new FilteredFloat(m_StepFactorPosition, m_StepFactorPosition);

                m_FilterSize = new FilteredFloat(m_StepFactorSize, m_StepFactorSize);

                m_FilterX.position = originalPosition.x;
                m_FilterY.position = originalPosition.y;

                m_FilterSize.position = originalSize;

                m_GameCamera.SetAutoMove(false);

                float timer = 0f;

                while (timer < m_Duration)
                {
                    Transform characterTransform = character.transform;

                    Vector3 targetPosition = characterTransform.position;
                    targetPosition += new Vector3(m_Offset.x, m_Offset.y, 0f);

                    float newX = m_FilterX.Step(targetPosition.x, Time.deltaTime);
                    float newY = m_FilterY.Step(targetPosition.y, Time.deltaTime);

                    Vector3 newPosition = new Vector3(newX, newY, originalPosition.z);
                    float newSize = m_FilterSize.Step(m_MinSize, Time.deltaTime);

                    m_GameCamera.SetPosition(newPosition);
                    m_GameCamera.SetSize(newSize);

                    timer += Time.deltaTime;

                    yield return null;
                }

                m_GameCamera.SetPosition(originalPosition);
                m_GameCamera.SetSize(originalSize);

                m_GameCamera.SetAutoMove(true);

                // Restore hud visibily.

                for (int characterIndex = 0; characterIndex < m_MatchController.charactersCount; ++characterIndex)
                {
                    GameObject characterGo = m_MatchController.GetCharacterByIndex(characterIndex);

                    tnCharacterViewController controller = characterGo.GetComponent<tnCharacterViewController>();
                    if (controller != null)
                    {
                        controller.SetPlayerNameVisible(true);
                        controller.SetEnergyBarVisible(true);

                        controller.SetArrowVisible(true);
                        controller.SetChargingForceBarVisible(true);
                    }
                }
            }
        }

        // Notify caller.

        if (i_OnCompleted != null)
        {
            i_OnCompleted();
        }
    }
}
