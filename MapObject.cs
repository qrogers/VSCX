using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour {

    protected Vector3 position;
    protected Vector3 coordinates;

    protected virtual void Start() {
        transform.position = position;
    }

    public int GetX() {
        return (int)coordinates.x;
    }

    public int GetY() {
        return (int)coordinates.y;
    }

    public int GetZ() {
        return (int)coordinates.z;
    }
}
