using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtentions
{
    static public Vector3 XZ(this Vector3 vec)
    {
        return new Vector3(vec.x,0,vec.z);
    }
}
