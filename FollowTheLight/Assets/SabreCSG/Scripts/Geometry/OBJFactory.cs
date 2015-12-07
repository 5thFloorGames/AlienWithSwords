#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sabresaurus.SabreCSG
{
	public static class OBJFactory
	{
		public static void ExportToFile (string path, List<Polygon> polygons, Material defaultMaterial)
		{
			string exportedText = ExportToString(polygons, defaultMaterial);

			// Use a StreamWriter as File.WriteAllText isn't available on Web Player even in Editor
			using (StreamWriter sw = new StreamWriter(path)) 
			{
				sw.Write(exportedText);
			}
		}

		public static string ExportToString(List<Polygon> polygons, Material defaultMaterial)
		{
			// Create polygon subsets for each material
			Dictionary<Material, List<Polygon>> polygonMaterialTable = new Dictionary<Material, List<Polygon>>();

			// Iterate through every polygon adding it to the appropiate material list
			foreach (Polygon polygon in polygons)
			{
				if(polygon.UserExcludeFromFinal)
				{
					continue;
				}

				Material material = polygon.Material;
				if(material == null)
				{
					material = defaultMaterial;
				}
				if (!polygonMaterialTable.ContainsKey(material))
				{
					polygonMaterialTable.Add(material, new List<Polygon>());
				}

				polygonMaterialTable[material].Add(polygon);
			}

			// Use a string builder as this should allow faster concatenation
			StringBuilder stringBuilder = new StringBuilder();

			OBJVertexList vertexList = new OBJVertexList();

			int positionIndexOffset = 0;
			int uvIndexOffset = 0;
			int normalIndexOffset = 0;

			int meshIndex = 1;
			// Create a separate mesh for polygons of each material so that we batch by material
			foreach (KeyValuePair<Material, List<Polygon>> polygonMaterialGroup in polygonMaterialTable)
			{
				List<List<OBJFaceVertex>> faces = new List<List<OBJFaceVertex>>(polygonMaterialGroup.Value.Count);

				// Iterate through every polygon and triangulate
				foreach (Polygon polygon in polygonMaterialGroup.Value)
				{
					List<OBJFaceVertex> faceVertices = new List<OBJFaceVertex>(polygon.Vertices.Length);

					for (int i = 0; i < polygon.Vertices.Length; i++)
					{
						OBJFaceVertex faceVertex = vertexList.AddOrGet(polygon.Vertices[i]);
						faceVertices.Add(faceVertex);
					}

					faces.Add(faceVertices);
				}

				List<Vector3> positions = vertexList.Positions;
				List<Vector2> uvs = vertexList.UVs;
				List<Vector3> normals = vertexList.Normals;

				// Start a new group for the mesh
				stringBuilder.AppendLine("g Mesh"+meshIndex);

				// Write all the positions
				stringBuilder.AppendLine("# Vertex Positions: " + (positions.Count-positionIndexOffset));

				for (int i = positionIndexOffset; i < positions.Count; i++) 
				{
					stringBuilder.AppendLine("v " + (-positions[i].x) + " " + positions[i].y + " " + positions[i].z);
				}

				// Write all the texture coordinates (UVs)
				stringBuilder.AppendLine("# Vertex UVs: " + (uvs.Count-uvIndexOffset));

				for (int i = uvIndexOffset; i < uvs.Count; i++) 
				{
					stringBuilder.AppendLine("vt " + uvs[i].x + " " + uvs[i].y);
				}

				// Write all the normals
				stringBuilder.AppendLine("# Vertex Normals: " + (normals.Count-normalIndexOffset));

				for (int i = normalIndexOffset; i < normals.Count; i++) 
				{
					stringBuilder.AppendLine("vn " + (-normals[i].x) + " " + normals[i].y + " " + normals[i].z);
				}

				// Write all the faces
				stringBuilder.AppendLine("# Faces: " + faces.Count);
				
				for (int i = 0; i < faces.Count; i++) 
				{
					stringBuilder.Append("f ");
					for (int j = faces[i].Count-1; j >=0; j--) 
					{
						stringBuilder.Append((faces[i][j].PositionIndex) + "/" + (faces[i][j].UVIndex) + "/" + (faces[i][j].NormalIndex) + " ");
					}
					stringBuilder.AppendLine();
				}

				// Add some padding between this and the next mesh
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();

				meshIndex++;

				// Update the offsets so that the next pass only writes new vertex information
				positionIndexOffset = positions.Count;
				uvIndexOffset = uvs.Count;
				normalIndexOffset = normals.Count;
			}

			return stringBuilder.ToString();
		}

	}
}
#endif