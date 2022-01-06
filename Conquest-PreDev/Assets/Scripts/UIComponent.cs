using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIComponent : MonoBehaviour
{
    // Start is called before the first frame update
    Image reticle;
    Image lockReticle;
    private void Awake()
    {
        reticle = GameObject.Find("Reticle").GetComponent<Image>();
        lockReticle = GameObject.Find("PLockReticle").GetComponent<Image>();
    }
}
