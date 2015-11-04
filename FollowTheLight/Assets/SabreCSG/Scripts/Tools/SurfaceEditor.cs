#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
    public class SurfaceEditor : Tool
    {
		enum Mode { None, Translate, Rotate };
		Mode currentMode = Mode.None;

		List<Polygon> selectedSourcePolygons = new List<Polygon>();
		Dictionary<Polygon, Brush> matchedBrushes = new Dictionary<Polygon, Brush>();

		// The primary polygon being interacted with, this is the polygon that the drag started on
		Polygon currentPolygon = null;
		float rotationDiameter = 0;

		// Used so that the MouseUp event knows that it was the end of a drag not the end of a click
		bool dragging = false;

		Vector3 lastWorldPoint;
		Vector3 currentWorldPoint;

		// Used to preserve movement while snapping
		Vector2 totalDelta;
		Vector2 appliedDelta;

		// Scale
		float scaleAmount = 1;

		// Rotation
		float rotationAmount = 0;

		// Used to preserve movement while snapping
		float fullDeltaAngle = 0;
		float unroundedDeltaAngle = 0;

		// Used to offset from e.mousePosition to GUI space. (Value from EditorStyles.toolbar.fixedHeight)
		const int TOOLBAR_HEIGHT = 18; 

		// Main UI rectangle for this tool's UI
		readonly Rect toolbarRect = new Rect(0, 50, 260, 240);

		// Displayed against the material field when multiple materials conflict (shows "---")
		static Object mixedObject = null;

		// Used to prevent MouseDown/MouseUp events when clicking the material field
		Rect materialFieldRect;

		public override void OnSceneGUI(SceneView sceneView, Event e)
        {
			base.OnSceneGUI(sceneView, e); // Allow the base logic to calculate first

			// GUI events
			if (e.type == EventType.Repaint || e.type == EventType.Layout)
			{
				OnRepaintGUI(sceneView, e);
			}

			if (e.type == EventType.Repaint)
			{
				OnRepaint(sceneView, e);
			}
			else if(e.type == EventType.MouseMove)
			{
				SceneView.RepaintAll();
			}
			else if(e.type == EventType.MouseDrag)
			{
				OnMouseDrag(sceneView, e);
			}
			else if(e.type == EventType.MouseDown && !materialFieldRect.Contains(e.mousePosition))
			{
				OnMouseDown(sceneView, e);
			}
			else if(e.type == EventType.MouseUp && !materialFieldRect.Contains(e.mousePosition))
			{
				OnMouseUp(sceneView, e);
			}
			else if(e.type == EventType.DragPerform)
			{
				OnDragPerform(sceneView, e);
			}
        }
		
		void OnMouseDown (SceneView sceneView, Event e)
		{
			if(e.button != 0 || CameraPanInProgress)
			{
				return;
			}
			
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Polygon builtPolygon = csgModel.RaycastBuiltPolygons(ray);

			if(builtPolygon != null)
			{
				Polygon sourcePolygon = csgModel.GetSourcePolygon(builtPolygon.UniqueIndex);
				
				if(!selectedSourcePolygons.Contains(sourcePolygon))
				{
					if(!e.shift && !e.control)
					{
						currentPolygon = null;
						selectedSourcePolygons.Clear();
						matchedBrushes.Clear();
					}
					
					if(builtPolygon != null)
					{
						if(!selectedSourcePolygons.Contains(sourcePolygon))
						{
							selectedSourcePolygons.Add(sourcePolygon);
							matchedBrushes.Add(sourcePolygon, csgModel.FindBrushFromPolygon(sourcePolygon));
						}
					}
				}
				
				totalDelta = Vector2.zero;
				appliedDelta = Vector2.zero;
				
				currentPolygon = sourcePolygon;

				Transform brushTransform = matchedBrushes[currentPolygon].transform;

				Vector3[] transformedPositions = new Vector3[currentPolygon.Vertices.Length];

				for (int j = 0; j < currentPolygon.Vertices.Length; j++)
				{
					transformedPositions[j] = brushTransform.TransformPoint(currentPolygon.Vertices[j].Position);
				}

				// Calculate the diameter of the the bounding circle for the transformed polygon (polygon aligned)
				for (int j = 1; j < transformedPositions.Length; j++)
				{
					float distance = Vector3.Distance(transformedPositions[1], transformedPositions[0]);
					if(distance > rotationDiameter)
					{
						rotationDiameter = distance;
					}
				}

				dragging = false;


				Plane plane = new Plane(brushTransform.TransformPoint(currentPolygon.Vertices[0].Position), 
				                        brushTransform.TransformPoint(currentPolygon.Vertices[1].Position), 
				                        brushTransform.TransformPoint(currentPolygon.Vertices[2].Position));
				
				float rayDistance;
				
				if(plane.Raycast(ray, out rayDistance))
				{
					Vector3 worldPoint = ray.GetPoint(rayDistance);
					
					lastWorldPoint = worldPoint;
				}

				// Set the appropiate mode based on whether the user wants to rotate or translate
				if(e.control)
				{
					currentMode = Mode.Rotate;
				}
				else
				{
					currentMode = Mode.Translate;
				}
				
				e.Use();
			}
		}

		void OnMouseDrag(SceneView sceneView, Event e)
		{
			if(e.button != 0 || CameraPanInProgress)
			{
				return;
			}

			// Used so that the MouseUp event knows that it was the end of a drag not the end of a click
			dragging = true; 

			if (currentMode == Mode.Rotate)
			{
				OnMouseDragRotate(sceneView, e);
			}
			else if (currentMode == Mode.Translate)
			{
				OnMouseDragTranslate(sceneView, e);
			}
		}

		void OnMouseDragTranslate (SceneView sceneView, Event e)
		{
			if(currentPolygon != null)
			{
				Transform brushTransform = matchedBrushes[currentPolygon].transform;

				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
//				currentPolygon.CalculatePlane();
//				Plane plane = currentPolygon.Plane;

				Vector3 transformedPosition1 = brushTransform.TransformPoint(currentPolygon.Vertices[0].Position);
				Vector3 transformedPosition2 = brushTransform.TransformPoint(currentPolygon.Vertices[1].Position);
				Vector3 transformedPosition3 = brushTransform.TransformPoint(currentPolygon.Vertices[2].Position);

				Plane plane = new Plane(transformedPosition1, transformedPosition2, transformedPosition3);

				float distance;

				if(plane.Raycast(ray, out distance))
				{
					Vector3 worldPoint = ray.GetPoint(distance);

					currentWorldPoint = worldPoint;
					Vector3 worldDelta = currentWorldPoint - lastWorldPoint;

//					worldDelta = brushTransform.TransformDirection(worldDelta);


					// Polygon normal in world space
					Vector3 normal = plane.normal;

					// World vector (normalized) from the first vertex to the next
					Vector3 tangent = (transformedPosition2 - transformedPosition1).normalized;

					// The UV vector that corresponds to the tangent vector
					Vector2 uvA = (currentPolygon.Vertices[1].UV - currentPolygon.Vertices[0].UV).normalized;

					// Find the angle that the tangent line is off 
					float angle = Mathf.Atan2(uvA.x, uvA.y) * Mathf.Rad2Deg;

					// Find the UV normal (if this is < 1, the UVs are flipped and we have to handle it accordingly)
					Vector2 uvB = (currentPolygon.Vertices[2].UV - currentPolygon.Vertices[1].UV).normalized;
					Vector3 uvNormal = Vector3.Cross(uvB, uvA);

//					Vector2 uvNorth = new Vector2(0,1);
//					Vector2 uvEast = new Vector2(1,0);


//					Vector3 uvA3 = new Vector3(uvA.x, 0, uvA.y);
//					Vector3 uvB3 = new Vector3(uvB.x, 0, uvB.y);
//					Vector3 uvNormal3 = Vector3.Cross(uvA3, uvB3);


					// The world vector that should correspond to a UV of (0,1)
					Vector3 worldVectorNorth = Quaternion.AngleAxis(180 - angle, normal) * tangent;
					// The world vector that should correspond to a UV of (1,0)
					Vector3 worldVectorEast = Quaternion.AngleAxis(270 - angle, normal) * tangent;

					if(uvNormal.z < 0)
					{
						if(((uvA.x + uvA.y) > 0 && (uvB.x + uvB.y) > 0)
						   || ((uvA.x + uvA.y) < 0 && (uvB.x + uvB.y) < 0))
						{
							 worldVectorNorth = Quaternion.AngleAxis(0 + angle, normal) * tangent;
							// The world vector that should correspond to a UV of (1,0)
							 worldVectorEast = Quaternion.AngleAxis(90 + angle, normal) * tangent;

							worldVectorNorth = -worldVectorNorth;
						}
						else 
						{
							worldVectorNorth = Quaternion.AngleAxis(180 + angle, normal) * tangent;
							// The world vector that should correspond to a UV of (1,0)
							worldVectorEast = Quaternion.AngleAxis(270 + angle, normal) * tangent;

							worldVectorEast = -worldVectorEast;
						}
					}


					// Calculate the change in UV from the world delta
					float uvNorthDelta = Vector3.Dot(worldVectorNorth, worldDelta);
					float uvEastDelta = Vector3.Dot(worldVectorEast, worldDelta);

					// Used to discount 
					// TODO: Calculate this from the polygon
					Vector2 uvScale = new Vector2(0.5f, 0.5f);

					Vector2 uvDelta = new Vector2(uvEastDelta * uvScale.x, uvNorthDelta * uvScale.y);

					if(CurrentSettings.Instance.PositionSnappingEnabled)
					{
						totalDelta += uvDelta;
						
						float snapDistance = CurrentSettings.Instance.PositionSnapDistance;
						
						Vector2 roundedTotal = MathHelper.RoundVector2(totalDelta, 0.5f * snapDistance);
						uvDelta = roundedTotal - appliedDelta;
						appliedDelta += uvDelta;// - totalDelta;
					}

					TransformUVs(UVUtility.TranslateUV, new UVUtility.TransformData(uvDelta,0));

					lastWorldPoint = currentWorldPoint;

					e.Use ();
				}
			}
		}


		void OnMouseDragRotate(SceneView sceneView, Event e)
		{
			Transform brushTransform = matchedBrushes[currentPolygon].transform;

			// Grab the world space plane from the polygon, so we can raycast against it
			Plane plane = new Plane(brushTransform.TransformPoint(currentPolygon.Vertices[0].Position), 
			                        brushTransform.TransformPoint(currentPolygon.Vertices[1].Position), 
			                        brushTransform.TransformPoint(currentPolygon.Vertices[2].Position));


			// Rotation will be around this axis
			Vector3 rotationAxis = plane.normal;
			// Brush center point
			Vector3 centerWorld = brushTransform.TransformPoint(currentPolygon.GetCenterPoint());

			// Where the mouse was last frame
			Vector2 lastPosition = e.mousePosition - e.delta;
			// Where the mouse is this frame
			Vector2 currentPosition = e.mousePosition;
			
			Ray lastRay = Camera.current.ScreenPointToRay(EditorHelper.ConvertMousePosition(lastPosition));
			Ray currentRay = Camera.current.ScreenPointToRay(EditorHelper.ConvertMousePosition(currentPosition));

			float lastRayHit;
			float currentRayHit;

			// Find where in the last frame the mouse ray intersected with this polygon's plane
			if(plane.Raycast(lastRay, out lastRayHit))
			{
				// Find where in the current frame the mouse ray intersected with this polygon's plane
				if(plane.Raycast(currentRay, out currentRayHit))
				{
					// Find the world points where the rays hit the rotation plane
					Vector3 lastRayWorld = lastRay.GetPoint(lastRayHit);
					Vector3 currentRayWorld = currentRay.GetPoint(currentRayHit);
					
					// Find the rotation needed to transform the points on the plane into XY aligned plane
					Quaternion cancellingRotation = Quaternion.Inverse(Quaternion.LookRotation(rotationAxis));
					
					// Subtract the brush's center point so the points are relative to the center of the brush
					currentRayWorld -= centerWorld;
					lastRayWorld -= centerWorld;
					
					// Rotate the world points by the cancelling rotation to put them on XY plane
					currentRayWorld = cancellingRotation * currentRayWorld;
					lastRayWorld = cancellingRotation * lastRayWorld;
					
					// Because the points have been transformed into XY plane, we can just use atan2 to find the angles
					float angle1 = Mathf.Rad2Deg * Mathf.Atan2(currentRayWorld.x, currentRayWorld.y);
					float angle2 = Mathf.Rad2Deg * Mathf.Atan2(lastRayWorld.x, lastRayWorld.y);
					// Change in angle is simply the new angle minus the last
					float deltaAngle = angle2 - angle1;
					
					// If snapping is enabled, apply snapping to the delta angle
					if(CurrentSettings.Instance.AngleSnappingEnabled)
					{
						deltaAngle += unroundedDeltaAngle;
						
						float roundedAngle = MathHelper.RoundFloat(deltaAngle, CurrentSettings.Instance.AngleSnapDistance);
						// Store the change in angle that hasn't been applied due to snapping
						unroundedDeltaAngle = deltaAngle - roundedAngle;
						// Snap out delta angle for the snapped delta
						deltaAngle = roundedAngle;
					}
					fullDeltaAngle += deltaAngle;
					
//					Undo.RecordObject(targetBrushTransform, "Rotated brush(es)");

					// Rotate the UV using the supplied angle
					RotateAroundCenter(deltaAngle);

					e.Use();
				}
			}
			
			SabreMouse.SetCursor(MouseCursor.RotateArrow);
		}

		void OnMouseUp (SceneView sceneView, Event e)
		{
			if(e.button != 0 || CameraPanInProgress)
			{
				return;
			}

			currentMode = Mode.None;
			SabreMouse.ResetCursor();

			if(!dragging)
			{
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				Polygon polygon = csgModel.RaycastBuiltPolygons(ray);

				if(!e.shift && !e.control)// && selectedSourcePolygons.Count > 0)
				{
					currentPolygon = null;
					selectedSourcePolygons.Clear();
					matchedBrushes.Clear();
					e.Use();
				}

				if(polygon != null)
				{
					Polygon sourcePolygon = csgModel.GetSourcePolygon(polygon.UniqueIndex);

					if(!selectedSourcePolygons.Contains(sourcePolygon))
					{
						// Not already in the list so add it
						selectedSourcePolygons.Add(sourcePolygon);
						matchedBrushes.Add(sourcePolygon, csgModel.FindBrushFromPolygon(sourcePolygon));
					}
					else
					{
						// Already in the list
						if(e.control)
						{
							// If ctrl is held and it's already selected, then deselect it
							selectedSourcePolygons.Remove(sourcePolygon);
							matchedBrushes.Remove(sourcePolygon);
						}
					}

					e.Use();
				}
			}

			dragging = false;
		}

		void OnRepaint (SceneView sceneView, Event e)
		{
			// Start drawing using the relevant material
			SabreGraphics.GetSelectedBrushMaterial().SetPass(0);

			Polygon[] allPolygons;

			// Highlight the polygon the mouse is over unless they are moving the camera
			if(!CameraPanInProgress && currentMode == Mode.None)
			{
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				Polygon polygon = csgModel.RaycastBuiltPolygons(ray);

				// Hovered polygon
				if(polygon != null)
				{
					allPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);
					SabreGraphics.DrawPolygons(new Color(0,1,0,0.5f), allPolygons);	
				}
			}

			// Draw each of the selcted polygons as a highlight
			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				allPolygons = csgModel.BuiltPolygonsByIndex(selectedSourcePolygons[i].UniqueIndex);
				SabreGraphics.DrawPolygons(Color.green, allPolygons);
			}

			// Draw the rotation gizmo
			if(currentMode == Mode.Rotate && currentPolygon != null)
			{
				SabreGraphics.GetSelectedBrushMaterial().SetPass(0);

				Brush brush = matchedBrushes[currentPolygon];
				Transform brushTransform = brush.transform;

				// Polygons are stored in local space, transform the polygon normal into world space
				Vector3 normal = brushTransform.TransformDirection(currentPolygon.Plane.normal);

				Vector3 worldCenterPoint = brushTransform.TransformPoint(currentPolygon.GetCenterPoint());
				// Offset the gizmo so it's very slightly above the polygon, to avoid depth fighting
				if(brush.Mode == CSGMode.Add)
				{
					worldCenterPoint += normal * 0.02f;
				}
				else
				{
					worldCenterPoint -= normal * 0.02f;
				}

				float radius = rotationDiameter * .5f;

				Vector3 initialRotationDirection = 5 * (brushTransform.TransformPoint(currentPolygon.Vertices[1].Position) - brushTransform.TransformPoint(currentPolygon.Vertices[1].Position)).normalized;

				// Draw the actual rotation gizmo
				SabreGraphics.DrawRotationCircle(worldCenterPoint, normal, radius, initialRotationDirection);
			}

			// If the mouse is down draw a point where the mouse is interacting in world space
			if(e.button == 0)
			{
				Camera sceneViewCamera = sceneView.camera;

				SabreGraphics.GetVertexMaterial().SetPass (0);
				GL.PushMatrix();
				GL.LoadPixelMatrix();
				
				GL.Begin(GL.QUADS);
				Vector3 target = sceneViewCamera.WorldToScreenPoint(currentWorldPoint);
				if(target.z > 0)
				{
					// Make it pixel perfect
					target = MathHelper.RoundVector3(target);
					SabreGraphics.DrawBillboardQuad(target, 8, 8);
				}
				GL.End();
				GL.PopMatrix();
			}
		}

		void OnDragPerform (SceneView sceneView, Event e)
		{
			if(DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is Material)
			{
				if(selectedSourcePolygons.Count > 0)
				{
					Material material = (Material)DragAndDrop.objectReferences[0];

					for (int i = 0; i < selectedSourcePolygons.Count; i++) 
					{
						TransferPolygon(selectedSourcePolygons[i], material, Color.white);
					}
					DragAndDrop.AcceptDrag();
				}

				e.Use();
			}
		}

        void OnRepaintGUI(SceneView sceneView, Event e)
        {
//			GUI.backgroundColor = Color.red; // Tint the background for debugging areas
            // Draw UI specific to this editor
			GUIStyle toolbar = new GUIStyle(EditorStyles.toolbar);
			toolbar.fixedHeight = toolbarRect.height;
			GUILayout.Window(2, toolbarRect, OnToolbarGUI, "", toolbar);
        }

		void TransformUVs (UVUtility.UVTransformation transformationMethod, UVUtility.TransformData transformData)
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];
				TransformUVs(polygon, transformationMethod, transformData);
			}
		}

		void TransformUVs (Polygon polygon, UVUtility.UVTransformation transformationMethod, UVUtility.TransformData transformData)
		{
			for (int vertexIndex = 0; vertexIndex < polygon.Vertices.Length; vertexIndex++) 
			{
				polygon.Vertices[vertexIndex].UV = transformationMethod(polygon.Vertices[vertexIndex].UV, transformData);
			}
			
			PolygonDirectory.PolygonMeshMapping mapping = csgModel.PolygonDirectory.FindMapping(polygon);
			Vector2[] uv = mapping.Mesh.uv;
			for (int vertexIndex = 0; vertexIndex < mapping.VertexIndices.Count; vertexIndex++) 
			{
				uv[mapping.VertexIndices[vertexIndex]] = transformationMethod(uv[mapping.VertexIndices[vertexIndex]], transformData);
			}
			mapping.Mesh.uv = uv;
		}

		void RotateAroundCenter (float rotationAmount)
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];
				
				Vector2 centerUV = polygon.GetCenterUV();
				
				Vector2 uvDelta1 = (polygon.Vertices[2].UV - polygon.Vertices[1].UV).normalized;
				// Find the UV delta along an adjacent edge too (so we can detect flipping)
				Vector2 uvDelta2 = (polygon.Vertices[0].UV - polygon.Vertices[1].UV).normalized;
				
				Vector3 uvNormal = Vector3.Cross(uvDelta1,uvDelta2).normalized;
				
				if(uvNormal.z < 0)
				{
					TransformUVs(polygon, UVUtility.RotateUV, new UVUtility.TransformData(centerUV,rotationAmount));
				}
				else // Flip the delta angle for flipped UVs
				{
					TransformUVs(polygon, UVUtility.RotateUV, new UVUtility.TransformData(centerUV,-rotationAmount));
				}
			}
		}

        void OnToolbarGUI(int windowID)
        {
			bool materialConflict = false;
			Object material = null;

			if(selectedSourcePolygons.Count > 0)
			{
				// Set the material to the first polygon's
				material = selectedSourcePolygons[0].Material;
				// Continue through the rest of the polygons determining if there is a conflict

				for (int i = 1; i < selectedSourcePolygons.Count; i++) 
				{
					// Different materials found
					if(selectedSourcePolygons[i].Material != material)
					{
						materialConflict = true;
					}
				}
			}

			if(material == null)
			{
				material = CSGFactory.GetDefaultMaterial();
			}

			if(materialConflict)
			{
				if(mixedObject == null)
				{
					mixedObject = new Material(Shader.Find("Diffuse"));
					mixedObject.name = "---";
				}
				material = mixedObject;
			}
			GUILayout.BeginHorizontal(GUILayout.Width(150));

			GUILayout.Label("Mat", SabreGUILayout.FormatStyle(Color.black));

			Material newMaterial = EditorGUILayout.ObjectField(material, typeof(Material), false, GUILayout.Width(100)) as Material;

			materialFieldRect = GUILayoutUtility.GetLastRect();
			materialFieldRect.center += toolbarRect.min;
			materialFieldRect.center -= new Vector2(0,EditorStyles.toolbar.fixedHeight);

			if(newMaterial != material)
			{
				for (int i = 0; i < selectedSourcePolygons.Count; i++) 
				{
					TransferPolygon(selectedSourcePolygons[i], newMaterial, Color.white);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(GUILayout.Width(150));
			
			if (GUILayout.Button("Flip X", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVX, new UVUtility.TransformData(Vector2.zero,0));
			}
			if (GUILayout.Button("Flip Y", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVY, new UVUtility.TransformData(Vector2.zero,0));
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal(GUILayout.Width(150));
			if (GUILayout.Button("Flip XY", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVXY, new UVUtility.TransformData(Vector2.zero,0));
			}
			if(GUILayout.Button("Auto UV", EditorStyles.miniButton))
			{
				AutoUV();
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal(GUILayout.Width(150));

			scaleAmount = EditorGUILayout.FloatField(scaleAmount, GUILayout.Width(50));
			
			if(GUILayout.Button("Scale", EditorStyles.miniButton) && scaleAmount > 0)
			{
				TransformUVs(UVUtility.ScaleUV, new UVUtility.TransformData(new Vector2(scaleAmount,scaleAmount),0));
			}
			
			if (GUILayout.Button("x 2", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.ScaleUV, new UVUtility.TransformData(new Vector2(2f,2f),0));
			}
			if (GUILayout.Button("/ 2", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.ScaleUV, new UVUtility.TransformData(new Vector2(.5f,.5f),0));
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(GUILayout.Width(150));

			rotationAmount = EditorGUILayout.FloatField(rotationAmount, GUILayout.Width(100));

			if(GUILayout.Button("Rotate", EditorStyles.miniButton))
			{
				RotateAroundCenter(rotationAmount);
			}

			GUILayout.EndHorizontal();
        }

		public override void ResetTool()
		{
		}

		public override void Deactivated ()
		{
		}

		public override bool BrushesHandleDrawing 
		{
			get 
			{
				return false;
			}
		}

		void AutoUV()
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];
				PolygonDirectory.PolygonMeshMapping mapping = csgModel.PolygonDirectory.FindMapping(polygon);
			
				UnityEngine.Plane plane = new UnityEngine.Plane(polygon.Vertices[0].Position, polygon.Vertices[1].Position, polygon.Vertices[2].Position);
				
				Quaternion cancellingRotation = Quaternion.Inverse(Quaternion.LookRotation(plane.normal));
				
				// Sets the UV at each point to the position on the plane
				for (int i = 0; i < polygon.Vertices.Length; i++) 
				{
					Vector2 uv = (cancellingRotation *  polygon.Vertices[i].Position) * 0.5f;;
					polygon.Vertices[i].UV = uv;
				}
				
				Vector3[] vertices = mapping.Mesh.vertices;
				
				Vector2[] uvs = mapping.Mesh.uv;

				Transform brushTransform = matchedBrushes[polygon].transform;

				for (int i = 0; i < mapping.VertexIndices.Count; i++) 
				{
					Vector3 position = brushTransform.InverseTransformPoint(vertices[mapping.VertexIndices[i]]);
					Vector2 uv = (cancellingRotation * position) * 0.5f;
					
					uvs[mapping.VertexIndices[i]] = uv;
				}
				mapping.Mesh.uv = uvs;
			}
		}

		void TransferPolygon(Polygon polygon, Material destinationMaterial, Color32 destinationColor)
		{
			// Only attempt to transfer the polygon if it's too a different material!
			if(polygon.Material == destinationMaterial)
			{
				return;
			}
			PolygonDirectory.PolygonMeshMapping originalMapping = csgModel.PolygonDirectory.FindMapping(polygon);
			
			// Repoint the polygon's material, so it will change with rebuilds
			polygon.Material = destinationMaterial;
			polygon.SetColor(destinationColor);
			
			if(originalMapping == null)
			{
				// This polygon hasn't actually been built
				return;
			}

			int verticesToAdd = originalMapping.VertexIndices.Count;
			
			int[] sourceTriangleIndices = new int[originalMapping.TriangleIndices.Count];
			originalMapping.TriangleIndices.CopyTo(sourceTriangleIndices);
			
			int[] sourceTriangles = originalMapping.Mesh.triangles;
			Vector3[] sourceVertices = originalMapping.Mesh.vertices;
			Vector2[] sourceUV = originalMapping.Mesh.uv;
			Vector3[] sourceNormals = originalMapping.Mesh.normals;
			
			Mesh newMesh = csgModel.GetMeshForMaterial(destinationMaterial, verticesToAdd);


#if UNITY_5_1
			int[] destTriangles;

			// Unfortunately in Unity 5.1 accessing .triangles on an empty mesh throws an error
			if(newMesh.vertexCount == 0)
			{
				destTriangles = new int[0];
			}
			else
			{
				destTriangles = newMesh.triangles;
			}
#else
			int[] destTriangles = newMesh.triangles;
#endif

			Vector3[] destVertices = newMesh.vertices;
			Vector2[] destUV = newMesh.uv;
			Vector3[] destNormals = newMesh.normals;
			Color32[] destColors = newMesh.colors32;
			
			int destVertexCount = destVertices.Length;

			
			
			System.Array.Resize(ref destVertices, destVertexCount + verticesToAdd);
			System.Array.Resize(ref destUV, destVertexCount + verticesToAdd);
			System.Array.Resize(ref destNormals, destVertexCount + verticesToAdd);
			System.Array.Resize(ref destColors, destVertexCount + verticesToAdd);
			
			int[] newTriangles = new int[originalMapping.TriangleIndices.Count];
//			int[] newTriangleIndices = new int[originalMapping.TriangleIndices.Count];
			
			for (int i = 0; i < verticesToAdd; i++) 
			{
				destVertices[destVertexCount + i] = sourceVertices[originalMapping.VertexIndices[i]];
				destUV[destVertexCount + i] = sourceUV[originalMapping.VertexIndices[i]];
				destNormals[destVertexCount + i] = sourceNormals[originalMapping.VertexIndices[i]];
				destColors[destVertexCount + i] = destinationColor;
				
				// Repoint triangles to the new location
				for (int j = 0; j < originalMapping.TriangleIndices.Count; j++) 
				{
					// TODO: Not sure about this bit
					if(sourceTriangles[originalMapping.TriangleIndices[j]] == originalMapping.VertexIndices[i])
					{
						newTriangles[j] = destVertexCount + i;
					}
				}
				// Repoint the vertex to the new location
				originalMapping.VertexIndices[i] = destVertexCount + i;
			}
			
			int destTrianglesCount = destTriangles.Length;
			int trianglesToAdd = originalMapping.TriangleIndices.Count;
			for (int i = 0; i < originalMapping.TriangleIndices.Count; i++) 
			{
				originalMapping.TriangleIndices[i] = destTrianglesCount + i;
			}
			
			System.Array.Resize(ref destTriangles, destTrianglesCount + trianglesToAdd);
			
			for (int i = 0; i < trianglesToAdd; i++) 
			{
				destTriangles[destTrianglesCount + i] = newTriangles[i]; //originalMapping.TriangleIndices[i];
			}
			
			
			newMesh.vertices = destVertices;
			newMesh.triangles = destTriangles;
			newMesh.uv = destUV;
			newMesh.colors32 = destColors;
			newMesh.normals = destNormals;
			
			
			
			for (int i = 0; i < sourceTriangleIndices.Length; i++) 
			{
				sourceTriangles[sourceTriangleIndices[i]] = 0;
			}
			
			
			
			
			originalMapping.Mesh.vertices = sourceVertices;
			originalMapping.Mesh.triangles = sourceTriangles;
			
			// Finally point the mapping to the new mesh
			originalMapping.Mesh = newMesh;
		}


    }
}
#endif