using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workspace : MonoBehaviour {

    public static Workspace instance;
    public static Vector3 Sizes {
        get { return new Vector3(0.2f, 0.2f, 0.2f); }
    }
    public static Model currentModel;

    public static bool WithinWorkspace(Vector3 position) {
        return Mathf.Abs(position.x) < Sizes.x / 2 && Mathf.Abs(position.y) < Sizes.y / 2 && Mathf.Abs(position.z) < Sizes.z / 2;
    }


    public ModelContainer modelContainer;
    public GameObject defaultBlockPrefab;
    public GameObject defaultGhostPrefab;
    public GameObject currentBlockPrefab;
    public GameObject currentGhostPrefab;

	// Use this for initialization
	void Awake () {
        instance = this;
        currentModel = new Model(10, 10, 10);
        currentBlockPrefab = defaultBlockPrefab;
        currentGhostPrefab = defaultGhostPrefab;
    }

    void Start() {
        modelContainer.LoadModel(currentModel);
    }
	
	// Update is called once per frame
	void Update () {
        modelContainer.RemoveAllGhostBlocks();
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.AddGhostBlock(gridPosition[0], gridPosition[1], gridPosition[2], currentGhostPrefab);
        }
    }

    public void AddAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.AddBlock(gridPosition[0], gridPosition[1], gridPosition[2], currentBlockPrefab);
        }
    }
    
    public void RemoveAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.RemoveBlock(gridPosition[0], gridPosition[1], gridPosition[2]);
        }
    }
}
