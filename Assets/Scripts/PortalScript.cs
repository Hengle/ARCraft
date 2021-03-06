﻿using UnityEngine;
using System.Collections;

// This class controls a portal in the warehouse area, which links to a created world.
public class PortalScript : MonoBehaviour, Interactor3D {

    public int worldIndex = -1;
    public GameObject innerQuad;

    private int fingerId = -1;

    void Update() {
        // Rotating portal special effect!
        innerQuad.transform.rotation *= Quaternion.AngleAxis(90 * Time.deltaTime, Vector3.forward);
    }

    public void FingerDown(Touch touch, RaycastHit hit) {
        fingerId = touch.fingerId;
    }
    public void FingerUp(Touch touch, RaycastHit hit) {
        if (fingerId == touch.fingerId) {
            Warehouse.instance.EditWorld(worldIndex);
        }
    }
    public void FingerEnter(Touch touch, RaycastHit hit) {}
    public void FingerExit(Touch touch, RaycastHit hit) {}
}
