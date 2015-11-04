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
			File.WriteAllText(path, exportedText);
		}

		public static string ExportToString(List<Polygon> polygons, Material defaultMaterial)
		{
			// Create polygon subsets for each material
			Dictionary<Material, List<Polygon>> polygonMaterialTable = new Dictionary<Material, List<Polygon>>();
			
//			bool removeDuplicateVertices = true;

			// For now put all polygons through as the same material
			polygonMaterialTable.Add(polygons[0].Material, polygons);

			/*
			// Iterate through every polygon adding it to the appropiate material list
			foreach (Polygon polygon in polygons)
			{
				if(polygon.ExcludeFromFinal)
				{
					continue;
				}
				Material material = polygon.Material;
				if(material == null)
				{
					//					Debug.LogWarning("Material should not be null!");
					material = defaultMaterial;
				}
				if (!polygonMaterialTable.ContainsKey(material))
				{
					polygonMaterialTable.Add(material, new List<Polygon>());
				}
				
				polygonMaterialTable[material].Add(polygon);
			}*/

			// Use a string builder as this should allow faster concatenation
			StringBuilder stringBuilder = new StringBuilder();

			OBJVertexList vertexList = new OBJVertexList();
			int counter = 1;
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

				stringBuilder.AppendLine("g Mesh"+counter);

				stringBuilder.AppendLine("# Vertex Positions: " + positions.Count);

				for (int i = 0; i < positions.Count; i++) 
				{
					stringBuilder.AppendLine("v " + (-positions[i].x) + " " + positions[i].y + " " + positions[i].z);
				}

				stringBuilder.AppendLine("# Vertex UVs: " + uvs.Count);

				for (int i = 0; i < uvs.Count; i++) 
				{
					stringBuilder.AppendLine("vt " + uvs[i].x + " " + uvs[i].y);
				}

				stringBuilder.AppendLine("# Vertex Normals: " + normals.Count);

				for (int i = 0; i < normals.Count; i++) 
				{
					stringBuilder.AppendLine("vn " + (-normals[i].x) + " " + normals[i].y + " " + normals[i].z);
				}


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

				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				
				counter ++;

				vertexList.ResetForNext();
			}

			return stringBuilder.ToString();
		}

	}
}