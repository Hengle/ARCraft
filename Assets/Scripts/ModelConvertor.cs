using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConvertor {

    public static Mesh ConvertToMesh(Model model) {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();

        Vector3 center = new Vector3(model.sizeX / 2f, model.sizeY / 2f, model.sizeZ / 2f);

        for (int i = 0; i < model.sizeX; i++) {
            for (int j = 0; j < model.sizeY; j++) {
                for (int k = 0; k < model.sizeZ; k++) {
                    Block block = model.GetBlock(i, j, k);
                    if (block != null) {
                        //right
                        Block blockr = model.GetBlock(i + 1, j, k);
                        if (blockr == null) {
                            //2 3 7 6
                            Vector3 v1 = TransformedPosition(i + 1, j, k, center);
                            Vector3 v2 = TransformedPosition(i + 1, j + 1, k, center);
                            Vector3 v3 = TransformedPosition(i + 1, j + 1, k + 1, center);
                            Vector3 v4 = TransformedPosition(i + 1, j, k + 1, center);
                            AddSquare(v1, v2, v3, v4, Vector3.right, block.color, vertices, triangles, normals, colors);
                        }

                        //left
                        Block blockl = model.GetBlock(i - 1, j, k);
                        if (blockl == null) {
                            //1 5 8 4
                            Vector3 v1 = TransformedPosition(i, j, k, center);
                            Vector3 v2 = TransformedPosition(i, j, k + 1, center);
                            Vector3 v3 = TransformedPosition(i, j + 1, k + 1, center);
                            Vector3 v4 = TransformedPosition(i, j + 1, k, center);
                            AddSquare(v1, v2, v3, v4, Vector3.left, block.color, vertices, triangles, normals, colors);
                        }

                        //up
                        Block blockf = model.GetBlock(i, j + 1, k);
                        if (blockf == null) {
                            //3 4 8 7
                            Vector3 v1 = TransformedPosition(i + 1, j + 1, k, center);
                            Vector3 v2 = TransformedPosition(i, j + 1, k, center);
                            Vector3 v3 = TransformedPosition(i, j + 1, k + 1, center);
                            Vector3 v4 = TransformedPosition(i + 1, j + 1, k + 1, center);
                            AddSquare(v1, v2, v3, v4, Vector3.up, block.color, vertices, triangles, normals, colors);
                        }

                        //down
                        Block blockb = model.GetBlock(i, j - 1, k);
                        if (blockb == null) {
                            // 1 2 6 5
                            Vector3 v1 = TransformedPosition(i, j, k, center);
                            Vector3 v2 = TransformedPosition(i + 1, j, k, center);
                            Vector3 v3 = TransformedPosition(i + 1, j, k + 1, center);
                            Vector3 v4 = TransformedPosition(i, j, k + 1, center);
                            AddSquare(v1, v2, v3, v4, Vector3.down, block.color, vertices, triangles, normals, colors);
                        }

                        //forward
                        Block blocku = model.GetBlock(i, j, k + 1);
                        if (blocku == null) {
                            //6 7 8 5
                            Vector3 v1 = TransformedPosition(i + 1, j, k + 1, center);
                            Vector3 v2 = TransformedPosition(i + 1, j + 1, k + 1, center);
                            Vector3 v3 = TransformedPosition(i, j + 1, k + 1, center);
                            Vector3 v4 = TransformedPosition(i, j, k + 1, center);
                            AddSquare(v1, v2, v3, v4, Vector3.forward, block.color, vertices, triangles, normals, colors);
                        }

                        //back 
                        Block blockd = model.GetBlock(i, j, k - 1);
                        if (blockd == null) {
                            //4 3 2 1
                            Vector3 v1 = TransformedPosition(i, j + 1, k, center);
                            Vector3 v2 = TransformedPosition(i + 1, j + 1, k, center);
                            Vector3 v3 = TransformedPosition(i + 1, j, k, center);
                            Vector3 v4 = TransformedPosition(i, j, k, center);
                            AddSquare(v1, v2, v3, v4, Vector3.back, block.color, vertices, triangles, normals, colors);
                        }
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.colors = colors.ToArray();
        return mesh;
    }

    private static void AddSquare(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 normal, Color color, List<Vector3> vertices, List<int> trangles, List<Vector3> normals, List<Color> colors) {
        int index = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        trangles.Add(index);
        trangles.Add(index + 1);
        trangles.Add(index + 2);
        trangles.Add(index + 2);
        trangles.Add(index + 3);
        trangles.Add(index);
    }

    private static Vector3 TransformedPosition(int x, int y, int z, Vector3 center) {
        return (new Vector3(x, y, z) - center) / 10;
    }
}


