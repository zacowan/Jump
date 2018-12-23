using System.Collections.Generic;
using UnityEngine;

public class DangerPooler : MonoBehaviour
{

    class Object
    {
        public Object(string name, int total, int min, int max, float spawnRate, GameObject prefab, Vector2 pos, bool offset)
        {
            m_name = name;
            m_total = total;
            m_min = min;
            m_max = max;
            m_spawnRate = spawnRate;
            m_offset = offset;
            
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
        private bool m_offset;
        private List<GameObject> m_objects = new List<GameObject>();

        public bool SpawnObject(int rand, Vector2 pos, float offset)
        {
            if (m_offset) pos = new Vector2(pos.x, pos.y + offset);
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
    private const float GLOBAL_SPAWN_RATE = 1.5f;

    private Vector2 POOL_POSITION = new Vector2(-25f, -25f);
 
    private const float SPAWN_POSITION_X = 10f;
    private const float SPAWN_POSITION_Y = -2.56f;

    private const float MIN_Y_OFFSET = 4.0f;
    private const float MAX_Y_OFFSET = 5.5f;

    private float spawnRange = GLOBAL_SPAWN_RATE;

    public GameObject m_barnaclePrefab;
    public GameObject m_halfSawPrefab;
    public GameObject m_sawPrefab;

    // Use this for initialization
    void Start()
    {
        m_objectList.Add(new Object("barnacle", 4, 1, 1, GLOBAL_SPAWN_RATE, m_barnaclePrefab, POOL_POSITION, false));
        m_objectList.Add(new Object("half saw",  4, 2, 2, 2.5f, m_halfSawPrefab, POOL_POSITION, false));
        m_objectList.Add(new Object("saw",  4, 3, 3, 3.5f, m_sawPrefab, POOL_POSITION, true));
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

            if (m_timeSinceLastSpawn >= spawnRange)
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
                    int num = Random.Range(1, 4);
                    float offset = Random.Range(MIN_Y_OFFSET, MAX_Y_OFFSET);

                    foreach (Object obj in m_objectList)
                    {
                        if (obj.SpawnObject(num, new Vector2(SPAWN_POSITION_X, SPAWN_POSITION_Y), offset))
                        {
                            m_timeSinceLastSpawn = 0.0f;
                            spawnRange = Random.Range(GLOBAL_SPAWN_RATE / 1.25f, GLOBAL_SPAWN_RATE);
                            break;
                        }
                    }
                }
            }
        }
    }
}
