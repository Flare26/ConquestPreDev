
using UnityEngine;
public class TurretAI : MonoBehaviour
{
    public CommandPost parentCPost;
    public GameObject weapon1Obj;
    public GameObject weapon2Obj;
    WeaponCore primary;
    WeaponCore secondary;

    GameObject primaryInst;
    GameObject secondaryInst;
    Transform primaryBulletSpawn;
    Transform secondaryBulletSpawn;
    public Transform primaryMount;
    public Transform secondaryMount;
    [SerializeField] GameObject tHead;
    
    public string targetName = "No Target";
    public Transform aimOrb;
    public float spreadFactor; // bullet spread range +/-
    public int turretHP;
    private int maxHP;
    
    public int dmg_Mod;
    
    private float tm = 0f; // Time since last shot
    public float turnSpeed; // clamped 0-1
    public float range;
    public float fireRate; // shots per sec

    TeamManager m_TM;
    Transform currentTarget;
    public float shotVelocityMult;
    NPCTargetingAgent mytgtagt;
    // Start is called before the first frame update
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(tHead.transform.position, range);
    }

    private void Start()
    {
        parentCPost = GetComponentInParent<Transform>().GetComponentInChildren<CommandPost>();
    }
    private void Awake()
    {
        GetComponent<NPCTargetingAgent>();
        primaryInst = Instantiate<GameObject>(weapon1Obj, primaryMount);
        secondaryInst = Instantiate<GameObject>(weapon2Obj, secondaryMount);
        primaryBulletSpawn = primaryInst.transform.Find("FP"); // find firepoints inside the instance
        secondaryBulletSpawn = secondaryInst.transform.Find("FP");
        primary = primaryInst.GetComponent<WeaponCore>();
        secondary = secondaryInst.GetComponent<WeaponCore>();
        mytgtagt = GetComponent<NPCTargetingAgent>();
        mytgtagt.mode = NPCTargetingAgent.Mode.Turret;
    }
    private void OnEnable()
    {
        turretHP = maxHP; // heal turret fully

        if (gameObject.transform.parent != null)
        {
            m_TM = GetComponentInParent<TeamManager>();

        }
        else
        {
            Debug.Log(gameObject.name + " is ORPHANED!"); // if no TurretBuilder in the parent, then the turret is not connected to a base.
        }

        InvokeRepeating("UpdateTarget", 0f, 0.42f); // This is called when the turret is "Built"
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            int dmg = collision.collider.GetComponent<Bullet>().m_dmg;
            turretHP -= dmg;
            collision.collider.GetComponent<Bullet>().DestroyBullet();
            //Debug.Log("Turet took dmg");
            if (turretHP <= 0)
            {
                //destroy turret
                Debug.Log("Turret has died");
                gameObject.SetActive(false); // does not update the bool array yet
            }
        }
    }

    void Shoot()
    {
        if (!currentTarget.Equals(null))
        {
           // Every shot recalculates accuracy of the aim orb.
                float x = Random.Range(-spreadFactor, spreadFactor);
                float y = Random.Range(-spreadFactor, spreadFactor);
                float z = Random.Range(-spreadFactor, spreadFactor);
                Vector3 spread = new Vector3(x, y / 4 , z);
                aimOrb.position += spread;
                currentTarget = aimOrb.transform;

            primary.Fire() ; // Instantiaite
            secondary.Fire();
        }
    }
    private void OnDisable()
    {
        // On turret disable, set back to neutral and reset targeting information
        currentTarget = null;
        parentCPost.turretQ.Enqueue(gameObject);
    }


    void UpdateTarget()
    {
        // This method ALSO applies innacuracy range X2 (pos, neg coords) should modify components
        Transform focus = mytgtagt.RequestClosestTarget();
        if (focus != null) {
            currentTarget = focus;
            aimOrb.position = currentTarget.position; // Set aim orb correctly
            
        } else
        {
            currentTarget = null;
        }
    }

    void Update()
    {
        
        // angles to rotate 
        if (currentTarget == null)
            return; // will return null if there are no enemy units within the targeting area
        
        Vector3 dir = currentTarget.position - (tHead.transform.position); // get the difference between points
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up); // have unity calculate quaternion based on this difference
        tHead.transform.rotation = Quaternion.Lerp(tHead.transform.rotation, rotation, 0.9f); // interpolate from current rotation to the one facing the target


            if (tm <= 0f)
            {
                Shoot();
                tm = fireRate;
            }

            tm -= Time.deltaTime;

    }
}
