using UnityEngine;
using System.Collections;

public static class Physics2DUtils
{
    public static Bounds BoundsOf(Collider2D collider)
    {
        BoxCollider2D bc = collider as BoxCollider2D;
        if (bc != null)
        {
            Bounds bounds = new Bounds();

            Vector2 ext = bc.size * 0.5f;
            bounds.Encapsulate(new Vector3(-ext.x, -ext.y, 0f));
            bounds.Encapsulate(new Vector3(ext.x, ext.y, 0f));
            return bounds;
        }

        CircleCollider2D cc = collider as CircleCollider2D;
        if (cc != null)
        {
            Bounds bounds = new Bounds();

            float r = cc.radius;
            bounds.Encapsulate(new Vector3(-r, -r, 0f));
            bounds.Encapsulate(new Vector3(r, r, 0f));
            return bounds;
        }

        return new Bounds(Vector3.zero, Vector3.zero);
    }
}