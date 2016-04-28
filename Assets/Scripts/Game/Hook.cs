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
    void Awake()
    {
        chain = GameObject.Find("Chain").GetComponent<LineRenderer>();
        grapplingPoint = GameObject.Find("GrapperPoint");
        creature = GameObject.Find("The_Creature");
        grapplingHookHand = GameObject.Find("grappling_hook_Right_Arm 1");
        grapper = GameObject.FindGameObjectWithTag("Grapper");
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
                    //Vector3 direction = (hit.point - temp.transform.position).normalized; // get the direction to shoot
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
                Debug.Log("Temp Enemy is here" + tempEnemy);
                Debug.Log("Temp is right here" + temp);
                if (tempEnemy.gameObject.name == "GrapplingLocation" && tempEnemy != null)
                {
                    Debug.Log("Should Move Creature HERE");
                   // this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, temp.gameObject.transform.position, 7 * Time.deltaTime);
                   // this.gameObject.transform.root.position = Vector3.MoveTowards(this.transform.root.position, temp.gameObject.transform.position, 7 * Time.deltaTime);
                    creature.gameObject.transform.position = Vector3.MoveTowards(creature.gameObject.transform.position, temp.gameObject.transform.position, 7 * Time.deltaTime);
                    var dist = Vector3.Distance(temp.gameObject.transform.position, this.gameObject.transform.root.position);
                    if(dist == 0)
                    {
                        temp.gameObject.transform.DetachChildren();
                        temp.gameObject.transform.parent = this.transform;
                    }
                }
                else  if(tempEnemy.gameObject.name == "Wall" && tempEnemy != null)
                {
                    temp.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime); // since we attached a collided object to our grappling hook...move grappling hook back
                    
                }
                if (temp.transform.position == initialPosition)
                {
                    temp.transform.parent = this.transform;
                    GrapplingSettings();
                    tempEnemy.transform.parent = null;
                    shoot = false;
                }
            }
        }
        if (Input.GetButtonDown("DetachRope"))
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
        GameObject temp = GameObject.FindGameObjectWithTag("Grapper");
        temp.transform.parent = this.transform;


        if (grapplingPart.ColliderObject != null)
        {
            Debug.Log("NAME OF THE STUFF IS: " + grapplingPart.ColliderObject.name);
            if (grapplingPart.ColliderObject.name == "Wall" || grapplingPart.ColliderObject.name == "GrapplingLocation")
            {
               
                GameObject tempEnemy = grapplingPart.ColliderObject;
                tempEnemy.transform.parent = null;
            }

        }


        temp.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime);
        if (temp.transform.position == initialPosition)
        {
            GrapplingSettings();
            detach = false;
            initialPosition = this.transform.position + new Vector3(0, 0, 1);
            shoot = false;
        }
    }

    private void GrapplingSettings()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Grapper");
     //   temp.transform.rotation = initialRotation;

        temp.transform.rotation = initialRotationForTheHook;
        Debug.Log("Rotation Should BE THIS: " + temp.transform.rotation);
       // temp.transform.localScale = initialScaling;
    }

}
