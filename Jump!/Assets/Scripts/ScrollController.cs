using UnityEngine;

public class ScrollController : MonoBehaviour
{

    private Rigidbody2D m_rb2d;

    // Use this for initialization
    void Start()
    {
        m_rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Player.info.CheckIfPlayerDead())
        {
            m_rb2d.velocity = Vector2.zero;
            if (gameObject.GetComponent<Animator>())
            {
                gameObject.GetComponent<Animator>().enabled = false;
            }
        }
        else
        {
            m_rb2d.velocity = Vector2.right * -GameController.instance.GetScrollSpeed();
        }
    }
}
