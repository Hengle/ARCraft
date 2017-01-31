using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The basic unit - block
[Serializable]
public class Block {

    public int blockIndex;
    [NonSerialized]
    public Color color;
    [NonSerialized]
    public Quaternion rotation;

    [SerializeField]
    private float[] _color;
    [SerializeField]
    private float[] _rotation;

    public Block() {
        blockIndex = -1;
    }

    public Block(int blockIndex) {
        this.blockIndex = blockIndex;
        color = Color.white;
    }

    public Block Clone() {
        Block clone = new Block(blockIndex);
        clone.color = color;
        clone.rotation = rotation;
        return clone;
    }

    public void PrepareForSerialization() {
        _color = new float[] { color.r, color.g, color.b, color.a };
        _rotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
    }
    public void RebuildFromDeserialization() {
        color = new Color(_color[0], _color[1], _color[2], _color[3]);
        rotation = new Quaternion(_rotation[0], _rotation[1], _rotation[2], _rotation[3]);
    }
}
