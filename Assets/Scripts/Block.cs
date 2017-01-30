using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The basic unit - block
public class Block {

    public int blockIndex;
    public Color color;

    public Block(int blockIndex) {
        this.blockIndex = blockIndex;
        color = Color.white;
    }
}
