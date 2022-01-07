using TMPro;
using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerManager : MonoBehaviour
{
    // UI
    [Header("UI References")]
    // Sliders
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider shSlider;
    [SerializeField] Slider boostSlider;
    // Texts
    [SerializeField] TMP_Text hpPrintout;
    [SerializeField] TMP_Text shPrintout;
    [SerializeField] Text boostPrintout;
    [SerializeField] Text wep0Name;
    [SerializeField] Text wep1Name;
    [Header("Other")]
    public int playerNumber = 0;
    public int currentHull;
    public float currentShield;
    public int maxHull;
    public int maxShield;
    public float shieldRate;
    public float shieldDelay;
    public GameObject weapon0;
    public GameObject weapon1;
    public List<GameObject> targetedBy = new List<GameObject>();
    public Transform wep0Mount;
    public Transform wep1Mount;
    public static Quaternion defaultAim;
    PlayerParticles particleManager;
    HoverController hvcon;
    public float sinceLastDMG = 0f;
    Rigidbody rb;
    


    // Boost
    public float sinceLastBoost = 0f;
    public float boostRechargeRate;
    public float boostChargeDelay;
    public float currentBoostEnergy;
    public float maxBoostEnergy;

    bool hasShield = false;
    // weapon instances
    GameObject i0;
    public WepV2 wpc0;
    GameObject i1;
    public WepV2 wpc1;
    // Start is called before the first frame update

    void OnEnable()
    {
        hasShield = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentHull = maxHull;
        currentShield = maxShield;
        hpSlider.maxValue = maxHull;
        hpSlider.value = currentHull;
        shSlider.maxValue = maxShield;
        shSlider.value = currentShield;
        boostSlider.value = currentBoostEnergy;
        boostSlider.maxValue = maxBoostEnergy;
        //give the proper stats based on the class script this player is using
        //Change the glow color of your hitbox

        var glowLight = transform.Find("TeamLight");
        Light glowColor = glowLight.GetComponent<Light>();
        particleManager = GetComponent<PlayerParticles>();
        hvcon = GetComponent<HoverController>();
        rb = GetComponent<Rigidbody>();
        
        
        switch (GetComponent<TeamManager>().m_Team)
        {
            //assign by team
            case Team.Red:
                glowColor.color = Color.red;
                Debug.Log("Set Glow To Red!");
                break;

            case Team.Blue:
                glowColor.color = Color.blue;
                Debug.Log("Set Glow To Blue!");
                break;
            default:
                glowColor.color = Color.white;
                break;
        }

        if (weapon0)
        {
            i0 = Instantiate(weapon0, wep0Mount);
            wpc0 = i0.GetComponent<WepV2>();
            wep0Name.text = wpc0.name;
        }

        if (weapon1)
        {
            i1 = Instantiate(weapon1, wep1Mount);
            wpc1 = i1.GetComponent<WepV2>();
            wep1Name.text = wpc1.name;
        }
            
        //defaultAim = primaryBulletSpawn.transform.rotation;
    }

    public void PrimaryFire(InputAction.CallbackContext context)
    {
        // on press down
            wpc0.SendMessage("Shoot", context);
    }

    public void PrimaryReload(InputAction.CallbackContext context)
    {
        // on press down
        if (wpc0.reloading == false)
            wpc0.SendMessage("Reload", context);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag.Equals("Bullet"))
        {
            Debug.Log("Player was hit!");
            //particleManager.BulletHitFX();
            TakeDamage(c.gameObject.GetComponent<Bullet>());
        }
    }

    private void TakeDamage(Bullet b)
    {
        var indmg = b.m_dmg;
        sinceLastDMG = 0;

        if (currentShield < indmg && hasShield)
        {
            particleManager.PopShield();
            // incoming dmg greater than shield, sub shield from dmg and apply to HP
            int bleed = Mathf.RoundToInt(indmg - currentShield);
            currentHull -= bleed;
            currentShield = 0;
            // Start the shield regen count at 0
            //Debug.Log("Dmg made it past shield!");
        }
        else if (hasShield && currentShield - indmg != 0)
        {
            //Debug.Log("Shield absorbs dmg");
            // incoming dmg is either same as shield or less so sub from shield
            currentShield -= indmg;
        }
        else if (currentShield - indmg == 0)
        {
            //Debug.Log("Perfect Pop");
            currentShield = 0;
            hasShield = false;
            particleManager.PopShield();
        }
        else if (!hasShield)
        {
            currentHull -= indmg;
            //Debug.Log("Took direct hit while shield DOWN! ");
        }

        Destroy(b.gameObject);
    }
    public void SetSpawn(Vector3 spawnPoint)
    {
        throw new System.NotImplementedException();
    }

    public void ReturnToBuildQueue(GameObject parent)
    {
        throw new System.NotImplementedException();
    }

    void CheckShieldCharge()
    {
        sinceLastDMG += Time.deltaTime;
        if (sinceLastDMG > shieldDelay && currentShield < maxShield)
        {
            currentShield += Time.deltaTime * shieldRate;
            hasShield = true;
        }
        else if (currentShield > maxShield)
        {
            currentShield = maxShield;
            hasShield = true;
        }
    }
    private void UpdateUI()
    {
        //speedometer.text = rb.velocity.ToString();
        hpSlider.value = currentHull;
        shSlider.value = currentShield;
        boostSlider.value = currentBoostEnergy;
        shPrintout.text = "["+Mathf.RoundToInt(shSlider.value)+"/"+maxShield+"]";
        hpPrintout.text = "[" + Mathf.RoundToInt(hpSlider.value) + "/" + maxHull + "]";
        boostPrintout.text = "[" + Mathf.RoundToInt(currentBoostEnergy) + "/" + boostSlider.maxValue + "]";
    }
    private void Update()
    {
        // Methods dependant on delta time
        CheckBoostEnergy();
        CheckShieldCharge();
        UpdateUI();
    }

    private void CheckBoostEnergy()
    {
        if (!hvcon.isBoosting)
        {
            sinceLastBoost += Time.deltaTime;
            if (currentBoostEnergy < maxBoostEnergy && sinceLastBoost > boostChargeDelay)
                currentBoostEnergy += Time.deltaTime * boostRechargeRate;
        }
        else
        {
            sinceLastBoost = 0;
            currentBoostEnergy -= Time.deltaTime;
        }

        if (currentBoostEnergy <= 0)
            hvcon.canBoost = false;
        else if (currentBoostEnergy > 0)
            hvcon.canBoost = true;
    }
}
