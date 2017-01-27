using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Projectile : Weapon {

    // Fields
    private Collider[] impalePoints;
    private Collider[] colliders;
    private bool isColliderTrigger;
    private List<Collider> colliderMap;
    private Vector3 lastVelocity;
    private ImpaleState impaleState;
    public float angleOfImpalement = 30;

    // Base properties
    private float calculatedAngle;
    private float minVelocity = 3f;
    //private float impaleDistance = 0f;
    private float collidedMass;
    private float lerpTime = 1f;
    private float currentLerpTime;
    private float currSpeed;

    /// <summary>
    /// Get or set whether or not this projectile should be looking to respond to collision.
    /// </summary>
    protected bool IsActive { get; private set; }

    /// <summary>
    /// Get the last collision obtained from the impale point.
    /// </summary>
    public Collider LastCollision
    {
        get
        {
            if (colliderMap.Count == 0)
                return null;
            return colliderMap[colliderMap.Count - 1];
        }
    }

    /// <summary>
    /// Get the current state of this projectile.
    /// </summary>
    public ImpaleState ImpaleState { get { return impaleState; } }


    protected override void Awake()
    {
        base.Awake();

        // set to active by default
        IsActive = true;

        // grab triggers
        List<Collider> tmpTriggers = new List<Collider>();
        List<Collider> tmpColliders = new List<Collider>();
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].isTrigger)
                tmpTriggers.Add(colliders[i]);
            else
                tmpColliders.Add(colliders[i]);

        this.colliders = tmpColliders.ToArray();
        this.impalePoints = tmpTriggers.ToArray();

        // init impale state as none
        colliderMap = new List<Collider>();
        impaleState = ImpaleState.None;
        isColliderTrigger = false;
        lastVelocity = Vector3.zero;
        calculatedAngle = Mathf.Cos(Mathf.Deg2Rad * angleOfImpalement);

    }

    void FixedUpdate()
    {
        // react based on state
        switch (impaleState)
        {
            case ImpaleState.None:
                // store last velocity
                lastVelocity = rigidbody.velocity;
                //Debug.Log(lastVelocity);
                break;

            case ImpaleState.Impaling:
                // thanks to: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
                //increment timer once per frame
                currentLerpTime += Time.fixedDeltaTime;
                if (currentLerpTime > lerpTime)
                    currentLerpTime = lerpTime;

                // below float value is dependent on the density of the penetrating object.
                currSpeed = Mathf.Lerp(lastVelocity.magnitude, 0, (currentLerpTime / lerpTime) * collidedMass);
                
                //transform.parent.InverseTransformDirection(
                if (transform.parent != null)
                    transform.localPosition += (transform.parent.InverseTransformDirection(transform.right) * currSpeed) * Time.fixedDeltaTime;
                // go until embedded
                if (currSpeed <= .05f)
                {
                    // reset velocities
                    rigidbody.velocity = Vector3.zero;
                    lastVelocity = Vector3.zero;

                    // reset lerp
                    currentLerpTime = 0;

                    // call after impale event
                    AfterImpale(LastCollision);

                    // set to embedded
                    impaleState = ImpaleState.Embedded;

                    // Turn off triggers
                    ToggleTriggers(false);

                    // ignore collision w/ CURRENTLY collided
                    for(int z = 0; z < colliderMap.Count; z++)
                        for (int i = 0; i < colliders.Length; i++)
                            Physics.IgnoreCollision(colliderMap[z], colliders[i]);
                }
                break;

            case ImpaleState.Embedded:
                break;
        }
    }

    /// <summary>
    /// For debug purposes only, will update calc angle when changed in inspector.
    /// </summary>
    void OnValidate()
    {
        calculatedAngle = Mathf.Cos(Mathf.Deg2Rad * angleOfImpalement);
    }


    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + lastVelocity, Color.red);
        Debug.DrawLine(transform.position, transform.right * 5, Color.blue);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (IsActive ||
            ImpaleState == ImpaleState.Impaling)
        {
            // if velocity is negative direction of forward, return
            // if speed is too little to impale, return.
            if (!PointOfCollisionCheck(collider))
                return;

            if (!colliderMap.Contains(collider))
            {
                
                /*
                if (collider.GetComponent<BodyPart>() != null)
                {
                    UnityEditor.Selection.activeGameObject = gameObject;
                    UnityEditor.SceneView.lastActiveSceneView.FrameSelected(true);
                    Debug.Break();
                }
                */

                // set parent   
                //Debug.Log("IN: " + collider.name);
                transform.parent = collider.transform;

                // stop checking for OnCollisionEnter
                IsActive = false;

                // go kinematic and stop detecting collision
                rigidbody.isKinematic = true;

                // set impale state to currently impaling
                impaleState = ImpaleState.Impaling;

                // see if collided object has a rigibody attachement.
                Rigidbody collidedRigidbody;
                if ((collidedRigidbody = collider.GetComponent<Rigidbody>()) != null)
                    collidedMass = collidedRigidbody.mass;
                else
                    collidedMass = 50f;

                if (!isColliderTrigger)
                    ToggleTriggers(true);

                // call on impale event
                //Debug.DrawLine(transform.position, transform.position + lastVelocity, Color.red);
                //Debug.DrawLine(transform.position, transform.right * 5, Color.blue);
                //Debug.DrawLine(transform.position, collider.transform.position - transform.position, Color.green);
                OnImpale(collider);
                //Debug.Break();
            }
            colliderMap.Add(collider);
        }
    }

    void OnTriggerExit(Collider collider)
    {

        colliderMap.Remove(collider);
        // escaped the object we originally entered
        // TODO: Reset speed.
        if (!colliderMap.Contains(collider))
            AfterImpale(collider);

        if (colliderMap.Count == 0 &&
            ImpaleState == ImpaleState.Impaling)
        {
            //Debug.Log("RESET");
            Reset();
            SetActive();
        }
        
    }

    private void ToggleTriggers(bool value)
    {
        isColliderTrigger = value;
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].isTrigger = isColliderTrigger;
    }

    
    private bool PointOfCollisionCheck(Collider collider)
    {
        if (Vector3.Dot(lastVelocity, transform.right) < minVelocity)
            return false;

        /*
        for (int i = 0; i < impalePoints.Length; i++)
        {
            if (Vector3.Dot(transform.right,
                collider.transform.position - impalePoints[i].bounds.center) > calculatedAngle)
                return true;
        }*/

        return true;
    }
    
    public abstract void AfterImpale(Collider collider);
    public abstract void OnImpale(Collider collider);


    // reset and set active.
    protected void Reset()
    {
        // reset state info
        IsActive = true;
        impaleState = ImpaleState.None;
        rigidbody.isKinematic = true;
        currentLerpTime = 0;

        // set last velocity back
        lastVelocity = transform.right * currSpeed;
        rigidbody.velocity = lastVelocity;
        currSpeed = 0;
    }

    protected void SetActive()
    {
        // if collision info is stored, detection collision again with that object
        while (colliderMap.Count != 0)
        {
            for (int i = 0; i < colliders.Length; i++)
                for(int z = 0; z < colliderMap.Count; z++)
                    Physics.IgnoreCollision(colliderMap[z], colliders[i], false);
        }
        colliderMap.Clear();

        // unparent
        transform.parent = null;

        // enable physics
        rigidbody.isKinematic = false;

        ToggleTriggers(false);
    }
}
