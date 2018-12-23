using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    //A reference to the player script
    public static Player info;
    //Constants
    private const float JUMP_VELOCITY = 13.5f;
    private const float IMMUNE_TIME = 8f;
    private const int GEM_VALUE = 10;
    //Components
    private Rigidbody2D m_rb2d;
    private BoxCollider2D m_bc2d;
    private CircleCollider2D m_cc2d;
    private Animator m_anim;
    //Bools
    private bool m_isJumping = false;
    private bool m_isDoubleJumping = false;
    private bool m_playerDead = false;
    private bool m_playerImmune = false;
    //Floats
    private float m_posX;
    private float m_posY;
    private float m_immuneCounter = 0f;
    private float deathTimer = 0f;
    private float deathTimerLength = 0.75f;
    private Collider2D m_groundCollider;

    void Awake()
    {
        if (info == null)
        {
            info = this;
        }
        else if (info != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        m_rb2d = GetComponent<Rigidbody2D>();
        m_bc2d = GetComponent<BoxCollider2D>();
        m_cc2d = GetComponent<CircleCollider2D>();
        m_anim = GetComponent<Animator>();
        m_posX = transform.position.x;
        m_posY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        SetPosition(m_posX);
        if (!m_playerDead && !GameController.instance.CheckIfGamePaused())
        {
            //Tells the player object to jump
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.y < (Screen.height - 300f) || touch.position.x < (Screen.width - 300f))
                    {
                        if (!m_isJumping && !m_isDoubleJumping)
                        {
                            m_isJumping = true;
                            Jump();
                        }
                        else if (m_isJumping && !m_isDoubleJumping)
                        {
                            m_isDoubleJumping = true;
                            Jump();
                        }
                    }
                }
            }

        //   if(Input.GetMouseButtonDown(0)) {
		//  		if (!GameController.instance.CheckIfGamePaused())
		//  		{
		//  			if (!m_isJumping && !m_isDoubleJumping)
		//  			{
		//  				m_isJumping = true;
		//  				Jump();
		//  			}
		//  			else if (m_isJumping && !m_isDoubleJumping)
		//  			{
		//  				m_isDoubleJumping = true;
		//  				Jump();
		//  			}
		//  		}
        //      }
            //Player object is immune to 
            if (m_playerImmune && m_immuneCounter <= IMMUNE_TIME)
            {
                GameController.instance.DisplayTimer(IMMUNE_TIME - m_immuneCounter);
                m_immuneCounter += (Time.deltaTime/Time.timeScale);
            }
            else if (m_playerImmune && m_immuneCounter >= IMMUNE_TIME)
            {
                m_playerImmune = false;
                m_immuneCounter = 0f;
                GameController.instance.DisplayTimer(-1f);
            }
        }
        //A timer that starts when the player dies and causes the player to fall after a set amount of time
        else if (m_playerDead && (deathTimer < deathTimerLength))
        {
            deathTimer += Time.deltaTime;
        }
        else if (m_playerDead && (deathTimer >= deathTimerLength))
        {
            m_bc2d.enabled = false;
            m_cc2d.enabled = false;
            m_rb2d.gravityScale = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && m_anim != null)
        {
            m_isJumping = false;
            m_isDoubleJumping = false;
            m_anim.SetBool("isJumping", false);
        }
        if (collision.gameObject.tag == "Danger" && !m_playerImmune && !m_playerDead)
        {
            Analytics.CustomEvent("playerDead", new Dictionary<string, object> 
		    {
                {"collision", collision.gameObject.name},
                {"roundCoins", NewDataPersistence.data.GetValue("Coins", true)},
                {"roundKeys", NewDataPersistence.data.GetValue("Keys", true)},
                {"gameSpeed", GameController.instance.GetTimeScale()},
                {"roundScore", NewDataPersistence.data.GetValue("High Score", true)}
		    });
            m_playerDead = true;
            collision.collider.enabled = false;
            m_groundCollider = collision.collider;
            m_anim.SetTrigger("isHit");
            m_rb2d.gravityScale = 0f;
            m_rb2d.velocity = Vector2.zero;
            transform.position = new Vector2(m_posX, transform.position.y);
            transform.rotation = Quaternion.identity;
            GameController.instance.PlayerDead();
        }
    }

    private void SetPosition(float positionX)
    {
        transform.position = new Vector2(positionX, transform.position.y);
    }

    public bool CheckIfPlayerDead()
    {
        if (m_playerDead)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RewardPlayer(string type)
    {
        AudioOutput.manager.PlayClip("item sound", 1.5f, 2);
        if (type == "Star")
        {
            m_playerImmune = true;
        }
        else if (type == "Gem")
        {
            NewDataPersistence.data.SetValue("Coins", (int)NewDataPersistence.data.GetValue("Coins", true) + GEM_VALUE, true);
        }
        else
        {
            NewDataPersistence.data.SetValue(type + "s", (int)NewDataPersistence.data.GetValue(type + "s", true) + 1, true);
        }
    }

    public float GetDeathTimerLength()
    {
        return deathTimerLength;
    }

    private void Jump()
    {
        m_rb2d.velocity = Vector2.up * JUMP_VELOCITY;
        m_anim.SetBool("isJumping", true);
        AudioOutput.manager.PlayClip("jump sound", 2f, 1);
    }
}
