using UnityEngine;
using System.Collections;
using System;

public class Spear : Weapon {
    // Fields
    [SerializeField]
    private ImpalePoint impalePoint = null;
    private new Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        type = RegularItemType.Weapon;
        amountOfItems = 1;
        damage = 10;

        impalePoint.AfterImpale += ImpalePoint_AfterImpale;
    }

    private void ImpalePoint_AfterImpale(Collision expectedObject)
    {
        AfterCollision();
    }
    
    // Use this for initialization
    void Start () {
        // test force
        rigidbody.AddRelativeForce(Vector3.up * 400);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void AfterCollision()
    {
        Debug.Log("After Collision!");
    }
}
