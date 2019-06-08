using UnityEngine;
using UnityEditor;

namespace TrueSync
{
    [CustomEditor(typeof(TrueSyncConfig))]
    public class TrueSyncConfigEditor : Editor
    {
        private const int OFFSET_LEFT = 30;
        private const int OFFSET_TOP = 5;
        private const int OFFSET_TOP_COLLUMNS = 20;
        private const int TOGGLE_SIZE = 10;
        private const int PIVOT_ANGLE = -90;
        private const int HEIGHT_LABEL = 16;
        private const int NUMBER_OF_LAYERS = 32;

        private struct LayerInfo
        {
            public int layerIndex;
            public string layerName;
            public Vector2 labelSize;
        }

        private LayerInfo[] m_LayersInfo = new LayerInfo[NUMBER_OF_LAYERS];
        private int m_ValidLayersCount;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TrueSyncConfig settings = (TrueSyncConfig)target;

            Undo.RecordObject(settings, "Edit TrueSyncConfig");

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            settings.syncWindow = EditorGUILayout.IntField("Sync Window", settings.syncWindow);
            if (settings.syncWindow < 0)
            {
                settings.syncWindow = 0;
            }

            settings.rollbackWindow = EditorGUILayout.IntField("Rollback Window", settings.rollbackWindow);
            if (settings.rollbackWindow < 0)
            {
                settings.rollbackWindow = 0;
            }

            settings.panicWindow = EditorGUILayout.IntField("Panic Window", settings.panicWindow);
            if (settings.panicWindow < 0)
            {
                settings.panicWindow = 0;
            }

            settings.lockedTimeStep = EditorGUILayout.FloatField("Locked Time Step", settings.lockedTimeStep.AsFloat());
            if (settings.lockedTimeStep < 0)
            {
                settings.lockedTimeStep = 0;
            }

            settings.showStats = EditorGUILayout.Toggle("Show Stats", settings.showStats);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);

            Vector2 gField2D = EditorGUILayout.Vector2Field("Gravity", settings.gravity.ToVector());
            settings.gravity.x = gField2D.x;
            settings.gravity.y = gField2D.y;

            settings.speculativeContacts = EditorGUILayout.Toggle("Speculative Contacts", settings.speculativeContacts);

            EditorGUILayout.LabelField("Layer Collision Matrix");

            DrawLayerMatrix(settings.collisionMatrix);

            if (GUILayout.Button("Align from standard matrix"))
            {
                AlignFromStandardMatrix(settings.collisionMatrix);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawLayerMatrix(bool[] i_IgnoreMatrix)
        {
            float maxLabelWidth = 0;

            m_ValidLayersCount = 0;

            for (int i = 0; i < NUMBER_OF_LAYERS; ++i)
            {
                if (LayerMask.LayerToName(i) != "")
                {
                    m_LayersInfo[m_ValidLayersCount].layerIndex = i;
                    m_LayersInfo[m_ValidLayersCount].layerName = LayerMask.LayerToName(i);

                    GUIContent labelGUIContent = new GUIContent(m_LayersInfo[m_ValidLayersCount].layerName);
                    Vector2 lSize = GUI.skin.label.CalcSize(labelGUIContent);
                    m_LayersInfo[m_ValidLayersCount].labelSize = lSize;

                    if (lSize.x > maxLabelWidth)
                    {
                        maxLabelWidth = lSize.x;
                    }

                    m_ValidLayersCount++;
                }
            }

            Rect lastAnchorRect = GUILayoutUtility.GetLastRect();

            DrawLayerMatrixColumns(lastAnchorRect, maxLabelWidth);
            DrawLayerMatrixRows(lastAnchorRect, maxLabelWidth);
            DrawLayerMatrixToggles(i_IgnoreMatrix, lastAnchorRect, maxLabelWidth);

            GUILayout.Space(m_ValidLayersCount * HEIGHT_LABEL + maxLabelWidth + OFFSET_TOP_COLLUMNS);
        }

        private void DrawLayerMatrixColumns(Rect i_LastAnchorRect, float i_MaxLabelWidth)
        {
            Matrix4x4 oldMatrix = GUI.matrix;

            Vector2 rectPosition = new Vector2(i_LastAnchorRect.xMin, i_LastAnchorRect.yMax + i_MaxLabelWidth + OFFSET_TOP_COLLUMNS);
            float offsetX = rectPosition.x + OFFSET_LEFT + i_MaxLabelWidth;
            GUIUtility.RotateAroundPivot(PIVOT_ANGLE, new Vector2(0, rectPosition.y));

            GUI.BeginGroup(new Rect(rectPosition, new Vector2(i_MaxLabelWidth, i_LastAnchorRect.width)));

            float offsetY = 0;

            for (int i = m_ValidLayersCount - 1; i >= 0; --i)
            {
                Vector2 lSize = m_LayersInfo[i].labelSize;
                GUI.Label(new Rect(0, offsetX + offsetY, lSize.x, lSize.y), m_LayersInfo[i].layerName);

                offsetY += lSize.y;
            }

            GUI.EndGroup();

            GUI.matrix = oldMatrix;
        }

        private void DrawLayerMatrixRows(Rect i_LastAnchorRect, float i_MaxLabelWidth)
        {
            Vector2 rectPosition = new Vector2(i_LastAnchorRect.xMin, i_LastAnchorRect.yMax + i_MaxLabelWidth + OFFSET_TOP);

            GUI.BeginGroup(new Rect(rectPosition, new Vector2(i_MaxLabelWidth + OFFSET_LEFT, i_LastAnchorRect.width)));

            float offsetY = 0;

            GUI.skin.label.alignment = TextAnchor.MiddleRight;

            for (int i = 0; i < m_ValidLayersCount; ++i)
            {
                Vector2 lSize = m_LayersInfo[i].labelSize;
                GUI.Label(new Rect(OFFSET_LEFT, offsetY, i_MaxLabelWidth, lSize.y), m_LayersInfo[i].layerName);

                offsetY += lSize.y;
            }

            GUI.EndGroup();
        }

        private void DrawLayerMatrixToggles(bool[] i_IgnoreMatrix, Rect i_LastAnchorRect, float i_MaxLabelWidth)
        {
            Vector2 rectPosition = new Vector2(i_LastAnchorRect.xMin + i_MaxLabelWidth + OFFSET_LEFT, i_LastAnchorRect.yMax + i_MaxLabelWidth + OFFSET_TOP);

            GUI.BeginGroup(new Rect(rectPosition, new Vector2(i_LastAnchorRect.width, i_LastAnchorRect.width - (i_MaxLabelWidth + OFFSET_LEFT))));

            int matrixIndex = -1;

            for (int i = 0; i < m_ValidLayersCount; ++i)
            {
                for (int j = 0, lengthJ = m_ValidLayersCount - i; j < lengthJ; ++j)
                {
                    int layerAIndex = m_LayersInfo[i].layerIndex;
                    int layerBIndex = m_LayersInfo[m_ValidLayersCount - j - 1].layerIndex;

                    matrixIndex = ((NUMBER_OF_LAYERS + NUMBER_OF_LAYERS - layerAIndex + 1) * layerAIndex) / 2 + layerBIndex;
                    i_IgnoreMatrix[matrixIndex] = GUI.Toggle(new Rect(j * HEIGHT_LABEL + 1, i * HEIGHT_LABEL, TOGGLE_SIZE, TOGGLE_SIZE), i_IgnoreMatrix[matrixIndex], "");
                }
            }

            GUI.EndGroup();
        }

        private void AlignFromStandardMatrix(bool[] i_IgnoreMatrix)
        {
            int matrixIndex = -1;

            for (int i = 0; i < m_ValidLayersCount; ++i)
            {
                for (int j = 0, lengthJ = m_ValidLayersCount - i; j < lengthJ; ++j)
                {
                    int layerAIndex = m_LayersInfo[i].layerIndex;
                    int layerBIndex = m_LayersInfo[m_ValidLayersCount - j - 1].layerIndex;

                    matrixIndex = ((NUMBER_OF_LAYERS + NUMBER_OF_LAYERS - layerAIndex + 1) * layerAIndex) / 2 + layerBIndex;
                    i_IgnoreMatrix[matrixIndex] = !UnityEngine.Physics2D.GetIgnoreLayerCollision(layerAIndex, layerBIndex);
                }
            }
        }
    }
}