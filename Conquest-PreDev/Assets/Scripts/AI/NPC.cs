using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEngine.AI;

public class NPC : GameUnit, IKillable
{
    // Nathan Frazier
    // these are going to be created and built with the team of the base
    Vector3 spawnXYZ;
    public CommandPost parentCPost;
    float turningRate = 10f; 
    HoverController_AI driver;
    ParticlePlayer fxplayer;
    TeamManager tm;
    NPCTargetingAgent tgtagt;
    public Transform currNavpoint;
    NavMeshAgent navAgent;
    private void Awake()
    {
        // Triggered when script is loaded into runtime
        TryGetComponent<CommandPost>(out parentCPost);
        tgtagt = GetComponent<NPCTargetingAgent>();
        navAgent = GetComponent<NavMeshAgent>();

        if (!TryGetComponent<HoverController_AI>(out driver))
            Debug.LogError("NPC Hover tank has no hover controller script! >" + gameObject.name );

        if (!TryGetComponent<ParticlePlayer>(out fxplayer))
            Debug.LogError("NPC does not have a particle player script! >" + gameObject.name);

        if (!TryGetComponent<TeamManager>(out tm))
            Debug.LogError("NPC does not have a Team Manager! >" + gameObject.name);
        if (!TryGetComponent<NPCTargetingAgent>(out tgtagt))
            Debug.LogError("NPC does not have a TargetingAgent! >" + gameObject.name);
    }

    void OnEnable()
    {
        // This is triggered every time NPC respawns. Do this instead of destroying
        h_current = h_max;
        sh_current = sh_max;
        hasShield = true;
        spawnXYZ = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary);
        secondaryInstance = Instantiate<GameObject>(secondaryWep, mount_Secondary);
        InvokeRepeating("RefreshCurrentTarget", 0.5f, 0.5f);
    }
    void RefreshCurrentTarget()
    {
        curr_targ = tgtagt.RequestClosestTarget();
        
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            TakeDamage(collision.gameObject.GetComponent<Bullet>());
    }

    public void TakeDamage(Bullet b)
    {
        var indmg = b.m_dmg;
        sinceLastDMG = 0;
        
        if (sh_current < indmg && hasShield)
        {
            fxplayer.PopShield();
            // incoming dmg greater than shield, sub shield from dmg and apply to HP
            int bleed = Mathf.RoundToInt( indmg - sh_current );
            h_current -= bleed;
            sh_current = 0;
            // Start the shield regen count at 0
            //Debug.Log("Dmg made it past shield!");
        } else if (hasShield && sh_current - indmg != 0)
        {
            //Debug.Log("Shield absorbs dmg");
            // incoming dmg is either same as shield or less so sub from shield
            sh_current -= indmg;
        } else if (sh_current - indmg == 0)
        {
            Debug.Log("Perfect Pop");
            sh_current = 0;
            hasShield = false;
            fxplayer.PopShield();
        } else if (!hasShield)
        {
            h_current -= indmg;
            //Debug.Log("Took direct hit while shield DOWN! ");
        }
    }

    public void DeathRoutine()
    {
        Debug.Log(name + " has died.");
        fxplayer.DeathFX();
        gameObject.SetActive(false);
        // Clear targeting systems
        //Debug.Log("Clearing tgtagt systems...");
        tgtagt.inRange.Clear();
        tgtagt.hostiles.Clear();

        if (spawnXYZ == null)
            return;
        else
            transform.position = spawnXYZ;
        
        if (parentCPost != null)
            parentCPost.turretQ.Enqueue(gameObject);
    }
    private void FixedUpdate()
    {
        //first check for death
        if (h_current < 1)
        {
            DeathRoutine();
        }
        // Navigation

        if (!currNavpoint.Equals(null))
        {
            navAgent.destination = currNavpoint.position;

        }
        if (curr_targ != null)
        {
            //If there is a current target
            //navigation
            navAgent.SetDestination(curr_targ.position);
            primaryInstance.GetComponent<WeaponCore>().CheckReload();
            secondaryInstance.GetComponent<WeaponCore>().CheckReload();
            primaryInstance.GetComponent<WeaponCore>().Fire();
            secondaryInstance.GetComponent<WeaponCore>().Fire();
            //Vector3 noY = new Vector3(transform.position.x, 0f, transform.position.z);
            Quaternion targetRotation = Quaternion.LookRotation(curr_targ.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turningRate * Time.deltaTime);
        }


    }
}
