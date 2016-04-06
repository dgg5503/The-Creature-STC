using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

    GameObject player;
    private bool pulled;
    private float currentLift;
    private float liftVal;
    private GameObject exitDoor;

    void Awake () {
        player = GameObject.Find("Player");
        pulled = false;
        liftVal = 4.8f / 300.0f;
        currentLift = 0.0f;
        exitDoor = GameObject.Find("Door");
    }

    void OnTriggerStay(Collider colWith)
    {
        if (colWith.tag == "Player")
        {
            if (Input.GetKeyDown("p") && player.GetComponent<CapsuleCollider>().height >= 333.5f)
            {
                pulled = true;
                
            }
        }
    }

    void Update () {
        if (currentLift < 4.8f && pulled == true)
        {
            currentLift += liftVal;
            exitDoor.transform.position += new Vector3(0.0f, liftVal, 0.0f);
        }
    }
}
