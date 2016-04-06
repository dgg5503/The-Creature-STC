﻿using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Joint name dicitionary
    // Will be expanded if new joints are added
    //[SerializeField]
    public static Dictionary<int, string> jointTypeDict;

    void Awake()
    { 
        // Have bparts ignore character colliders
        Physics.IgnoreLayerCollision(9, 10);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
