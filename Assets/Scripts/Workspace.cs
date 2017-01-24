using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Representing the working area of the user (the blue box)
public class Workspace : MonoBehaviour {

    public enum FingerMode {
        Painting,
        Transforming
    }

    public static Workspace instance;
    public static Vector3 Sizes {
        get { return new Vector3(0.2f, 0.2f, 0.2f); }
    }
    public static Model currentModel;

    public static bool WithinWorkspace(Vector3 position) {
        return Mathf.Abs(position.x) < Sizes.x / 2 && Mathf.Abs(position.y) < Sizes.y / 2 && Mathf.Abs(position.z) < Sizes.z / 2;
    }

    public GameObject mainCamera;
    public ModelContainer modelContainer;
    public GameObject defaultBlockPrefab;
    public GameObject defaultGhostPrefab;
    public GameObject currentBlockPrefab;
    public GameObject currentGhostPrefab;
    public Color currentBrushColor = Color.black;
    public float rotationCoefficient = 0.6f;
    public float rotationThreshold = 10;
    public float rotationAnimationDuration = 0.4f;

    public FingerMode fingerMode = FingerMode.Transforming;

    private Quaternion cumulatedRotation = Quaternion.identity;
    private bool rotated = false;
    private bool rotating = false;
    private Quaternion originalRotation, targetRotation;
    private float rotationProgress = 0;
    private Dictionary<int, Vector3> oldTouchPositions;

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

        if (fingerMode == FingerMode.Painting) {
            for (int i = 0; i < Input.touchCount; i++) {
                ShootPaint(Input.GetTouch(i).position);
            }
        }

        if (fingerMode == FingerMode.Transforming) {
            if (Input.touchCount == 0) {
                cumulatedRotation = Quaternion.identity;
                rotated = false;
            } else if (!rotated && !rotating) {
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
            rotationProgress += Time.deltaTime / rotationAnimationDuration;
            modelContainer.transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, rotationProgress);
            if (rotationProgress >= 1) {
                rotating = false;
            }
        }

        oldTouchPositions = new Dictionary<int, Vector3>();
        for (int i = 0; i < Input.touchCount; i++) {
            oldTouchPositions[Input.GetTouch(i).fingerId] = Input.GetTouch(i).position;
        }
    }

    private Regex blockParser = new Regex(@"B(\d+)-(\d+)-(\d+)");
    public void ShootPaint(Vector2 screenPosition) {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.GetComponent<Camera>().ScreenPointToRay(screenPosition), out hit)) {
            string name = hit.collider.gameObject.name;
            Match match = blockParser.Match(name);
            if (match.Success) {
                modelContainer.ColorBlock(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), currentBrushColor);
            }
        }
        
    }

    public void Rotate(int axis, bool positiveAngle) {
        if (!rotating) {
            rotating = true;
            rotationProgress = 0;
            Vector3 axisVector = Vector3.zero;
            axisVector[axis] = 1;
            originalRotation = modelContainer.transform.localRotation;
            targetRotation = Quaternion.AngleAxis(positiveAngle ? 90 : -90, axisVector) * modelContainer.transform.localRotation;
        }
    }

    // Triggered when the add button is pushed
    public void AddAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.AddBlock(gridPosition[0], gridPosition[1], gridPosition[2], currentBlockPrefab);
        }
    }

    // Triggered when the remove button is pushed
    public void RemoveAction() {
        if (WithinWorkspace(Cursor3D.Position)) {
            int[] gridPosition = modelContainer.WorkspaceToGridPosition(Cursor3D.Position);
            modelContainer.RemoveBlock(gridPosition[0], gridPosition[1], gridPosition[2]);
        }
    }
}
