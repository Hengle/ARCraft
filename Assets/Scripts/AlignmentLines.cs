using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for the alignment lines, which intersect at the Cursor3D's position
public class AlignmentLines : MonoBehaviour {

    public GameObject lineX, lineY, lineZ;

    public Vector3 sizes = new Vector3(0.2f, 0.2f, 0.2f);

	// Update is called once per frame
	void Update () {
        Vector3 cursorPos = Cursor3D.Position;
		if (Mathf.Abs(cursorPos.x) < sizes.x / 2 && Mathf.Abs(cursorPos.y) < sizes.y / 2 && Mathf.Abs(cursorPos.z) < sizes.z / 2) {
            lineX.transform.localPosition = new Vector3(0, cursorPos.y, cursorPos.z);
            lineY.transform.localPosition = new Vector3(cursorPos.x, 0, cursorPos.z);
            lineZ.transform.localPosition = new Vector3(cursorPos.x, cursorPos.y, 0);
        } else {
            lineX.transform.localPosition = new Vector3(1000, 0, 0);
            lineY.transform.localPosition = new Vector3(1000, 0, 0);
            lineZ.transform.localPosition = new Vector3(1000, 0, 0);
        }
	}
	
}
