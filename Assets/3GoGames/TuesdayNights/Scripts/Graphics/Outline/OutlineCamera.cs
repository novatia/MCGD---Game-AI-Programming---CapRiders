using UnityEngine;

using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class OutlineCamera : MonoBehaviour
{
    private static int s_BufferMaterialsPoolSize = 256;
    private static int s_CacheSize = 256;

    private List<Renderer> m_OutlinedRenderers = new List<Renderer>();
    private List<Renderer> m_EraserRenderers = new List<Renderer>();

    private Camera m_Camera = null;
    private Camera m_OutlineCamera = null;

    private Shader m_OutlinedShader = null;
    private Shader m_OutlinedBufferShader = null;

    private Material m_OutlinedShaderMaterial = null;
    private Deque<Material> m_OutlinedBufferMaterialsPool = new Deque<Material>(s_BufferMaterialsPoolSize);
    private Deque<Material> m_EraserBufferMaterialsPool = new Deque<Material>(s_BufferMaterialsPoolSize);

    private Deque<Material> m_MaterialsCache = new Deque<Material>(s_CacheSize);
    private Deque<int> m_LayersCache = new Deque<int>(s_CacheSize);

    private RenderTexture m_RenderTexture = null;

    private bool m_Initialized = false;

    public Color lineColor = Color.black;
    public float lineThickness = 1f;
    [Range(0f, 1f)]
    public float alphaCutoff = 0.95f;

    // MonoBehaviour' s interface

    void Awake()
    {
        m_Camera = GetComponent<Camera>();

        CreateMaterials();

        CreateOutlineCamera();

        m_Initialized = true;
    }

    void OnDestroy()
    {
        m_Initialized = false;

        if (m_OutlineCamera)
        {
            Destroy(m_OutlineCamera.gameObject);
            m_OutlineCamera = null;
        }

        DestroyMaterials();
    }

    // Render pipeline callbacks

    void OnPreCull()
    {
        int width = m_Camera.pixelWidth;
        int height = m_Camera.pixelHeight;

        m_RenderTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.Default);
        m_OutlineCamera.targetTexture = m_RenderTexture;

        // Save current Renderer Materials into m_MaterialsCache and overwrite them with buffer Materials.

        for (int rendererIndex = 0; rendererIndex < m_OutlinedRenderers.Count; ++rendererIndex)  // RENDERERS
        {
            Renderer currentRenderer = m_OutlinedRenderers[rendererIndex];
            if (currentRenderer != null)
            {
                // Get a copy of the current materials list.

                Material[] materials = currentRenderer.sharedMaterials; 
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    Material currentMaterial = materials[materialIndex];
                    Texture currentTexture = currentMaterial.mainTexture;

                    m_MaterialsCache.AddBack(currentMaterial); // Save current Material into the cache.

                    Material newMaterial = m_OutlinedBufferMaterialsPool.RemoveFront(); // Take an unused material from the pool.
                    if (newMaterial == null)
                    {
                        newMaterial = CreateMaterial(new Color(1f, 0f, 0f, 0f)); // Fallback case. You should never enter here and do runtime allocations.
                        Debug.LogWarning("[OutlineCamera] Material instance created during runtime. Check current pool size.");
                    }

                    newMaterial.mainTexture = currentTexture; // Set texture from current material.
                    materials[materialIndex] = newMaterial;
                }

                // Overwrite materials (temporary).

                currentRenderer.sharedMaterials = materials; 

                m_LayersCache.AddBack(currentRenderer.gameObject.layer); // Save current layer into the cache.

                currentRenderer.gameObject.layer = LayerMask.NameToLayer("Outline"); // Overwrite layer (temporary).
            }
        }

        for (int rendererIndex = 0; rendererIndex < m_EraserRenderers.Count; ++rendererIndex) // ERASERS
        {
            Renderer currentRenderer = m_EraserRenderers[rendererIndex];
            if (currentRenderer != null)
            {
                // Get a copy of the current materials list.

                Material[] materials = currentRenderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    Material currentMaterial = materials[materialIndex];
                    Texture currentTexture = currentMaterial.mainTexture;

                    m_MaterialsCache.AddBack(currentMaterial); // Save current Material into the cache.

                    Material newMaterial = m_EraserBufferMaterialsPool.RemoveFront(); // Take an unused material from the pool.
                    if (newMaterial == null)
                    {
                        newMaterial = CreateMaterial(new Color(0f, 0f, 0f, 0f)); // Fallback case. You should never enter here and do runtime allocations.
                        Debug.LogWarning("[OutlineCamera] Material instance created during runtime. Check current pool size.");
                    }

                    newMaterial.mainTexture = currentTexture; // Set texture from current material.
                    materials[materialIndex] = newMaterial;
                }

                // Overwrite materials (temporary).

                currentRenderer.sharedMaterials = materials;

                m_LayersCache.AddBack(currentRenderer.gameObject.layer); // Save current layer into the cache.

                currentRenderer.gameObject.layer = LayerMask.NameToLayer("Outline"); // Overwrite layer (temporary).
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Render to texture.

        m_OutlineCamera.Render();

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        for (int rendererIndex = 0; rendererIndex < m_OutlinedRenderers.Count; ++rendererIndex)
        {
            Renderer currentRenderer = m_OutlinedRenderers[rendererIndex];
            if (currentRenderer != null)
            {
                Material[] materials = currentRenderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    Material tempMaterial = materials[materialIndex];
                    tempMaterial.SetTexture(0, null); // Release temporary assigned texture.

                    m_OutlinedBufferMaterialsPool.AddBack(tempMaterial); // Re-insert into pool.
                    
                    Material originalMaterial = m_MaterialsCache.RemoveFront();
                    materials[materialIndex] = originalMaterial; // Restore original Renderer Material.
                }

                currentRenderer.sharedMaterials = materials;

                int originalLayer = m_LayersCache.RemoveFront();
                currentRenderer.gameObject.layer = originalLayer; // Overwrite layer (temporary).
            }
        }

        for (int rendererIndex = 0; rendererIndex < m_EraserRenderers.Count; ++rendererIndex)
        {
            Renderer currentRenderer = m_EraserRenderers[rendererIndex];
            if (currentRenderer != null)
            {
                Material[] materials = currentRenderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
                {
                    Material tempMaterial = materials[materialIndex];
                    tempMaterial.SetTexture(0, null); // Release temporary assigned texture.

                    m_EraserBufferMaterialsPool.AddBack(tempMaterial); // Re-insert into pool.

                    Material originalMaterial = m_MaterialsCache.RemoveFront();
                    materials[materialIndex] = originalMaterial; // Restore original Renderer Material.
                }

                currentRenderer.sharedMaterials = materials;

                int originalLayer = m_LayersCache.RemoveFront();
                currentRenderer.gameObject.layer = originalLayer; // Overwrite layer (temporary).
            }
        }
    }

    void OnRenderImage(RenderTexture i_Source, RenderTexture i_Destination)
    {
#if UNITY_EDITOR
        SetOutlinedShaderMaterialsProperty(); // Refresh materials property if you are in Editor.
#endif

        m_OutlinedShaderMaterial.SetTexture("_OutlineSource", m_RenderTexture);
        Graphics.Blit(i_Source, i_Destination, m_OutlinedShaderMaterial);
        RenderTexture.ReleaseTemporary(m_RenderTexture);
    }

    // LOGIC

    public void RegisterOutlineRenderer(Renderer i_Renderer)
    {
        if (!m_OutlinedRenderers.Contains(i_Renderer))
        {
            m_OutlinedRenderers.Add(i_Renderer);

            if (m_Initialized)
            {
                UpdateRendererMaterialsPoolSize();
            }
        }
    }

    public void UnregisterOutlineRenderer(Renderer i_Renderer)
    {
        m_OutlinedRenderers.Remove(i_Renderer);
    }

    public void RegisterEraseRenderer(Renderer i_Renderer)
    {
        if (!m_EraserRenderers.Contains(i_Renderer))
        {
            m_EraserRenderers.Add(i_Renderer);

            if (m_Initialized)
            {
                UpdateEraserMaterialsPoolSize();
            }
        }
    }

    public void UnregisterEraseRenderer(Renderer i_Renderer)
    {
        m_EraserRenderers.Remove(i_Renderer);
    }

    // INTERNALS

    private void UpdateRendererMaterialsPoolSize()
    {
        int desiredSize = m_OutlinedRenderers.Count * 16;
        desiredSize = Mathf.Max(s_CacheSize, desiredSize);

        int delta = desiredSize - m_OutlinedBufferMaterialsPool.Count;

        if (delta > 0)
        {
            for (int newInstanceIndex = 0; newInstanceIndex < delta; ++newInstanceIndex)
            {
                Material newMaterialInstance = CreateMaterial(new Color(1f, 0f, 0f, 0f));
                m_OutlinedBufferMaterialsPool.AddBack(newMaterialInstance);
                SetBufferShaderMaterialsProperty(newMaterialInstance);
            }
        } 
    }

    private void UpdateEraserMaterialsPoolSize()
    {
        int desiredSize = m_EraserRenderers.Count * 16;
        desiredSize = Mathf.Max(s_CacheSize, desiredSize);

        int delta = desiredSize - m_EraserBufferMaterialsPool.Count;

        if (delta > 0)
        {
            for (int newInstanceIndex = 0; newInstanceIndex < delta; ++newInstanceIndex)
            {
                Material newMaterialInstance = CreateMaterial(new Color(0f, 0f, 0f, 0f));
                m_EraserBufferMaterialsPool.AddBack(newMaterialInstance);
                SetBufferShaderMaterialsProperty(newMaterialInstance);
            }
        }
    }

    private void CreateMaterials()
    {
        if (m_OutlinedShader == null)
        {
            m_OutlinedShader = Resources.Load<Shader>("Shaders/OutlineEffect/OutlineShader");
        }

        if (m_OutlinedBufferShader == null)
        {
            m_OutlinedBufferShader = Resources.Load<Shader>("Shaders/OutlineEffect/OutlineBufferShader");
        }

        if (m_OutlinedShaderMaterial == null)
        {
            m_OutlinedShaderMaterial = new Material(m_OutlinedShader);
            m_OutlinedShaderMaterial.hideFlags = HideFlags.DontSave;

            SetOutlinedShaderMaterialsProperty();
        }

        UpdateRendererMaterialsPoolSize();
        UpdateEraserMaterialsPoolSize();
    }

    private void DestroyMaterials()
    {
        for (int poolIndex = 0; poolIndex < m_EraserBufferMaterialsPool.Count; ++poolIndex)
        {
            Material m = m_EraserBufferMaterialsPool[poolIndex];
            if (m != null)
            {
                Destroy(m);
            }
        }

        m_EraserBufferMaterialsPool.Clear();

        for (int poolIndex = 0; poolIndex < m_OutlinedBufferMaterialsPool.Count; ++poolIndex)
        {
            Material m = m_OutlinedBufferMaterialsPool[poolIndex];
            if (m != null)
            {
                Destroy(m);
            }
        }

        m_OutlinedBufferMaterialsPool.Clear();

        Destroy(m_OutlinedShaderMaterial);
        m_OutlinedShaderMaterial = null;

        m_OutlinedBufferShader = null;
        m_OutlinedShader = null;
    }

    private void SetBufferShaderMaterialsProperty(Material i_BufferShaderMaterial)
    {
        if (i_BufferShaderMaterial != null)
        {
            i_BufferShaderMaterial.SetFloat("_AlphaCutoff", alphaCutoff);
        }
    }

    private void SetOutlinedShaderMaterialsProperty()
    {
        if (m_OutlinedShaderMaterial != null)
        {
            m_OutlinedShaderMaterial.SetFloat("_LineThicknessX", lineThickness / 1000);
            m_OutlinedShaderMaterial.SetFloat("_LineThicknessY", (lineThickness * 2) / 1000);
            m_OutlinedShaderMaterial.SetColor("_LineColor", lineColor);
        }
    }

    private Material CreateMaterial(Color i_EmissionColor)
    {
        Material m = new Material(m_OutlinedBufferShader);

        m.SetColor("_Color", i_EmissionColor);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;

        return m;
    }

    private void CreateOutlineCamera()
    {
        GameObject outlineCameraGo = new GameObject("OutlineCamera");
        outlineCameraGo.hideFlags = HideFlags.DontSave;
        outlineCameraGo.SetParent(m_Camera.gameObject, true);
        m_OutlineCamera = outlineCameraGo.AddComponent<Camera>();

        m_OutlineCamera.CopyFrom(m_Camera);
        m_OutlineCamera.renderingPath = RenderingPath.Forward;
        m_OutlineCamera.enabled = false;
        m_OutlineCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        m_OutlineCamera.clearFlags = CameraClearFlags.SolidColor;
        m_OutlineCamera.cullingMask = LayerMask.GetMask("Outline");
    }
}
