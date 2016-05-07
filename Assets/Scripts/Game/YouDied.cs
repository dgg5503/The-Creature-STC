using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class YouDied : MonoBehaviour {

    void Start()
    {
        this.gameObject.GetComponent<Canvas>().enabled = false;
            }

    public void backToMainMenu() {
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("The_Creature"));
        Destroy(GameObject.Find("Canvas"));
        Destroy(GameObject.Find("FreeLookCameraRig"));
        SceneManager.LoadScene("UI Main Menu");
    }

    public void restartGame() {
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("The_Creature"));
        Destroy(GameObject.Find("Canvas"));
        Destroy(GameObject.Find("FreeLookCameraRig"));
        SceneManager.LoadScene("Start");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //  Debug.Log(SceneManager.GetActiveScene().name);
    }


}
