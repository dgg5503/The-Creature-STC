using UnityEngine;
using System.Collections;

public class MoveBlade : MonoBehaviour {

    float wallLow;
    float wallHigh;
    public bool moveUp;
    public bool MoveUp
    {
        get
        {
            return moveUp; 
        }
        set
        {
            moveUp = value;
        }
    }
    // Use this for initialization
    void Start () {
        wallHigh = 3.35f;
        wallLow = -1.129f;
        moveUp = true;
    }
	
    

	// Update is called once per frame
	void Update () {
        if (moveUp == true)
        {
            
            if (this.transform.position.y <= wallHigh)
            {
                this.transform.position += new Vector3(0.0f, 0.09f, 0.0f);
                
            }
            
        }
        if(moveUp == false)
        {
            if (this.transform.position.y >= wallLow)
            {
                this.transform.position += new Vector3(0.0f, -0.09f, 0.0f);
                
            }
        }

	}
}
