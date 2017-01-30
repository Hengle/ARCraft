using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The model represent an object made from blocks.
[Serializable]
public class Model {

    public int sizeX, sizeY, sizeZ;
    public Block[,,] blocks;
	
    public Model(int sizeX, int sizeY, int sizeZ) {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;
        blocks = new Block[sizeX, sizeY, sizeZ];
    }

    public Block GetBlock(int x, int y, int z) {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && z >= 0 && z < sizeZ) {
            return blocks[x, y, z];
        }
        return null;
    }

    public void AddBlock(int x, int y, int z, Block block) {
        blocks[x, y, z] = block;
    }

    public void RemoveBlock(int x, int y, int z) {
        if (x >= 0 && x < sizeX && y >= 0 && y < sizeY && z >= 0 && z < sizeZ) {
            blocks[x, y, z] = null;
        }
    }

    public void ClampToModelSize(int[] position) {
        if (position[0] < 0) position[0] = 0;
        if (position[0] >= sizeX) position[0] = sizeX - 1;
        if (position[1] < 0) position[1] = 0;
        if (position[1] >= sizeY) position[1] = sizeY - 1;
        if (position[2] < 0) position[2] = 0;
        if (position[2] >= sizeZ) position[2] = sizeZ - 1;
    }

    public void PrepareForSerialization() {
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                for (int k = 0; k < sizeZ; k++) {
                    if (blocks[i, j, k] != null) {
                        blocks[i, j, k].PrepareForSerialization();
                    }
                }
            }
        }
    }

    public void RebuildFromDeserialization() {
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                for (int k = 0; k < sizeZ; k++) {
                    if (blocks[i, j, k] != null) {
                        if (blocks[i, j, k].blockIndex == -1) {
                            blocks[i, j, k] = null;
                        } else {
                            blocks[i, j, k].RebuildFromDeserialization();
                        }
                    }
                }
            }
        }
    }

}
