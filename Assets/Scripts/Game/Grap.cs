using UnityEngine;
using System.Collections;

public class Grap : MonoBehaviour {
    private bool shoot = false;
    private GameObject collideWith;

    public GameObject ColliderObject {
        get { return collideWith; }
        set { collideWith = value; }
    }
    public bool Shoot {
        get { return shoot;}
        set { shoot = value; }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Wall")
        {
            this.GetComponent<Rigidbody>().isKinematic = true;
            col.gameObject.transform.parent = this.transform;
            ColliderObject = col.gameObject;
        }
        shoot = true;
    }
}
