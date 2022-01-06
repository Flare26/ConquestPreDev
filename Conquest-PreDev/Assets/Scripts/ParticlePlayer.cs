using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    // Particle FX
    [SerializeField] GameObject shieldPopFX;
    [SerializeField] GameObject deathPopFX;
    GameObject shieldFX;
    GameObject deathFX;
    float particleLifetime = 3f;

    public void PopShield()
    {
        shieldFX = (GameObject)Instantiate(shieldPopFX, transform.position, transform.rotation);
        Destroy(shieldFX, particleLifetime);
    }

    public void DeathFX()
    {
        deathFX = (GameObject)Instantiate(deathPopFX, transform.position, transform.rotation);
        Destroy(deathFX, particleLifetime);
    }

}
