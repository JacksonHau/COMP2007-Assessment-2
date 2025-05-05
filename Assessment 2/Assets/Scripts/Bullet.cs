using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 35; 

    public float lifeTime = 3f;
    public GameObject hitEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ZombieAI zombie = collision.gameObject.GetComponent<ZombieAI>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage, collision.contacts[0].point); 
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
