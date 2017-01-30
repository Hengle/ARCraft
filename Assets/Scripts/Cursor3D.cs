using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class provides the current position of the 3D cursor in the workspace space.
public class Cursor3D : MonoBehaviour {

    private static Vector3 position;
    public static Vector3 Position {
        get { return position; }
    }

    public Transform spaceTransform;
	
	// Update is called once per frame
	void Update () {
        position = spaceTransform.InverseTransformPoint(transform.position);
	}
}
