using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    public PlayerShooter shooter;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI ammoCountText;

    // Update is called once per frame
    void Update()
    {
        if (shooter == null || shooter.guns.Count == 0) return;

        var gun = shooter.CurrentGun;

        weaponNameText.text = gun.gunName;

        string mag = gun.currentMagazine.ToString();
        string reserve = gun.hasUnlimitedAmmo ? "∞" : gun.currentAmmo.ToString();
        ammoCountText.text = $"{mag} / {reserve}";
    }
}
