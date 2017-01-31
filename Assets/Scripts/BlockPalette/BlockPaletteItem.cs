using UnityEngine;
using System.Collections;

// This class represent a selectable block on the BlockPalette.
public class BlockPaletteItem : MonoBehaviour, Interactor3D {

    public int blockIndex;

    private int fingerId = -1;

    public void FingerDown(Touch touch, RaycastHit hit) {
        fingerId = touch.fingerId;
    }
    public void FingerUp(Touch touch, RaycastHit hit) {
        if (touch.fingerId == fingerId) {
            Workspace.instance.currentBlockIndex = blockIndex;
        }
    }
    public void FingerEnter(Touch touch, RaycastHit hit) {}
    public void FingerExit(Touch touch, RaycastHit hit) {}
}
