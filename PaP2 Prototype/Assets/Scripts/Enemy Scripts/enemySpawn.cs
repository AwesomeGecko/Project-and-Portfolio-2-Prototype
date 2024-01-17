using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] public int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] List<GameObject> spawnList = new List<GameObject>();

    int spawnCount;
    bool isSpawning;
    bool startSpawning;


    void Start()
    {
        gameManager.instance.updateGameGoal(numToSpawn);
    }


    void Update()
    {
        if (startSpawning && spawnCount < numToSpawn && !isSpawning)
        {
            //spawn stuff
            StartCoroutine(spawn());
        }
    }

    public void heyIDied()
    {
        numToSpawn--;
        spawnCount--;
    }

    IEnumerator spawn()
    {

        isSpawning = true;

        //Transform spawnPoint = GetRandomSpawnPoint();

        
            int arrayPos = Random.Range(0, spawnPos.Length - 1);
            GameObject objectClone = Instantiate(objectToSpawn, spawnPos[arrayPos].transform.position, spawnPos[arrayPos].transform.rotation);
            objectClone.GetComponent<EnemyAI>().mySpawner = this;

            spawnList.Add(objectClone);
            spawnCount++;

        //if (!IsSpawnPointOccupied(spawnPoint.position))
        //{

        //}


            yield return new WaitForSeconds(timeBetweenSpawns);
        isSpawning = false;

    }

    bool IsSpawnPointOccupied(Vector3 position)
    {
        // Check if there's already an enemy at the specified position
        Collider[] colliders = Physics.OverlapSphere(position, 1.0f);

        // If there are colliders (enemies or other objects), the spawn point is considered occupied
        return colliders.Length > 0;
    }
    Transform GetRandomSpawnPoint()
    {
        return spawnPos[Random.Range(10, spawnPos.Length)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = false;

            for (int i = 0; i < spawnList.Count; i++)
            {
                Destroy(spawnList[i]);
            }

            spawnList.Clear();
            spawnCount = 0;
        }
    }
}
