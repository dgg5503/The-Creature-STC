using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Every projectile must have a collider.
/// </summary>
[RequireComponent(typeof(Collider))]

/*
    - A projectile will look for collisions until it hits something
    - Once hit, the OnCollision abstract function is called to apply actions to the projectile.
*/
public abstract class Projectile : MonoBehaviour {

    /// <summary>
    /// Get or set whether or not this projectile should be looking to respond to collision.
    /// </summary>
    protected bool IsActive { get; set; }

    protected delegate void OnCollisionEnterAction(Collision collision);
    protected event OnCollisionEnterAction onCollisionEnterAction;

    protected virtual void Awake()
    {
        IsActive = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLIDED WITH " + collision.collider.name);
        if (IsActive)
            onCollisionEnterAction(collision);
    }

    /// <summary>
    /// Reimplmenet this function to have actions occur when the projectile enters
    /// </summary>
    /// <param name="collision"></param>
    //protected abstract void OnCollisionAction(Collision collision);
}
