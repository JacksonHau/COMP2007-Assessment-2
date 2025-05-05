using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPickup : MonoBehaviour
{
    public int healAmount = 25;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure it's a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }

        // optional feedback:
        Debug.Log($"Picked up healing pack: +{healAmount} HP");

        Destroy(gameObject);
    }
}
