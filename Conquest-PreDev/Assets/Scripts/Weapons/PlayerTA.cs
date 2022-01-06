using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTA : MonoBehaviour
{
    
    // Player Targeting Agent
    
    public Image trackingReticle;
    public Image reticle;
    [SerializeField] float targetingZoneRadius;
    GameObject currentLock;
    int h = 0; // always start 0
    public List<GameObject> inRange;
    [Header("Crosshair Auto Lock Parameters")]
    public float lockRange = 50f;
    public float spherecast_r = 10f;
    public float sweetspot = 0.10f; // in viewport space on scale from 0 left to 1 right
    TeamManager myTM;
    public Camera pcam;
    RaycastHit camRay;
    public GameObject aimIndicator;
    CapsuleCollider targetingAreaTrigger;
    Transform pbs;
    Transform sbs;
    // make an activate + deactivate method for the lock on and have them erase the Q when turned off
    void Awake()
    {
        targetingAreaTrigger = GetComponent<CapsuleCollider>();
        myTM = gameObject.GetComponent<TeamManager>();
        inRange = new List<GameObject>();
        InvokeRepeating("TargetingTick", 0.25f, 0.25f);
        trackingReticle.enabled = false;
        targetingAreaTrigger.radius = targetingZoneRadius;
    }


    public void MovedIntoRange(GameObject tgt)
    {
        inRange.Add(tgt);
        inRange.TrimExcess();
    }
    public void RemoveAndTrim(GameObject t)
    {
        inRange.Remove(t);
        inRange.TrimExcess();
    }

    public void MovedOutOfRange(GameObject tgt)
    {
        inRange.Remove(tgt);
        inRange.TrimExcess();
    }

    void TargetingTick()
    {
        // re-design the targeting tick
    }

    /*

    void TargetingTick()
    {
        //Debug.DrawRay(pcam.transform.position, pcam.transform.forward, Color.cyan, 0.5f);
        if (Physics.SphereCast(pcam.transform.position, spherecast_r, pcam.transform.forward, out camRay, lockRange))
        {
            //Debug.Log("Spherecast hit something!");
            TeamManager tm;
            aimIndicator.transform.position = camRay.point;
            if (camRay.transform.gameObject.TryGetComponent<TeamManager>(out tm))
            {
                if (tm.isTgtable == false)
                    return;

                if (!tm.m_Team.Equals(myTM.m_Team) && !inRange.Contains(camRay.transform.gameObject))
                {
                    // Dont add if its ouside of the sweet spot
                    var viewportPt = Camera.main.WorldToViewportPoint(camRay.transform.position);
                    if (viewportPt.x < 0.5f - sweetspot || viewportPt.x > 0.5f + sweetspot)
                        return;
                    Debug.Log("Enemy Detected");
                    MovedIntoRange(camRay.transform.gameObject);

                    currentLock = camRay.transform.gameObject;
                }

            }
        }

        for (int i = 0; i < inRange.Count; i++)
        {
            if (!inRange[i])
                return;

            Renderer r = inRange[i].gameObject.GetComponentInChildren<Renderer>();

            var viewportPt = Camera.main.WorldToViewportPoint(inRange[i].transform.position);
            //Debug.Log(viewportPt + inRange[i].name);

            if (viewportPt.x < 0.5f - sweetspot || viewportPt.x > 0.5f + sweetspot)
            {
                Debug.Log("Moved out of sweetspot");
                RemoveAndTrim(inRange[i]);
            }
            // check into r.isVisible alternatives
            else if (!r.isVisible || Vector3.Distance(gameObject.transform.position, inRange[i].transform.position) > lockRange)
            {
                Debug.Log("Target became not visible or too far away");
                RemoveAndTrim(inRange[i]);
            }
        }
        if (inRange.Count < 1)
            currentLock = null;
    }*/
        void AimGuns(Transform t)
        {
            // Need direction to the target
            //Debug.Log("Aim Dem Guns");

            pbs.LookAt(t.position);
            sbs.LookAt(t.position);
        }

        void TrackReticle(Image r, GameObject t)
        {
        // phase out camera.main eventually PLEASE
            Vector3 viewport = Camera.main.WorldToViewportPoint(t.transform.position);
            Vector3 screen = Camera.main.ViewportToScreenPoint(viewport);
            r.gameObject.transform.position = screen;
        }
        
        void FixedUpdate()
        {
            if (currentLock)
            {
            Debug.Log("Player is engaging!");
            Transform aimspot = currentLock.transform;
                trackingReticle.enabled = true;
                AimGuns(aimspot);
                TrackReticle(trackingReticle, currentLock);
            }
            else
        {
            //Debug.Log("Lock is null!");
            trackingReticle.enabled = false;
        }
        

            
        }
    
    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.gameObject.SendMessageUpwards("MovedIntoRange", gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.gameObject.SendMessageUpwards("MovedOutOfRange", gameObject);
        }

    }
    void Update()
        {
        }
}
