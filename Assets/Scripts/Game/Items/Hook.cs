using UnityEngine;
using System.Collections;


public class Hook : MonoBehaviour {

    private float timer;
    public Camera camera;
    public float forceToPush;
    private Grap grapplingPart;
    private bool detach = false;
    private int raycastDistance = 10000;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScaling;
    Vector3 directionToMove;
    public LineRenderer chain;
    private GameObject grapplingPoint;
    private GameObject creature;
    private bool shoot = false;
    private Quaternion initialRotationForTheHook = new Quaternion(0,0,0,0);
    private GameObject grapplingHookHand;
    private GameObject grapper;
    private GameObject grapperPoint;

    public bool Detach
    {
        get { return detach; }
        set { detach = value; }
    }


    void Awake()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        chain = GameObject.Find("Chain").GetComponent<LineRenderer>();
        grapplingPoint = GameObject.Find("GrapperPoint");
        creature = GameObject.FindGameObjectWithTag("Player");
        grapplingHookHand = GameObject.Find("grappling_hook_Right_Arm 1");
        grapper = GameObject.FindGameObjectWithTag("Grapper");
        grapperPoint = GameObject.Find("GrapperPoint");
        gameObject.layer = 11;
    }
	// Use this for initialization
	void Start () {
        grapplingPart = GameObject.FindGameObjectWithTag("Grapper").GetComponent<Grap>();
        initialPosition = this.transform.position;
       // initialRotation = grapplingPart.transform.rotation;
        initialScaling = grapplingPart.transform.localScale;
        Debug.Log("Grappling Hook initial Position:"  + grapplingPart.transform.position);
        chain.enabled = false;
        Physics.IgnoreCollision(grapplingHookHand.GetComponent<Collider>(), grapper.GetComponent<Collider>());
        Physics.IgnoreCollision(creature.GetComponent<Collider>(), grapper.GetComponent<Collider>());
	} 
	
	// Update is called once per frame
	void Update () {
        
        initialPosition = this.transform.position;
        chain.SetPosition(0, this.transform.position);
        chain.SetPosition(1, grapplingPoint.transform.position);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (shoot == false) // WE shoot 
        {
         
            chain.enabled = true;
            if (Input.GetButtonDown("Jump") && Input.GetButton("Jump") && this.transform.root.name == "The_Creature") // Check if we pressed button (space)
            {
                shoot = true;
                if (Physics.Raycast(ray, out hit))
                {
                    chain.enabled = true;
                    this.transform.DetachChildren(); // detach grappling hook itself
                    GameObject temp = GameObject.FindGameObjectWithTag("Grapper"); // Grappling Hook
                    directionToMove = (hit.point - temp.transform.position).normalized;
                    temp.GetComponent<Rigidbody>().isKinematic = false; // turn off kinematics on the grapper
                    temp.GetComponent<Rigidbody>().AddForce(directionToMove * forceToPush); // add force in order to shoot


                }
                timer = Time.time; // get the time since the button was pressed.
            }
        }
        else if (Input.GetButton("Jump") && !Input.GetButtonUp("Jump")) // if button is still pressed.
        {
            float passedTimer = Time.time - timer; // get the timer (to detect the timer)
            if (passedTimer > 0.3)
            {
                GameObject temp = GameObject.FindGameObjectWithTag("Grapper");// get the Grapper part
                GameObject tempEnemy = grapplingPart.ColliderObject; // get the object with which it collided
                if (tempEnemy != null && tempEnemy.gameObject.name == "GrapplingLocation")
                {
                    creature.gameObject.transform.position = Vector3.MoveTowards(creature.gameObject.transform.position, temp.gameObject.transform.position, 7 * Time.deltaTime);
                    var dist = Vector3.Distance(creature.gameObject.transform.position, temp.gameObject.transform.root.position);
                    Debug.Log(dist);
                    if(dist < 1.5f)
                    {
                        temp.gameObject.transform.DetachChildren();
                        temp.gameObject.transform.parent = this.transform;
                        grapperPoint.transform.parent = temp.gameObject.transform;
                        temp.gameObject.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime);
                    }
                }
                else if (tempEnemy != null && tempEnemy.gameObject.name.Contains("Villager"))
                {
                    temp.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime); // since we attached a collided object to our grappling hook...move grappling hook back

                }
                if (temp.transform.position == initialPosition)
                {
                    if (tempEnemy.gameObject.name.Contains("Villager"))
                    {
                        tempEnemy.gameObject.GetComponent<Enemy>().IsHitWithGrapple(false);
                    }
                    resetHookShot(temp, tempEnemy);
                }
            }
        }

        if (Input.GetButton("DetachRope"))
        {
            detach = true;   
        }

        if (detach)
        {
            DetachChain();
        }
        
	}


    private void DetachChain()
    {
        GameObject grapple = GameObject.FindGameObjectWithTag("Grapper");
        grapple.transform.parent = this.transform;


        if (grapplingPart.ColliderObject != null)
        {
            if (grapplingPart.ColliderObject.name.Contains("Villager") || grapplingPart.ColliderObject.name == "GrapplingLocation")
            {

                GameObject tempEnemy = grapplingPart.ColliderObject;
                tempEnemy.transform.parent = null;
                if (tempEnemy.gameObject.name.Contains("Villager") && tempEnemy.gameObject.GetComponent<Enemy>())
                {
                    tempEnemy.GetComponent<Enemy>().IsHitWithGrapple(false);
                }
            }

        }

        grapple.transform.position = Vector3.MoveTowards(grapple.transform.position, initialPosition, 7 * Time.deltaTime);  
        if (grapple.transform.position == initialPosition)
        {
            GrapplingSettings();
            detach = false;
           // initialPosition = this.transform.position + new Vector3(0, 0, 1);
            shoot = false;
            grapplingPart.ColliderObject = null;
        }
    }

    private void GrapplingSettings()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Grapper");
        temp.transform.rotation = initialRotationForTheHook;
    }

    private void resetHookShot(GameObject temp, GameObject tempEnemy) {
        temp.transform.parent = this.transform;
        GrapplingSettings();
        tempEnemy.transform.parent = null;
        shoot = false;
    }

}
