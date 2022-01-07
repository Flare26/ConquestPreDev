
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public float TTL = 3f; // time to live in sec
    public float bullet_velocity = 50f;
    public int m_dmg;
    Rigidbody m_Rigidbody;
    [SerializeField] ParticleSystem emitterOnCollision;
    private void Awake()
    {
        //Ignore other bullet collisions
        m_Rigidbody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(9, 9);
    }
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Bullet hit somethign with tag " + collision.transform.tag);
        var other = collision.gameObject;
        if (other.transform.CompareTag("Wall"))
        {
            //Debug.Log("Congrats you shot a wall...");
            var tmp = Instantiate(emitterOnCollision, transform, true);
            DestroyBullet();
            return;
        }
        if (other.transform.CompareTag("Floor"))
        {
            //Debug.Log("Congrats you shot the floor...");
            Debug.Log("Bullet hit floow");
            var tmp = Instantiate(emitterOnCollision);
            tmp.Play();
            Destroy(tmp, 5f);
            DestroyBullet();
            return;
        }
        if (other.CompareTag("Bullet"))
        {
            DestroyBullet();
            return;
        }
        if (other.transform.CompareTag("Environment"))
        {
            Debug.Log("Bullet hit environment");

            DestroyBullet();
            return;
        }
    }
    private void OnDestroy()
    {
        var tmp = Instantiate(emitterOnCollision, transform.position, transform.rotation);
        tmp.Play();
        Destroy(tmp, 5f);
    }

    public void HitTarget()
    {
        //Debug.Log("A bullet hit it's target!");
        DestroyBullet();
    }

    public void DestroyBullet()
    {
        // Call Hit Target, not Destroy bullet please! :)
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        m_Rigidbody.velocity = transform.forward * bullet_velocity;

        //if (Vector3.Distance(targetTransform.position, transform.position) <= speed * Time.deltaTime)
           // HitTarget();

        if (TTL > 0)
            TTL -= Time.deltaTime;
        else
            Destroy(gameObject); 
    }
    
}
