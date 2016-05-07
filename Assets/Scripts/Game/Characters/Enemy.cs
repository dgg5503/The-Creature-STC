using UnityEngine;
using System.Collections.Generic;

/*
 * Enemy
 * AI
 * - cast a ray from face up for AIMING AND THROWING
 * - cast a ray from BOTTOM OF COLLIDER FORWARD to see if he can walk forward, otherwise navigate.
 * 
 * Attack
 * - Periodically equip a spear if none and throw it with X amount of force
 * - Throw equiped object (from right hand only???)
 */

enum EnemyState
{
    Idle, 
    Wonder,
    Attack,
    Flee,
    Alert,
    Equipping,
    Grappled,
    None
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

    // Delegate
    public delegate void DeathCallback();
    public event DeathCallback deathCallback;

    // FIelds
    private float sphereCastRadius = 5f;
    public float coneAngle = 30;
    public float maxIdleTime = 5;
    public float minIdleTime = 1;
    public float maxWonderTime = 7;
    public float minWonderTime = 3;
    public float fleeTime = 4;
    public float equipTime = 5;
    private float aimTime = 2;
    public float maxDistance = 10;

    // nav mesh agent info
    private NavMeshAgent navMeshAgent;

    // raycast fields
    private Vector3 positionDifference;
    private float calculatedAngle;
    private Transform headPoint;
    private RaycastHit[] raycastHits;
    private Character targetedCharacter;

    // state machine
    private EnemyState enemyState;
    private float stateTimer;

    // animation handling
    //private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        foreach(KeyValuePair<int, CustomJoint> kvp in joints)
            if(kvp.Value.BodyPart != null)
                kvp.Value.BodyPart.bodyPartHitCallbacks += BodyPart_bodyPartHitCallbacks;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.stoppingDistance = .5f;

        // reference to head cast position.
        headPoint = joints[0].BodyPart.transform;
        calculatedAngle = Mathf.Cos(Mathf.Deg2Rad * coneAngle);
        targetedCharacter = null;

        // start idle
        enemyState = EnemyState.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void BodyPart_bodyPartHitCallbacks(int health)
    {
        Die();
    }

    protected override void Start()
    {
        // DEBUG EQUIP.
        //EquipItem();
        //Die();
    }

    /// <summary>
    /// For debug purposes only, will update calc angle when changed in inspector.
    /// </summary>
    void OnValidate()
    {
        calculatedAngle = Mathf.Cos(Mathf.Deg2Rad * coneAngle);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // state machine
        // handle state actions here
        switch (enemyState)
        {
            case EnemyState.Idle:
                // update internal timer, dont move for a little while
                stateTimer -= Time.deltaTime;
                
                //animator.Play("idle");
                if (stateTimer <= 0)
                    ChangeStateTo(EnemyState.Wonder);

                // scan for the enemy
                if (targetedCharacter == null)
                    ScanForPlayer();
                break;

            case EnemyState.Wonder:
                //animator.speed = navMeshAgent.desiredVelocity.magnitude;
                
                // if destination reached, go back to idle.
                // if the path was invalid, just go back to idle.
                
                if (navMeshAgent.isOnNavMesh && !navMeshAgent.pathPending)
                {
                    if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                    {
                        if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude <= .16)
                        {
                            ChangeStateTo(EnemyState.Idle);
                        }
                    }
                }

                // scan for the enemy
                if (targetedCharacter == null)
                    ScanForPlayer();
                break;

            case EnemyState.Attack:
                //Debug.Log("ATTACKING");
                // ensure target is within distance
                if(Vector3.Distance(transform.position, targetedCharacter.transform.position) > maxDistance)
                {
                    ChangeStateTo(EnemyState.Wonder);
                    UseItem(CreatureBodyBones.Right_Arm_Part_2, KeyState.KEY_UP);
                    targetedCharacter = null;
                    break;    
                }

                stateTimer -= Time.deltaTime;

                // draw line
                Debug.DrawLine(headPoint.position, targetedCharacter.transform.position, Color.red);

                // look at target and use item.
                Vector3 targetVector = targetedCharacter.transform.position - headPoint.transform.position;
                Vector3 lookDirection = targetVector.normalized;
                lookDirection.y = 0;
                transform.forward = lookDirection;

                if(stateTimer <= 0)
                {
                    UseItem(CreatureBodyBones.Right_Arm_Part_2, KeyState.KEY_UP);
                    //stateTimer = equipTime;
                    ChangeStateTo(EnemyState.Equipping);
                }
                else
                {
                    // calculate angle from forward up or down for target
                    
                    UseItem(CreatureBodyBones.Right_Arm_Part_2, KeyState.KEY_DOWN);
                }
                break;

            case EnemyState.Equipping:
                if (Vector3.Distance(transform.position, targetedCharacter.transform.position) > maxDistance)
                {
                    ChangeStateTo(EnemyState.Wonder);
                    //UseItem(CreatureBodyBones.Right_Arm_Part_2, KeyState.KEY_DOWN);
                    targetedCharacter = null;
                    break;
                }

                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    ChangeStateTo(EnemyState.Attack);
                    EquipItem();
                }
                break;

            case EnemyState.Flee:
                // After fleeing for a while, stay alert.
                stateTimer -= Time.deltaTime;
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete ||
                    navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || 
                    stateTimer <= 0)
                    ChangeStateTo(EnemyState.Alert);
                break;

            case EnemyState.Alert:
                // stay on alert for a little while
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                    ChangeStateTo(EnemyState.Wonder);
                break;

            case EnemyState.Grappled:
                break;

            case EnemyState.None:
               // Debug.Log("hi");
                break;
        }
        
    }

    private void ChangeStateTo(EnemyState state)
    {
        // if already set, just return
        if (state == enemyState)
            return;

        // set state
        enemyState = state;
        
        // make changes
        switch (state)
        {
            case EnemyState.Idle:
                // set state timer to idle time random
                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                //animator.speed = 1;
                //animator.Play("idle");
                //Debug.Log("IDLE!!");
                break;

            case EnemyState.Wonder:
                // get random direction
                navMeshAgent.Resume();
                //UseItem(CreatureBodyBones.Right_Arm_Part_2, KeyState.KEY_UP);
                if (navMeshAgent.isOnNavMesh)
                    navMeshAgent.destination = RandomNavSphere(transform.position, 5f, -1); //transform.position + (new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * 5f);

                //animator.Play("walk");
                //Debug.Log("WONDER!!");
                //Debug.Log("Current Pos: " + transform.position);
                //Debug.Log("Dest set: " + navMeshAgent.destination);
                break;

            case EnemyState.Attack:
                // stop moving for now
                Debug.Log("going to attack");
                navMeshAgent.Stop();
                stateTimer = aimTime;
                break;

            case EnemyState.Equipping:
                stateTimer = equipTime;
                break;

            case EnemyState.Flee:
                // get destination to that opposite of the creature
                //navMeshAgent
                break;

            case EnemyState.Alert:
                break;

            case EnemyState.Grappled:
                stateTimer = 0;
                navMeshAgent.Stop();
                break;

            case EnemyState.None:
                navMeshAgent.enabled = false;
                stateTimer = 0;
                break;
        }

        // set state in animator
        //animator.SetInteger("characterState", (int)state);
    }
    
    private void EquipItem()
    {
        RegularItem tmpItem = (Instantiate(GameManager.PrefabDictionary["spear"]) as GameObject).GetComponent<RegularItem>();
        MountItem(tmpItem, CreatureBodyBones.Right_Arm_Part_2);
    }

    private void ScanForPlayer()
    {
        // get all objs in vicinty based on head
        Debug.DrawLine(headPoint.position, headPoint.position + (Vector3.forward * sphereCastRadius));

        // get all hits limited to character mask
        raycastHits = Physics.SphereCastAll(headPoint.position, sphereCastRadius, Vector3.up, sphereCastRadius, GameManager.CharacterLayerMask);

        // see if we hit the player
        for (int i = 0; i < raycastHits.Length; ++i)
        {
            if (raycastHits[i].collider.CompareTag("Player"))
            {
                // grab position difference, check out to see if in cone LOS.
                positionDifference = (raycastHits[i].collider.transform.position - transform.position).normalized;

                /*
                 * 1. Project position location of player relative to head on to forward of this guy (dot product)
                 * 2. Angle of cone is now in 3D space from forward, inside cone is when normalized dot product is larger than the radian angle (our calculated theta)
                 */
                if ((Vector3.Dot(transform.forward, positionDifference)) > calculatedAngle &&
                    Vector3.Distance(transform.position, raycastHits[i].collider.transform.position) < maxDistance)
                {
                    // Logical reaction
                    //Debug.DrawLine(headPoint.position, raycastHits[i].collider.transform.position, Color.red);
                    SeePlayerResponse(raycastHits[i].collider.GetComponent<Character>());
                    break;
                }
            }
        }
    }

    protected override void Die()
    {
        ChangeStateTo(EnemyState.None);
        if (deathCallback != null)
            deathCallback();
        base.Die();
    }

    private void SeePlayerResponse(Character character)
    {
        targetedCharacter = character;

        // if no weapon, get one.
        EquipItem();

        ChangeStateTo(EnemyState.Attack);
        // set state based on current stats
        // have a weapon? try to attack
        // if hasWeapon
        // changeStateTo Attack
        // else (has no weapon)
        // changeStateTo Flee

    }

    /// <summary>
    /// Taken from: http://forum.unity3d.com/threads/solved-random-wander-ai-using-navmesh.327950/
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="dist"></param>
    /// <param name="layermask"></param>
    /// <returns></returns>
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        // get random direction in given distance
        Vector3 randDirection = Random.insideUnitSphere * dist;

        // add this to origin (nav mesh)
        randDirection += origin;

        // sample the nav mesh and return.
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
            return navHit.position;

        return transform.position;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "KillVillager")
            Die();
    }

    public void IsHitWithGrapple(bool status)
    {
        if (status)
            ChangeStateTo(EnemyState.Grappled);
        else
            ChangeStateTo(EnemyState.Idle);
    }

    protected override void ProcessMovement()
    {
        velocity = navMeshAgent.desiredVelocity;
        navMeshAgent.nextPosition = transform.position;
    }

    public override void CalculateAimPoint()
    {
        AimingAt = Vector3.ProjectOnPlane((targetedCharacter.transform.position - transform.position).normalized, transform.right);
    }
}
