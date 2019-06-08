using UnityEngine;

namespace TrueSync
{
    /**
     *  @brief Represents information about a contact point
     **/
    public class TSContactPoint2D
    {
        /**
         *  @brief Contact point between two bodies
         **/
        public TSVector2 point;

        /**
         *  @brief Normal vector from the contact point
         **/
        public TSVector2 normal;
    }

    /**
     *  @brief Represents information about a contact between two 2D bodies
     **/
    public class TSCollision2D
    {
        /**
         *  @brief An array of {@link TSContactPoint}
         **/
        public TSContactPoint2D[] contacts = new TSContactPoint2D[1];

        /**
         *  @brief GameObject of the body hit
         **/
        public GameObject gameObject;

        /**
         *  @brief {@link TSCollider} of the body hit
         **/
        public TSCollider2D collider;

        /**
         *  @brief {@link TSRigidBody} of the body hit, if there is one attached
         **/
        public TSRigidBody2D rigidbody;

        /**
         *  @brief {@link TSTransform} of the body hit
         **/
        public TSTransform2D transform;

        /**
         *  @brief The {@link TSTransform} of the body hit
         **/
        public TSVector2 relativeVelocity;

        internal void Update(GameObject i_OtherGO, Physics2D.Contact i_Contact)
        {
            gameObject = i_OtherGO;

            if (i_OtherGO != null)
            {
                collider = i_OtherGO.GetComponent<TSCollider2D>();
                rigidbody = i_OtherGO.GetComponent<TSRigidBody2D>();
                transform = i_OtherGO.GetComponent<TSTransform2D>();
            }
            else
            {
                collider = null;
                rigidbody = null;
                transform = null;
            }

            if (i_Contact != null)
            {
                if (contacts[0] == null)
                {
                    contacts[0] = new TSContactPoint2D();
                }

                TSVector2 normal;
                Physics2D.FixedArray2<TSVector2> points;

                i_Contact.GetWorldManifold(out normal, out points);

                contacts[0].normal = normal;
                contacts[0].point = points[0];

                relativeVelocity = i_Contact.CalculateRelativeVelocity();
            }
            else
            {
                relativeVelocity = TSVector2.zero;

                if (contacts[0] != null)
                {
                    contacts[0].normal = TSVector2.zero;
                    contacts[0].point = TSVector2.zero;
                }
            }
        }

        internal void Clear()
        {
            Update(null, null);
        }
    }
}