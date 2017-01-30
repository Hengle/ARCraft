using UnityEngine;
using System.Collections.Generic;

public class ModelLibrary : MonoBehaviour {

    public static List<Model> worlds;
    public static List<Model> blocks;
    public static List<GameObject> blockObjects;
    public static List<GameObject> ghostObjects;

    private static Material modelBlockMat;
    private static Material modelGhostMat;

    public GameObject basicBlockPrefab;
    public GameObject basicGhostPrefab;
    public Material modelBlockMaterial;
    public Material modelGhostMaterial;

    void Awake() {
        worlds = new List<Model>();
        blocks = new List<Model>();
        blocks.Add(null);
        blockObjects = new List<GameObject>();
        blockObjects.Add(basicBlockPrefab);
        ghostObjects = new List<GameObject>();
        ghostObjects.Add(basicGhostPrefab);
        modelBlockMat = modelBlockMaterial;
        modelGhostMat = modelGhostMaterial;
    }

    public static int AddWorld(Model model) {
        worlds.Add(model);
        return worlds.Count - 1;
    }

    public static int AddBlock(Model model) {
        Mesh mesh = ModelConvertor.ConvertToMesh(model);
        GameObject obj = new GameObject();
        obj.AddComponent<MeshFilter>().sharedMesh = mesh;
        obj.AddComponent<MeshRenderer>().sharedMaterial = modelBlockMat;
        obj.AddComponent<BoxCollider>().center = Vector3.zero;
        obj.GetComponent<BoxCollider>().size = Vector3.one;
        obj.layer = LayerMask.NameToLayer("AR background 2");
        obj.SetActive(false);
        GameObject ghost = new GameObject();
        ghost.AddComponent<MeshFilter>().sharedMesh = mesh;
        ghost.AddComponent<MeshRenderer>().sharedMaterial = modelGhostMat;
        ghost.layer = LayerMask.NameToLayer("AR background 2");
        ghost.SetActive(false);
        blocks.Add(model);
        blockObjects.Add(obj);
        ghostObjects.Add(ghost);
        return blocks.Count - 1;
    }

    public static void UpdateBlock(int index) {
        Mesh mesh = ModelConvertor.ConvertToMesh(blocks[index]);
        Mesh sharedMesh = blockObjects[index].GetComponent<MeshFilter>().sharedMesh;
        sharedMesh.vertices = mesh.vertices;
        sharedMesh.triangles = mesh.triangles;
        sharedMesh.normals = mesh.normals;
        sharedMesh.colors = mesh.colors;
    }

}
