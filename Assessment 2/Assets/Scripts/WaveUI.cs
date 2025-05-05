using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    public ZombieSpawner spawner;
    public TextMeshProUGUI waveText;

    // Update is called once per frame
    void Update()
    {
        if (spawner != null && waveText != null)
        {
            waveText.text = "Wave " + spawner.GetWaveNumber().ToString();
        }
    }
}
