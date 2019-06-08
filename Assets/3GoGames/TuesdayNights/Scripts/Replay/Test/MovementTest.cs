using UnityEngine;

public class MovementTest : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 2f;
    [SerializeField]
    private bool m_MoveInFixed = false;

    public float speed
    {
        get
        {
            return m_Speed;
        }
    }

    // MonoBehaviour's interface

    void Update()
    {
        if (m_MoveInFixed)
            return;

        Movement(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!m_MoveInFixed)
            return;

        Movement(Time.fixedDeltaTime);
    }

    // INTERNALS

    private void Movement(float i_DeltaTime)
    {
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal += -1f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal += 1f;
        }

        float vertical = 0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vertical += 1f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            vertical += -1f;
        }

        Vector2 direction = new Vector2(horizontal, vertical);
        direction.Normalize();
        Vector2 offset = direction * m_Speed * i_DeltaTime;

        Vector3 pos = transform.position;
        pos += new Vector3(offset.x, offset.y, 0f);
        transform.position = pos;
    }
}
