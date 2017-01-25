using UnityEngine;
using System.Collections;

// An interface representing a touchable 3D object in the scene. 
public interface Interactor3D {

    // Function executed when the user start touching this object
    void FingerDown(Touch t, RaycastHit hit);

    // Function executed when the user release the finger above this object
    void FingerUp(Touch t, RaycastHit hit);

    // Function executed when the user drags into this object
    void FingerEnter(Touch t, RaycastHit hit);

    // Function executed when the user drags out of this object
    void FingerExit(Touch t, RaycastHit hit);

}
