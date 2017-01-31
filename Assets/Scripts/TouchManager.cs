using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

// This component sends touch events to all the Interactor3D objects in the scene.  
public class TouchManager : MonoBehaviour {

    public static TouchManager instance;
    public GameObject mainCamera;
    public GraphicRaycaster raycaster;

    private Dictionary<int, Interactor3D> touchedInteractor;

    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        touchedInteractor = new Dictionary<int, Interactor3D>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < Input.touchCount; i++) {
            Touch touch = Input.GetTouch(i);
            if (!IsValidTouch(touch)) {
                return;
            }

            // Check if an interactor is touched
            Interactor3D interactor = null;
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(touch.position), out hit)) {
                interactor = hit.collider.GetComponent<Interactor3D>();
            }

            // Send events
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

    public static bool IsValidTouch(Touch touch) {
        PointerEventData pointer = new PointerEventData(null);
        pointer.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();
        instance.raycaster.Raycast(pointer, results);
        return results.Count == 0;
    }
}
