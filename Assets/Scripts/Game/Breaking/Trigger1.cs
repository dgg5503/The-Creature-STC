using UnityEngine;
using System.Collections;

public class Trigger1 : MonoBehaviour {

    public string fallTagName;
    
    void Awake()
    {
        
    }

    void Update()
    {
        
    }

	void OnTriggerEnter(Collider col)
    {
        if(col.tag != fallTagName && col.tag != "Untagged")
        {
            Debug.Log(col.tag);
            GameObject[] fallGrp1 = GameObject.FindGameObjectsWithTag(fallTagName);
            foreach(GameObject obj in fallGrp1)
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                if(fallTagName == "FallGrp3" || fallTagName == "FallGrp4" || fallTagName == "FallGrp5")
                {
                    
                    Destroy(obj, 3);
                }
            }
        }
    }
}
