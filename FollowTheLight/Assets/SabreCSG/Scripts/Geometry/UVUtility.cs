#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Sabresaurus.SabreCSG
{
	public static class UVUtility
	{
		public class TransformData
		{
			public Vector2 Vector;
			public float Float1;

			public TransformData(Vector2 vector, float float1)
			{
				this.Vector = vector;
				this.Float1 = float1;
			}
		}

		public delegate Vector2 UVTransformation(Vector2 source, TransformData transformData);

		public static Vector2 RotateUV(Vector2 source, TransformData transformData)
		{
			return transformData.Vector + (source - transformData.Vector).Rotate(transformData.Float1);
		}

		public static Vector2 TranslateUV(Vector2 source, TransformData transformData)
		{
			return source + transformData.Vector;
		}
		
		public static Vector2 ScaleUV(Vector2 source, TransformData transformData)
		{
			return new Vector2(source.x * transformData.Vector.x, source.y * transformData.Vector.y);
		}
		
		public static Vector2 FlipUVX(Vector2 source, TransformData transformData)
		{
			source.x = 1-source.x;
			return source;
		}
		
		public static Vector2 FlipUVY(Vector2 source, TransformData transformData)
		{
			source.y = 1-source.y;
			return source;
		}

		public static Vector2 FlipUVXY(Vector2 source, TransformData transformData)
		{
			return new Vector2(source.y,source.x);
		}
	//
	//    public static void ScaleUVs(Polygon[] polygons, float scale)
	//    {
	//        for (int i = 0; i < polygons.Length; i++)
	//        {
	//            for (int j = 0; j < polygons[i].Vertices.Length; j++)
	//            {
	//                polygons[i].Vertices[j].UV *= scale;
	//            }
	//        }
	//    }
	//
	//    public static void FlipUVsXY(Polygon[] polygons)
	//    {
	//        for (int i = 0; i < polygons.Length; i++)
	//        {
	//            for (int j = 0; j < polygons[i].Vertices.Length; j++)
	//            {
	//                Vector2 uv = polygons[i].Vertices[j].UV;
	//                uv = new Vector2(uv.y, uv.x);
	//                polygons[i].Vertices[j].UV = uv;
	//            }
	//        }
	//    }
	//
	//    public static void FlipUVsX(Polygon[] polygons)
	//    {
	//        for (int i = 0; i < polygons.Length; i++)
	//        {
	//            for (int j = 0; j < polygons[i].Vertices.Length; j++)
	//            {
	//                Vector2 uv = polygons[i].Vertices[j].UV;
	//                uv.x = 1 - uv.x;
	//                polygons[i].Vertices[j].UV = uv;
	//            }
	//        }
	//    }
	//
	//    public static void FlipUVsY(Polygon[] polygons)
	//    {
	//        for (int i = 0; i < polygons.Length; i++)
	//        {
	//            for (int j = 0; j < polygons[i].Vertices.Length; j++)
	//            {
	//                Vector2 uv = polygons[i].Vertices[j].UV;
	//                uv.y = 1 - uv.y;
	//                polygons[i].Vertices[j].UV = uv;
	//            }
	//        }
	//    }
	}
}
#endif