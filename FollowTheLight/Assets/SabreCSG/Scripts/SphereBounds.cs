using UnityEngine;
using System.Collections;

namespace Sabresaurus.SabreCSG
{
	public struct SphereBounds
	{
		public Vector3 center;
		public float radius;

		public SphereBounds(Vector3 center)
		{
			this.center = center;
			radius = 0;
		}

		public SphereBounds(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public void Encapsulate(Vector3 point)
		{
			float distance = Vector3.Distance(point, center);

			if(distance > radius)
			{
				radius = distance;
			}
		}

		public bool Contains(Vector3 point)
		{
			// Check if the square distance between the two points is smaller than the square radius
			// (same as checking if the distance is smaller than the radius, but no sqrts needed
			if((point - center).sqrMagnitude <= (radius * radius))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}