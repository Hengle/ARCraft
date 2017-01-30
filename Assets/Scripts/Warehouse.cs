using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Warehouse : MonoBehaviour {

    public static Warehouse instance;

    public GameObject UI;
    public GameObject mainCamera;
    public GameObject addButton, editButton;
    public GameObject deleteConfirmPanel;
    public ModelContainer modelContainer;
    public GameObject portalPrefab;

    public Vector3 sizes = new Vector3(0.2f, 0.2f, 0.2f);

    private bool cursorOnModel = false;

    // Use this for initialization
    void Awake() {
        instance = this;
    }

	void Start() {
        modelContainer.LoadModel(new Model(10, 10, 10));
        for (int i = 1; i < ModelLibrary.blocks.Count; i++) {
            PlaceBlock(i);
        }
        for (int i = 0; i < ModelLibrary.worlds.Count; i++) {
            PlaceWorld(i);
        }
    }

    private Regex blockParser = new Regex(@"B(\d+)-(\d+)-(\d+)");
    // Update is called once per frame
    void Update() {
        int[] pos = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
        if (modelContainer.model.GetBlock(pos[0], pos[1], pos[2]) == null) {
            if (cursorOnModel) {
                cursorOnModel = false;
                addButton.SetActive(true);
                editButton.SetActive(false);
            }
        } else {
            if (!cursorOnModel) {
                cursorOnModel = true;
                addButton.SetActive(false);
                editButton.SetActive(true);
            }
        }

        for (int i = 0; i < Input.touchCount; i++) {
            if (TouchManager.IsValidTouch(Input.GetTouch(i))) {
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(i).position), out hit)) {
                    string name = hit.collider.gameObject.name;
                    Match match = blockParser.Match(name);
                    if (match.Success) {
                        EditBlock(modelContainer.model.GetBlock(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)).blockIndex);
                    }
                }
            }
        }
    }

    public void CreateNewBlock() {
        Workspace.instance.ToggleEditMode(Workspace.EditMode.Block);
        Workspace.instance.LoadNewModel();
        SwitchToWorkspace();
    }

    public void CreateNewWorld() {
        Workspace.instance.ToggleEditMode(Workspace.EditMode.World);
        Workspace.instance.LoadNewModel();
        SwitchToWorkspace();
    }

    public void EditBlock(int index) {
        Workspace.instance.ToggleEditMode(Workspace.EditMode.Block);
        Workspace.instance.LoadModel(ModelLibrary.blocks[index], index);
        SwitchToWorkspace();
    }

    public void EditWorld(int index) {
        Workspace.instance.ToggleEditMode(Workspace.EditMode.World);
        Workspace.instance.LoadModel(ModelLibrary.worlds[index], index);
        SwitchToWorkspace();
    }

    public void EditBlockAtCursorPosition() {
        int[] pos = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
        Block block = modelContainer.model.GetBlock(pos[0], pos[1], pos[2]);
        if (block != null) {
            EditBlock(block.blockIndex);
        }
    }

    public void BringUpDeleteComfirmPanel() {
        deleteConfirmPanel.SetActive(true);
    }

    public void HideDeleteComfirmPanel() {
        deleteConfirmPanel.SetActive(false);
    }

    public void PlaceBlock(int blockIndex) {
        int x = 1, y = 0, z = 1;
        for (y = 0; y <= 8; y += 2) {
            for (z = 1; z <= 7; z += 2) {
                for (x = 1; x <= 7; x += 2) {
                    if (modelContainer.model.GetBlock(x, y, z) == null) {
                        goto Found;
                    }
                }
            }
        }
        Found:
        modelContainer.AddBlock(x, y, z, blockIndex, Color.white, Quaternion.identity);
    }

    public void PlaceWorld(int worldIndex) {
        float x = (worldIndex % 3 - 1) * 0.06f;
        float y = (worldIndex / 3 - 1) * 0.06f;
        GameObject newPortal = Instantiate(portalPrefab);
        newPortal.transform.SetParent(transform, false);
        newPortal.transform.localPosition = new Vector3(x, y, sizes.z / 2 - 0.005f);
        newPortal.GetComponent<PortalScript>().worldIndex = worldIndex;
    }

    public void SwitchToWorkspace() {
        gameObject.SetActive(false);
        UI.SetActive(false);
        Workspace.instance.gameObject.SetActive(true);
        Workspace.instance.UI.SetActive(true);
    }
}
