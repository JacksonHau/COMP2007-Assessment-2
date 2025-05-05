using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunData
{
    public string gunName;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public float timeBetweenShots = 0.3f;
    public int bulletDamage = 35;

    public int magazineSize = 6;
    public int currentMagazine = 6;

    public int maxAmmo = 30;          // total reserve
    public int currentAmmo = 30;      // current reserve
    public bool hasUnlimitedAmmo = false;

    [Header("Audio Clips")]
    public AudioClip fireSound;    // drag each gun’s fire clip here
    public AudioClip reloadSound;  // drag each gun’s reload clip here
}
