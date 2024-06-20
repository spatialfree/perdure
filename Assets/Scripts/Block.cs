using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	// remap the value for aesthetics
	public Vector2 valueRange = new Vector2(0.1f, 0.96f);
	public List<LayerData> layerData = new List<LayerData>();

	[Header("References")]
	public MeshFilter meshFilter;

	Mesh mesh;
	List<Vector3> verts = new List<Vector3>();
	List<int> tris = new List<int>();
	List<Color> colors = new List<Color>();
	int startIndex = 0;

	public void BlockMesh()
	{
		verts.Clear();
		tris.Clear();
		colors.Clear();
		mesh = new Mesh();
		meshFilter.mesh = mesh;

		for (int i = 0; i < layerData.Count; i++)
		{
			LayerMesh(i);
		}

		// No need! the visual illusion jitters them enough
		// JitterVerts();

		mesh.vertices = verts.ToArray(); 
		mesh.triangles = tris.ToArray();
		mesh.colors = colors.ToArray();
	}

	private void LayerMesh(int layer)
	{
		// set layer color;
		for (int c = 0; c < 8; c++)
		{
			float value = 1 - layerData[layer].value;
			value *= value;
			colors.Add(Color.white * Mathf.Lerp(valueRange.x, valueRange.y, 1 - value));
		}
		// shade opposite sides of a layer the same way everytime to simulate lighting

		float y = -0.5f; // bottom
		y += ((float)layer + 1) / (float)layerData.Count; // move up based on layer

		layerData[layer].posY = y - ((1 / (float)layerData.Count) / 2);

		Vector3 zero = new Vector3(-0.5f, y, -0.5f); // 0 
		Vector3 drawPos = zero;

		for (int t = 0; t < 2; t++)
		{
			verts.Add(drawPos);
			drawPos += Vector3.right;
			verts.Add(drawPos);
			drawPos += Vector3.forward;
			verts.Add(drawPos);
			drawPos += Vector3.left;
			verts.Add(drawPos);

			drawPos += Vector3.back;
			drawPos += Vector3.down / (float)layerData.Count;
		}
		
		for (int side = 0; side < 4; side++)
		{
			int startVert = 8 * layer;

			tris.Add(side + startVert);
			tris.Add(vertIndex(side + 1) + startVert);
			tris.Add(vertIndex(side + 1) + 4 + startVert);

			tris.Add(side + startVert);
			tris.Add(vertIndex(side + 1) + 4 + startVert);
			tris.Add(side + 4 + startVert);
		}
	}

	int vertIndex (int vert)
	{
		// (0, 1, 2, 3, 0, 1, 2, 3, 0) etc
		int index = 0;
		for (int i = 0; i < vert; i++)
		{
			index++;
			if (index > 3)
				index = 0;
		}

		return index;
	}

	private void JitterVerts ()
	{
		List<Vector3> jitterHitList = new List<Vector3>();

		for (int j = 0; j < verts.Count; j++)
		{
			Vector3 jitter = new Vector3(-0.5f + Random.value, -0.5f + Random.value, -0.5f + Random.value) * (0.1f / (float)layerData.Count);
			List<int> indexes = new List<int>();
			startIndex = 0;
			
			Vector3 randVertex = verts[Random.Range(0, verts.Count)];
			
			if (jitterHitList.Contains(randVertex))
				return;

			jitterHitList.Add(randVertex);

			for (int i = 0; i < 10; i++)
			{
				startIndex = verts.IndexOf(randVertex, startIndex + 1);

				if (startIndex < 0)
				{
					break;
				}

				indexes.Add(startIndex);
			}

			foreach(int vertPos in indexes)
			{
				verts[vertPos] += jitter;
			}
		}
	}
}