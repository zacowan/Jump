using System.Collections.Generic;
using UnityEngine;

public class ItemPooler : MonoBehaviour
{

    class Object
    {
        public Object(string name, int total, int min, int max, float spawnRate, GameObject prefab, Vector2 pos)
        {
            m_name = name;
            m_total = total;
            m_min = min;
            m_max = max;
            m_spawnRate = spawnRate;
            //min >= 1, min < max <= 200
            
            for (int i = 0; i < total; i++)
            {
                m_objects.Add(Instantiate(prefab, pos, Quaternion.identity));
            }
        }

        private string m_name;
        private int m_total;
        private int m_min;
        private int m_max;
        private int m_current = 0;
        private float m_spawnRate;
        private float m_timeSinceLastSpawn = 0.0f;
        private List<GameObject> m_objects = new List<GameObject>();

        public bool SpawnObject(int rand, Vector2 pos)
        {
            if (rand >= m_min && rand <= m_max && m_timeSinceLastSpawn >= m_spawnRate)
            {
                //Debug.Log(m_name + " " + m_current.ToString());
                m_objects[m_current].transform.position = pos;

                m_current++;
                if (m_current >= m_total) m_current = 0;

                m_timeSinceLastSpawn = 0.0f;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IncrementSpawnTime(float time) { m_timeSinceLastSpawn += time; }
        public bool ReadyToSpawn() { return m_timeSinceLastSpawn >= m_spawnRate;  }
    }

    private List<Object> m_objectList = new List<Object>();

    private float m_timeSinceLastSpawn = 0.0f;
    private const float GLOBAL_SPAWN_RATE = 0.75f;

    private Vector2 POOL_POSITION = new Vector2(-25f, -25f);
 
    private const float SPAWN_POSITION_X = 8f;

    private const float MIN_Y = -2.5f;
    private const float MAX_Y= 2.5f;

    public GameObject m_coinPrefab;
    public GameObject m_gemPrefab;
    public GameObject m_keyPrefab;
    public GameObject m_starPrefab;

    // Use this for initialization
    void Start()
    {
        m_objectList.Add(new Object("coin", 16, 1, 160, GLOBAL_SPAWN_RATE, m_coinPrefab, POOL_POSITION));
        m_objectList.Add(new Object("gem",  1, 161, 185, 10.0f, m_gemPrefab, POOL_POSITION));
        m_objectList.Add(new Object("key",  1, 186, 194, 10.0f, m_keyPrefab, POOL_POSITION));
        m_objectList.Add(new Object("star", 1, 195, 200, 20.0f, m_starPrefab, POOL_POSITION));
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.info.CheckIfPlayerDead() && !GameController.instance.CheckIfGamePaused())
        {
            // increment globa spawn timer
            m_timeSinceLastSpawn += Time.deltaTime;

            // increment time since last spawn for all rewards
            foreach (Object obj in m_objectList)
            {
                obj.IncrementSpawnTime(Time.deltaTime);
            }

            if (m_timeSinceLastSpawn >= GLOBAL_SPAWN_RATE)
            {
                // check for valid spawns
                bool canSpawn = false;
                foreach (Object obj in m_objectList)
                {
                    if (obj.ReadyToSpawn())
                    {
                        canSpawn = true;
                        break;
                    }
                }

                if (canSpawn)
                {
                    int num = Random.Range(1, 201);
                    float spawnPositionY = Random.Range(MIN_Y, MAX_Y);

                    foreach (Object obj in m_objectList)
                    {
                        if (obj.SpawnObject(num, new Vector2(SPAWN_POSITION_X, spawnPositionY)))
                        {
                            m_timeSinceLastSpawn = 0.0f;
                            break;
                        }
                    }
                }
            }
        }
    }
}
