using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConvert
{
//	public static Vector3[] s_vertices;
//	public static int[] s_triangularFacets;
//	public static Color[] s_colors;
//	public static Vector3[] s_normals;
	public static MeshFilter mf;
	public static Mesh mesh;




	public GameObject defaultBlockPrefab;
	public Vector3 center;
	public GameObject gameobject;

	private int sizeX;
	private int sizeY;
	private int sizeZ;

	private Model currentModel;

	private List<Vector3> vertices;
	private List<int> triangularFacets;
	private List<Vector3> normals;
	private List<Color> colors;


	public ModelConvert ()
	{
		vertices = new List<Vector3> ();
		triangularFacets = new List<int> ();
		normals = new List<Vector3> ();
		colors = new List<Color> ();
		mf = gameobject.GetComponent<MeshFilter> ();
		mesh = mf.mesh;
		mesh.Clear ();
	}

	public Mesh ConvertToMesh(Model model){
		currentModel = model;
		sizeX = currentModel.sizeX;
		sizeY = currentModel.sizeY;
		sizeZ = currentModel.sizeZ;
		center = new Vector3 (sizeX / 2f, sizeY / 2f, sizeZ / 2f);
		AddParameters ();
		MeshConvert ();
		return mesh;
	}

	public Vector3 getSize(){
		return new Vector3 (sizeX, sizeY, sizeZ);
	}

	public void AddParameters(){
		for (int i = 0; i < sizeX; i++) {
			for (int j = 0; j < sizeY; j++) {
				for (int k = 0; k < sizeZ; k++) {
					Block block = currentModel.GetBlock(i, j, k);
					if(block!=null){
						//right
						Block blockr = currentModel.GetBlock(i+1, j, k);
						if(blockr == null){
							//2 3 7 6
							Vector3 v1 = new Vector3 (i + 1, j, k);
							Vector3 v2 = new Vector3 (i + 1, j + 1, k);
							Vector3 v3 = new Vector3 (i + 1, j + 1, k + 1);
							Vector3 v4 = new Vector3 (i + 1, j, k + 1);
							AddVerticesNormalFacet(v1,v2,v3, v4,Vector3.right);
							AddColors (block.color);
						}
						//down
						Block blockb = currentModel.GetBlock(i, j-1, k);
						if(blockb == null){
							// 1 2 6 5
							Vector3 v1 = new Vector3 (i , j , k);
							Vector3 v2 = new Vector3 (i + 1, j, k);
							Vector3 v3 = new Vector3 (i + 1, j, k + 1);
							Vector3 v4 = new Vector3 (i , j , k + 1);

							AddVerticesNormalFacet(v1,v2,v3, v4,Vector3.down);
							AddColors (block.color);
						}
						//left
						Block blockl = currentModel.GetBlock(i-1, j, k);
						if(blockl == null){
							//1 5 8 4
							Vector3 v1 = new Vector3 (i , j, k);
							Vector3 v2 = new Vector3 (i , j, k + 1);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i , j+1 , k);

							AddVerticesNormalFacet(v1,v2,v3,v4,Vector3.left);
							AddColors (block.color);
						}
						//up
						Block blockf = currentModel.GetBlock(i, j+1, k);
						if(blockf == null){
							//3 4 8 7
							Vector3 v1 = new Vector3 (i+1 , j+1, k);
							Vector3 v2 = new Vector3 (i , j+1 , k);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i+1 , j+1, k + 1);
							AddVerticesNormalFacet(v1,v2,v3,v4,Vector3.up);
							AddColors (block.color);
						}
						//forward
						Block blocku = currentModel.GetBlock(i, j, k+1);
						if(blocku == null){
							//6 7 8 5
							Vector3 v1 = new Vector3 (i+1 , j, k+1);
							Vector3 v2 = new Vector3 (i+1 , j+1 , k+1);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i , j, k + 1);
							AddVerticesNormalFacet(v1,v2,v3,v4,Vector3.forward);						
							AddColors (block.color);
						}
						//back 
						Block blockd = currentModel.GetBlock(i, j, k-1);
						if(blockd == null){
							//4 3 2 1
							Vector3 v1 = new Vector3 (i , j+1 , k );
							Vector3 v2 = new Vector3 (i+1 , j+1 , k);
							Vector3 v3 = new Vector3 (i+1 , j, k);
							Vector3 v4 = new Vector3 (i , j, k );					
							AddVerticesNormalFacet(v1,v2,v3,v4,Vector3.back);
							AddColors (block.color);
						}


					}
				}
			}
		}
	}


	private void AddVerticesNormalFacet(Vector3 v1,Vector3 v2, Vector3 v3, Vector3 v4, Vector3 normal){
		int index = vertices.Count;
		vertices.Add (v1);
		vertices.Add (v2);
		vertices.Add (v3);
		vertices.Add (v4);

		normals.Add (normal);
		normals.Add (normal);
		normals.Add (normal);
		normals.Add (normal);

		triangularFacets.Add(index);
		triangularFacets.Add(index+1);
		triangularFacets.Add(index+2);
		triangularFacets.Add(index+2);
		triangularFacets.Add(index+3);
		triangularFacets.Add(index);

	}

	public void AddColors(Color c){
		colors.Add (c);
		colors.Add (c);
		colors.Add (c);
		colors.Add (c);
	}

	public void ColorConvert(Color c, int index){
		colors[index] = c;
	}

	public void MeshConvert(){
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangularFacets.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.colors = colors.ToArray ();
	}


	public void AddVertices(){

		int[,,] points = new int[sizeX*2,sizeY*2,sizeZ*2];

		for (int i = 0; i < sizeX; i++) {
			for (int j = 0; j < sizeY; j++) {
				for (int k = 0; k < sizeZ; k++) {
					Block block = currentModel.GetBlock(i, j, k);
					if (block != null) {
						points [i, j, k] += 1;
						points [i, j+1, k] += 1;
						points [i, j, k+1] += 1;
						points [i, j+1, k+1] += 1;
						points [i+1, j, k] += 1;
						points [i+1, j, k+1] += 1;
						points [i+1, j+1, k] += 1;
						points [i+1, j+1, k+1] += 1;
					}
				}
			}
		}

	}

}


