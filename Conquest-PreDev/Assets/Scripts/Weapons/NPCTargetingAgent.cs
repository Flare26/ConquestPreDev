using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

// CURRENTLY CRASHES BECUASE ITS COUNTING BULLETS AS HOSTILES FOR SOME REASON
public class NPCTargetingAgent : MonoBehaviour
{
    public enum Mode
    {
        NPC,
        Turret
    }
    // This script at it's core should search for a suitable target in range for the turret that it is a parent of. It should present the turret with a target every time it requests one.
    // Made to be used with a targetArea collision trigger sphere or object 
    [SerializeField] public List<GameObject> hostiles;
    [SerializeField] public List<GameObject> inRange;
    [SerializeField] SphereCollider targetingArea;
    TeamManager tm;
    public Mode mode;

    private void OnEnable()
    {
        inRange = new List<GameObject>();
        tm = GetComponent<TeamManager>();

        // Assess whether or not this agent will acquire targets for a turret, or NPC
        //Initialize based on what type of unit the target agent is on
        if (gameObject.tag == "Turret")
        {
            //Debug.Log("Initializing TargetingAgent | Turret");
            var tm = GetComponentInParent<TeamManager>();
            var ai = GetComponentInParent<TurretAI>();
            targetingArea.radius = ai.range; // set the range collider to proper size based on the turret range stat
        }
        else if (gameObject.tag == "NPC")
        {
            //Debug.Log("Initializing TargetingAgent | NPC");
            
            targetingArea.radius = GetComponentInParent<NPC>().range;

        }
        else
        {
            Debug.Log("Targeting Agent has incorrect/null parent!");
        }
        
        
    }

    public void SetTeam(Team t)
    {
        tm.m_Team = t;
    }
    // TurretRoutine and NPCRoutine both populate the hostiles list however they filter what gets added to it differently.
    private void TurretRoutine()
    {
        foreach (GameObject g in inRange)
        {
            // if something dies in the middle of this routine check for that.
            if (g == null)
                return;

            TeamManager utm;
            if (g.TryGetComponent<TeamManager>(out utm))
                {
                Team t = utm.m_Team;
                //Debug.Log("MY TEAM IS " + tm.m_Team + " THE inRange's TEAM IS " + t);
                    if (!utm.m_Team.Equals(tm.m_Team) && !hostiles.Contains(g))
                        hostiles.Add(g);
            }
            else
            {
                Debug.Log("No Team manager found on the inRange");
            }
        }
    }

    private void NPCRoutine()
    {
        foreach (GameObject g in inRange)
        {
            // if something dies in the middle of this routine check for that.
            if (g == null)
                return;

            TeamManager utm;
            if (g.tag.Equals("NPC") || g.tag.Equals("TurretHead"))
            {
                if ( g.TryGetComponent<TeamManager>(out utm) )
                {
                    if (!utm.m_Team.Equals(tm.m_Team) && !hostiles.Contains(g))
                        hostiles.Add(g);
                }
            }
        }
    }

    public void MovedIntoRange(GameObject unit)
    {
        inRange.Add(unit);
    }

    public void MovedOutOfRange(GameObject unit)
    {
        inRange.Remove(unit);
        hostiles.Remove(unit);
    }

    public Transform RequestClosestTarget()
    {
        // In-range includes both turrets and NPCS. Set the hostiles appropriately 

        switch (mode)
        {
            case Mode.Turret:
                TurretRoutine();
                break;
            case Mode.NPC:
                NPCRoutine();
                break;
        }
        hostiles.TrimExcess();
        inRange.TrimExcess();
        if (hostiles.Count == 0)
            return null;

            float dist_low = float.MaxValue;
            Transform close_targ = null;
            List<int> idxs = new List<int>();
            for (int i = 0; i < hostiles.Count; i++)
            {
                if (!hostiles[i].Equals(null))
                {
                Transform t = hostiles[i].transform;
                // if hostile is not active then remove it from list
                if (hostiles[i].activeSelf == false)
                    {
                    Debug.Log("Removing inactives from NPC Targeting Agent");
                    idxs.Add(i);
                    }
                    
                    float dist_targ = Vector3.Distance(transform.position, t.transform.position);
                    if (dist_targ < dist_low )
                    {
                        dist_low = dist_targ;
                        close_targ = t;
                    }

                }
            }

            foreach (int idx in idxs)
        {
            hostiles.RemoveAt(idx);
            inRange.RemoveAt(idx);
        }

        return close_targ;
    }
    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.SendMessageUpwards("MovedIntoRange", gameObject);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            other.SendMessageUpwards("MovedOutOfRange", gameObject);

        }
    }
    public void RequestWeakestTarget()
    {
        //not implemented
    }

    public void RequestStrongestTarget()
    {
        //not implemented
    }

}
