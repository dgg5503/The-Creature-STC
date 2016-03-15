using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Holds position and rotation values.
/// </summary>
public struct PosRot
{
    public Vector3 position;
    public Quaternion rotation;

    public PosRot(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

namespace Assets.Scripts.Game
{
    class Utils
    {
    }
}
