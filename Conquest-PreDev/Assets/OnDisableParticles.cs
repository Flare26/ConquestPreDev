using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDisableParticles : MonoBehaviour
{
    [SerializeField] GameObject destroyFX;
    private void OnDisable()
    {
        //Debug.Log("Spawning on disable particles");
        var tmp = Instantiate(destroyFX, transform.position, transform.rotation, transform.parent);
        Destroy(tmp, 5f);
    }
}
