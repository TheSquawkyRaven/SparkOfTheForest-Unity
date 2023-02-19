using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public int TreesDestroyed => FireLost.treesDestroyed;
    public int EnemiesCount => TR.childCount;

    public Transform TR;
    public GameObject waterObj;
    public float difficulty;

    private float spawnC;

    public float SpawnOutsideRadius;

    private float spawnTime;
    private int maxEnemies;

    private void Awake()
    {
        maxEnemies = MaxEnemies();
        spawnTime = SpawnTime();
    }

    private void Update()
    {
        spawnC += Time.deltaTime;
        float spawnSpeed = spawnTime;
        if (spawnC > spawnSpeed)
        {
            spawnC = 0;
            TrySpawn();
            spawnTime = SpawnTime();
        }
    }

    private float SpawnTime()
    {
        float spawnTime = (float)(EnemiesCount + 1) / maxEnemies * (10f / difficulty);
        return spawnTime;
    }

    private int MaxEnemies()
    {
        //Depends on treesdestroyed
        float factor = Mathf.Sqrt(TreesDestroyed);
        int max = Mathf.RoundToInt(factor);
        if (max <= 0)
        {
            max = 1;
        }

        return max;
    }

    private void TrySpawn()
    {
        maxEnemies = MaxEnemies();
        if (EnemiesCount >= maxEnemies)
        {
            return;
        }
        Spawn();
    }

    private void Spawn()
    {
        Vector3 pos = Random.insideUnitCircle + (Vector2)Player.Instance.TR.localPosition;
        pos.Normalize();
        pos *= SpawnOutsideRadius;
        Instantiate(waterObj, pos, Quaternion.identity, TR);
    }



}
