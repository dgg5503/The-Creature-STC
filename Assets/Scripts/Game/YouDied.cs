﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class YouDied : MonoBehaviour {

    public void backToMainMenu() {
        SceneManager.LoadScene("UI Main Menu");
    }

    public void restartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      //  Debug.Log(SceneManager.GetActiveScene().name);
    }


}
