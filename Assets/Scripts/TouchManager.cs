﻿using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour {

    public GameObject mainCamera;

    private Dictionary<int, Interactor3D> touchedInteractor;

	// Use this for initialization
	void Start () {
        touchedInteractor = new Dictionary<int, Interactor3D>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);
            Interactor3D interactor = null;
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(touch.position), out hit)) {
                interactor = hit.collider.GetComponent<Interactor3D>();
            }
            if (touch.phase == TouchPhase.Began) {
                if (interactor != null) {
                    touchedInteractor.Add(touch.fingerId, interactor);
                    interactor.FingerDown(touch, hit);
                }

            } else if (touch.phase == TouchPhase.Moved) {
                if (interactor != touchedInteractor[touch.fingerId]) {
                    if (interactor != null) {
                        interactor.FingerEnter(touch, hit);
                    }
                    if (touchedInteractor[touch.fingerId] != null) {
                        touchedInteractor[touch.fingerId].FingerExit(touch, hit);
                    }
                    touchedInteractor[touch.fingerId] = interactor;
                }
            } else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                if (interactor != null) {
                    interactor.FingerUp(touch, hit);
                }
                touchedInteractor.Remove(touch.fingerId);
            }
        }
    }
}
