using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorF
{
    //this Class is same as Math F. 
    //But will let us have even numbers on the grid, we dont want 0.01 ya dig ;)

    public static Vector2 Round(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public static Vector3 Round(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.y));
    }
}