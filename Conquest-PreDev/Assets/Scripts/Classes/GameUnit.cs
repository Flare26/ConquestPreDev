using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Nathan Frazier
public abstract class GameUnit : MonoBehaviour
{
    enum State
    {
        AttackHQ,
        DefendHQ,
        DefendPost,
        HoldGround
    }

    // Start is called before the first frame update


    [Header("Base Stats")]
    // Basic Stats
    public int h_current;
    public int h_max;
    public float sh_current;
    public float sh_max;
    public float sh_rate;
    public float sh_delay;
    public float range = 20f;
    

    // Weaponry
    [Header("Weaponry")]
    [SerializeField] public GameObject primaryWep;
    [SerializeField] public GameObject secondaryWep;
    public Transform curr_targ;
    [HideInInspector] public GameObject primaryInstance;
    [HideInInspector] public GameObject secondaryInstance;

    [Header("Backend")]
    // Backend
    [SerializeField] State stance;
    public Transform mount_Primary;
    public Transform mount_Secondary;
    public List<GameObject> targetedBy;
    public bool hasShield;
    public float sinceLastDMG = 0f;
    TeamManager TeamManager;

    private void Awake()
    {
        TeamManager = GetComponent<TeamManager>();
    }
    private void OnEnable()
    {
        // On enable set the correct team color

        if (primaryWep != null)
        {
            primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary.position, mount_Primary.rotation);
        }

        if (secondaryWep != null)
        {
            secondaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary.position, mount_Secondary.rotation);
        }
        // Secondary wep not accounted for yet

        // in the prefab the mount points should be first 2 children on the npc
        // The target sphere should always be the last

        NavMeshAgent navAgent;
        if (TryGetComponent<NavMeshAgent>(out navAgent))
        {
            Debug.Log("NavMeshAgent Found! ");
        }else
        {
            Debug.Log("NO NavMeshAgent on object: " + gameObject.name);
        }
    }
    void CheckShieldCharge()
    {
        sinceLastDMG += Time.deltaTime;
        if (sinceLastDMG > sh_delay && sh_current < sh_max)
        {
            sh_current += Time.deltaTime * sh_rate;
            hasShield = true;
        }
        else if (sh_current > sh_max)
        {
            sh_current = sh_max;
            hasShield = true;
        }
    }
    void Update()
    {
        
        CheckShieldCharge();
    }
}
