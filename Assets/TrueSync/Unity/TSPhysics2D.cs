using UnityEngine;

using System;

namespace TrueSync
{
    /**
    *  @brief Helpers for 2D physics.
    **/
    public class TSPhysics2D
    {
        public enum TSCapsuleDirection2D
        {
            VERTICAL,
            HORIZONTAL
        }

        private static object OverlapGeneric(Physics2D.Shape i_Shape, TSVector2 i_Position, Physics2D.BodySpecialSensor i_SensorType, int i_Mask)
        {
            Physics2D.World world = (Physics2D.World)PhysicsManager.GetWorld();

            Physics2D.Body body = Physics2D.BodyFactory.CreateBody(world);
            body.CreateFixture(i_Shape);

            body.BodyType = Physics2D.BodyType.Static;

            body.CollisionCategories = Physics2D.Category.All; // Category.All is used for sweep test objects.
            body.CollidesWith = (Physics2D.Category)i_Mask;

            body.CollisionGroup = 0;

            body.IsSensor = true;
            body.SpecialSensor = i_SensorType;
            body.SpecialSensorMask = (int)Physics2D.Category.All;

            body.Position = i_Position;

            world.RemoveBody(body);
            world.ProcessRemovedBodies();

            if (body._specialSensorResults.Count > 0)
            {
                if (i_SensorType == Physics2D.BodySpecialSensor.ActiveOnce)
                {
                    return PhysicsManager.GetGameObject(body._specialSensorResults[0]).GetComponent<TSCollider2D>();
                }
                else
                {
                    TSCollider2D[] result = new TSCollider2D[body._specialSensorResults.Count];
                    for (int i = 0; i < body._specialSensorResults.Count; i++)
                    {
                        result[i] = PhysicsManager.GetGameObject(body._specialSensorResults[i]).GetComponent<TSCollider2D>();
                    }

                    return result;
                }
            }

            return null;
        }

        private static object _OverlapCircle(TSVector2 i_Point, FP i_Radius, Physics2D.BodySpecialSensor i_SensorType, int i_Mask)
        {
            return OverlapGeneric(new Physics2D.CircleShape(i_Radius, 1), i_Point, i_SensorType, i_Mask);
        }

        public static TSCollider2D OverlapCircle(TSVector2 i_Point, FP i_Radius, int i_Mask)
        {
            return (TSCollider2D)_OverlapCircle(i_Point, i_Radius, Physics2D.BodySpecialSensor.ActiveOnce, i_Mask);
        }

        public static TSCollider2D[] OverlapCircleAll(TSVector2 i_Point, FP i_Radius, int i_Mask)
        {
            return (TSCollider2D[])_OverlapCircle(i_Point, i_Radius, Physics2D.BodySpecialSensor.ActiveAll, i_Mask);
        }

        public static object _OverlapArea(TSVector2 i_PointA, TSVector2 i_PointB, Physics2D.BodySpecialSensor i_SensorType, int i_Mask)
        {
            TSVector2 center;
            center.x = (i_PointA.x + i_PointB.x) * FP.Half;
            center.y = (i_PointA.y + i_PointB.y) * FP.Half;

            Physics2D.Vertices vertices = new Physics2D.Vertices(4);
            vertices.Add(new TSVector2(i_PointA.x, i_PointA.y) - center);
            vertices.Add(new TSVector2(i_PointB.x, i_PointA.y) - center);
            vertices.Add(new TSVector2(i_PointB.x, i_PointB.y) - center);
            vertices.Add(new TSVector2(i_PointA.x, i_PointB.y) - center);

            return OverlapGeneric(new Physics2D.PolygonShape(vertices, 1), center, i_SensorType, i_Mask);
        }

        public static TSCollider2D OverlapArea(TSVector2 i_PointA, TSVector2 i_PointB, int i_Mask)
        {
            return (TSCollider2D)_OverlapArea(i_PointA, i_PointB, Physics2D.BodySpecialSensor.ActiveOnce, i_Mask);
        }

        public static TSCollider2D[] OverlapAreaAll(TSVector2 i_PointA, TSVector2 i_PointB, int i_Mask)
        {
            return (TSCollider2D[])_OverlapArea(i_PointA, i_PointB, Physics2D.BodySpecialSensor.ActiveAll, i_Mask);
        }

        private static object _OverlapBox(TSVector2 i_Point, TSVector2 i_Size, FP i_Angle, Physics2D.BodySpecialSensor i_SensorType, int i_Mask)
        {
            i_Size *= FP.Half;
            i_Angle *= FP.Deg2Rad;

            return OverlapGeneric(new Physics2D.PolygonShape(Physics2D.PolygonTools.CreateRectangle(i_Size.x, i_Size.y, i_Point, i_Angle * -1), 1), i_Point, i_SensorType, i_Mask);
        }

        public static TSCollider2D OverlapBox(TSVector2 i_Point, TSVector2 i_Size, FP i_Angle, int i_Mask)
        {
            return (TSCollider2D)_OverlapBox(i_Point, i_Size, i_Angle, Physics2D.BodySpecialSensor.ActiveOnce, i_Mask);
        }

        public static TSCollider2D[] OverlapBoxAll(TSVector2 i_Point, TSVector2 i_Size, FP i_Angle, int i_Mask)
        {
            return (TSCollider2D[])_OverlapBox(i_Point, i_Size, i_Angle, Physics2D.BodySpecialSensor.ActiveAll, i_Mask);
        }

        public static TSRaycastHit2D[] Raycast(TSVector2 i_Origin, TSVector2 i_Direction, FP i_Distance, int i_Mask)
        {
            Physics2DWorldManager physicsManager = (Physics2DWorldManager)PhysicsManager.Instance;
            return physicsManager.Raycast(i_Origin, i_Direction, i_Distance, i_Mask);
        }
    }
}