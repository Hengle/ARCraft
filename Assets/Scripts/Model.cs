using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return blocks[x, y, z];
    }

    public void AddBlock(int x, int y, int z, Block block) {
        blocks[x, y, z] = block;
    }

    public void RemoveBlock(int x, int y, int z) {
        blocks[x, y, z] = null;
    }

}
