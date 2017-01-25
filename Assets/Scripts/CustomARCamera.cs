using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just a small modification to the original ARCamera: move LateUpdate() to Update() so that the Cursor3D (which is fixed to the camera) is correctly displayed and positioned.
// Update orders are also specified in the setting.
public class CustomARCamera : ARCamera {

    // Update is called once per frame
    public void Update() {
        base.LateUpdate();
    }

    public new void LateUpdate() {

    }
}
