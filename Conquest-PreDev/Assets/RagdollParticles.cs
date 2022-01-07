using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollParticles : MonoBehaviour
{
    // Start is called before the first frame update
       [SerializeField] Transform transformToAttachTo;
    [SerializeField] GameObject ragdollParticles;
    ParticleSystem psys;
    GameObject instance;
    // Update is called once per frame
    private void Awake()
    {
        instance = Instantiate(ragdollParticles, null); // set null parent for it to be world space
        psys = instance.GetComponent<ParticleSystem>();
        instance.transform.position = transformToAttachTo.position;

        psys.Play();
    }
    void FixedUpdate()
    {
        instance.transform.position = transformToAttachTo.position;
    }
}
