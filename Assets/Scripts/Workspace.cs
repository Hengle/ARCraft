using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// Representing the working area of the user (the blue box). This class also handles the main operations by the user.
public class Workspace : MonoBehaviour {
    public enum EditMode {
        Block,
        World
    }

    public enum CursorMode {
        OneByOne,
        Continue,
        Box
    }

    public enum FingerMode {
        Painting,
        Transforming,
        RotatingBlock
    }

    public static Workspace instance;

    public GameObject UI;
    public GameObject mainCamera;
    public ModelContainer modelContainer;
    public ColorPicker3D colorPicker;
    public BlockPalette blockPalette;
    public GameObject blockEditingWalls;
    public GameObject worldEditingWalls;
    public Image colorImageBlock;
    public Image colorImageBrush;
    public GameObject quitConfirmPanel;
    public ExpandableButton fingerModeButton;
    public Button blockRotationButton;
    public Text debugText;

    public Vector3 sizes = new Vector3(0.2f, 0.2f, 0.2f);
    public Color defaultBlockColor = Color.white;
    public Color defaultBrushColor = Color.black;
    public float rotationCoefficient = 0.6f;
    public float rotationThreshold = 10;
    public float rotationAnimationDuration = 0.4f;

    public Model currentModel;
    public bool isNewModel = true;
    public int currentModelIndex = -1;
    public int currentBlockIndex = 0;
    public Color currentBlockColor;
    public Color currentBrushColor;
    public EditMode editMode = EditMode.Block;
    public CursorMode cursorMode = CursorMode.OneByOne;
    public FingerMode fingerMode = FingerMode.Painting;

    private bool isAdding = false;
    private bool isRemoving = false;
    private int[] addingStartPosition;
    private int[] removingStartPosition;

    private Quaternion cumulatedRotation = Quaternion.identity;
    private bool rotated = false;
    private bool rotating = false;
    private Quaternion originalRotation, targetRotation;
    private float rotationProgress = 0;
    private Dictionary<int, Vector3> oldTouchPositions;

    // Use this for initialization
    void Awake () {
        instance = this;
        SetBlockColor(defaultBlockColor);
        SetBrushColor(defaultBrushColor);
        gameObject.SetActive(false);
    }

    void Start() {
        //LoadNewModel();
    }
	
	// Update is called once per frame
	void Update () {
        Quaternion blockRotation = (editMode == EditMode.Block ? Quaternion.identity : blockPalette.GetBlockRotation(currentBlockIndex));

        modelContainer.RemoveAllGhostBlocks();
        if (WithinWorkspace(Cursor3D.Position)) {
            // Add a transparent box at the position of the Cursor3D.
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            
            modelContainer.AddGhostBlock(gridPosition[0], gridPosition[1], gridPosition[2], 1, 1, 1, currentBlockIndex, blockRotation);
        }

        if (cursorMode == CursorMode.Continue) {
            if (isAdding) {
                AddAction();
            }
            if (isRemoving) {
                RemoveAction();
            }
        } else if (cursorMode == CursorMode.Box) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            currentModel.ClampToModelSize(gridPosition);

            if (isAdding && addingStartPosition != null) {
                int x, y, z, w, h, d;
                GetRectangle(gridPosition, addingStartPosition, out x, out y, out z, out w, out h, out d);
                modelContainer.RemoveAllGhostBlocks();
                if (currentBlockIndex == 0) {
                    modelContainer.AddGhostBlock(x, y, z, w, h, d, 0, Quaternion.identity);
                } else {
                    for (int i = x; i < x + w; i++) {
                        for (int j = y; j < y + h; j++) {
                            for (int k = z; k < z + d; k++) {
                                modelContainer.AddGhostBlock(i, j, k, 1, 1, 1, currentBlockIndex, blockRotation);
                            }
                        }
                    }
                }
            } else if (isRemoving && removingStartPosition != null) {
                int x, y, z, w, h, d;
                GetRectangle(gridPosition, removingStartPosition, out x, out y, out z, out w, out h, out d);
                modelContainer.RemoveAllGhostBlocks();
                modelContainer.AddGhostBlock(x, y, z, w, h, d, 0, Quaternion.identity);
            }
        }

        if (fingerMode == FingerMode.Painting) {
            for (int i = 0; i < Input.touchCount; i++) {
                if (TouchManager.IsValidTouch(Input.GetTouch(i))) {
                    ShootPaint(Input.GetTouch(i).position);
                }
            }
        } else if (fingerMode == FingerMode.Transforming || fingerMode == FingerMode.RotatingBlock) {
            if (Input.touchCount == 0) {
                cumulatedRotation = Quaternion.identity;
                rotated = false;
            } else if (!rotated && !rotating) {
                // My gestural rotation detection method
                float minEdge = Mathf.Min(Screen.width, Screen.height);
                float dist = (mainCamera.transform.position - transform.position).magnitude;
                float coefficient = dist / minEdge / rotationCoefficient;
                Debug.Log(coefficient);
                for (int i = 0; i < Input.touchCount; i++) {
                    Touch touch = Input.GetTouch(i);
                    if (oldTouchPositions.ContainsKey(touch.fingerId)) {
                        Vector3 p1 = new Vector3(oldTouchPositions[touch.fingerId].x - Screen.width / 2, oldTouchPositions[touch.fingerId].y - Screen.height / 2, 0) * coefficient;
                        Vector3 p1t = transform.InverseTransformPoint(mainCamera.transform.TransformPoint(p1));
                        Vector3 p2 = new Vector3(touch.position.x - Screen.width / 2, touch.position.y - Screen.height / 2, 0) * coefficient;
                        Vector3 p2t = transform.InverseTransformPoint(mainCamera.transform.TransformPoint(p2));
                        cumulatedRotation = Quaternion.FromToRotation(p1t, p2t) * cumulatedRotation;
                    }
                }

                Vector3 axis;
                float angle;
                cumulatedRotation.ToAngleAxis(out angle, out axis);
                axis *= angle;
                float angleX = Vector3.Dot(axis, Vector3.right);
                float angleY = Vector3.Dot(axis, Vector3.up);
                float angleZ = Vector3.Dot(axis, Vector3.forward);
                Debug.Log(angleX + " " + angleY + " " + angleZ);
                if (Mathf.Abs(angleX) >= rotationThreshold && Mathf.Abs(angleX) >= Mathf.Abs(angleY) && Mathf.Abs(angleX) >= Mathf.Abs(angleZ)) {
                    Rotate(0, angleX > 0);
                    rotated = true;
                } else if (Mathf.Abs(angleY) >= rotationThreshold && Mathf.Abs(angleY) >= Mathf.Abs(angleZ)) {
                    Rotate(1, angleY > 0);
                    rotated = true;
                } else if (Mathf.Abs(angleZ) >= rotationThreshold) {
                    Rotate(2, angleZ > 0);
                    rotated = true;
                }
            }
        }

        if (rotating) {
            // Rotation animation
            rotationProgress += Time.deltaTime / rotationAnimationDuration;
            modelContainer.transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, rotationProgress);
            if (rotationProgress >= 1) {
                rotating = false;
            }
        }

        // Keep track of the finger positions in the last frame
        oldTouchPositions = new Dictionary<int, Vector3>();
        for (int i = 0; i < Input.touchCount; i++) {
            oldTouchPositions[Input.GetTouch(i).fingerId] = Input.GetTouch(i).position;
        }
    }

    public bool WithinWorkspace(Vector3 position) {
        return Mathf.Abs(position.x) < sizes.x / 2 && Mathf.Abs(position.y) < sizes.y / 2 && Mathf.Abs(position.z) < sizes.z / 2;
    }

    public void LoadNewModel() {
        LoadModel(new Model(10, 10, 10), -1);
    }

    public void LoadModel(Model model, int modelIndex) {
        currentModel = model;
        currentModelIndex = modelIndex;
        modelContainer.LoadModel(model);
        isNewModel = modelIndex < 0;

        ResetState();
    }

    public void ResetState() {
        modelContainer.transform.rotation = Quaternion.identity;
        if (!fingerModeButton.IsStarted) {
            ToggleFingerMode(1);
        } else {
            fingerModeButton.SelectAction(1);
        }
        SetBlockColor(defaultBlockColor);
        SetBrushColor(defaultBrushColor);
        isAdding = false;
        isRemoving = false;
        rotating = false;
    }

    private Regex blockParser = new Regex(@"B(\d+)-(\d+)-(\d+)");
    public void ShootPaint(Vector2 screenPosition) {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(screenPosition), out hit)) {
            string name = hit.collider.gameObject.name;
            // Get the position of the block from its name, which is "BX-Y-Z".
            Match match = blockParser.Match(name);
            if (match.Success) {
                modelContainer.ColorBlock(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), currentBrushColor);
            }
        }
        
    }

    // Start a rotation
    public void Rotate(int axis, bool positiveAngle) {
        if (fingerMode == FingerMode.RotatingBlock) {
            blockPalette.Rotate(currentBlockIndex, axis, positiveAngle);
            return;
        }
        if (!rotating) {
            rotating = true;
            rotationProgress = 0;
            Vector3 axisVector = Vector3.zero;
            axisVector[axis] = 1;
            originalRotation = modelContainer.transform.localRotation;
            targetRotation = Quaternion.AngleAxis(positiveAngle ? 90 : -90, axisVector) * originalRotation;
        }
    }

    // Triggered when the add button is pushed down
    public void StartAddingAction() {
        isAdding = true;
        if (cursorMode == CursorMode.OneByOne || cursorMode == CursorMode.Continue) {
            AddAction();
        }
        addingStartPosition = WithinWorkspace(Cursor3D.Position) ? modelContainer.WorkspaceToGridPosition(Cursor3D.Position) : null;
    }

    // Triggered when the add button is released
    public void StopAddingAction() {
        if (cursorMode == CursorMode.Box && isAdding && addingStartPosition != null) {
            Quaternion blockRotation = (editMode == EditMode.Block ? Quaternion.identity : blockPalette.GetBlockRotation(currentBlockIndex));
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            currentModel.ClampToModelSize(gridPosition);
            int x, y, z, w, h, d;
            GetRectangle(gridPosition, addingStartPosition, out x, out y, out z, out w, out h, out d);
            for (int i = x; i < x + w; i++) {
                for (int j = y; j < y + h; j++) {
                    for (int k = z; k < z + d; k++) {
                        modelContainer.AddBlock(i, j, k, currentBlockIndex, currentBlockColor, blockRotation);
                    }
                }
            }
        }
        isAdding = false;
    }

    // Triggered when the remove button is pushed down
    public void StartRemovingAction() {
        isRemoving = true;
        if (cursorMode == CursorMode.OneByOne || cursorMode == CursorMode.Continue) {
            RemoveAction();
        }
        removingStartPosition = WithinWorkspace(Cursor3D.Position) ? modelContainer.WorkspaceToGridPosition(Cursor3D.Position) : null;
    }

    // Triggered when the remove button is released
    public void StopRemovingAction() {
        if (cursorMode == CursorMode.Box && isRemoving && removingStartPosition != null) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            currentModel.ClampToModelSize(gridPosition);
            int x, y, z, w, h, d;
            GetRectangle(gridPosition, removingStartPosition, out x, out y, out z, out w, out h, out d);
            for (int i = x; i < x + w; i++) {
                for (int j = y; j < y + h; j++) {
                    for (int k = z; k < z + d; k++) {
                        modelContainer.RemoveBlock(i, j, k);
                    }
                }
            }
        }
        isRemoving = false;
    }

    // Add a block at the cursor position
    public void AddAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            Quaternion blockRotation = (editMode == EditMode.Block ? Quaternion.identity : blockPalette.GetBlockRotation(currentBlockIndex));
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.AddBlock(gridPosition[0], gridPosition[1], gridPosition[2], currentBlockIndex, currentBlockColor, blockRotation);
        }
    }

    // Remove a block at the cursor position
    public void RemoveAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.RemoveBlock(gridPosition[0], gridPosition[1], gridPosition[2]);
        }
    }

    public void ToggleEditMode(EditMode mode) {
        editMode = mode;
        if (editMode == EditMode.Block) {
            blockPalette.gameObject.SetActive(false);
            blockEditingWalls.SetActive(true);
            worldEditingWalls.SetActive(false);
            blockRotationButton.enabled = false;
            currentBlockIndex = 0;
        } else if (editMode == EditMode.World) {
            blockPalette.gameObject.SetActive(true);
            blockPalette.Init();
            blockEditingWalls.SetActive(false);
            worldEditingWalls.SetActive(true);
            blockRotationButton.enabled = true;
        }
    }

    // Called by the button
    public void ToggleCursorMode(int index) {
        if (index == 0) {
            cursorMode = CursorMode.OneByOne;
        } else if (index == 1) {
            cursorMode = CursorMode.Continue;
        } else if (index == 2) {
            cursorMode = CursorMode.Box;
        }
    }

    // Called by the button
    public void ToggleFingerMode(int index) {
        if (index == 0) {
            fingerMode = FingerMode.Painting;
        } else if (index == 1) {
            fingerMode = FingerMode.Transforming;
        } else if (index == 2) {
            fingerMode = FingerMode.RotatingBlock;
        }
    }

    // Called by the buttons
    public void ToggleColorPicker(int type) {
        if (!colorPicker.gameObject.activeSelf) {
            colorPicker.gameObject.SetActive(true);
            colorPicker.currentType = type;
        } else if (colorPicker.currentType != type) {
            colorPicker.currentType = type;
        } else {
            colorPicker.gameObject.SetActive(false);
        }
    }

    public void SetBlockColor(Color color) {
        currentBlockColor = color;
        colorImageBlock.color = color;
    }

    public void SetBrushColor(Color color) {
        currentBrushColor = color;
        colorImageBrush.color = color;
    }

    public void SaveModel() {
        if (currentModelIndex == -1) {
            if (editMode == EditMode.Block) {
                currentModelIndex = ModelLibrary.AddBlock(currentModel);
            } else {
                currentModelIndex = ModelLibrary.AddWorld(currentModel);
            }
        } else if (editMode == EditMode.Block) {
            ModelLibrary.UpdateBlock(currentModelIndex);
        }
        ModelLibrary.SaveFile();
    }

    public void BringUpQuitConfirmPanel() {
        quitConfirmPanel.SetActive(true);
    }

    public void HideQuitConfirmPanel() {
        quitConfirmPanel.SetActive(false);
    }

    public void SwitchToWarehouse(bool save) {
        if (save && !currentModel.IsEmpty()) {
            SaveModel();
        }
        gameObject.SetActive(false);
        UI.SetActive(false);
        quitConfirmPanel.SetActive(false);
        Warehouse.instance.gameObject.SetActive(true);
        Warehouse.instance.UI.SetActive(true);
        if (isNewModel) {
            if (editMode == EditMode.Block) {
                Warehouse.instance.PlaceBlock(currentModelIndex);
            } else {
                Warehouse.instance.PlaceWorld(currentModelIndex);
            }
        }
    }

    private void GetRectangle(int[] p1, int[] p2, out int x, out int y, out int z, out int w, out int h, out int d) {
        x = Mathf.Min(p1[0], p2[0]);
        y = Mathf.Min(p1[1], p2[1]);
        z = Mathf.Min(p1[2], p2[2]);
        w = Mathf.Abs(p1[0] - p2[0]) + 1;
        h = Mathf.Abs(p1[1] - p2[1]) + 1;
        d = Mathf.Abs(p1[2] - p2[2]) + 1;
    }
}
