using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : ParticleAgent
{
    // Start is called before the first frame update
    public GameObject boostFXPrefab;
    public GameObject bulletHitFX;
    public GameObject shieldPopFX;

    ParticleSystem boostInsta;
    ParticleSystem hitInsta;
    ParticleSystem shPopInsta;
    public float lifetime = 3f;
    // Update is called once per frame
    private void Awake()
    {
        boostInsta = Instantiate(boostFXPrefab, gameObject.transform).GetComponent<ParticleSystem>();
        hitInsta = Instantiate(bulletHitFX, gameObject.transform).GetComponent<ParticleSystem>();
        shPopInsta = Instantiate(shieldPopFX, gameObject.transform).GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        boostInsta.Stop();
        hitInsta.Stop();
        shPopInsta.Stop();
    }
    public void PlayBoost(bool boosting)
    {
        if (boosting)
        {
            boostInsta.Play();
        } else
        {
            boostInsta.Stop();
        }
    }

    public void BulletHitFX()
    {
        hitInsta.Stop();
        hitInsta.Play();
    }
    public void PopShield()
    {
        shPopInsta.Stop();
        shPopInsta.Play();
    }

    public void RegenShield()
    {
        Debug.Log("No shield regen particle FX");
    }

    private void FixedUpdate()
    {
        
    }
}
