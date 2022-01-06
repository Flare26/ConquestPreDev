using System.Collections;
using UnityEngine;
// Nathan Frazier
public class HoverController_AI : MonoBehaviour
{
    [SerializeField] GameObject [] m_hoverPoints;
    [SerializeField] bool[] isRayHit;
    private Rigidbody m_body;

    public int m_currStrafe { get; private set; }
    public float m_hoverForce = 2000;
    public float m_hoverHeight;
    public float m_bwardAcl = 5000;
    public float m_fwardAcl = 9000;
    public int m_boostSpeed { get; private set; }

    public float m_turnStrength = 500;

    public float m_currThrust = 0;
    public float m_currTurn = 0;
    public bool isBoosting = false;
    private float m_baseAclFWD;
    public float m_baseAclBWD;

    private void Awake()
    {
        m_body = GetComponent<Rigidbody>();
        isRayHit = new bool[m_hoverPoints.Length];

    }
    void FixedUpdate()
    {
        // Physics calculations

        //IDEA: Raycast once and store wether or not each hover point hits. If it does, check to see if the length is <= hover height
        // * if it is <= hover height, then apply the force at position to that point
        //Hover Force
        RaycastHit hit;
        bool[] isDownward = new bool[m_hoverPoints.Length];

        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
            var hoverPoint = m_hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -transform.up, out hit) && hit.distance < m_hoverHeight) // if there is a hit and that same hit distance is <= hover height
            {
                m_body.AddForceAtPosition(Vector3.up * m_hoverForce * (1.0f - (hit.distance / m_hoverHeight)), hoverPoint.transform.position);
                isRayHit[i] = true ;
            }
            else
            {
                // Is on it's side or on a hitch
                m_currThrust = 0;
                isBoosting = false;
                isRayHit[i] = false;
            }
        }

        //Forward
        if (System.Math.Abs((double)m_currThrust) > 0)
            m_body.AddForce(transform.forward * m_currThrust);

        //Turn
        // multiplication by m_currTurn is required because it can be negative 
        if (m_currTurn > 0)
        {
            m_body.AddRelativeTorque(Vector3.up  * m_turnStrength);
        }
        else if (m_currTurn < 0)
        {
            m_body.AddRelativeTorque(- Vector3.up * m_turnStrength);
        }

        //Strafe

        if (m_currStrafe != 0)
        {
            //If player is strafing either A or D check the raycasts
            for (int i = 0; i < m_hoverPoints.Length; i++)
            {
                GameObject hoverPoint = m_hoverPoints[i];
                // if there is no hit on any one of the 4 then that means that corner is in the air technically
                // player should not be able to work against gravity by adding force sideways while on an angle
                if (!Physics.Raycast(hoverPoint.transform.position, -transform.up, out hit, m_hoverHeight))
                    m_currStrafe = 0;

            }

            if (m_currStrafe == 1) // if D
            {
                m_body.AddForce(transform.right * m_bwardAcl);
            }
            if (m_currStrafe == -1) // if A
            {

                m_body.AddForce(-transform.right * m_bwardAcl);
            }
        }

        // If boosting, then buff the speed over time. If not boosting then return to normal values.
        if (isBoosting)
        {
            //forward acl
            if (m_fwardAcl < m_baseAclFWD + m_boostSpeed)
            {
                m_fwardAcl += Time.deltaTime * m_boostSpeed;
            }

            // reverse acl + strafe acl
            if (m_bwardAcl < m_baseAclBWD + m_boostSpeed)
            {
                m_bwardAcl += Time.deltaTime * m_boostSpeed;
            }

        }
        else if (m_fwardAcl > m_baseAclFWD && m_bwardAcl > m_baseAclBWD)
        {
            //Decays when input is not w a s d shift
            m_fwardAcl -= Time.deltaTime * m_boostSpeed;
            m_bwardAcl -= Time.deltaTime * m_boostSpeed;
        }
    }
}
