using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The basic unit - block
public class Block {

    public GameObject prefab;
    public Color color;

    public Block(GameObject prefab) {
        this.prefab = prefab;
        color = Color.white;
    }
}
