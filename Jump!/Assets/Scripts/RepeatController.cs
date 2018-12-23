using UnityEngine;

public class RepeatController : MonoBehaviour
{

    private BoxCollider2D bc2d;
    private float horizontalSize;

    // Use this for initialization
    void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();
        horizontalSize = bc2d.size.x * transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.x < -horizontalSize)
        {
            SetPosition();
        }
    }

    //Sets the position of the object
    private void SetPosition()
    {
        Vector2 groundOffset = new Vector2(Mathf.Ceil(horizontalSize * 2f), 0);
        transform.position = (Vector2)transform.position + groundOffset;
    }
}
