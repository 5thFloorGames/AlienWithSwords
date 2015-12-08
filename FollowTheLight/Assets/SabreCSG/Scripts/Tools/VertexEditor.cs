#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	public class VertexEditor : Tool
	{
		const float EDGE_SCREEN_TOLERANCE = 15f;

		List<Vertex> selectedVertices = new List<Vertex>();
		bool moveInProgress = false; 

		bool isMarqueeSelection = false; // Whether the user is (or could be) dragging a marquee box

		Vector2 marqueeStart;
		Vector2 marqueeEnd;

		bool pivotNeedsReset = false;

		Dictionary<Vertex, Vector3> startPositions = new Dictionary<Vertex, Vector3>();

		public void TranslateSelectedVertices(Vector3 localDelta)
		{
			Polygon[] polygons = targetBrush.Polygons;
			// So we know which polygons need to have their normals recalculated
			List<Polygon> affectedPolygons = new List<Polygon>();

			for (int i = 0; i < polygons.Length; i++) 
			{
				Polygon polygon = polygons[i];
				
				int vertexCount = polygon.Vertices.Length;
				
				Vector3[] newPositions = new Vector3[vertexCount];
				Vector2[] newUV = new Vector2[vertexCount];
				
				for (int j = 0; j < vertexCount; j++) 
				{
					newPositions[j] = polygon.Vertices[j].Position;
					newUV[j] = polygon.Vertices[j].UV;
				}

				bool polygonAffected = false;
				
				for (int j = 0; j < vertexCount; j++) 
				{
					Vertex vertex = polygon.Vertices[j];
					if(selectedVertices.Contains(vertex))
					{
						Vector3 startPosition = startPositions[vertex];
						Vector3 newPosition = vertex.Position + localDelta;

						Vector3 accumulatedDelta = newPosition - startPosition;

						if(CurrentSettings.PositionSnappingEnabled)
						{
							float snapDistance = CurrentSettings.PositionSnapDistance;
//							newPosition = targetBrush.transform.TransformPoint(newPosition);
							accumulatedDelta = MathHelper.RoundVector3(accumulatedDelta, snapDistance);
//							newPosition = targetBrush.transform.InverseTransformPoint(newPosition);
						}

						newPosition = startPosition + accumulatedDelta;
						
						newPositions[j] = newPosition;

						newUV[j] = GetUVForPosition(polygon, newPosition);
						
						polygonAffected = true;
					}
				}
				
				if(polygonAffected)
				{
					affectedPolygons.Add(polygon);
				}
				
				// Apply all the changes to the polygon
				for (int j = 0; j < vertexCount; j++) 
				{
					Vertex vertex = polygon.Vertices[j];
					vertex.Position = newPositions[j];
					vertex.UV = newUV[j];
				}

				polygon.CalculatePlane();
			}
			
			for (int i = 0; i < affectedPolygons.Count; i++) 
			{
				affectedPolygons[i].ResetVertexNormals();
			}

			targetBrush.Invalidate();

			targetBrush.BreakTypeRelation();
		}

		
		// Calculates the new UV for the target position on the polygon based on the current UV set up
		// From: http://answers.unity3d.com/questions/383804/calculate-uv-coordinates-of-3d-point-on-plane-of-m.html
		public Vector2 GetUVForPosition(Polygon polygon, Vector3 newPosition)
		{
			// Update UVs
			Vector3 pos1 = polygon.Vertices[0].Position;
			Vector3 pos2 = polygon.Vertices[1].Position;
			Vector3 pos3 = polygon.Vertices[2].Position;
			
			UnityEngine.Plane plane = new UnityEngine.Plane(pos1,pos2,pos3);
			Vector3 planePoint = MathHelper.ClosestPointOnPlane(newPosition, plane);
			
			Vector2 uv1 = polygon.Vertices[0].UV;
			Vector2 uv2 = polygon.Vertices[1].UV;
			Vector2 uv3 = polygon.Vertices[2].UV;
			
			// calculate vectors from point f to vertices p1, p2 and p3:
			Vector3 f1 = pos1-planePoint;
			Vector3 f2 = pos2-planePoint;
			Vector3 f3 = pos3-planePoint;
			
			// calculate the areas (parameters order is essential in this case):
			Vector3 va = Vector3.Cross(pos1-pos2, pos1-pos3); // main triangle cross product
			Vector3 va1 = Vector3.Cross(f2, f3); // p1's triangle cross product
			Vector3 va2 = Vector3.Cross(f3, f1); // p2's triangle cross product
			Vector3 va3 = Vector3.Cross(f1, f2); // p3's triangle cross product
			
			float a = va.magnitude; // main triangle area
			
			// calculate barycentric coordinates with sign:
			float a1 = va1.magnitude/a * Mathf.Sign(Vector3.Dot(va, va1));
			float a2 = va2.magnitude/a * Mathf.Sign(Vector3.Dot(va, va2));
			float a3 = va3.magnitude/a * Mathf.Sign(Vector3.Dot(va, va3));
			
			// find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
			Vector2 uv = uv1 * a1 + uv2 * a2 + uv3 * a3;
			
			return uv;
		}

		public void SnapSelectedVertices()
		{
			Polygon[] polygons = targetBrush.Polygons;
			// So we know which polygons need to have their normals recalculated
			List<Polygon> affectedPolygons = new List<Polygon>();

			for (int i = 0; i < polygons.Length; i++) 
			{
				Polygon polygon = polygons[i];
				
				int vertexCount = polygon.Vertices.Length;
				
				Vector3[] newPositions = new Vector3[vertexCount];
				Vector2[] newUV = new Vector2[vertexCount];
				
				for (int j = 0; j < vertexCount; j++) 
				{
					newPositions[j] = polygon.Vertices[j].Position;
					newUV[j] = polygon.Vertices[j].UV;
				}

				bool polygonAffected = false;
				for (int j = 0; j < vertexCount; j++) 
				{
					Vertex vertex = polygon.Vertices[j];
					if(selectedVertices.Contains(vertex))
					{
						Vector3 newPosition = vertex.Position;
						
						float snapDistance = CurrentSettings.PositionSnapDistance;
						newPosition = targetBrush.transform.TransformPoint(newPosition);
						newPosition = MathHelper.RoundVector3(newPosition, snapDistance);
						newPosition = targetBrush.transform.InverseTransformPoint(newPosition);
						
						newPositions[j] = newPosition;

						newUV[j] = GetUVForPosition(polygon, newPosition);

						polygonAffected = true;
					}
				}

				if(polygonAffected)
				{
					affectedPolygons.Add(polygon);
				}
				
				// Apply all the changes to the polygon
				for (int j = 0; j < vertexCount; j++) 
				{
					Vertex vertex = polygon.Vertices[j];
					vertex.Position = newPositions[j];
					vertex.UV = newUV[j];
				}

				polygon.CalculatePlane();
			}

			for (int i = 0; i < affectedPolygons.Count; i++) 
			{
				affectedPolygons[i].ResetVertexNormals();
			}

			targetBrush.Invalidate();

			targetBrush.BreakTypeRelation();
		}

		public bool AnySelected
		{
			get
			{
				if(selectedVertices.Count > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		// Used so that the gizmo for moving the points is positioned at the average between the selection
		public Vector3 GetSelectedCenter()
		{
			Vector3 average = Vector3.zero;
			int numberFound = 0;
			
			for (int i = 0; i < selectedVertices.Count; i++) 
			{
				average += selectedVertices[i].Position;
				numberFound++;
			}
			
			if(numberFound > 0)
			{
				return average / numberFound;
			}
			else
			{
				return Vector3.zero;
			}
		}

		public override void ResetTool ()
		{
			selectedVertices.Clear();
		}

		public override void OnSceneGUI (UnityEditor.SceneView sceneView, Event e)
		{
			base.OnSceneGUI(sceneView, e); // Allow the base logic to calculate first

			if(targetBrush != null && AnySelected)
			{
				if(startPositions.Count == 0)
				{
					for (int i = 0; i < selectedVertices.Count; i++) 
					{
						startPositions.Add(selectedVertices[i], selectedVertices[i].Position);
					}
				}

				// Make the handle respect the Unity Editor's Local/World orientation mode
				Quaternion handleDirection = Quaternion.identity;
				if(Tools.pivotRotation == PivotRotation.Local)
				{
					handleDirection = targetBrush.transform.rotation;
				}
				
				// Grab a source point and convert from local space to world
				Vector3 sourceLocalPosition = GetSelectedCenter();
				Vector3 sourceWorldPosition = targetBrush.transform.TransformPoint(sourceLocalPosition);
				
				EditorGUI.BeginChangeCheck();
				// Display a handle and allow the user to determine a new position in world space
				Vector3 newWorldPosition = Handles.PositionHandle(sourceWorldPosition, handleDirection);
				
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(targetBrush.transform, "Moved Vertices");
					Undo.RecordObject(targetBrush, "Moved Vertices");
					// Convert the new world space position back to local space
					Vector3 newLocalPosition = targetBrush.transform.InverseTransformPoint(newWorldPosition);
					
					Vector3 deltaLocal = newLocalPosition - sourceLocalPosition;
					//				if(deltaLocal.sqrMagnitude > 0)
					//				{
					TranslateSelectedVertices(deltaLocal);
					isMarqueeSelection = false;
					moveInProgress = true;
					EditorUtility.SetDirty (targetBrush);
					e.Use();
					// Shouldn't reset the pivot while the vertices are being manipulated, so make sure the pivot
					// is set to get reset at next opportunity
					pivotNeedsReset = true;
				}
				else
				{
					// The user is no longer moving a handle
					if(pivotNeedsReset)
					{
						// Pivot needs to be reset, so reset it!
						targetBrush.ResetPivot();
						pivotNeedsReset = false;
					}

					startPositions.Clear();
				}

			}

			if(targetBrush != null)
			{
				if(!EditorHelper.IsMousePositionNearSceneGizmo(e.mousePosition))
				{
					if (e.type == EventType.MouseDown) 
					{
						OnMouseDown(sceneView, e);
					}
					else if (e.type == EventType.MouseDrag) 
					{
						OnMouseDrag(sceneView, e);
					}
					// If you mouse up on a different scene view to the one you started on it's surpressed as Ignore, when
					// doing marquee selection make sure to check the real type
					else if (e.type == EventType.MouseUp || (isMarqueeSelection && e.rawType == EventType.MouseUp))
					{
						OnMouseUp(sceneView, e);
					}
				}
			}

//			if(e.type == EventType.Repaint)
			{
				OnRepaint(sceneView, e);
			}
		}

		void OnToolbarGUI(int windowID)
		{
// DISABLED: This code isn't working very reliably yet. Disabled until it works well.
//			if (GUILayout.Button("Connect", EditorStyles.miniButton))
//			{
//				if(selectedVertices != null)
//				{
//					Undo.RecordObject(targetBrush.transform, "Connect Vertices");
//					Undo.RecordObject(targetBrush, "Connect Vertices");
//
//					Polygon[] newPolygons = VertexFactory.ConnectVertices(targetBrush.Polygons, selectedVertices);
//					
//					if(newPolygons != null)
//					{
//						targetBrush.SetPolygons(newPolygons);
//					}
//
//					selectedVertices.Clear();
//				}
//			}
//
//			if (GUILayout.Button("Weld", EditorStyles.miniButton))
//			{
//				if(selectedVertices != null)
//				{
//					Undo.RecordObject(targetBrush.transform, "Weld Vertices");
//					Undo.RecordObject(targetBrush, "Weld Vertices");
//
//					Polygon[] newPolygons = VertexFactory.WeldVertices(targetBrush.Polygons, selectedVertices);
//					
//					if(newPolygons != null)
//					{
//						targetBrush.SetPolygons(newPolygons);
//					}
//
//					selectedVertices.Clear();
//				}
//			}

			// Button should only be enabled if there are any vertices selected
			GUI.enabled = selectedVertices.Count > 0;
			if (GUILayout.Button("Snap Verts To Grid", EditorStyles.miniButton))
			{
				Undo.RecordObject(targetBrush.transform, "Snap Vertices");
				Undo.RecordObject(targetBrush, "Snap Vertices");

				SnapSelectedVertices();
			}
		}

		public void OnRepaint (SceneView sceneView, Event e)
		{
			if(isMarqueeSelection && sceneView == SceneView.lastActiveSceneView)
			{
				Vector2 point1 = EditorHelper.ConvertMousePosition(marqueeStart);
				Vector2 point2 = EditorHelper.ConvertMousePosition(marqueeEnd);

				Rect rect = new Rect(point1.x, point1.y, point2.x - point1.x, point2.y - point1.y);

				SabreGraphics.GetSelectedBrushMaterial().SetPass(0);

				GL.PushMatrix();
				GL.LoadPixelMatrix();

				GL.Begin(GL.QUADS);
				GL.Color(new Color(0.2f,0.3f,.8f, 0.3f));
				
				// Marquee fill (draw double sided)
				SabreGraphics.DrawScreenRectFill(rect);

				GL.End();
				
				// Marquee border
				GL.Begin(GL.LINES);
				GL.Color(Color.white);

				// Draw marquee box edges
				SabreGraphics.DrawScreenRectOuter(rect);
				
				GL.End();
				GL.PopMatrix();

			}

			if(targetBrush != null)
			{
				DrawVertices(sceneView, e);
			}

			// Draw UI specific to this editor
			Rect rectangle = new Rect(0, 50, 140, 140);
			GUIStyle toolbar = new GUIStyle(EditorStyles.toolbar);
			toolbar.normal.background = SabreGraphics.ClearTexture;
			toolbar.fixedHeight = rectangle.height;
			GUILayout.Window(140002, rectangle, OnToolbarGUI, "", toolbar);
		}

		void OnMouseDown (SceneView sceneView, Event e)
		{
			isMarqueeSelection = false;
			moveInProgress = false;

			marqueeStart = e.mousePosition;
		}

		void OnMouseDrag (SceneView sceneView, Event e)
		{
			if(!CameraPanInProgress)
			{
				marqueeEnd = e.mousePosition;
				if(!moveInProgress && e.button == 0)
				{
					isMarqueeSelection = true;
					sceneView.Repaint();
				}
			}
		}

		// Select any vertices
		void OnMouseUp (SceneView sceneView, Event e)
		{
			if(e.button == 0 && !CameraPanInProgress)
			{
				Transform sceneViewTransform = sceneView.camera.transform;
				Vector3 sceneViewPosition = sceneViewTransform.position;
				if(moveInProgress)
				{

				}
				else
				{
					Polygon[] polygons = targetBrush.Polygons;

					if(isMarqueeSelection) // Marquee vertex selection
					{
						isMarqueeSelection = false;
						
						marqueeEnd = e.mousePosition;
						
						Vector2 point1 = EditorHelper.ConvertMousePosition(marqueeStart);
						Vector2 point2 = EditorHelper.ConvertMousePosition(marqueeEnd);
						
						float minX = Mathf.Min(point1.x, point2.x);
						float maxX = Mathf.Max(point1.x, point2.x);
						
						float minY = Mathf.Min(point1.y, point2.y);
						float maxY = Mathf.Max(point1.y, point2.y);
						
						for (int i = 0; i < polygons.Length; i++) 
						{
							Polygon polygon = polygons[i];
							
							for (int j = 0; j < polygon.Vertices.Length; j++) 
							{
								Vertex vertex = polygon.Vertices[j];
								
								Vector3 worldPosition = targetBrush.transform.TransformPoint(vertex.Position);
								Vector3 screenPoint = sceneView.camera.WorldToScreenPoint(worldPosition);
								
								// Point is contained within marquee box
								if(screenPoint.z > 0 && 
								   screenPoint.x > minX && screenPoint.x < maxX
								   && screenPoint.y > minY && screenPoint.y < maxY)
								{
									if(EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control))
									{
										// Only when holding control should a deselection occur from a valid point
										selectedVertices.Remove(vertex);
									}
									else
									{
										// Point was in marquee (and ctrl wasn't held) so select it!
										if(!selectedVertices.Contains(vertex))
										{
											selectedVertices.Add(vertex);
										}
									}
								}
								else if(!EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control) 
								        && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift))
								{
									selectedVertices.Remove(vertex);
								}
							}
						}
					}
					else // Clicking style vertex selection
					{
						Vector2 mousePosition = e.mousePosition;


						// TODO Find the closest point
						bool clickedAnyPoints = false;
						Vertex closestVertexFound = null;
						float closestDistanceSquare = float.PositiveInfinity;

						for (int i = 0; i < polygons.Length; i++) 
						{
							Polygon polygon = polygons[i];
							
							for (int j = 0; j < polygon.Vertices.Length; j++) 
							{
								Vertex vertex = polygon.Vertices[j];
								
								Vector3 worldPosition = targetBrushTransform.TransformPoint(vertex.Position);

								float vertexDistanceSquare = (sceneViewPosition - worldPosition).sqrMagnitude;

								if(EditorHelper.InClickZone(mousePosition, worldPosition) && vertexDistanceSquare < closestDistanceSquare)
								{
									closestVertexFound = vertex;
									clickedAnyPoints = true;
									closestDistanceSquare = vertexDistanceSquare;
								}
							}
						}

						if(!EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control) && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift))
						{
							selectedVertices.Clear();
						}

						for (int i = 0; i < polygons.Length; i++) 
						{
							Polygon polygon = polygons[i];
							
							for (int j = 0; j < polygon.Vertices.Length; j++) 
							{
								Vertex vertex = polygon.Vertices[j];
								
								if(clickedAnyPoints && vertex.Position == closestVertexFound.Position)
								{
									if(EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control))
									{
										if(!selectedVertices.Contains(vertex))
										{
											selectedVertices.Add(vertex);
										}
										else
										{
											selectedVertices.Remove(vertex);
										}
									}
									else
									{
										if(!selectedVertices.Contains(vertex))
										{
											selectedVertices.Add(vertex);
										}
									}
								}
								else if(!EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control) 
								        && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift))
								{
									selectedVertices.Remove(vertex);
								}
							}
						}

						if(!clickedAnyPoints) // Couldn't click any directly, next try to click an edge
						{
//							Ray ray = Camera.current.ScreenPointToRay (EditorHelper.ConvertMousePosition(mousePosition));
//							bool clickedAnyEdges = false;
							List<Vertex> selectedEdgePoints = new List<Vertex>();
							List<Vertex> selectedPolygonPoints = new List<Vertex>();
							// Used to track the closest edge clicked, so if we could click through several edges with
							// one click, then we only count the closest
							float closestFound = float.PositiveInfinity;
//							Vector3 closestPointOnEdge = Vector3.zero;

							for (int i = 0; i < polygons.Length; i++) 
							{
								Polygon polygon = polygons[i];
								for (int j = 0; j < polygon.Vertices.Length; j++) 
								{
									Vector3 worldPoint1 = targetBrush.transform.TransformPoint(polygon.Vertices[j].Position);
									Vector3 worldPoint2 = targetBrush.transform.TransformPoint(polygon.Vertices[(j+1) % polygon.Vertices.Length].Position);

									// Distance from the mid point of the edge to the camera
									float squareDistance = (Vector3.Lerp(worldPoint1,worldPoint2,0.5f) - Camera.current.transform.position).sqrMagnitude;

									float screenDistance = HandleUtility.DistanceToLine(worldPoint1, worldPoint2);
									if(screenDistance < EDGE_SCREEN_TOLERANCE && squareDistance < closestFound)
									{
										selectedEdgePoints.Clear();
										selectedEdgePoints.Add(polygon.Vertices[j]);
										selectedEdgePoints.Add(polygon.Vertices[(j+1) % polygon.Vertices.Length]);
//										clickedAnyEdges = true;
										closestFound = squareDistance;
//										closestPointOnEdge = MathHelper.ClosestPointOnLine(ray, worldPoint1, worldPoint2);
									}
								}
							}

							List<Vertex> newSelectedVertices = new List<Vertex>();

							if(selectedEdgePoints.Count > 0)
							{
								newSelectedVertices = selectedEdgePoints;
							}
							else if(selectedPolygonPoints.Count > 0)
							{
								newSelectedVertices = selectedPolygonPoints;
							}


							// Regardless of whether we hit an edge or face
							for (int i = 0; i < polygons.Length; i++) 
							{
								Polygon polygon = polygons[i];
								
								for (int j = 0; j < polygon.Vertices.Length; j++) 
								{
									Vertex vertex = polygon.Vertices[j];
									
									for (int k = 0; k < newSelectedVertices.Count; k++) 
									{
										if(newSelectedVertices[k].Position == vertex.Position)
										{
											if(!selectedVertices.Contains(vertex))
											{
												selectedVertices.Add(vertex);
											}
											break;
										}
									}
								}
							}
						}
					}
					moveInProgress = false;

					
					// Repaint all scene views to show the selection change
					SceneView.RepaintAll();
				}

				if(selectedVertices.Count > 0)
				{
					e.Use();
				}
			}
		}

		void DrawVertices(SceneView sceneView, Event e)
		{
			Polygon[] polygons = targetBrush.Polygons;

			Camera sceneViewCamera = sceneView.camera;

			SabreGraphics.GetVertexMaterial().SetPass (0);
			GL.PushMatrix();
			GL.LoadPixelMatrix();

			GL.Begin(GL.QUADS);
			Vector3 target;

			for (int i = 0; i < polygons.Length; i++) 
			{
				for (int j = 0; j < polygons[i].Vertices.Length; j++) 
				{
					Vertex vertex = polygons[i].Vertices[j];
					
					if(selectedVertices.Contains(vertex))
					{
						GL.Color(new Color32(0, 255, 128, 255));
					}
					else
					{
						GL.Color(Color.white);
					}

					target = sceneViewCamera.WorldToScreenPoint(targetBrush.transform.TransformPoint(vertex.Position));
					if(target.z > 0)
					{
						// Make it pixel perfect
						target = MathHelper.RoundVector3(target);
						SabreGraphics.DrawBillboardQuad(target, 8, 8);
					}
				}
			}
			GL.End();
			GL.PopMatrix();
		}

		public override void Deactivated ()
		{
			
		}
	}
}
#endif