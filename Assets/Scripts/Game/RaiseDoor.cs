using UnityEngine;
using System.Collections;

public class RaiseDoor : MonoBehaviour {


    private GameObject exitDoor;
    private bool isColliding;
    private float liftVal;
    private float currentLift;

    void Awake()
    {
        exitDoor = GameObject.Find("Door");
        isColliding = false;
        liftVal = 0.85f/60.0f;
        currentLift = 0.0f;
        //Debug.Log("Start");
    }
    
    void OnTriggerStay(Collider colWith)
    {
        if(colWith.tag == "BodyPart")
        {
            //Debug.Log("stay");
            isColliding = true;
            if (currentLift < 0.85f) {
                currentLift += liftVal;
                exitDoor.transform.position += new Vector3(0.0f, liftVal, 0.0f);
            }
           
        }
    }
    void OnTriggerExit(Collider colWith)
    {
        if (colWith.tag == "BodyPart")
        {
            isColliding = false;
        }
       // Debug.Log("Left");
    }
    /*
    void OnCollisionStay(Collision colInfo)
    {
        if (colInfo.collider.tag == "BodyPart" && isDown == true)
        {
            Debug.Log("stay");
            isDown = false;
            isColliding = true;
            exitDoor.transform.position += new Vector3(0.0f, 60.0f, 0.0f);
        }
    }
    
    void OnCollisionExit(Collision colInfo)
    {
        if (colInfo.collider.tag == "BodyPart")
        {
            isColliding = false;
        }
        Debug.Log("Left");
    }
    */
    void Update()
    {
        if(isColliding == false)
        {
            //isDown = true;
            if (currentLift > 0.0f)
            {
                currentLift -= liftVal;
                exitDoor.transform.position += new Vector3(0.0f, -liftVal, 0.0f);
            }
           
        }
    }
}
