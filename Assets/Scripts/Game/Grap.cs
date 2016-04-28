using UnityEngine;
using System.Collections;

public class Grap : MonoBehaviour {
    private bool shoot = false;
    private GameObject collideWith;
    private GameObject player;



    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    
    }

    public GameObject ColliderObject {
        get { return collideWith; }
        set { collideWith = value; }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Wall")
        {
            this.GetComponent<Rigidbody>().isKinematic = true;
            col.gameObject.transform.parent = this.transform;
            ColliderObject = col.gameObject;
            
        }
        else if (col.gameObject.name == "GrapplingLocation")
        {
            this.GetComponent<Rigidbody>().isKinematic = true;
          //  player.transform.parent = col.gameObject.transform.parent;
           // col.gameObject.transform.parent = this.transform;
          this.transform.parent = col.gameObject.transform.parent;
            ColliderObject = col.gameObject;
        }

        ColliderObject = col.gameObject;
    }
}
