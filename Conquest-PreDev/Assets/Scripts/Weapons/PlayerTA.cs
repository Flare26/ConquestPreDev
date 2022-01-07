using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTA : MonoBehaviour
{
    
    // Player Targeting Agent
    
    public Image trackingReticle;
    RectTransform trackingReticleTform;
    public float tgtIndicatorVertOffset;
    public Image reticle;
    [SerializeField] float targetingZoneRadius;
    public Transform worldspaceLock;
    int h = 0; // always start 0
    public List<Transform> inRange;
    [Header("Crosshair Auto Lock Parameters")]
    public float lockRange = 50f;
    public float spherecast_r = 10f;
    public float sweetspot = 0.10f; // in viewport space on scale from 0 left to 1 right
    TeamManager myTM;
    PlayerManager myPM;
    public Camera playerCameraReference;
    
    public GameObject aimIndicator;
    CapsuleCollider targetingAreaTrigger;
    WepV2 primaryWep;
    WepV2 secondaryWep;

    RaycastHit lastCastHit;
    // make an activate + deactivate method for the lock on and have them erase the Q when turned off
    void Awake()
    {
        myPM = GetComponent<PlayerManager>();
        myTM = gameObject.GetComponent<TeamManager>();
        trackingReticleTform = trackingReticle.rectTransform;
        targetingAreaTrigger = GetComponent<CapsuleCollider>();
        inRange = new List<Transform>();

        InvokeRepeating("TargetingTick", 0.25f, 0.25f);
        trackingReticle.enabled = false;
        targetingAreaTrigger.radius = targetingZoneRadius;
    }


    public void MovedIntoRange(Transform tgt)
    {
        foreach ( Transform t in inRange)
        {
            if (tgt.Equals(t))
            {
                return;
            }
        }
        inRange.Add(tgt);
        
    }
    public void RemoveAndTrim(Transform t)
    {
        inRange.Remove(t);
        inRange.TrimExcess();
    }

    public void MovedOutOfRange(Transform tgt)
    {
        inRange.Remove(tgt);
        inRange.TrimExcess();
    }
    void BarrelToTarget(WepV2 wepv2instance)
        {
        // Need direction to the target
        //Debug.Log("Aim Dem Guns");
        // firepoint = wpc.curr fire point Transform
        Transform firepoint = primaryWep.firePoints[primaryWep.activeFP];
        wepv2instance.AimFP(firepoint, worldspaceLock.transform);
            //sbs.LookAt(t.position);
        }

    void LockReticleUpdate(Vector3 worldSpaceTarg)
        {
        // phase out camera.main eventually PLEASE
        Vector3 tmp = worldSpaceTarg;
        tmp.y += tgtIndicatorVertOffset;
        trackingReticleTform.position = Camera.main.WorldToScreenPoint(tmp);
        }
    void TargetingTick()
    {
        // re-design the targeting tick
        RaycastHit camRayInfo;
        //int layer = 7; // for ray layer masking
        if (Physics.SphereCast(transform.position, spherecast_r, playerCameraReference.transform.forward, out camRayInfo, lockRange))
        {
            lastCastHit = camRayInfo;
            var colliderfound = camRayInfo.collider;
            // each tick check for 
            if ( colliderfound.CompareTag("NPC") )
            {
                Debug.Log("Enemy Detected");
                MovedIntoRange(camRayInfo.transform);
                var w2vp = Camera.main.WorldToViewportPoint(camRayInfo.transform.position); // world to view point
                
                if (w2vp.x < 0.5f - sweetspot && w2vp.x > 0.5f + sweetspot)
                    Debug.Log("Enemy Left Sweet Spot");
                worldspaceLock = colliderfound.transform;
                trackingReticle.enabled = true;
            }
                
        }


    }
        
    void FixedUpdate()
        {
        var targViewportPoint = Camera.main.WorldToViewportPoint(worldspaceLock.position);
        if (worldspaceLock)
        {
            LockReticleUpdate(worldspaceLock.position);
            //BarrelToTarget(myPM.wpc0);
            // IF x > 1 OR z is NEGATIVE then the point is behind the camera viewport
            if (targViewportPoint.x > 1 || targViewportPoint.z < 0)
            {
                trackingReticle.enabled = false; // keep tracking reticle off
            }
            else
                trackingReticle.enabled = true; // else we have a current lock so true 
            Debug.Log("Viewport point of current target - " + targViewportPoint);
            //Debug.Log("Player is engaging!");
            
            
            
        }
        else
            trackingReticle.enabled = false; // else currentLock should be empty here so we dont need reticle

        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.gameObject.SendMessageUpwards("MovedIntoRange", gameObject);
        }
        else if (other.gameObject.CompareTag("NPC"))
            SendMessageUpwards("MovedIntoRange", other.transform);

    }

    private void OnTriggerExit(Collider other)
    {
        var name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.gameObject.SendMessageUpwards("MovedOutOfRange", gameObject);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lastCastHit.point, 1f);
    }
}
