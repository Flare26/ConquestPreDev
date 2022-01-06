using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;
// Nathan Frazier
// these are going to be created and built with the team of the base
public class NPCAI : MonoBehaviour
{

    [SerializeField] GameObject shieldPopFX;
    [SerializeField] GameObject deathPopFX;
    [SerializeField] Transform destination;
    NavMeshAgent navAgent;
    [SerializeField] Team m_Team;
    [SerializeField] Renderer meshRenderer;
    Transform tgt_Transform;
    Transform mount_Primary;
    Transform mount_Secondary;
    public Transform p_FP; // primary fire point
    public Transform s_FP; // secondary fire point
    public GameObject primaryWep;
    public GameObject secondaryWep;
    public List<GameObject> targetedBy;
    public string targetName;
    public int hull = 10;
    public int shield = 5;
    public int magSize = 3;
    private int roundCount = 5;
    public int maxShield = 5;
    public float shieldDowntime = 3f;
    bool hasShield = true;
    private GameObject shieldFX;
    private GameObject deathFX;
    public float range = 20f;
    GameObject primaryInstance;
    private float tm = 0f;
    public float rldTime = 1; // reload time in seconds
    public float rrld; // remainder reload time
    public float fireRate;
    GameManager.UnitState state;

    private void OnEnable()
    {
        // Hard code the state to following for testing purpose
        state = GameManager.UnitState.Following;

        // On enable set the correct team color
        targetedBy = new List<GameObject>();

        state = GameManager.UnitState.Attacking;

        mount_Primary = transform.GetChild(0).transform;
        mount_Secondary = transform.GetChild(2).transform;

        if (primaryWep != null)
        {
            primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary.position, this.transform.rotation);
        }
            // Secondary wep not accounted for yet

        // in the prefab the mount points should be first 2 children on the npc
        // The target sphere should always be the last

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.Log("NAV ERROR: Please attach NavMeshAgent to " + gameObject.name);
        }
        else
            UpdateDestination();
        InvokeRepeating("UpdateTarget", 0, 0.42f);
        InvokeRepeating("UpdateDestination", 0, 0.42f);
    }

    void RefreshMeshColor()
    {
        var color = meshRenderer.material;
        switch (m_Team)
        {
            case Team.Neutral:
                color.SetColor("_Color", Color.gray);
                break;
            case Team.Red:
                color.SetColor("_Color", Color.red);
                break;
            case Team.Blue:
                color.SetColor("_Color", Color.blue);
                break;
        }
    }
    void UpdateTarget()
    {

        m_Team = GetComponent<TeamManager>().m_Team;
        RefreshMeshColor(); // for good measure
        
        tgt_Transform = GetComponent<NPCTargetingAgent>().RequestClosestTarget(); // Call on the agent to find us who we can aggro
        if (tgt_Transform == null)
        {
            targetName = "Out of Range";
        }
        else
        {
            targetName = tgt_Transform.name;
        }
    }
    void FirePrimary()
    {
        if (primaryWep != null)
        {
            primaryWep.GetComponent<WeaponCore>().Fire(); // The angle of fire can be adjusted via firepoint
        }
    }

    void FireSecondary()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Bullet hitbox component
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(collision.gameObject.GetComponent<Bullet>()); // Get Bullet script instance to see what type of bullet has hit us
            collision.gameObject.GetComponent<Bullet>().HitTarget();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;
        NPCTargetingAgent tmp;

        if (name.Equals("TargetingArea"))
        {
            tmp = other.gameObject.GetComponentInParent<NPCTargetingAgent>();
            if (tmp.inRange.Contains(other.gameObject))
                return;
            //Debug.Log("Unit " + gameObject.name + " has moved into the targeting area of " + tmp.gameObject.name);
            tmp.inRange.Add(other.gameObject);
            if (!targetedBy.Contains(tmp.gameObject))
                targetedBy.Add(tmp.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        String name = other.gameObject.name;
        NPCTargetingAgent tmp; // the TargetingAgent component of the NPC whose range we just entered
        if ( name.Equals("TargetingArea") )
        {
            // if the collision has a targeting agent
            tmp = other.gameObject.GetComponentInParent<NPCTargetingAgent>();
            if (tmp.hostiles.Contains(gameObject)) // If the npc has come into the range of an enemy aligned agent, add to hostiles.
                tmp.hostiles.Remove(gameObject);

            targetedBy.Remove(tmp.gameObject);
        }
    }
    void TakeDamage(Bullet b)
    {
        //Destroy(gameObject);
        
        if (hasShield == false) {
            hull -= b.m_dmg;
        }
            
        if (b.m_dmg >= shield && hasShield) // if dmg is greater than current shield
        {
            //Debug.Log("Shield Pop!!");
            hasShield = false;
            shieldFX = (GameObject)Instantiate(shieldPopFX, transform.position, transform.rotation);
            Destroy(shieldFX, 3f);
            hull -= (b.m_dmg - shield); // use the remaining shield to mitigate
            shield = 0;
        } else if (b.m_dmg < shield)
        {
            shield -= b.m_dmg;
            return;
        }
        if (hull <= 0)
            DeathRoutine();
        //Debug.Log(gameObject.name + " took dmg " + b.dmg);
    }

    private void UpdateDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navAgent.stoppingDistance = 5;
            navAgent.Warp(transform.position);
            //navAgent.SetDestination(targetVector);
        }
        
            navAgent.stoppingDistance = 3;
            navAgent.SetDestination(GameManager.GetPlayerPos(0));

    }

    void RegenShield()
    {
        float chargeRate = 5;
        if ( shield < maxShield )
           shield += (int) ( Time.deltaTime * chargeRate * 100 ) ;
    }

    public void Update()
    {
        //Vector3 dir = tgt_Transform.position - transform.position;

        if (!targetName.Equals("Out of Range"))
        {
            transform.LookAt(tgt_Transform);
            if (roundCount != 0) // if there are not 0 rounds in the mag then try to fire
            {
                if (tm <= 0f)
                {
                    FirePrimary();
                    roundCount -= 1;
                    tm = fireRate;
                }
            }


        }

        if (roundCount == 0)
        {
            if (rrld == 0)
                rrld = rldTime; // re set the remaining reload time counter if it's currently 0 from the last reload....
            if(rrld > 0)
            {
                rrld -= Time.deltaTime;
                if (rrld < 0)
                {
                    roundCount = magSize; // if this iteration causes the counter to reach 0, the reload is complete and round count should be refreshed
                    rrld = 0; // The engine is not perfect at detecting the instant it reaches 0 :( 
                }
            }
        }

        tm -= Time.deltaTime; // fire rate will decrement regardless of having a target
    }

    private void AttackRoutine()
    {
        // each lane should have an array of turrets for each team. 2d array, left lane team 1 turrets are found under lane0[0][x] team 2 turrets are found lane0[1][x]
        // these are static turrets not including base turrets

        // set target to this.lane
    }

    private void DeathRoutine()
    {

        // If this NPC was being targeted by targeting agents at time of death, ensure that it is removed from the lists

        foreach (GameObject g in targetedBy)
        {
            // first, check that whatever was targeting us didnt get destroyed already first
            if (g.Equals(null))
                return;

            NPCTargetingAgent agt;
            g.TryGetComponent<NPCTargetingAgent>(out agt);
                agt.inRange.Remove(gameObject);
                agt.hostiles.Remove(gameObject);
        }

        deathFX = (GameObject)Instantiate(deathPopFX, transform.position, transform.rotation);
        Destroy(shieldFX,1f);
        Destroy(deathFX,1f);
        Destroy(primaryInstance);
        //Destroy(secondaryInstance);
        Destroy(gameObject);
        return;
    }

    private void FixedUpdate()
    {

        

        if (primaryInstance != null)
        {
            primaryInstance.transform.position = p_FP.position;
            primaryInstance.transform.rotation = p_FP.rotation;
        }
            
        if (shieldFX != null)
        {
            shieldFX.transform.position = this.transform.position;
        }

        switch (state)
        {
            case GameManager.UnitState.Attacking:
                break;
        }
 
    }
}
