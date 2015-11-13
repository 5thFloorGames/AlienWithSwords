using UnityEngine;

namespace Sabresaurus.SabreCSG
{
	public static class MathHelper
	{
	    public static float InverseLerpNoClamp(float from, float to, float value)
	    {
	        if (from < to)
	        {
	            value -= from;
	            value /= to - from;
	            return value;
	        }
	        else
	        {
	            return 1f - (value - to) / (from - to);
	        }
	    }

	    public static Vector3 VectorInDirection(Vector3 sourceVector, Vector3 direction)
	    {
	        return direction * Vector3.Dot(sourceVector, direction);
	    }

	    public static Vector3 ClosestPointOnPlane(Vector3 point, Plane plane)
	    {
	        float signedDistance = plane.GetDistanceToPoint(point);

	        return point - plane.normal * signedDistance;
	    }

	    // From http://answers.unity3d.com/questions/344630/how-would-i-find-the-closest-vector3-point-to-a-gi.html
	    public static float DistanceToRay(Vector3 X0, Ray ray)
	    {
	        Vector3 X1 = ray.origin; // get the definition of a line from the ray
	        Vector3 X2 = ray.origin + ray.direction;
	        Vector3 X0X1 = (X0 - X1);
	        Vector3 X0X2 = (X0 - X2);

	        return (Vector3.Cross(X0X1, X0X2).magnitude / (X1 - X2).magnitude); // magic
	    }

	    // From: http://wiki.unity3d.com/index.php/3d_Math_functions
	    // Two non-parallel lines which may or may not touch each other have a point on each line which are closest
	    // to each other. This function finds those two points. If the lines are not parallel, the function 
	    // outputs true, otherwise false.
	    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	    {
	        closestPointLine1 = Vector3.zero;
	        closestPointLine2 = Vector3.zero;

	        float a = Vector3.Dot(lineVec1, lineVec1);
	        float b = Vector3.Dot(lineVec1, lineVec2);
	        float e = Vector3.Dot(lineVec2, lineVec2);

	        float d = (a * e) - (b * b);

	        //lines are not parallel
	        if (d != 0.0f)
	        {
	            Vector3 r = linePoint1 - linePoint2;
	            float c = Vector3.Dot(lineVec1, r);
	            float f = Vector3.Dot(lineVec2, r);

	            float s = (b * f - c * e) / d;
	            float t = (a * f - c * b) / d;

	            closestPointLine1 = linePoint1 + lineVec1 * Mathf.Clamp01(s);
				closestPointLine2 = linePoint2 + lineVec2 * Mathf.Clamp01(t);

	            return true;
	        }
	        else
	        {
	            return false;
	        }
	    }

		public static Vector3 ClosestPointOnLine(Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 rayStart = ray.origin;
			Vector3 rayDirection = ray.direction * 10000;

			// Outputs
			Vector3 closestPointLine1;
			Vector3 closestPointLine2;
			
			MathHelper.ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, rayStart, rayDirection, lineStart, lineEnd - lineStart);

			// Only interested in the closest point on the line (lineStart -> lineEnd), not the ray
			return closestPointLine1;
		}



	    public static float RoundFloat(float value, float gridScale)
	    {
	        float reciprocal = 1f / gridScale;
	        return gridScale * Mathf.Round(reciprocal * value);
		}
		
		public static Vector3 RoundVector3(Vector3 vector)
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
			vector.z = Mathf.Round(vector.z);
			return vector;
		}
		
		public static Vector3 RoundVector3(Vector3 vector, float gridScale)
		{
			// By dividing the source value by the scale, rounding it, then rescaling it, we calculate the rounding
			float reciprocal = 1f / gridScale;
			vector.x = gridScale * Mathf.Round(reciprocal * vector.x);
			vector.y = gridScale * Mathf.Round(reciprocal * vector.y);
			vector.z = gridScale * Mathf.Round(reciprocal * vector.z);
			return vector;
		}
		
		public static Vector2 RoundVector2(Vector3 vector)
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
			return vector;
		}
		
		public static Vector2 RoundVector2(Vector2 vector, float gridScale)
		{
			// By dividing the source value by the scale, rounding it, then rescaling it, we calculate the rounding
			float reciprocal = 1f / gridScale;
			vector.x = gridScale * Mathf.Round(reciprocal * vector.x);
			vector.y = gridScale * Mathf.Round(reciprocal * vector.y);
			return vector;
		}

	    public static Vector3 VectorAbs(Vector3 vector)
	    {
	        vector.x = Mathf.Abs(vector.x);
	        vector.y = Mathf.Abs(vector.y);
	        vector.z = Mathf.Abs(vector.z);
	        return vector;
	    }

	    public static int Wrap(int i, int range)
	    {
	        if (i < 0)
	        {
	            i = range - 1;
	        }
	        if (i >= range)
	        {
	            i = 0;
	        }
	        return i;
	    }

		public static float Wrap(float i, float range)
		{
			if (i < 0)
			{
				i = range - 1;
			}
			if (i >= range)
			{
				i = 0;
			}
			return i;
		}

		const float EPSILON_LOWER = 0.0001f;

		public static bool PlaneEqualsLooser(Plane plane1, Plane plane2)
		{
			if(
				Mathf.Abs(plane1.distance - plane2.distance) < EPSILON_LOWER
				&& Mathf.Abs(plane1.normal.x - plane2.normal.x) < EPSILON_LOWER 
				&& Mathf.Abs(plane1.normal.y - plane2.normal.y) < EPSILON_LOWER 
				&& Mathf.Abs(plane1.normal.z - plane2.normal.z) < EPSILON_LOWER)
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