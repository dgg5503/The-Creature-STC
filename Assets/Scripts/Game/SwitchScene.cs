using UnityEngine;
using System.Collections;

public class SwitchScene : MonoBehaviour {

    public string SceneLevel;

    void OnTriggerEnter(Collider collidedWith)
    {

        if (collidedWith.gameObject.name == "The_Creature")
        {
            //GameObject headHealthBar = GameObject.FindGameObjectWithTag("HHB");
            //headHealthBar.SetActive(true);
            if (SceneLevel != "Start" && SceneLevel != "UI Main Menu") { 
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
