using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeaterClass : GameUnit
{
    public int hull;
    public int shield;
    public int shield_rate;
    public GameObject weaopn1;
    public GameObject weapon2;
    // Start is called before the first frame update
    void Awake()
    {
        this.hull = 100;
        this.shield = 50;
    }

}
