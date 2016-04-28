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
        Debug.Log(col.collider.name);
        if (col.gameObject.name == "Wall")
        {
            col.gameObject.transform.parent = this.transform;
            ColliderObject = col.gameObject;

        }
        else if (col.gameObject.name == "GrapplingLocation")
        {

            this.transform.parent = col.gameObject.transform.parent;
            ColliderObject = col.gameObject;
        }

        
        this.GetComponent<Rigidbody>().isKinematic = true;
        //ColliderObject = col.gameObject;
    }
}
