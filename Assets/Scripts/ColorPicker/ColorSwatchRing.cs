using UnityEngine;
using System.Collections;

public class ColorSwatchRing : MonoBehaviour, Interactor3D {

    public float outerRadius = 0.125f;
    public float innerRadius = 0.10625f;
    public ColorPicker3D colorPicker;

    public void FingerDown(Touch touch, RaycastHit hit) {
        float distance = (hit.point - transform.position).magnitude;
        if (distance > innerRadius && distance < outerRadius) {
            float angle = Mathf.Atan2(Vector3.Dot(hit.point - transform.position, transform.up), Vector3.Dot(hit.point - transform.position, transform.right));
            float clockwiseAngle = 90 - angle * Mathf.Rad2Deg;
            if (clockwiseAngle < 0) {
                clockwiseAngle += 360;
            }
            float hue = Mathf.Round(clockwiseAngle / 30) / 12;
            colorPicker.UpdateHue(hue);
        }
    }

    public void FingerUp(Touch touch, RaycastHit hit) { }
    public void FingerEnter(Touch touch, RaycastHit hit) { }
    public void FingerExit(Touch touch, RaycastHit hit) { }

}
