using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The ModelContainer contains a Model and do the visualization.
public class ModelContainer : MonoBehaviour {

    public Model model;
    public GameObject[,,] blockObjects;
    public float ghostObjectExtendingDistance = 0.04f;

    private List<GameObject> ghostObjects;

    // Use this for initialization
    void Awake () {
        ghostObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int[] WorkspaceToGridPosition(Vector3 positionInWorkspace) {
        Vector3 positionInContainerSpace = transform.InverseTransformPoint(transform.parent.TransformPoint(positionInWorkspace));
        return GetGridPosition(positionInContainerSpace);
    }

    public Vector3 GridPositionToWorkspace(int x, int y, int z) {
        Vector3 positionInContainerSpace = GetVectorPosition(x, y, z);
        return transform.parent.InverseTransformPoint(transform.TransformPoint(positionInContainerSpace)); ;
    }

    private int[] GetGridPosition(Vector3 positionInContainerSpace) {
        return new int[] {
            Mathf.FloorToInt(positionInContainerSpace.x + model.sizeX / 2f),
            Mathf.FloorToInt(positionInContainerSpace.y + model.sizeY / 2f),
            Mathf.FloorToInt(positionInContainerSpace.z + model.sizeZ / 2f) };
    }

    private Vector3 GetVectorPosition(int x, int y, int z) {
        return new Vector3(x + 0.5f - model.sizeX / 2f, y + 0.5f - model.sizeY / 2f, z + 0.5f - model.sizeZ / 2f);
    }

    public void Clear() {
        model = null;
        if (blockObjects != null) {
            foreach (GameObject obj in blockObjects) {
                Destroy(obj);
            }
            blockObjects = null;
        }
        RemoveAllGhostBlocks();
    }

    public void LoadModel(Model model) {
        Clear();
        this.model = model;
        blockObjects = new GameObject[model.sizeX, model.sizeY, model.sizeZ];
        for (int x = 0; x < model.sizeX; x++) {
            for (int y = 0;y < model.sizeY; y++) {
                for (int z = 0; z < model.sizeZ; z++) {
                    Block block = model.GetBlock(x, y, z);
                    if (block != null) {
                        GameObject newBlock = Instantiate(block.prefab);
                        newBlock.transform.SetParent(transform, false);
                        newBlock.transform.localPosition = GetVectorPosition(x, y, z);
                        blockObjects[x, y, z] = newBlock;
                    }
                }
            }
        }
    }

    public void AddBlock(int x, int y, int z, GameObject blockPrefab) {
        if (model.GetBlock(x, y, z) == null) {
            model.AddBlock(x, y, z, new Block(blockPrefab));
            GameObject newBlock = Instantiate(blockPrefab);
            newBlock.name = "B" + x + "-" + y + "-" + z;
            newBlock.transform.SetParent(transform, false);
            newBlock.transform.localPosition = GetVectorPosition(x, y, z);
            blockObjects[x, y, z] = newBlock;
        }
    }

    public void RemoveBlock(int x, int y, int z) {
        if (model.GetBlock(x, y, z) != null) {
            model.RemoveBlock(x, y, z);
            Destroy(blockObjects[x, y, z]);
            blockObjects[x, y, z] = null;
        }
    }

    public void ColorBlock(int x, int y, int z, Color color) {
        if (model.GetBlock(x, y, z) != null) {
            model.GetBlock(x, y, z).color = color;
            blockObjects[x, y, z].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void AddGhostBlock(int x, int y, int z, int width, int height, int depth, GameObject blockPrefab) {
        GameObject newBlock = Instantiate(blockPrefab);
        newBlock.transform.SetParent(transform, false);
        newBlock.transform.localPosition = (GetVectorPosition(x, y, z) + GetVectorPosition(x + width - 1, y + height - 1, z + depth - 1)) / 2;
        newBlock.transform.localScale = new Vector3(width, height, depth) + 2 * ghostObjectExtendingDistance * Vector3.one;
        ghostObjects.Add(newBlock);
    }

    public void RemoveAllGhostBlocks() {
        foreach (GameObject obj in ghostObjects) {
            Destroy(obj);
        }
        ghostObjects.Clear();
    }
}
