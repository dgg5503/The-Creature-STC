using UnityEngine;
using System.Collections;

public class SwitchScene : MonoBehaviour {

    public string SceneLevel;

    void OnTriggerEnter(Collider collidedWith)
    {
        if(collidedWith.tag == "Player")
        {
            //Application.LoadLevel(SceneLevel);
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneLevel);
        }
    }
}
