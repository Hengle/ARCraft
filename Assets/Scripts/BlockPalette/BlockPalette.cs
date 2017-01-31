using UnityEngine;
using System.Collections.Generic;

public class BlockPalette : MonoBehaviour {

    public GameObject floor;

    public float gapWidth = 0.5f;
    public float hoverHeight = 0.3f;
    public float rotationAnimationDuration = 0.4f;
    public List<GameObject> blocks;
    public List<GameObject> blocksContainer;

    private bool rotating = false;
    private float rotationProgress = 0;
    private int rotatingIndex = -1;
    private Quaternion originalRotation;
    private Quaternion targetRotation;

    void Update() {
        for (int i = 0; i < blocksContainer.Count; i++) {
            blocksContainer[i].transform.localRotation = Workspace.instance.modelContainer.transform.localRotation;
        }

        if (rotating) {
            // Rotation animation
            rotationProgress += Time.deltaTime / rotationAnimationDuration;
            blocks[rotatingIndex].transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, rotationProgress);
            if (rotationProgress >= 1) {
                rotating = false;
            }
        }
    }

    public void Init() {
        if (blocks != null) {
            for (int i = 0; i < blocks.Count; i++) {
                Destroy(blocks[i]);
                Destroy(blocksContainer[i]);
            }
        }
        blocks = new List<GameObject>();
        blocksContainer = new List<GameObject>();
        for (int i = 0; i < ModelLibrary.blockObjects.Count; i++) {
            blocksContainer.Add(new GameObject());
            blocksContainer[i].transform.SetParent(transform, false);
            blocksContainer[i].transform.localPosition = new Vector3((i % 10) * (1 + gapWidth), 0.5f + hoverHeight, - (i / 10) * (1 + gapWidth));

            blocks.Add(Instantiate(ModelLibrary.blockObjects[i]));
            blocks[i].SetActive(true);
            blocks[i].transform.SetParent(blocksContainer[i].transform, false);
            blocks[i].transform.localPosition = Vector3.zero;
            blocks[i].AddComponent<BlockPaletteItem>().blockIndex = i;
        }
        floor.transform.localPosition = new Vector3((Mathf.Min(ModelLibrary.blockObjects.Count, 10) - 1) * (1 + gapWidth) / 2, 0, - ((ModelLibrary.blockObjects.Count - 1) / 10) * (1 + gapWidth) / 2);
        floor.transform.localScale = new Vector3(Mathf.Min(ModelLibrary.blockObjects.Count, 10) * (1 + gapWidth) - gapWidth, 1, ((ModelLibrary.blockObjects.Count + 9) / 10) * (1 + gapWidth) - gapWidth);
    }

    public void Rotate(int index, Quaternion rotation) {
        if (!rotating) {
            rotating = true;
            rotationProgress = 0;
            rotatingIndex = index;
            originalRotation = blocks[index].transform.localRotation;
            targetRotation = rotation * originalRotation;
        }
    }

    public Quaternion GetBlockAnimatedRotation(int blockIndex) {
        return blocks[blockIndex].transform.localRotation;
    }
    public Quaternion GetBlockRotation(int blockIndex) {
        if (rotating && rotatingIndex == blockIndex) {
            return targetRotation;
        } else {
            return blocks[blockIndex].transform.localRotation;
        }
    }
}
