using UnityEngine;
using System.Collections;

// Script for the color picker.
public class ColorPicker3D : MonoBehaviour {

    public int currentType; // 0:Block, 1:Brush

    public GameObject cubePrefab;
    public float cubeSize = 0.02f;
    public float gapSize = 0.01f;
    public int matrixSize = 5;

    private float currentHue = 0;
    private GameObject[,] cubeMatrix;

	// Use this for initialization
	void Start () {
        cubeMatrix = new GameObject[matrixSize, matrixSize];
        float d = (matrixSize - 1) * (cubeSize + gapSize) / 2;
        for (int i = 0; i < matrixSize; i++) {
            for (int j = 0; j < matrixSize; j++) {
                cubeMatrix[i, j] = Instantiate(cubePrefab);
                cubeMatrix[i, j].transform.localScale = Vector3.one * cubeSize;
                cubeMatrix[i, j].transform.SetParent(transform, false);
                cubeMatrix[i, j].transform.localPosition = new Vector3(i * (cubeSize + gapSize) - d, j * (cubeSize + gapSize) - d, 0);
                cubeMatrix[i, j].AddComponent<ColorSwatchCube>().colorPicker = this;
            }
        }
        UpdateHue(currentHue);
    }

	public void UpdateHue(float hue) {
        currentHue = hue;
        for (int i = 0; i < matrixSize; i++) {
            for (int j = 0; j < matrixSize; j++) {
                cubeMatrix[i, j].GetComponent<ColorSwatchCube>().SetColor(Color.HSVToRGB(currentHue, i / (matrixSize - 1f), j / (matrixSize - 1f)));
            }
        }
    }
}
