using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject bulletPickupPrefab;
    public GameObject healingPickupPrefab;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnInterval = 20f;
    public float spawnRadius = 15f;
    public float spawnHeightOffset = 0.3f;

    [Range(0f, 1f)]
    public float healChance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPickupsRoutine());
    }

    IEnumerator SpawnPickupsRoutine()
    {
        var wait = new WaitForSeconds(spawnInterval);

        while (player != null && !player.GetComponent<PlayerHealth>().IsDead())
        {
            yield return wait;

            // pick a random point around the player
            Vector3 spawnPos = player.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = player.position.y + spawnHeightOffset;

            // choose ammo or healing
            GameObject toSpawn = (Random.value < healChance)
                ? healingPickupPrefab
                : bulletPickupPrefab;

            Instantiate(toSpawn, spawnPos, Quaternion.identity);
            Debug.Log($"Spawned {(toSpawn == healingPickupPrefab ? "Healing" : "Ammo")} pickup at {spawnPos}");
        }
    }
}
