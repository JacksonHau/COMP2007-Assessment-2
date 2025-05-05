using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public int ammoAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[AmmoBox] Trigger with: {other.name} (tag={other.tag})");
        if (other.CompareTag("Player"))
        {
            var shooter = other.GetComponent<PlayerShooter>();
            if (shooter != null)
            {
                shooter.PickupAmmo("Shotgun", ammoAmount);
                shooter.PickupAmmo("Rifle", ammoAmount);
                Debug.Log($"Picked up {ammoAmount} for both Shotgun & Rifle");
            }
            Destroy(gameObject);
        }
    }
}

