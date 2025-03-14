using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("LevelInitializer Start");
        List<GameObject> spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("EnemySpawn"));

        Debug.Log("LevelInitializer choose");
        for (int i = 0; i < SaveDataManager.Instance.EnemyCount; i++)
        {
            Debug.Log("LevelInitializer random");
            // choose a random spawn point
            int randomIndex = Random.Range(0, spawnPoints.Count);
            EnemySpawnPoint spawnPoint = spawnPoints[randomIndex].GetComponent<EnemySpawnPoint>();


            Debug.Log("LevelInitializer spawn");
            // spawn the enemy
            GameObject enemy = GameObject.Instantiate(spawnPoint.GetRandomEnemy(), TileManager.Instance.transform);
            enemy.transform.position = spawnPoints[randomIndex].transform.position;


            Debug.Log("LevelInitializer remove");
            // remove the spawnpoint
            spawnPoints.RemoveAt(randomIndex);
            GameObject.Destroy(spawnPoint.gameObject);
        }

        Debug.Log("LevelInitializer destroy");
        foreach (GameObject obj in spawnPoints)
        {
            // remove any remaining spawnpoints
            GameObject.Destroy(obj);
        }

        Debug.Log("LevelInitializer set player");
        // init the player
        CharacterInfo player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterInfo>();    // Find the player object
        player.Attacks = SaveDataManager.Instance.Attacks;
        player.MaxHP = SaveDataManager.Instance.HP;
        player.HP = SaveDataManager.Instance.HP;
        player.Speed = SaveDataManager.Instance.Speed;

        Debug.Log("LevelInitializer End");
    }
}
