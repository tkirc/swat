using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    //FollowDetection

    public GameObject target;
    public float viewRadius = 10;    // Radius of the enemy view
    public float viewAngle = 120;    // Angle of the enemy view
    public string targetTag = "TargetTag";    // Chosse Target
    public LayerMask obstacleMask;  // Choose Obstacles
    public FollowWaypoints pathHandler;
    public Material deathMaterial;


    private Transform currenttarget_transform;
    private TargetEventChecker checker;
    private bool isTargeting = false;
    private bool canShoot = true;


    private float agentSpeed;


    [SerializeField] public float followDuration = 5f;
    [SerializeField] public float shotCooldown = 3f;
    [SerializeField] public float minimumDistanceToTarget = 2f;
    [SerializeField] private float moveForce = 30.0f;
    [SerializeField] private float maxMoveVelocity = 10.0f;



    [SerializeField] private GunMock gun;

    [Header("Debug Info")]
    [SerializeField] private Vector2 moveInput;
    private PlayerControls controls;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentSpeed = agent.speed;
        agent.SetDestination(pathHandler.NextWayPoint());

        checker = GetComponent<TargetEventChecker>();

    }

    // Update is called once per fram
    void Update()
    {


        if (checker != null && checker.getIsDeath())
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material = deathMaterial;
            agent.speed = 0;

            StartCoroutine("hide", 2.0f);

            return;
        }
        else
        {
            Detection();
            //Debug.Log("isTargeting: " + isTargeting);
            /*Debug.Log("isOnIsland: " + isOnIsland.getIsOnIsland());*/
            if (isTargeting)
            {
                //Chase Player
                //agent.speed = agentSpeed + chaseSpeed;

                if (canShoot)
                {
                    gun.shoot();
                    canShoot = false;
                    StartCoroutine(StartShootingAfterDelay());
                }


                agent.destination = currenttarget_transform.position;

                if (agent.remainingDistance <= minimumDistanceToTarget)
                {
                    //viewRadius = init_viewRadius;  // Reactivate Detection 
                    agent.SetDestination(pathHandler.NextWayPoint());
                }

            }
            else
            {
                //Follow Path
                agent.speed = agentSpeed;
                agent.SetDestination(pathHandler.NextWayPoint());

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    //viewRadius = init_viewRadius;  // Reactivate Detection 
                    agent.SetDestination(pathHandler.NextWayPoint());
                }
            }

        }

    }

    public IEnumerator hide(float duration)
    {

        yield return new WaitForSeconds(duration);

        GetComponent<MeshRenderer>().enabled = false;
    }

    void Detection()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, viewRadius);   //  Make an overlap sphere around the enemy to detect the playermask in the view radius

        for (int i = 0; i < targetInRange.Length; i++)
        {

            if (targetInRange[i].CompareTag(targetTag)) // Check if the Collided Object has follow this certain tag. 
            {
                // Save current target. 
                //Debug.Log("gotDetected: ");
                target = targetInRange[i].gameObject;
                currenttarget_transform = target.transform;
                checker = target.GetComponent<TargetEventChecker>();
                StartCoroutine(StopFollowingAfterDelay());

                Transform target_transform = targetInRange[i].transform;
                Vector3 dirToTarget = (target_transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target_transform.position);          //  Distance of the enemy and the player
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        isTargeting = true;
                    }
                    else
                    {
                        /*
                         *  If the player is behind a obstacle the player position will not be registered
                         * */
                        isTargeting = false;

                    }
                }
                if (Vector3.Distance(transform.position, target_transform.position) > viewRadius)
                {
                    /*
                     *  If the player is further than the view radius, then the enemy will no longer keep the player's current position.
                     *  Or the enemy is a safe zone, the enemy will no chase
                     * */
                    isTargeting = false;
                    //  Change the sate of chasing
                }

            }

        }
    }

    IEnumerator StopFollowingAfterDelay()
    {
        yield return new WaitForSeconds(followDuration);

        // Stoppe das Verfolgen, indem isFollowing auf false gesetzt wird
        isTargeting = false;
        StopCoroutine(StopFollowingAfterDelay());
    }


    IEnumerator StartShootingAfterDelay()
    {
        yield return new WaitForSeconds(shotCooldown);

        // Stoppe das Verfolgen, indem isFollowing auf false gesetzt wird
        canShoot = true;
        StopCoroutine(StartShootingAfterDelay());
    }




}
