using UnityEngine;
using System.Collections;

public class BlockPaletteItem : MonoBehaviour, Interactor3D {

    public int blockIndex;

	public void FingerDown(Touch touch, RaycastHit hit) {
        Workspace.instance.currentBlockIndex = blockIndex;
    }
    public void FingerUp(Touch touch, RaycastHit hit) {}
    public void FingerEnter(Touch touch, RaycastHit hit) {}
    public void FingerExit(Touch touch, RaycastHit hit) {}
}
