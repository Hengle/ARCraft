using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentLines : MonoBehaviour {

    public GameObject lineX, lineY, lineZ;

	// Update is called once per frame
	void Update () {
        Vector3 cursorPos = Cursor3D.Position;
		if (Workspace.WithinWorkspace(cursorPos)) {
            lineX.transform.localPosition = new Vector3(0, cursorPos.y, cursorPos.z);
            lineY.transform.localPosition = new Vector3(cursorPos.x, 0, cursorPos.z);
            lineZ.transform.localPosition = new Vector3(cursorPos.x, cursorPos.y, 0);
        } else {
            lineX.transform.localPosition = new Vector3(1000, 0, 0);
            lineY.transform.localPosition = new Vector3(1000, 0, 0);
            lineZ.transform.localPosition = new Vector3(1000, 0, 0);
        }
	}


//fgh;kjhgfddfhgjkl;


	
}
