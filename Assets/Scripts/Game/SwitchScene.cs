using UnityEngine;
using System.Collections;

public class SwitchScene : MonoBehaviour
{

    public string SceneLevel;
    private Hook hookPoint;
    void OnTriggerEnter(Collider collidedWith)
    {
        Debug.Log("Entered Trigger");
        if (GameObject.Find("HookPoint"))
        {
            GameObject.Find("HookPoint").GetComponent<Hook>().Detach = true;
            GameObject.Find("HookPoint").GetComponent<Hook>().DetachChain();
            Debug.Log("Hook is: " + hookPoint);
            Debug.Log("I Found Grappling Hook");
        }
        else
        {
            Debug.Log("I Didn't find hook");
        }
        if (collidedWith.gameObject.name == "The_Creature")
        {
            //GameObject headHealthBar = GameObject.FindGameObjectWithTag("HHB");
            //headHealthBar.SetActive(true);
            if (SceneLevel != "Start" && SceneLevel != "UI Main Menu")
            {
                DontDestroyOnLoad(GameObject.Find("GameManager"));
                DontDestroyOnLoad(GameObject.Find("The_Creature"));
                GameObject canv = GameObject.Find("Canvas");
                DontDestroyOnLoad(canv);
                //canv.SetActive(false);
                DontDestroyOnLoad(GameObject.Find("FreeLookCameraRig"));
            }



            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneLevel);
        }

    }
}
