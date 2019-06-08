using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour
{
    public Transform[] backgrounds;

    public float minZ = 0f;
    public float maxZ = 10f;

    public float parallaxScaleAtMinZ = 0f;
    public float parallaxScaleAtMaxZ = 1f;

    public float linearityFactor = 1f;

    public float smoothing = 8f;

    public float maxDistance = 15f;

    private Transform m_CameraTransform = null;
    private Vector3 m_PreviousCameraPosition;

    void Start()
    {
        if (Camera.main != null)
        {
            m_CameraTransform = Camera.main.transform;
            m_PreviousCameraPosition = m_CameraTransform.position;
        }
    }

    void Update()
    {
        if (m_CameraTransform == null)
            return;

        float cameraOffset = m_PreviousCameraPosition.x - m_CameraTransform.position.x;

        for (int i = 0; i < backgrounds.Length; ++i)
        {
            float distanceFromCamera = Mathf.Abs(m_CameraTransform.position.x - backgrounds[i].position.x);

            if (distanceFromCamera < maxDistance)
            {
                float parallaxScale = EvaluateParallaxScale(backgrounds[i].position.z);
                float parallax = cameraOffset * parallaxScale;
                float backgroundTargetPosX = backgrounds[i].position.x + parallax;

                Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

                if (smoothing > 0f)
                {
                    backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
                }
                else
                {
                    backgrounds[i].position = backgroundTargetPos;
                }
            }
        }

        m_PreviousCameraPosition = m_CameraTransform.position;
    }

    private float EvaluateParallaxScale(float i_Z)
    {
        float perc = Mathf.Clamp01((i_Z - minZ) / (maxZ - minZ));
        float factor = Mathf.Pow(perc, linearityFactor);
        float parallaxScale = Mathf.Lerp(parallaxScaleAtMinZ, parallaxScaleAtMaxZ, factor);
        return parallaxScale;
    }
}
