using UnityEngine;
using System.Collections.Generic;

public class BlockPalette : MonoBehaviour {

    public GameObject floor;

    public float gapWidth = 0.5f;
    public float hoverHeight = 0.3f;
    public List<GameObject> blocks;

	// Use this for initialization
	void Awake () {
        blocks = new List<GameObject>();
    }

    public void Init() {
        for (int i = 0; i < blocks.Count; i++) {
            Destroy(blocks[i]);
        }
        blocks.Clear();
        for (int i = 0; i < ModelLibrary.blockObjects.Count; i++) {
            blocks.Add(Instantiate(ModelLibrary.blockObjects[i]));
            blocks[i].SetActive(true);
            blocks[i].transform.SetParent(transform, false);
            blocks[i].transform.localPosition = new Vector3(i * (1 + gapWidth), 0.5f + hoverHeight,  0);
            blocks[i].AddComponent<BlockPaletteItem>().blockIndex = i;
        }
        floor.transform.localPosition = new Vector3((ModelLibrary.blockObjects.Count - 1) * (1 + gapWidth) / 2, 0, 0);
        floor.transform.localScale = new Vector3(ModelLibrary.blockObjects.Count * (1 + gapWidth) - gapWidth, 1, 1);
    }
}
