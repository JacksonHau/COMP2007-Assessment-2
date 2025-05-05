using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject bulletPickupPrefab;
    public Transform player;

    public int zombiesPerWave = 5;
    public float spawnRadius = 15f;
    public int waveNumber = 0;

    public float pickupSpawnInterval = 20f;

    private bool isSpawning = false;
    private List<GameObject> aliveZombies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnPickupRoutine());
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnPickupRoutine()
    {
        while (player != null)
        {
            yield return new WaitForSeconds(pickupSpawnInterval);

            Vector3 randomPos = player.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = player.position.y;

            Instantiate(bulletPickupPrefab, randomPos, Quaternion.identity);
        }
    }

    IEnumerator SpawnWaveRoutine()
    {
        while (player != null)
        {
            if (aliveZombies.Count == 0 && !isSpawning && !player.GetComponent<PlayerHealth>().IsDead())
            {
                isSpawning = true;
                yield return new WaitForSeconds(5f);

                waveNumber++;
                SpawnWave();
                isSpawning = false;
            }
            yield return null;
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < zombiesPerWave; i++)
        {
            Vector3 spawnPos = player.position + Random.onUnitSphere * spawnRadius;
            spawnPos.y = player.position.y;

            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
            aliveZombies.Add(zombie);
        }

        zombiesPerWave += 2;
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    void Update()
    {
        aliveZombies.RemoveAll(z => z == null);
    }
}