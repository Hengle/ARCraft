using UnityEngine;
using System.Collections;

public class ColorSwatchCube : MonoBehaviour, Interactor3D {

    public ColorPicker3D colorPicker;

    private Color color;
    private int lastFingerID = -1;

    public void SetColor(Color color) {
        this.color = color;
        GetComponent<MeshRenderer>().material.color = color;
    }

    public void FingerDown(Touch touch, RaycastHit hit) {
        lastFingerID = touch.fingerId;
    }

    public void FingerUp(Touch touch, RaycastHit hit) {
        if (touch.fingerId == lastFingerID) {
            if (colorPicker.currentType == 0) {
                Workspace.instance.SetBlockColor(color);
            } else {
                Workspace.instance.SetBrushColor(color);
            }
            Workspace.instance.ToggleColorPicker(colorPicker.currentType);
        }
    }

    public void FingerEnter(Touch touch, RaycastHit hit) { }
    public void FingerExit(Touch touch, RaycastHit hit) { }
}
