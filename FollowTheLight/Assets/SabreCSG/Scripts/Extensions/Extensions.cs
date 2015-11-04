using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Sabresaurus.SabreCSG
{
	public static class Extensions
	{
		public static Vector3 Abs(this Vector3 a)
	    {
	        return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
	    }

	    public static Vector3 Multiply(this Vector3 a, Vector3 b)
	    {
	        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	    }

		public static Vector3 Divide(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector2 Multiply(this Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}
		
		public static Vector2 Divide(this Vector2 a, Vector2 b)
		{
			return new Vector2(a.x / b.x, a.y / b.y);
		}

		public static bool HasComponent<T>(this MonoBehaviour behaviour) where T : Component
	    {
	        return (behaviour.GetComponent<T>() != null);
	    }

	    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
	    {
	        return (gameObject.GetComponent<T>() != null);
	    }

		public static T AddOrGetComponent<T>(this MonoBehaviour behaviour) where T : Component
		{
			T component = behaviour.GetComponent<T>();
			if(component != null)
			{
				return component;
			}
			else
			{
				return behaviour.gameObject.AddComponent<T>();
			}
		}

		public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T>();
			if(component != null)
			{
				return component;
			}
			else
			{
				return gameObject.AddComponent<T>();
			}
		}

		public static Vector2 Rotate(this Vector2 vector, float angle)
		{
			angle *= Mathf.Deg2Rad;
			return new Vector2( vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
								vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle));
		}

//		public static GameObject Duplicate(this GameObject sourceObject)
//		{
//			GameObject duplicate = GameObject.Instantiate(sourceObject) as GameObject;
//			duplicate.transform.parent = sourceObject.transform.parent;
//			duplicate.name = sourceObject.name;
//			return duplicate;
//		}

		public static float GetSmallestExtent(this Bounds bounds)
		{
			if(bounds.extents.x < bounds.extents.y && bounds.extents.x < bounds.extents.z)
			{
				return bounds.extents.x;
			}
			else if(bounds.extents.y < bounds.extents.x && bounds.extents.y < bounds.extents.z)
			{
				return bounds.extents.y;
			}
			else
			{
				return bounds.extents.z;
			}
		}

		public static float GetLargestExtent(this Bounds bounds)
		{
			if(bounds.extents.x > bounds.extents.y && bounds.extents.x > bounds.extents.z)
			{
				return bounds.extents.x;
			}
			else if(bounds.extents.y > bounds.extents.x && bounds.extents.y > bounds.extents.z)
			{
				return bounds.extents.y;
			}
			else
			{
				return bounds.extents.z;
			}
		}

		public static bool Equals(this Color32 color, Color32 other)
		{
			if(color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool NotEquals(this Color32 color, Color32 other)
		{
			if(color.r != other.r || color.g != other.g || color.b != other.b || color.a != other.a)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void DestroyChildrenImmediate(this Transform parentTransform)
		{
			int childCount = parentTransform.childCount;
			for (int i = 0; i < childCount; i++) 
			{
				GameObject.DestroyImmediate(parentTransform.GetChild(0).gameObject);
				
	//			GameObject.DestroyImmediate(parentTransform.GetChild(i).gameObject);
			}
		}

		public static string ToStringLong(this Vector3 source)
		{
			return string.Format("{0},{1},{2}", source.x,source.y,source.z);
		}

		public static string ToStringWithSuffix(this int number, string suffixSingular, string suffixPlural)
		{
			if(number == 1)
			{
				return number + suffixSingular;
			}
			else
			{
				return number + suffixPlural;
			}
		}
	}
}