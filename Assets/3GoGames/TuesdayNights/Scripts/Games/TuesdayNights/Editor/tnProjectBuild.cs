using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

using System.IO;
using System.Collections;

using TuesdayNights;

public static class tnProjectBuild
{
    private static string s_ScenesFolder = "Assets/3GoGames/TuesdayNights/Scenes/Stadiums/";

    public static string s_FieldTag = "Field";

    public static string s_Crowd = "Crowd_SFX";

    private static string s_SpawnGoal0 = "Spawn_Goal_0";
    private static string s_SpawnGoal1 = "Spawn_Goal_1";
    private static string s_SpawnBall = "Spawn_Ball";
    private static string s_SpawnScorePanel = "Spawn_Score";

    private static string s_NullTopLeft = "NULL_CornerTopLeft";
    private static string s_NullBottomRight = "NULL_CornerBottomRight";
    private static string s_NullMidfield = "NULL_Midfield";

    private static string s_NullGoalTop = "NULL_GoalTop";
    private static string s_NullGoalBottom = "NULL_GoalBottom";

    private static string s_NullAreaTop = "NULL_AreaTop";
    private static string s_NullAreaBottom = "NULL_AreaBottom";

    private static string s_Lights = "Lights";
    private static string s_Panels = "Panels";

    private static string s_GoalEffectorSX = "GoalEffectorSX";
    private static string s_GoalEffectorDX = "GoalEffectorDX";

    private static string s_TopEffector = "TopEffector";
    private static string s_BottomEffector = "BottomEffector";

    private static float s_FieldZ = 15f;
    private static float s_PanelsZ = 9.5f;

    // private static float s_ScorePanelZ = 7.5f;

    private static float s_BallZ = 5f;
    private static float s_CollidersZ = 0f;
    private static float s_EffectorsZ = 0f;
    private static float s_CrowdZ = 0f;
    private static float s_NullZ = 0f;
    private static float s_GoalZ = -7.5f;
    private static float s_LightsZ = -9.5f;

    private static int s_MaxSupporters = 1000;

    private static int s_LayerDefault = LayerMask.NameToLayer("Default");
    private static int s_LayerField = LayerMask.NameToLayer("Field");
    private static int s_LayerSlowmo = LayerMask.NameToLayer("Slowmo");
    private static int s_LayerPlayerWalls = LayerMask.NameToLayer("PlayerWalls");

    private static int s_BallLayerMask = (1 << LayerMask.NameToLayer("Ball"));
    private static int s_CharactersLayerMask = (1 << LayerMask.NameToLayer("Characters"));

    private static float s_TopEffectorAngle = -90f;
    private static float s_BottomEffectorAngle = 90f;

    private static float s_GoalEffectorSxAngle = 0f;
    private static float s_GoalEffectorDxAngle = 180f;

    [MenuItem("TuesdayNights/Process Scenes")]
    public static bool CheckSceneConsistency()
    {
        Scene prevActiveScene = EditorSceneManager.GetActiveScene();

        string prevActiveScenePath = prevActiveScene.path;

        EditorUtility.DisplayProgressBar("Processing", "Processing Scenes...", 0f);

        bool retValue = false;

        DirectoryInfo dir = new DirectoryInfo(s_ScenesFolder);
        FileInfo[] info = dir.GetFiles("*.unity");

        for (int fileIndex = 0; fileIndex < info.Length; ++fileIndex)
        {
            FileInfo fileInfo = info[fileIndex];
            EditorUtility.DisplayProgressBar("Processing", "Processing Scene [" + fileInfo.Name + "]...", (float)fileIndex / info.Length);
            EditorSceneManager.OpenScene(fileInfo.FullName, OpenSceneMode.Single);

            Debug.Log("[" + fileInfo.Name + "] Checking...");

            bool errorOccurred = false;

            // (*) An object named Field, with tag Field

            {
                GameObject fieldGo = null;
                errorOccurred |= CheckForTag(s_FieldTag, out fieldGo);
                errorOccurred |= CheckForZ(fieldGo, s_FieldZ);
                errorOccurred |= CheckForLayer(fieldGo, s_LayerDefault);
                errorOccurred |= CheckForStatic(fieldGo);
            }

            // (*) An object named Spawn_Goal_0
            // (*) An object named Spawn_Goal_1

            {
                GameObject spawnGoal0 = null;
                GameObject spawnGoal1 = null;

                errorOccurred |= CheckForObject(s_SpawnGoal0, out spawnGoal0);
                errorOccurred |= CheckForObject(s_SpawnGoal1, out spawnGoal1);

                errorOccurred |= CheckForZ(spawnGoal0, s_GoalZ);
                errorOccurred |= CheckForZ(spawnGoal1, s_GoalZ);

                errorOccurred |= CheckForLayer(spawnGoal0, s_LayerDefault);
                errorOccurred |= CheckForLayer(spawnGoal1, s_LayerDefault);
            }

            // (*) An object named Spawn_Ball

            {
                GameObject spawnBall = null;
                errorOccurred |= CheckForObject(s_SpawnBall, out spawnBall);
                errorOccurred |= CheckForZ(spawnBall, s_BallZ);
                errorOccurred |= CheckForLayer(spawnBall, s_LayerDefault);
            }

            // (*) An object named Spawn_Score

            {
                GameObject spawnScore = null;
                errorOccurred |= CheckForObject(s_SpawnScorePanel, out spawnScore);

                //errorOccurred |= CheckForZ(spawnScore, s_ScorePanelZ);

                errorOccurred |= CheckForLayer(spawnScore, s_LayerDefault);
            }

            // (*) An NULL named NULL_CornerTopLeft
            // (*) An NULL named NULL_CornerBottomRight
            // (*) An NULL named NULL_Midfield

            {
                GameObject topLeft = null;
                GameObject bottomRight = null;
                GameObject midfield = null;

                errorOccurred |= CheckForObject(s_NullTopLeft, out topLeft);
                errorOccurred |= CheckForObject(s_NullBottomRight, out bottomRight);
                errorOccurred |= CheckForObject(s_NullMidfield, out midfield);

                errorOccurred |= CheckForZ(topLeft, s_NullZ);
                errorOccurred |= CheckForZ(bottomRight, s_NullZ);
                errorOccurred |= CheckForZ(midfield, s_NullZ);

                errorOccurred |= CheckAnchorPositions(topLeft, bottomRight);

                /*errorOccurred |= CheckMidfieldPosition(midfield);*/

                errorOccurred |= CheckForLayer(topLeft, s_LayerDefault);
                errorOccurred |= CheckForLayer(bottomRight, s_LayerDefault);
                errorOccurred |= CheckForLayer(midfield, s_LayerDefault);
            }

            // (*) An NULL named NULL_GoalTop
            // (*) An NULL named NULL_GoalBottom

            {
                GameObject goalTop = null;
                GameObject goalBottom = null;

                errorOccurred |= CheckForObject(s_NullGoalTop, out goalTop);
                errorOccurred |= CheckForObject(s_NullGoalBottom, out goalBottom);

                errorOccurred |= CheckForZ(goalTop, s_NullZ);
                errorOccurred |= CheckForZ(goalBottom, s_NullZ);

                errorOccurred |= CheckRectAnchors(goalTop, goalBottom);

                errorOccurred |= CheckForLayer(goalTop, s_LayerDefault);
                errorOccurred |= CheckForLayer(goalBottom, s_LayerDefault);
            }

            // (*) An NULL named NULL_AreaTop
            // (*) An NULL named NULL_AreaBottom

            {
                GameObject areaTop = null;
                GameObject areaBottom = null;

                errorOccurred |= CheckForObject(s_NullAreaTop, out areaTop);
                errorOccurred |= CheckForObject(s_NullAreaBottom, out areaBottom);

                errorOccurred |= CheckForZ(areaTop, s_NullZ);
                errorOccurred |= CheckForZ(areaBottom, s_NullZ);

                errorOccurred |= CheckRectAnchors(areaTop, areaBottom);

                errorOccurred |= CheckForLayer(areaTop, s_LayerDefault);
                errorOccurred |= CheckForLayer(areaBottom, s_LayerDefault);
            }

            // (*) Lights

            {
                GameObject lights = null;
                /*errorOccurred |=*/
                CheckForObject(s_Lights, out lights);
                /*errorOccurred |=*/
                CheckForZ(lights, s_LightsZ);
                /*errorOccurred |=*/
                CheckForLayer(lights, s_LayerDefault);
                /*errorOccurred |=*/
                CheckForStatic(lights);
            }

            // (*) Panels

            {
                GameObject panels = null;
                /*errorOccurred |=*/
                CheckForObject(s_Panels, out panels);
                /*errorOccurred |=*/
                CheckForZ(panels, s_PanelsZ);
                /*errorOccurred |=*/
                CheckForLayer(panels, s_LayerDefault);
            }

            // (*) Colliders

            {
                /*errorOccurred |=*/
                CheckColliders();
            }

            // (*) Supporters

            {
                errorOccurred |= CheckSupporters();
            }

            // (*) Goal effectors

            {
                GameObject goalEffectorSx = null;
                GameObject goalEffectorDx = null;

                errorOccurred |= CheckForObject(s_GoalEffectorSX, out goalEffectorSx);
                errorOccurred |= CheckForObject(s_GoalEffectorDX, out goalEffectorDx);

                errorOccurred |= CheckForZ(goalEffectorSx, s_EffectorsZ);
                errorOccurred |= CheckForZ(goalEffectorDx, s_EffectorsZ);

                errorOccurred |= CheckEffector(goalEffectorSx);
                errorOccurred |= CheckEffector(goalEffectorDx);

                errorOccurred |= CheckEffectorColliderMask(goalEffectorSx, s_CharactersLayerMask);
                errorOccurred |= CheckEffectorColliderMask(goalEffectorDx, s_CharactersLayerMask);

                errorOccurred |= CheckEffectorForce(goalEffectorSx);
                errorOccurred |= CheckEffectorForce(goalEffectorDx);

                errorOccurred |= CheckEffectorAngle(goalEffectorSx, s_GoalEffectorSxAngle);
                errorOccurred |= CheckEffectorAngle(goalEffectorDx, s_GoalEffectorDxAngle);

                errorOccurred |= CheckForLayer(goalEffectorSx, s_LayerField);
                errorOccurred |= CheckForLayer(goalEffectorDx, s_LayerField);
            }

            // (*) Arena effectors

            {
                GameObject topEffector = null;
                GameObject bottomEffector = null;

                /*errorOccurred |=*/
                CheckForObject(s_TopEffector, out topEffector);
                /*errorOccurred |=*/
                CheckForObject(s_BottomEffector, out bottomEffector);

                /*errorOccurred |=*/
                CheckForZ(topEffector, s_EffectorsZ);
                /*errorOccurred |=*/
                CheckForZ(bottomEffector, s_EffectorsZ);

                /*errorOccurred |=*/
                CheckEffector(topEffector);
                /*errorOccurred |=*/
                CheckEffector(bottomEffector);

                /*errorOccurred |=*/
                CheckEffectorColliderMask(topEffector, s_BallLayerMask);
                /*errorOccurred |=*/
                CheckEffectorColliderMask(bottomEffector, s_BallLayerMask);

                /*errorOccurred |=*/
                CheckEffectorForce(topEffector);
                /*errorOccurred |=*/
                CheckEffectorForce(bottomEffector);

                /*errorOccurred |=*/
                CheckEffectorAngle(topEffector, s_TopEffectorAngle);
                /*errorOccurred |=*/
                CheckEffectorAngle(bottomEffector, s_BottomEffectorAngle);

                /*errorOccurred |=*/
                CheckForLayer(topEffector, s_LayerField);
                /*errorOccurred |=*/
                CheckForLayer(bottomEffector, s_LayerField);
            }

            // (*) Crowd

            {
                GameObject crowd = null;
                errorOccurred |= CheckForObject(s_Crowd, out crowd);
                errorOccurred |= CheckForZ(crowd, s_CrowdZ);
                errorOccurred |= CheckForLayer(crowd, s_LayerDefault);

                errorOccurred |= CheckAudioSource(crowd);
            }

            // (*) Holes

            {

            }

            // (*) Particle Systems

            {
                ParticleSystem[] particleSystems = GameObject.FindObjectsOfType<ParticleSystem>();

                foreach (ParticleSystem p in particleSystems)
                {
                    errorOccurred |= CheckParticleSystem(p);
                }
            }

            // (*) Objects scale

            {
                Transform[] transforms = GameObject.FindObjectsOfType<Transform>();

                foreach (Transform t in transforms)
                {
                    errorOccurred |= CheckObjectScale(t);
                }
            }

            // (*) Objects activation state

            {
                GameObject[] sceneObjects = GameObject.FindObjectsOfType<GameObject>();

                foreach (GameObject o in sceneObjects)
                {
                    errorOccurred |= CheckForActive(o);
                }
            }

            if (errorOccurred)
            {
                Debug.LogError("[" + fileInfo.Name + "] Scene contains errors.");
            }
            else
            {
                Debug.Log("[" + fileInfo.Name + "] Scene OK.");
            }

            retValue |= errorOccurred;
        }

        EditorUtility.ClearProgressBar();

        if (prevActiveScenePath != "")
        {
            EditorSceneManager.OpenScene(prevActiveScenePath, OpenSceneMode.Single);
        }
        else
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }

        return retValue;
    }

    private static bool CheckForObject(string i_Name, out GameObject o_Go)
    {
        GameObject targetGo = GameObject.Find(i_Name);

        if (targetGo == null)
        {
            LogWarning("Missing " + i_Name + ".");
        }

        o_Go = targetGo;

        return (o_Go == null);
    }

    private static bool CheckForTag(string i_Tag, out GameObject o_Go)
    {
        GameObject targetGo = GameObject.FindGameObjectWithTag(i_Tag);

        if (targetGo == null)
        {
            LogWarning("Missing object with tag " + i_Tag + ".");
        }

        o_Go = targetGo;

        return (o_Go == null);
    }

    private static bool CheckForActive(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        if (!i_Go.activeSelf)
        {
            LogWarning(i_Go.name + " is inactive.");
            return true;
        }

        return false;
    }

    private static bool CheckObjectScale(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return true;
        }

        if (!i_Transform.HasUniformScale())
        {
            LogWarning(i_Transform.name + " has a non-uniform scale [" + i_Transform.localScale.x + ", " + i_Transform.localScale.y + ", " + i_Transform.localScale.z + "]");
            return true;
        }

        return false;
    }

    private static bool CheckAnchorPositions(GameObject i_TopLeft, GameObject i_BottomRight)
    {
        if (i_TopLeft == null || i_BottomRight == null)
        {
            return true; // ERROR.
        }

        if (i_TopLeft.transform.position.x > i_BottomRight.transform.position.x)
        {
            LogWarning("Anchors have wrong h-position.");
            return true; // ERROR.
        }

        if (i_TopLeft.transform.position.y < i_BottomRight.transform.position.y)
        {
            LogWarning("Anchors have wrong v-position.");
            return true; // ERROR.
        }

        return false; // OK.
    }

    private static bool CheckTopLeftPosition(GameObject i_TopLeft)
    {
        if (i_TopLeft == null)
        {
            return true;
        }

        bool errorOccurred = false;

        if (i_TopLeft.transform.position.x > 0f)
        {
            LogWarning("TopLeft has invalid X-position.");
            errorOccurred |= true;
        }

        if (i_TopLeft.transform.position.y < 0f)
        {
            LogWarning("TopLeft has invalid Y-position.");
            errorOccurred |= true;
        }

        return errorOccurred;
    }

    private static bool CheckBottomRightPosition(GameObject i_BottomRight)
    {
        if (i_BottomRight == null)
        {
            return true;
        }

        bool errorOccurred = false;

        if (i_BottomRight.transform.position.x < 0f)
        {
            LogWarning("BottomRight has invalid X-position.");
            errorOccurred |= true;
        }

        if (i_BottomRight.transform.position.y > 0f)
        {
            LogWarning("BottomRight has invalid Y-position.");
            errorOccurred |= true;
        }

        return errorOccurred;
    }

    private static bool CheckMidfieldPosition(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        bool checkX = !Mathf.Approximately(i_Go.transform.position.x, 0f);
        bool checkY = !Mathf.Approximately(i_Go.transform.position.y, 0f);
        bool checkZ = !Mathf.Approximately(i_Go.transform.position.z, 0f);

        if (checkX || checkY || checkZ)
        {
            LogWarning("Midfield is not in origin.");
        }

        return (checkX || checkY || checkZ);
    }

    private static bool CheckRectAnchors(GameObject i_TopLeft, GameObject i_BottomRight)
    {
        if (i_TopLeft == null || i_BottomRight == null)
        {
            return true;
        }

        bool errorOccurred = false;

        if (i_TopLeft.transform.position.x > i_BottomRight.transform.position.x)
        {
            LogWarning(i_TopLeft.name + "/" + i_BottomRight.name + " have invalid horizontal position.");
            errorOccurred |= true;
        }

        if (i_TopLeft.transform.position.y < i_BottomRight.transform.position.y)
        {
            LogWarning(i_TopLeft.name + "/" + i_BottomRight.name + " have invalid vertical position.");
            errorOccurred |= true;
        }

        return errorOccurred;
    }

    private static bool CheckHorizontalPosition(GameObject i_Go, float i_Pos)
    {
        if (i_Go == null)
        {
            return true;
        }

        if (i_Go.transform.position.x * i_Pos < 0f)
        {
            LogWarning(i_Go.name + " has wrong horizontal position.");
            return true;
        }

        return false;
    }

    private static bool CheckForZ(GameObject i_Go, float i_Z)
    {
        if (i_Go == null)
        {
            return true;
        }

        bool errorOccurred = !Mathf.Approximately(i_Go.transform.position.z, i_Z);

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has wrong Z-position, it should be " + i_Z + ".");
        }

        return errorOccurred;
    }

    private static bool CheckForLayer(GameObject i_Go, int i_Layer)
    {
        if (i_Go == null)
        {
            return true;
        }

        bool errorOccurred = (i_Go.layer != i_Layer);

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has invalid layer, it should be " + LayerMask.LayerToName(i_Layer) + ".");
        }

        return errorOccurred;
    }

    private static bool CheckForStatic(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        bool errorOccurred = !i_Go.isStatic;

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has should be static.");
        }

        return errorOccurred;
    }

    private static bool CheckEffector(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        Effector2D effector2D = i_Go.GetComponent<Effector2D>();

        if (effector2D == null)
        {
            LogWarning(i_Go.name + " hasn't a valid effector.");
            return true;
        }

        Collider2D collider2D = i_Go.GetComponent<Collider2D>();

        if (collider2D == null)
        {
            LogWarning(i_Go.name + " hasn't a valid collider.");
            return true;
        }

        if (collider2D.usedByEffector && !collider2D.isTrigger)
        {
            LogWarning(i_Go.name + " is not a valid trigger.");
            return true;
        }

        if (collider2D.sharedMaterial != null)
        {
            LogWarning(i_Go.name + " has a PhysicsMaterial2D assigned. Remove it.");
            return true;
        }

        return false;
    }

    private static bool CheckEffectorColliderMask(GameObject i_Go, int i_LayerMask)
    {
        if (i_Go == null)
        {
            return true;
        }

        Effector2D effector2D = i_Go.GetComponent<Effector2D>();

        if (effector2D == null)
        {
            LogWarning(i_Go.name + " hasn't a valid effector.");
            return true;
        }

        bool errorOccurred = (!effector2D.useColliderMask || ((effector2D.colliderMask & i_LayerMask) == 0));

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has an effector with an invalid ColliderMask.");
        }

        return errorOccurred;
    }

    private static bool CheckEffectorForce(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        Effector2D effector2D = i_Go.GetComponent<Effector2D>();

        if (effector2D == null)
        {
            LogWarning(i_Go.name + " hasn't a valid effector.");
            return true;
        }

        bool errorOccurred = false;

        if (effector2D is AreaEffector2D)
        {
            AreaEffector2D areaEffector2D = (AreaEffector2D)effector2D;
            errorOccurred = (Mathf.Abs(areaEffector2D.forceMagnitude) < 0.1f);
        }

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has an invalid force magnitude.");
        }

        return errorOccurred;
    }

    private static bool CheckEffectorAngle(GameObject i_Go, float i_Angle)
    {
        if (i_Go == null)
        {
            return true;
        }

        Effector2D effector2D = i_Go.GetComponent<Effector2D>();

        if (effector2D == null)
        {
            LogWarning(i_Go.name + " hasn't a valid effector.");
            return true;
        }

        bool errorOccurred = false;

        if (effector2D is AreaEffector2D)
        {
            AreaEffector2D areaEffector2D = (AreaEffector2D)effector2D;
            errorOccurred = !Mathf.Approximately(areaEffector2D.forceAngle, i_Angle);
        }

        if (errorOccurred)
        {
            LogWarning(i_Go.name + " has an invalid force angle.");
        }

        return errorOccurred;
    }

    private static bool CheckAudioSource(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return true;
        }

        AudioSource audioSource = i_Go.GetComponent<AudioSource>();

        if (audioSource == null || !audioSource.isActiveAndEnabled)
        {
            LogWarning(i_Go.name + " hasn't a valid AudioSource.");
            return true;
        }

        if (audioSource.clip == null)
        {
            LogWarning(i_Go.name + " hasn't a valid AudioClip.");
            return true;
        }

        if (audioSource.outputAudioMixerGroup == null)
        {
            LogWarning(i_Go.name + " hasn't a valid AudioMixerGroup.");
            return true;
        }

        if (!audioSource.playOnAwake)
        {
            LogWarning(i_Go.name + " hasn't playOnAwake.");
            return true;
        }

        if (!audioSource.loop)
        {
            LogWarning(i_Go.name + " is not looping.");
            return true;
        }

        return false; // It's all ok.
    }

    private static bool CheckSupporters()
    {
        bool errorOccurred = false;

        int supportersCount = 0;

        tnSupporterArea[] supportersArea = GameObject.FindObjectsOfType<tnSupporterArea>();

        for (int areaIndex = 0; areaIndex < supportersArea.Length; ++areaIndex)
        {
            tnSupporterArea area = supportersArea[areaIndex];
            supportersCount += area.maxSupporters;

            Collider2D collider2D = area.GetComponent<Collider2D>();

            if (collider2D == null)
            {
                errorOccurred |= true;

                LogWarning(area.gameObject.name + " missing collider.");
            }
            else
            {
                if (!collider2D.isTrigger)
                {
                    errorOccurred |= true;

                    LogWarning(area.gameObject.name + "'s collider is not a trigger.");
                }
            }

            errorOccurred |= CheckForLayer(area.gameObject, s_LayerDefault);
        }

        Debug.Log("Supporters:" + supportersCount + "/" + supportersArea.Length);

        if (supportersCount > s_MaxSupporters)
        {
            errorOccurred |= true;

            LogWarning("Too many supporters.");
        }

        return errorOccurred;
    }

    private static bool CheckColliders()
    {
        bool errorOccurred = false;

        Collider2D[] colliders = GameObject.FindObjectsOfType<Collider2D>();

        foreach (Collider2D c in colliders)
        {
            if (c.transform.position.z != s_CollidersZ)
            {
                errorOccurred |= true;

                LogWarning(c.name + " has invalid Z. It should be 0");
            }

            if (c.gameObject.layer == s_LayerSlowmo)
            {
                if (!c.isTrigger)
                {
                    errorOccurred |= true;

                    LogWarning(c.name + " is a " + LayerMask.LayerToName(s_LayerSlowmo) + " object. It must be set as trigger.");
                }
            }
            else
            {
                if (c.gameObject.layer == s_LayerPlayerWalls)
                {
                    if (c.isTrigger && !c.usedByEffector)
                    {
                        errorOccurred |= true;

                        LogWarning(c.name + " is a " + LayerMask.LayerToName(s_LayerPlayerWalls) + " object. It must be set as non-trigger.");
                    }
                }
                else
                {
                    // It must be Field.

                    if (c.gameObject.layer != s_LayerField && (!c.isTrigger || c.usedByEffector))
                    {
                        errorOccurred |= true;

                        LogWarning(c.name + " has invalid Layer. It should be '" + LayerMask.LayerToName(s_LayerField) + "'");
                    }
                }
            }

            if (c.isTrigger)
            {
                if (c.sharedMaterial != null) // A trigger must have a null material.
                {
                    errorOccurred |= true;

                    LogWarning(c.name + " must have a null PhysicsMaterial2D.");
                }
            }
            else
            {
                if (c.sharedMaterial == null) // A non-trigger must have a valid material.
                {
                    errorOccurred |= true;

                    LogWarning(c.name + " has invalid PhysicsMaterial2D.");
                }
            }
        }

        return errorOccurred;
    }

    private static bool CheckParticleSystem(ParticleSystem i_ParticleSystem)
    {
        if (i_ParticleSystem == null)
        {
            return true;
        }

        bool errorOccurred = false;

        ParticleSystemRenderer particleSystemRenderer = i_ParticleSystem.GetComponent<ParticleSystemRenderer>();

        if (particleSystemRenderer.renderMode != ParticleSystemRenderMode.Billboard)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid RenderMode. It should be 'Billboard'.");
        }

        if (particleSystemRenderer.normalDirection != 1f)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid normal direction. It should be 1.");
        }

        if (particleSystemRenderer.sharedMaterial == null)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid material.");
        }

        if (particleSystemRenderer.sortMode != ParticleSystemSortMode.None)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid SortMode. It should be 'None'.");
        }

        if (particleSystemRenderer.sortingFudge != 0f)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid sorting fudge. It should be 0.");
        }

        if (particleSystemRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid ShadowCastingMode. It should be 'Off'.");
        }

        if (particleSystemRenderer.sortingLayerName != "Default")
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid sorting layer name. It should be 'Default'.");
        }

        if (particleSystemRenderer.sortingOrder != 0)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid sorting order. It should be 0.");
        }

        if (particleSystemRenderer.alignment != ParticleSystemRenderSpace.View)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": Invalid alignment. It should be 'View'.");
        }

        if (particleSystemRenderer.reflectionProbeUsage != ReflectionProbeUsage.Off)
        {
            errorOccurred |= true;
            LogWarning("ParticleSystem " + i_ParticleSystem.name + ": light probes enabled. They should be disabled.");
        }

        return errorOccurred;
    }

    private static void LogWarning(string i_Text)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        Debug.LogWarning("[" + activeScene.name + "]" + " " + i_Text);
    }

    public static void OnPreBuild()
    {
#if PHOTON

        // Take PhotonServerSettings and apply the correct AppId.

        string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

        ServerSettings photonServerSettings = AssetDatabase.LoadAssetAtPath<ServerSettings>(serverSettingsAssetPath);

#if BUILD_DEBUG

        photonServerSettings.AppID = BuildInfo.s_PhotonAppId_Debug;

#elif BUILD_TEST

        photonServerSettings.AppID = BuildInfo.s_PhotonAppId_Test;

#elif BUILD_RELEASE

        photonServerSettings.AppID = BuildInfo.s_PhotonAppId_Release;

#else 

        photonServerSettings.AppID = BuildInfo.s_PhotonAppId_Debug;

#endif

#endif // PHOTON

        AssetDatabase.SaveAssets();
    }

    public static void OnPostBuild()
    {

    }

    public static void OnPreExport()
    {
        OnPreBuild();
    }

    public static void OnPostExport()
    {
        OnPostBuild();
    }

    [MenuItem("TuesdayNights/Build/Windows/x86")]
    public static void Build32()
    {
        Build(BuildTarget.StandaloneWindows, BuildInfo.s_ExecutableName_Win32);
    }

    [MenuItem("TuesdayNights/Build/Windows/x64")]
    public static void Build64()
    {
        Build(BuildTarget.StandaloneWindows64, BuildInfo.s_ExecutableName_Win64);
    }

	[MenuItem("TuesdayNights/Build/Mac OSx/Universal")]
	public static void BuildMacUniversal()
	{
		Build (BuildTarget.StandaloneOSX, BuildInfo.s_BuildName_Mac_Universal);
	}

    private static void Build(BuildTarget i_Target, string i_ExecutableName)
    {
		if (i_Target != BuildTarget.StandaloneWindows && i_Target != BuildTarget.StandaloneWindows64 && i_Target != BuildTarget.StandaloneOSX)
        {
            Debug.LogError("Invalid Platform selected.");
            return;
        }

        //if (CheckSceneConsistency())
        //{
        //    Debug.LogError("Some scenes contain errors. Build aborted.");
        //    return;
        //}

        string path = EditorUtility.SaveFolderPanel("Select output folder", "", "");

        if (path.Length == 0)
            return;

        // Pre-Build.

        OnPreBuild();

        // Build process.

        EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;

        int scenesCount = buildSettingsScenes.Length;

        if (scenesCount == 0)
        {
            Debug.LogError("Invalid scenes list.");
            return;
        }

        string[] scenes = new string[scenesCount];

        for (int sceneIndex = 0; sceneIndex < scenes.Length; ++sceneIndex)
        {
            EditorBuildSettingsScene buildSettingsScene = buildSettingsScenes[sceneIndex];
            string scenePath = buildSettingsScene.path;

            scenes[sceneIndex] = scenePath;
        }

        BuildPipeline.BuildPlayer(scenes, path + "/" + i_ExecutableName + ".exe", i_Target, BuildOptions.None);

        // Post-Build.

        OnPostBuild();
    }
}