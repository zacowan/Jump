using UnityEngine;

public class ItemController : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.info.RewardPlayer(tag.ToString());
            transform.position = new Vector2(-25f, -25f);
        }
        else if (collision.tag == "Danger")
        {
            transform.position = new Vector2(-25f, -25f);
        }
    }

}
