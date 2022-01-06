using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeaterWEP : WeaponCore
{
    // Start is called before the first frame update
    void Awake()
    {
        m_WepName = "Repeater";
        clipSize = 3;
        roundsInClip = clipSize;
        
    }
    
}
