using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public abstract class WeaponCore : MonoBehaviour
{
    [Header("Customize The Fields!")]
    public string m_WepName;
    public GameObject projectile;
    public Transform firepoint;
    [Header("DO NOT CHANGE")]
    public float shotVelocityMult = 1;
    public int roundsInClip;
    public int clipSize;
    public float fireRate = 25f; // bullets per second
    public float sinceLastShot;
    public float reloadTime = 1f; // default 1 sec
    public float reloadTimer = 0; // cooldown for reload
    public bool reloading = false;

    public void Fire()
    {
        if ( reloading == false && sinceLastShot > (6 / fireRate))
        {
            sinceLastShot = 0f;
            GameObject bulletObj = Instantiate(projectile, firepoint.position, firepoint.rotation); // Instantiaite
            bulletObj.GetComponent<Bullet>().bullet_velocity *= shotVelocityMult; // after creating the bullet, multiply the speed immediately
            roundsInClip -= 1;
            if (roundsInClip == 0)
                CheckReload();
        }
        else
        {
            Debug.Log(gameObject.name + " tried to fire while reloading!");
        }

    }

    public void CheckReload()
    {
        if (roundsInClip < 1 && reloading == false)
        {
            //If we depleted our clip and our bool is still true, change it to false and add time to the reload timer
            reloadTimer = reloadTime;
            reloading = true;
        }
        else if (reloading == true && reloadTimer < 0)
        {
            //else if we have been waiting and the reload timer finally has run out, add bullets and change bool
            reloading = false;
            roundsInClip = clipSize;
        } else if (roundsInClip == clipSize)
        {
            reloading = false;
        }

    }

    private void FixedUpdate()
    {
        sinceLastShot += Time.deltaTime;
        if (reloading == true && roundsInClip < 1 && reloadTimer > 0)
        {
            // else if we have set the bool and we've got reload time but not added bullets to the clip yet, keep waiting
            reloadTimer -= Time.deltaTime;
            if (reloadTimer < 0)
                CheckReload();
        }
    }
}
