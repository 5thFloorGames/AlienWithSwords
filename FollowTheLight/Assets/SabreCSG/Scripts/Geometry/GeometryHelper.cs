#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Sabresaurus.SabreCSG
{
	public static class GeometryHelper
	{
		public static Polygon RaycastPolygons(List<Polygon> polygons, Ray ray, out float hitDistance, float polygonSkin = 0)
		{
			Polygon closestPolygon = null;
			float closestSquareDistance = float.PositiveInfinity;
			hitDistance = 0;

			if(polygons != null)
			{
				// 
				for (int i = 0; i < polygons.Count; i++) 
				{
					if(polygons[i].ExcludeFromFinal)
					{
						continue;
					}
					if(GeometryHelper.RaycastPolygon(polygons[i], ray, polygonSkin))
					{
						// Get the real hit point by testing the ray against the polygon's plane
						Plane plane = polygons[i].Plane;

						float rayDistance;
						plane.Raycast(ray, out rayDistance);
						Vector3 hitPoint = ray.GetPoint(rayDistance);

						// Find the square distance from the camera to the hit point (squares used for speed)
						float squareDistance = (ray.origin - hitPoint).sqrMagnitude;
						// If the distance is closer than the previous closest polygon, use this one.
						if(squareDistance < closestSquareDistance)
						{
							closestPolygon = polygons[i];
							closestSquareDistance = squareDistance;
							hitDistance = rayDistance;
						}
					}
				}
			}

			return closestPolygon;
		}

		public static bool RaycastPolygon(Polygon polygon, Ray ray, float polygonSkin = 0)
		{
			// TODO: This probably won't work if the ray and polygon are coplanar, but right now that's not a usecase
//			polygon.CalculatePlane();
			Plane plane = polygon.Plane;
			float distance = 0;

			// First of all find if and where the ray hit's the polygon's plane
			if(plane.Raycast(ray, out distance))
			{
				Vector3 hitPoint = ray.GetPoint(distance);

				// Now find out if the point on the polygon plane is behind each polygon edge
				for (int i = 0; i < polygon.Vertices.Length; i++) 
				{
					Vector3 point1 = polygon.Vertices[i].Position;
					Vector3 point2 = polygon.Vertices[(i+1)%polygon.Vertices.Length].Position;

					Vector3 edge = point2 - point1; // Direction from a vertex to the next
					Vector3 polygonNormal = plane.normal;

					// Cross product of the edge with the polygon's normal gives the edge's normal
					Vector3 edgeNormal = Vector3.Cross(edge, polygonNormal);

					Vector3 edgeCenter = (point1+point2) * 0.5f;

					if(polygonSkin != 0)
					{
						edgeCenter += edgeNormal.normalized * polygonSkin;
					}

					Vector3 pointToEdgeCentroid = edgeCenter - hitPoint;

					// If the point is outside an edge this will return a negative value
					if(Vector3.Dot(edgeNormal, pointToEdgeCentroid) < 0)
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
#endif