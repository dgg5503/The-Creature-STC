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




    void Awake()
    {
        chain = GameObject.Find("Chain").GetComponent<LineRenderer>();
        grapplingPoint = GameObject.Find("GrapperPoint");
    }
	// Use this for initialization
	void Start () {
        grapplingPart = GameObject.FindGameObjectWithTag("Grapper").GetComponent<Grap>();
        initialPosition = this.transform.position;
        initialRotation = grapplingPart.transform.rotation;
        initialScaling = grapplingPart.transform.localScale;
       
        Debug.Log(chain);
        chain.enabled = false;
	} 
	
	// Update is called once per frame
	void Update () {
        initialPosition = this.transform.position;
        chain.SetPosition(0, this.transform.position);
        chain.SetPosition(1, grapplingPoint.transform.position);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (grapplingPart.Shoot == false) // WE shoot 
        {
            chain.enabled = true;
            if (Input.GetButtonDown("Jump") && Input.GetButton("Jump")) // Check if we pressed button (space)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    chain.enabled = true;
                    this.transform.DetachChildren(); // detach grappling hook itself
                    GameObject temp = GameObject.FindGameObjectWithTag("Grapper"); // Grappling Hook
                    //Vector3 direction = (hit.point - temp.transform.position).normalized; // get the direction to shoot
                    directionToMove = (hit.point - temp.transform.position).normalized;
                    temp.GetComponent<Rigidbody>().isKinematic = false; // turn off kinematics on the grapper
                   // temp.GetComponent<Rigidbody>().AddForce(direction * forceToPush); // add force in order to shoot
                    temp.GetComponent<Rigidbody>().AddForce(directionToMove * forceToPush); // add force in order to shoot


                }
                Debug.Log("One Press");
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
                Debug.Log(tempEnemy);
                if (tempEnemy.gameObject.tag == "GrapplingLocation")
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, temp.gameObject.transform.position, 7 * Time.deltaTime);
                    var dist = Vector3.Distance(temp.gameObject.transform.position, this.gameObject.transform.position);
                    if(dist == 0)
                    {
                        temp.gameObject.transform.DetachChildren();
                        temp.gameObject.transform.parent = this.transform;
                    }
                }
                else
                {
                    temp.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime); // since we attached a collided object to our grappling hook...move grappling hook back
                }
                if (temp.transform.position == initialPosition)
                {
                    temp.transform.parent = this.transform;
                    GrapplingSettings();
                    tempEnemy.transform.parent = null;
                    grapplingPart.Shoot = false;
                }
                Debug.Log("Button is Hodling");
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
            GameObject tempEnemy = grapplingPart.ColliderObject;

            tempEnemy.transform.parent = null;
        }


        temp.transform.position = Vector3.MoveTowards(temp.transform.position, initialPosition, 7 * Time.deltaTime);
        if (temp.transform.position == initialPosition)
        {
            GrapplingSettings();
            detach = false;
            grapplingPart.Shoot = false;
            initialPosition = this.transform.position + new Vector3(0, 0, 1);
        }
    }

    private void GrapplingSettings()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Grapper");
        temp.transform.rotation = initialRotation;
       // temp.transform.localScale = initialScaling;
    }

}


/*
            if (hit.transform.tag == "GrapplingLocation")
            {
                hook.GetComponent<Rigidbody>().isKinematic = true; // turn off kinnemetic
                Vector3 tempDist = hook.transform.position - hit.point; // get the temp position
                directionOfTheSwing = tempDist.normalized; // get the direction of the swing
                line.enabled = true; // enable line rendere
                line.SetPosition(0, hook.transform.position); // set its posistion
                line.SetPosition(1, hit.point); // set another position
                // Debug.Log(grapplingHook.GetComponent<Transform>().position);
                hook.GetComponent<HingeJoint>().connectedBody = hit.rigidbody; // add connection
                hook.GetComponent<HingeJoint>().anchor = hit.point; // attach joint
                hook.GetComponent<HingeJoint>().axis = directionOfTheSwing; swint
                hook.transform.position = Vector3.MoveTowards(hook.transform.position, hit.point, 7 * Time.deltaTime); // move towards

            }
            else
            {
                hook.GetComponent<Rigidbody>().isKinematic = true;
                line.enabled = false;
                hook.transform.position = Vector3.MoveTowards(hook.transform.position, initialPosition, 3 * Time.deltaTime);
                
            }
*/