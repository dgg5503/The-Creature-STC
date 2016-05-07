using UnityEngine;
using System.Collections;

public class Grap : MonoBehaviour {
    private GameObject collideWith;
    private Hook hookScript;
    public GameObject ColliderObject {
        get { return collideWith; }
        set { collideWith = value; }
    }

    void Awake()
    {
        hookScript = GameObject.Find("HookPoint").GetComponent<Hook>();

    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(col.collider.name);
        if (col.gameObject.name.Contains("Villager"))
        {
            col.gameObject.transform.parent = this.transform;
            ColliderObject = col.gameObject;
            if (ColliderObject.GetComponent<Enemy>())
            {
                ColliderObject.GetComponent<Enemy>().IsHitWithGrapple(true);
            }
        }
        else if (col.gameObject.name == "GrapplingLocation")
        {

            this.transform.parent = col.gameObject.transform.parent;
            ColliderObject = col.gameObject;
        }
        else
        {
            hookScript.Detach = true;
        }
        GetComponent<Rigidbody>().isKinematic = true;

    }
}
