using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.SceneManagement;

// The ModelLibrary is where all the block and world models are saved. It also builds the corresponding GameObjects of blocks to be put in the scene.
// The ModelLibrary supports save & load on local file system.
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

        LoadFile();
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

    public static void UpdateWorld(int index, Model newModel) {
        worlds[index] = newModel;
    }

    public static void UpdateBlock(int index, Model newModel) {
        blocks[index] = newModel;
        Mesh mesh = ModelConvertor.ConvertToMesh(blocks[index]);
        Mesh sharedMesh = blockObjects[index].GetComponent<MeshFilter>().sharedMesh;
        sharedMesh.Clear();
        sharedMesh.vertices = mesh.vertices;
        sharedMesh.triangles = mesh.triangles;
        sharedMesh.normals = mesh.normals;
        sharedMesh.colors = mesh.colors;
    }

    public static void SaveFile() {
        for (int i = 1; i < blocks.Count; i++) {
            blocks[i].PrepareForSerialization();
        }
        for (int i = 0; i < worlds.Count; i++) {
            worlds[i].PrepareForSerialization();
        }

        ModelLibrarySave save = new ModelLibrarySave();
        save.worlds = worlds;
        save.blocks = blocks;

        string path = Application.persistentDataPath + "/save.mls";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Create(path);
        formatter.Serialize(saveFile, save);
        saveFile.Close();
    }

    public static void LoadFile() {
        string path = Application.persistentDataPath + "/save.mls";
        if (!File.Exists(path)) {
            return;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = null;
        try {
            saveFile = File.Open(path, FileMode.Open);
            ModelLibrarySave save = (ModelLibrarySave)formatter.Deserialize(saveFile);
            for (int i = 1; i < save.blocks.Count; i++) {
                save.blocks[i].RebuildFromDeserialization();
                AddBlock(save.blocks[i]);
            }
            for (int i = 0; i < save.worlds.Count; i++) {
                save.worlds[i].RebuildFromDeserialization();
                AddWorld(save.worlds[i]);
            }
        } catch (Exception e) {
            
        } finally {
            if (saveFile != null) {
                saveFile.Close();
            }
        }
    }

    public static void DeleteFile() {
        string path = Application.persistentDataPath + "/save.mls";
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    public void DeleteSaveFile() {
        DeleteFile();
        SceneManager.LoadScene("ARCraft");
    }
}

// The serializable version of the ModelLibrary.
[Serializable]
public class ModelLibrarySave {
    public List<Model> worlds;
    public List<Model> blocks;
}
