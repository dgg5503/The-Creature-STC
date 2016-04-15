using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallManager : MonoBehaviour {

    private GameObject[] buttonArray;

    //button[button number]WallArray[Switch Value];
    //switch1
    private GameObject[] button1WallArray1;
    private GameObject[] button2WallArray1;
    private GameObject[] button3WallArray1;
    private GameObject[] button4WallArray1;
    private GameObject[] button5WallArray1;
    //switch2
    private GameObject[] button1WallArray2;
    private GameObject[] button2WallArray2;
    private GameObject[] button3WallArray2;
    private GameObject[] button4WallArray2;
    private GameObject[] button5WallArray2;

    private Dictionary<int,GameObject[]> buttonDictionary;
    public int onSwitch;
    public int lastButtonTouched;
    // Use this for initialization
    void Start () {
        buttonArray = GameObject.FindGameObjectsWithTag("FloorPlate");
        onSwitch = 1;
        lastButtonTouched = 0;
        //********************************************
        
        //assign button values in the array
        //button 1
        button1WallArray1 = new GameObject[2];
        button1WallArray2 = new GameObject[1];
        button1WallArray1[0] = GameObject.Find("Side1"); button1WallArray1[1] = GameObject.Find("Blade2");
        button1WallArray2[0] = GameObject.Find("Blade2");
        //button 2
        button2WallArray1 = new GameObject[4];
        button2WallArray2 = new GameObject[2];
        button2WallArray1[0] = GameObject.Find("Blade4"); button2WallArray1[1] = GameObject.Find("Side4"); button2WallArray1[2] = GameObject.Find("Blade5"); button2WallArray1[3] = GameObject.Find("Side2");
        button2WallArray2[0] = GameObject.Find("Side2"); button2WallArray2[1] = GameObject.Find("Blade5");

        //button 3
        button3WallArray1 = new GameObject[2];
        button3WallArray2 = new GameObject[3];
        button3WallArray1[0] = GameObject.Find("Blade5"); button3WallArray1[1] = GameObject.Find("Blade2"); 
        button3WallArray2[0] = GameObject.Find("Blade8"); button3WallArray2[1] = GameObject.Find("Side5"); button3WallArray2[2] = GameObject.Find("Side6");

        //button 4
        button4WallArray1 = new GameObject[2];
        button4WallArray1[0] = GameObject.Find("Side6"); button4WallArray1[1] = GameObject.Find("Blade8");
        button4WallArray2 = new GameObject[1];
        button4WallArray2[0] = GameObject.Find("Side6");

        //button 5
        button5WallArray1 = new GameObject[1];
        button5WallArray2 = new GameObject[4];
        button5WallArray1[0] = GameObject.Find("Blade12");
        button5WallArray2[0] = GameObject.Find("Side5"); button5WallArray2[1] = GameObject.Find("Side3"); button5WallArray2[2] = GameObject.Find("Blade9"); button5WallArray2[3] = GameObject.Find("Blade8");


        //add array to the dictionary
        //if it is switch 2 it will add +5 to the key
        buttonDictionary = new Dictionary<int, GameObject[]>();
        
        //button 1
        buttonDictionary.Add(1, button1WallArray1);
        buttonDictionary.Add(6, button1WallArray2);

        //button 2
        buttonDictionary.Add(2, button2WallArray1);
        buttonDictionary.Add(7, button2WallArray2);

        //button 3
        buttonDictionary.Add(3, button3WallArray1);
        buttonDictionary.Add(8, button3WallArray2);

        //button 4
        buttonDictionary.Add(4, button4WallArray1);
        buttonDictionary.Add(9, button4WallArray2);

        //button 5
        buttonDictionary.Add(5, button5WallArray1);
        buttonDictionary.Add(10, button5WallArray2);
        //access arrary through dictionary key number
        //pass the array to the button to activate and deactivate walls
        //*************************************************
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ManageButtons(int buttonNum)//one is canceling the other out....
    {
        UnactivateWalls(buttonNum);
        ActivateWalls(buttonNum); 
    }

    void UnactivateWalls(int buttonNum)
    {
        foreach (GameObject notButton in buttonArray)
        {
            if (notButton.GetComponent<EnvironmentButton>().buttonNumber != buttonNum)
            {
                if (onSwitch == 1)
                {
                    notButton.GetComponent<EnvironmentButton>().UnActivate(buttonDictionary[notButton.GetComponent<EnvironmentButton>().buttonNumber]);
                }
                else if (onSwitch == 2)
                {
                    Debug.Log("Made switch 2 un");
                    Debug.Log(notButton.GetComponent<EnvironmentButton>().buttonNumber);
                    notButton.GetComponent<EnvironmentButton>().UnActivate(buttonDictionary[(notButton.GetComponent<EnvironmentButton>().buttonNumber + 5)]);
                }
            }
        }
    }

    void ActivateWalls(int buttonNum)
    {
        foreach (GameObject button in buttonArray)//some buttons share the same door so you cancel it out again if you dont do this twice atm
        {
            if (button.GetComponent<EnvironmentButton>().buttonNumber == buttonNum)
            {
                if (onSwitch == 1)
                {
                    button.GetComponent<EnvironmentButton>().Activate(buttonDictionary[button.GetComponent<EnvironmentButton>().buttonNumber]);
                }
                else if (onSwitch == 2)
                {
                    Debug.Log("Made switch 2 ac");
                    Debug.Log(button.GetComponent<EnvironmentButton>().buttonNumber);
                    button.GetComponent<EnvironmentButton>().Activate(buttonDictionary[(button.GetComponent<EnvironmentButton>().buttonNumber + 5)]);
                }

            }
        }
    }

    public void ResetWalls()
    {
        GameObject[] wallList = new GameObject[18];
        wallList = GameObject.FindGameObjectsWithTag("Blade");
        
        for(int i = 0; i < wallList.Length; i++)
        {
            wallList[i].GetComponent<MoveBlade>().MoveUp = true;
        }
    }

}
