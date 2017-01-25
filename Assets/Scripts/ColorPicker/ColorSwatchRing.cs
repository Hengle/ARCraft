using UnityEngine;
using System.Collections;

// The color ring of the color picker. Touching it will change the hue of the color picker.
public class ColorSwatchRing : MonoBehaviour, Interactor3D {

    public float outerRadius = 0.125f;
    public float innerRadius = 0.10625f;
    public ColorPicker3D colorPicker;
    public GameObject colorRingSelection;

    public void FingerDown(Touch touch, RaycastHit hit) {
        float distance = (hit.point - transform.position).magnitude;
        if (distance > innerRadius && distance < outerRadius) {
            // The user touched on the ring, calculting hue.
            float angle = Mathf.Atan2(Vector3.Dot(hit.point - transform.position, transform.up), Vector3.Dot(hit.point - transform.position, transform.right));
            float clockwiseAngle = 90 - angle * Mathf.Rad2Deg;
            if (clockwiseAngle < 0) {
                clockwiseAngle += 360;
            }
            float hue = Mathf.Round(clockwiseAngle / 30) / 12;

            colorPicker.UpdateHue(hue);

            // Update the position of the selection highlighting sprite
            float selectionDistance = (outerRadius + innerRadius) / 2;
            clockwiseAngle = Mathf.Round(clockwiseAngle / 30) * 30;
            angle = (90 - clockwiseAngle) * Mathf.Deg2Rad;
            colorRingSelection.transform.localPosition = new Vector3(selectionDistance * Mathf.Cos(angle), selectionDistance * Mathf.Sin(angle), transform.localPosition.z - 0.001f);
            colorRingSelection.transform.localRotation = Quaternion.AngleAxis(clockwiseAngle, Vector3.back);
        }
    }

    public void FingerUp(Touch touch, RaycastHit hit) { }
    public void FingerEnter(Touch touch, RaycastHit hit) { }
    public void FingerExit(Touch touch, RaycastHit hit) { }

}
