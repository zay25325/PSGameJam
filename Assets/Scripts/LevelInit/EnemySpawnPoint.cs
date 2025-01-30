using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs = new List<GameObject>();

    public GameObject GetRandomEnemy()
    {
        return EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];
    }
}
