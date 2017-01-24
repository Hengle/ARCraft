using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConvert
{
	public static Vector3[] s_peaks;
	public static Vector3[] s_triangularFacets;
	public static Color[] s_colors;
	public static Vector3[] s_normals;
	public static MeshFilter mf;
	public static Mesh mesh;




	public Model currentModel;
	public GameObject defaultBlockPrefab;

	private int cubeNum;
	private int sizeX;
	private int sizeY;
	private int sizeZ;

	private List<Vector3> peaks;
	private List<Vector4> facets;
	private List<Vector3> triangularFacets;
	private List<Vector3> normals;
	private List<Color> colors;

//	private int[,,] points;


	public ModelConvert ()
	{
		cubeNum = currentModel.blocks.Length;
		sizeX = currentModel.sizeX;
		sizeY = currentModel.sizeY;
		sizeZ = currentModel.sizeZ;

		peaks = new List<Vector3> ();
		facets = new List<Vector4> ();
		triangularFacets = new List<Vector3> ();
		normals = new List<Vector3> ();
		colors = new List<Color> ();
		mesh = new Mesh ();

	}

	public void AddPeaks(){

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


		for (int i = 0; i < sizeX*2; i++) {
			for (int j = 0; j < sizeY*2; j++) {
				for (int k = 0; k < sizeZ*2; k++) {
					int a = points [i, j, k];
					if ((a < 8)&&(a > 0)) {
						peaks.Add (new Vector3(i, j, k));
					}
				}
			}
		}
			
	}


	public void AddTriangularFacets(){
		for (int i = 0; i < sizeX; i++) {
			for (int j = 0; j < sizeY; j++) {
				for (int k = 0; k < sizeZ; k++) {
					Block block = currentModel.GetBlock(i, j, k);
//					int[] p = new int[8];
					int[] s = new int[6];
					if(block!=null){
						//right
						Block blockr = currentModel.GetBlock(i+1, j, k);
						if(blockr == null){
							//2 3 7 6
							Vector3 v1 = new Vector3 (i + 1, j, k);
							Vector3 v2 = new Vector3 (i + 1, j + 1, k);
							Vector3 v3 = new Vector3 (i + 1, j + 1, k + 1);
							Vector3 v4 = new Vector3 (i + 1, j, k + 1);
							InsertFacet(v1,v2,v3, v4,Vector3.right,i,j,k);
							s [0] = 1;
						}
						//back
						Block blockb = currentModel.GetBlock(i, j-1, k);
						if(blockb == null){
							//2 1 5 6
							Vector3 v1 = new Vector3 (i + 1, j, k);
							Vector3 v2 = new Vector3 (i , j , k);
							Vector3 v3 = new Vector3 (i , j , k + 1);
							Vector3 v4 = new Vector3 (i + 1, j, k + 1);
							InsertFacet(v1,v2,v3, v4,Vector3.back,i,j,k);
							s [1] = 1;
						}
						//left
						Block blockl = currentModel.GetBlock(i-1, j, k);
						if(blockl == null){
							//1 4 8 5
							Vector3 v1 = new Vector3 (i , j, k);
							Vector3 v2 = new Vector3 (i , j+1 , k);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i , j, k + 1);
							InsertFacet(v1,v2,v3,v4,Vector3.left,i,j,k);
							s [2] = 1;
						}
						//forward
						Block blockf = currentModel.GetBlock(i, j+1, k);
						if(blockf == null){
							//2 4 8 7
							Vector3 v1 = new Vector3 (i+1 , j+1, k);
							Vector3 v2 = new Vector3 (i , j+1 , k);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i+1 , j+1, k + 1);
							InsertFacet(v1,v2,v3,v4,Vector3.forward,i,j,k);
							s [3] = 1;
						}
						//up
						Block blocku = currentModel.GetBlock(i, j, k+1);
						if(blocku == null){
							//6 7 8 5
							Vector3 v1 = new Vector3 (i+1 , j, k+1);
							Vector3 v2 = new Vector3 (i+1 , j+1 , k+1);
							Vector3 v3 = new Vector3 (i , j+1 , k + 1);
							Vector3 v4 = new Vector3 (i , j, k + 1);
							InsertFacet(v1,v2,v3,v4,Vector3.up,i,j,k);
							s [4] = 1;
						}
						//down
						Block blockd = currentModel.GetBlock(i, j, k-1);
						if(blockd == null){
							s [5] = 1;
							//2 3 4 1
							Vector3 v1 = new Vector3 (i+1 , j, k);
							Vector3 v2 = new Vector3 (i+1 , j+1 , k);
							Vector3 v3 = new Vector3 (i , j+1 , k );
							Vector3 v4 = new Vector3 (i , j, k );
							InsertFacet(v1,v2,v3,v4,Vector3.down,i,j,k);
						}


					}
				}
			}
		}
	}


	private void InsertFacet(Vector3 v1,Vector3 v2, Vector3 v3, Vector3 v4, Vector3 nor,int xi, int y,int z){
		int index1 = peaks.FindIndex (x => x==v1);
		int index2 = peaks.FindIndex (x => x==v2);
		int index3 = peaks.FindIndex (x => x==v3);
		int index4 = peaks.FindIndex (x => x==v4);
		if (index1 < 0) {
			peaks.Add (v1);
			normals.Add (nor);
			normals.Add (nor);
			normals.Add (nor);
			index1 = peaks.Count;
		} else {
			NormalConvert (index1, nor);
		}
		if (index2 < 0) {
			peaks.Add (v2);
			normals.Add (nor);
			normals.Add (nor);
			normals.Add (nor);
			index2 = peaks.Count;
		}else {
			NormalConvert (index2, nor);
		}
		if (index3 < 0) {
			peaks.Add (v3);
			normals.Add (nor);
			normals.Add (nor);
			normals.Add (nor);
			index3 = peaks.Count;
		}else {
			NormalConvert (index3, nor);
		}
		if (index4 < 0) {
			peaks.Add (v4);
			normals.Add (nor);
			normals.Add (nor);
			normals.Add (nor);
			index4 = peaks.Count;
		}else {
			NormalConvert (index4, nor);
		}
		Vector3 tmp = new Vector3 (index1, index2, index3);
		if( triangularFacets.FindIndex(x => x==tmp) < 0){
			triangularFacets.Add (new Vector3 (index1, index2, index3));
			triangularFacets.Add (new Vector3 (index3, index4, index1));
			colors.Add (currentModel.blocks [xi, y, z].color);
			colors.Add (currentModel.blocks [xi, y, z].color);
		}
	}

	private void NormalConvert(int index, Vector3 nor){
		if (normals [index * 3] == normals [index * 3 + 1]) {
			normals [index * 3 + 1] = new Vector3 (nor.x,nor.y,nor.z);
		} else if (normals [index * 3] == normals [index * 3 + 2]) {
			normals [index * 3 + 2] = new Vector3 (nor.x,nor.y,nor.z);
		}
	}

	public void ColorConvert(Color c, int index){
		colors [index] = c;
	}



}


