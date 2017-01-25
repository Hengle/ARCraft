using UnityEngine;
using System.Collections;

public interface Interactor3D {

    void FingerDown(Touch t, RaycastHit hit);

    void FingerUp(Touch t, RaycastHit hit);

    void FingerEnter(Touch t, RaycastHit hit);

    void FingerExit(Touch t, RaycastHit hit);

}
