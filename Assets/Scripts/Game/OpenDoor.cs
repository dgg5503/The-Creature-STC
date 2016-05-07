using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenDoor : MonoBehaviour {

    GameObject player;
    private bool pulled;
    private float currentLift;
    private float liftVal;
    private GameObject exitDoor;
    private float neededHeight = 2.0f;
    private bool showText = false;
    private bool showText2 = false;
    GameObject creature;

    void Awake () {
        player = GameObject.Find("The_Creature");
        pulled = false;
        liftVal = 4.8f / 200.0f;
        currentLift = 0.0f;
        exitDoor = GameObject.Find("Door");
        creature = GameObject.Find("The_Creature");
    }

    void OnTriggerStay(Collider colWith)
    {
        

        if (colWith.name == "The_Creature" && creature.GetComponent<Character>().IsJointOccupied(4) && creature.GetComponent<Character>().IsJointOccupied(8))
        {
            showText = false;
            showText2 = true;
            GameObject.Find("HelpText").GetComponent<Text>().text = "Press P to Pull";
            if (Input.GetKeyDown("p") && player.GetComponent<CapsuleCollider>().height >= neededHeight)
            {
                pulled = true;
                               
            }
        }
        else if(colWith.name == "The_Creature" && creature.GetComponent<Character>().IsJointOccupied(4) == false && creature.GetComponent<Character>().IsJointOccupied(8) == false){
            showText = true;
        }
        
    }

    void Update () {
        if (currentLift < 4.8f && pulled == true)
        {
            currentLift += liftVal;
            exitDoor.transform.position += new Vector3(0.0f, liftVal, 0.0f);
        }

        if (creature.GetComponent<Character>().IsJointOccupied(4) && creature.GetComponent<Character>().IsJointOccupied(8) && showText2 == false)
        {
            GameObject.Find("HelpText").GetComponent<Text>().text = "";
        }
        else if(showText == true)
        {
            GameObject.Find("HelpText").GetComponent<Text>().text = "Not enough body parts";
        }
    }
}
