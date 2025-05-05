using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    [Header("Weapon Setup")]
    public List<GunData> guns = new List<GunData>();
    public List<GameObject> gunModels = new List<GameObject>();

    [Header("Shooting")]
    public Transform bulletSpawn;
    private float nextFireTime = 0f;

    [Header("Audio")]
    private AudioSource audioSource;
    private PlayerHealth playerHealth;

    private int currentGunIndex = 0;
    public int CurrentGunIndex => currentGunIndex;
    public GunData CurrentGun => guns[CurrentGunIndex];

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();

        // initialize ammo
        foreach (var gun in guns)
        {
            gun.currentMagazine = gun.magazineSize;
            gun.currentAmmo = gun.maxAmmo;
        }

        UpdateGunModel();
    }

    // Update is called once per frame
    void Update()
    {
        // DON’T allow any shooting or switching while paused
        if (PauseMenu.isPaused)
            return;

        // Also early-out if dead
        if (playerHealth != null && playerHealth.IsDead())
            return;

        if (playerHealth != null && playerHealth.IsDead())
            return;

        HandleWeaponSwitching();

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            if (CurrentGun.currentMagazine <= 0)
            {
                Reload();
                // Add a small delay so reload sound can play
                nextFireTime = Time.time + (CurrentGun.reloadSound?.length ?? 0.5f);
            }
            else
            {
                Shoot();
                nextFireTime = Time.time + CurrentGun.timeBetweenShots;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetGun(2);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SetGun((currentGunIndex + 1) % guns.Count);
        if (scroll < 0f) SetGun((currentGunIndex - 1 + guns.Count) % guns.Count);
    }

    void SetGun(int idx)
    {
        if (idx < 0 || idx >= guns.Count || idx == currentGunIndex)
            return;
        currentGunIndex = idx;
        UpdateGunModel();
    }

    void UpdateGunModel()
    {
        for (int i = 0; i < gunModels.Count; i++)
            gunModels[i].SetActive(i == currentGunIndex);
    }

    void Shoot()
    {
        var gun = CurrentGun;
        if (gun.currentMagazine <= 0)
        {
            Debug.Log("Empty magazine! Press R to reload.");
            return;
        }

        // Play gun-specific fire sound
        if (audioSource != null && gun.fireSound != null)
            audioSource.PlayOneShot(gun.fireSound);

        gun.currentMagazine--;

        if (gun.gunName == "Shotgun")
        {
            int pellets = 5;
            float spreadYaw = 10f;
            for (int i = 0; i < pellets; i++)
            {
                // only randomize yaw (Y axis)
                Quaternion spreadRot = bulletSpawn.rotation *
                    Quaternion.Euler(0f, Random.Range(-spreadYaw, spreadYaw), 0f);
                FireBullet(gun, spreadRot);
            }
        }
        else
        {
            FireBullet(gun, bulletSpawn.rotation);
        }
    }

    void FireBullet(GunData gun, Quaternion rot)
    {
        var b = Instantiate(gun.bulletPrefab, bulletSpawn.position, rot);
        if (b.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = b.transform.forward * gun.bulletSpeed;
        if (b.TryGetComponent<Bullet>(out var bs))
            bs.damage = gun.bulletDamage;
        Destroy(b, 2f);
    }

    void Reload()
    {
        var gun = CurrentGun;
        bool didReload = false;

        if (gun.hasUnlimitedAmmo)
        {
            if (gun.currentMagazine < gun.magazineSize)
            {
                gun.currentMagazine = gun.magazineSize;
                didReload = true;
            }
        }
        else
        {
            int needed = gun.magazineSize - gun.currentMagazine;
            if (needed > 0 && gun.currentAmmo > 0)
            {
                int load = Mathf.Min(needed, gun.currentAmmo);
                gun.currentAmmo -= load;
                gun.currentMagazine += load;
                didReload = true;
            }
        }

        if (didReload)
        {
            Debug.Log($"{gun.gunName} reloaded ({gun.currentMagazine}/{gun.magazineSize}).");
            // Play gun-specific reload sound
            if (audioSource != null && gun.reloadSound != null)
                audioSource.PlayOneShot(gun.reloadSound);
        }
        else
        {
            Debug.Log("Nothing to reload.");
        }
    }

    public void PickupAmmo(string gunName, int amount)
    {
        foreach (var gun in guns)
        {
            if (gun.gunName == gunName && !gun.hasUnlimitedAmmo)
            {
                gun.currentAmmo = Mathf.Min(gun.currentAmmo + amount, gun.maxAmmo);
                Debug.Log($"{gunName} reserve: {gun.currentAmmo}/{gun.maxAmmo}");
            }
        }
    }
}
