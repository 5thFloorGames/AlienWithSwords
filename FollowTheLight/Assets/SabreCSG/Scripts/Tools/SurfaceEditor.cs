#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
    public class SurfaceEditor : Tool
    {
		class UVOrientation
		{
			public Vector3 NorthVector;
			public Vector3 EastVector;
		}

		enum Mode { None, Translate, Rotate };
		enum AlignDirection { Top, Bottom, Left, Right };
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
		bool pointSet = false;

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
		readonly Rect toolbarRect = new Rect(0, 50, 163, 160);

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
			else if(e.type == EventType.MouseDown 
				&& !materialFieldRect.Contains(e.mousePosition) 
				&& !EditorHelper.IsMousePositionNearSceneGizmo(e.mousePosition))
			{
				OnMouseDown(sceneView, e);
			}
			else if(e.type == EventType.MouseUp
				&& !materialFieldRect.Contains(e.mousePosition) 
				&& !EditorHelper.IsMousePositionNearSceneGizmo(e.mousePosition))
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
					pointSet = true;

					UVOrientation worldOrientation = GetNorthEastVectors(currentPolygon, brushTransform);

					Vector3 worldVectorNorth = worldOrientation.NorthVector;
					Vector3 worldVectorEast = worldOrientation.EastVector;

					// Calculate the change in UV from the world delta
					float uvNorthDelta = Vector3.Dot(worldVectorNorth, worldDelta);
					float uvEastDelta = Vector3.Dot(worldVectorEast, worldDelta);

					// Used to discount 
					// TODO: Calculate this from the polygon
					Vector2 uvScale = new Vector2(0.5f, 0.5f);

					Vector2 uvDelta = new Vector2(uvEastDelta * uvScale.x, uvNorthDelta * uvScale.y);

					if(CurrentSettings.PositionSnappingEnabled)
					{
						totalDelta += uvDelta;
						
						float snapDistance = CurrentSettings.PositionSnapDistance;
						
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
					if(CurrentSettings.AngleSnappingEnabled)
					{
						deltaAngle += unroundedDeltaAngle;
						
						float roundedAngle = MathHelper.RoundFloat(deltaAngle, CurrentSettings.AngleSnapDistance);
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
			pointSet = false;
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


			SabreGraphics.GetSelectedBrushDashedMaterial().SetPass(0);
			// Draw each of the selcted polygons
			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				allPolygons = csgModel.BuiltPolygonsByIndex(selectedSourcePolygons[i].UniqueIndex);
				SabreGraphics.DrawPolygonsOutlineDashed(Color.green, allPolygons);
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
			if(e.button == 0 && pointSet)
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
            // Draw UI specific to this editor
			GUIStyle toolbar = new GUIStyle(EditorStyles.toolbar);

			// Set the background tint
			if(EditorGUIUtility.isProSkin)
			{
				toolbar.normal.background = SabreGraphics.HalfBlackTexture;
			}
			else
			{
				toolbar.normal.background = SabreGraphics.HalfWhiteTexture;
			}
			// Set the style height to match the rectangle (so it stretches instead of tiling)
			toolbar.fixedHeight = toolbarRect.height;
			// Draw the actual GUI via a Window
			GUILayout.Window(140009, toolbarRect, OnToolbarGUI, "", toolbar);
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
			// Update the source polygon, so rebuilding is correct
			for (int vertexIndex = 0; vertexIndex < polygon.Vertices.Length; vertexIndex++) 
			{
				polygon.Vertices[vertexIndex].UV = transformationMethod(polygon.Vertices[vertexIndex].UV, transformData);
			}

			// Update the built polygons in case we need to use them for something else
			Polygon[] builtPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);
			for (int polygonIndex = 0; polygonIndex < builtPolygons.Length; polygonIndex++) 
			{
				Polygon builtPolygon = builtPolygons[polygonIndex];
				for (int vertexIndex = 0; vertexIndex < builtPolygon.Vertices.Length; vertexIndex++) 
				{
					builtPolygon.Vertices[vertexIndex].UV = transformationMethod(builtPolygon.Vertices[vertexIndex].UV, transformData);
				}
			}

			// Update the actual built mesh
			PolygonDirectory.PolygonMeshMapping mapping = csgModel.PolygonDirectory.FindMapping(polygon);
			if(mapping != null)
			{
				Vector2[] uv = mapping.Mesh.uv;
				for (int vertexIndex = 0; vertexIndex < mapping.VertexIndices.Count; vertexIndex++) 
				{
					uv[mapping.VertexIndices[vertexIndex]] = transformationMethod(uv[mapping.VertexIndices[vertexIndex]], transformData);
				}
				mapping.Mesh.uv = uv;
			}
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

		void DrawMaterialBox()
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
				material = CSGModel.GetDefaultMaterial();
			}

			GUILayout.BeginHorizontal(GUILayout.Width(150));

			GUILayout.Label("Mat", SabreGUILayout.GetForeStyle());

			Material newMaterial = null;

			if(materialConflict)
			{
				material = null;

				EditorGUI.showMixedValue = true;
				newMaterial = EditorGUILayout.ObjectField(material, typeof(Material), false, GUILayout.Width(100)) as Material;
				EditorGUI.showMixedValue = false;
			}
			else
			{
				newMaterial = EditorGUILayout.ObjectField(material, typeof(Material), false, GUILayout.Width(100)) as Material;
			}

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
		}

		void DrawExcludeBox()
		{
			bool excludeConflict = false;
			bool excludeState = false;

			if(selectedSourcePolygons.Count > 0)
			{
				// Set the state to the first polygon's
				excludeState = selectedSourcePolygons[0].UserExcludeFromFinal;

				// Continue through the rest of the polygons determining if there is a conflict
				for (int i = 1; i < selectedSourcePolygons.Count; i++) 
				{
					// Different materials found
					if(selectedSourcePolygons[i].UserExcludeFromFinal != excludeState)
					{
						excludeConflict = true;
						excludeState = false;
					}
				}
			}

			GUILayout.BeginHorizontal(GUILayout.Width(50));
			GUILayout.Label("Exclude", SabreGUILayout.GetForeStyle());
			EditorGUI.showMixedValue = excludeConflict; // Show mixed state if necessary
			bool newExcludeState = EditorGUILayout.Toggle(excludeState);
			EditorGUI.showMixedValue = false; // Reset mixed state
			GUILayout.EndHorizontal();

			if(newExcludeState != excludeState)
			{
				for (int i = 0; i < selectedSourcePolygons.Count; i++) 
				{
					selectedSourcePolygons[i].UserExcludeFromFinal = newExcludeState;

					if(newExcludeState)
					{
						UserExcludePolygon(selectedSourcePolygons[i]);
					}
					else
					{
						UserIncludePolygon(selectedSourcePolygons[i]);
					}

					EditorUtility.SetDirty(matchedBrushes[selectedSourcePolygons[i]]);
				}
			}
		}

        void OnToolbarGUI(int windowID)
        {
			// Allow the user to change the material on the selected polygons
			DrawMaterialBox();

			// Allow the user to change whether the selected polygons will be excluded from the final mesh
			DrawExcludeBox();

			GUIStyle newStyle = new GUIStyle(EditorStyles.miniButton);
			newStyle.padding = new RectOffset(0,0,0,0);

			if(GUI.Button(new Rect(80,27,80,15), "▲", newStyle))
			{
				Align(AlignDirection.Top);
			}
			if(GUI.Button(new Rect(80,42,40,15), "◄", EditorStyles.miniButton))
			{
				Align(AlignDirection.Left);
			}
			if(GUI.Button(new Rect(120,42,40,15), "▶", EditorStyles.miniButton))
			{
				Align(AlignDirection.Right);
			}
			if(GUI.Button(new Rect(80,57,80,15), "▼", newStyle))
			{
				Align(AlignDirection.Bottom);
			}

			if(GUILayout.Button("Auto UV", EditorStyles.miniButton, GUILayout.Width(70)))
			{
				AutoUV();
			}

			if(GUILayout.Button("Auto Fit", EditorStyles.miniButton, GUILayout.Width(70)))
			{
				AutoFit();
			}

			GUILayout.BeginHorizontal(GUILayout.Width(150));
			
			if (GUILayout.Button("Flip X", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVX, new UVUtility.TransformData(Vector2.zero,0));
			}
			if (GUILayout.Button("Flip Y", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVY, new UVUtility.TransformData(Vector2.zero,0));
			}
			if (GUILayout.Button("Flip XY", EditorStyles.miniButton))
			{
				TransformUVs(UVUtility.FlipUVXY, new UVUtility.TransformData(Vector2.zero,0));
			}
			GUILayout.EndHorizontal();





			GUILayout.BeginHorizontal(GUILayout.Width(150));

			if(GUILayout.Button("All", EditorStyles.miniButton))
			{
				SelectAll();
			}

			if(GUILayout.Button("None", EditorStyles.miniButton))
			{
				SelectNone();
			}

			if(GUILayout.Button("Invert", EditorStyles.miniButton))
			{
				InvertSelection();
			}

			GUILayout.EndHorizontal();

//	Commented out as not ready for release
//			if(GUILayout.Button("Extrude", EditorStyles.miniButton))
//			{
//				ExtrudeSelection();
//			}

			GUILayout.BeginHorizontal(GUILayout.Width(150));

			scaleAmount = EditorGUILayout.FloatField(scaleAmount, GUILayout.Width(50));
			
			if(GUILayout.Button("Scale", EditorStyles.miniButton) && scaleAmount > 0)
			{
				TransformUVs(UVUtility.ScaleUV, new UVUtility.TransformData(new Vector2(scaleAmount,scaleAmount),0));
			}

			if (SabreGUILayout.Button("x 2", EditorStyles.miniButtonLeft))
			{
				TransformUVs(UVUtility.ScaleUV, new UVUtility.TransformData(new Vector2(2f,2f),0));
			}
			if (SabreGUILayout.Button("/ 2", EditorStyles.miniButtonRight))
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
			Selection.activeObject = null;
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

		void SelectAll()
		{
			// Set the selection to all the possible selectable polygons
			selectedSourcePolygons = csgModel.GetAllSourcePolygons();

			// Recalculate the matched brushes
			matchedBrushes.Clear();

			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				matchedBrushes.Add(selectedSourcePolygons[i], csgModel.FindBrushFromPolygon(selectedSourcePolygons[i]));
			}
		}

		void SelectNone()
		{
			// Set the selection and matches brushes to empty
			selectedSourcePolygons.Clear();
			matchedBrushes.Clear();
		}

		void InvertSelection()
		{
			// Construct a list of all polygons that are possible to select
			List<Polygon> newList = csgModel.GetAllSourcePolygons();

			// Remove from that list polygons that are already selected
			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				newList.Remove(selectedSourcePolygons[i]);
			}

			// Update the selected list with the new inverted selection
			selectedSourcePolygons = newList;

			// Recalculate the matched brushes
			matchedBrushes.Clear();

			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				matchedBrushes.Add(selectedSourcePolygons[i], csgModel.FindBrushFromPolygon(selectedSourcePolygons[i]));
			}
		}


		void ExtrudeSelection()
		{
			for (int i = 0; i < selectedSourcePolygons.Count; i++) 
			{
				Polygon[] polygons = PolygonFactory.ExtrudePolygon(selectedSourcePolygons[i]);

				GameObject newObject = ((PrimitiveBrush)matchedBrushes[selectedSourcePolygons[i]]).Duplicate();

				// Finally give the new brush the other set of polygons
				newObject.GetComponent<PrimitiveBrush>().SetPolygons(polygons, true);

			}
		}

		void AutoUV()
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];

			
				UnityEngine.Plane plane = new UnityEngine.Plane(polygon.Vertices[0].Position, polygon.Vertices[1].Position, polygon.Vertices[2].Position);
				
				Quaternion cancellingRotation = Quaternion.Inverse(Quaternion.LookRotation(-plane.normal));
				
				// Sets the UV at each point to the position on the plane
				for (int i = 0; i < polygon.Vertices.Length; i++) 
				{
					Vector2 uv = (cancellingRotation * polygon.Vertices[i].Position) * 0.5f;
					polygon.Vertices[i].UV = uv;
				}

				// Update the built polygons in case we need to use them for something else
				Transform brushTransform = matchedBrushes[polygon].transform;

				Polygon[] builtPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);

				for (int builtPolygonIndex = 0; builtPolygonIndex < builtPolygons.Length; builtPolygonIndex++) 
				{
					Polygon builtPolygon = builtPolygons[builtPolygonIndex];
					for (int vertexIndex = 0; vertexIndex < builtPolygon.Vertices.Length; vertexIndex++) 
					{
						Vector3 position = brushTransform.InverseTransformPoint(builtPolygon.Vertices[vertexIndex].Position);
						Vector2 uv = (cancellingRotation * position) * 0.5f;
						builtPolygon.Vertices[vertexIndex].UV = uv;
					}
				}

				// Update the actual built mesh 
				PolygonDirectory.PolygonMeshMapping mapping = csgModel.PolygonDirectory.FindMapping(polygon);

				Vector3[] vertices = mapping.Mesh.vertices;
				Vector2[] uvs = mapping.Mesh.uv;

				for (int i = 0; i < mapping.VertexIndices.Count; i++) 
				{
					Vector3 position = brushTransform.InverseTransformPoint(vertices[mapping.VertexIndices[i]]);
					Vector2 uv = (cancellingRotation * position) * 0.5f;
					
					uvs[mapping.VertexIndices[i]] = uv;
				}
				mapping.Mesh.uv = uvs;
			}
		}

		UVOrientation GetNorthEastVectors(Polygon polygon, Transform brushTransform)
		{
			Vector3 transformedPosition1 = brushTransform.TransformPoint(polygon.Vertices[0].Position);
			Vector3 transformedPosition2 = brushTransform.TransformPoint(polygon.Vertices[1].Position);
			Vector3 transformedPosition3 = brushTransform.TransformPoint(polygon.Vertices[2].Position);

			Plane worldPlane = new Plane(transformedPosition1, transformedPosition2, transformedPosition3);

			// Polygon normal in world space
			Vector3 normal = worldPlane.normal;

			// World vector (normalized) from the first vertex to the next
			Vector3 tangent = (transformedPosition2 - transformedPosition1).normalized;

			// The UV vector that corresponds to the tangent vector
			Vector2 uvA = (polygon.Vertices[1].UV - polygon.Vertices[0].UV).normalized;

			// Find the angle that the tangent line is off 
			float angle = Mathf.Atan2(uvA.x, uvA.y) * Mathf.Rad2Deg;

			// Find the UV normal (if this is < 1, the UVs are flipped and we have to handle it accordingly)
			Vector2 uvB = (polygon.Vertices[2].UV - polygon.Vertices[1].UV).normalized;
			Vector3 uvNormal = Vector3.Cross(uvB, uvA);


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

			return new UVOrientation()
			{
				NorthVector = worldVectorNorth,
				EastVector = worldVectorEast
			};
		}

		void AutoFit()
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];
				PolygonDirectory.PolygonMeshMapping mapping = csgModel.PolygonDirectory.FindMapping(polygon);

				Transform brushTransform = matchedBrushes[polygon].transform;

				UVOrientation worldOrientation = GetNorthEastVectors(polygon, brushTransform);

				Vector3 worldVectorNorth = worldOrientation.NorthVector;
				Vector3 worldVectorEast = worldOrientation.EastVector;

				// These are flipped for some reason
				worldVectorNorth *= -1;
				worldVectorEast *= -1;

				Vector3 polygonCenterLocal = polygon.GetCenterPoint();
				Vector3 polygonCenterWorld = brushTransform.TransformPoint(polygonCenterLocal);

				// World vertices
				Polygon[] builtPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);
				Vertex northernVertex = builtPolygons[0].Vertices[0];
				Vertex southernVertex = builtPolygons[0].Vertices[0];
				Vertex easternVertex = builtPolygons[0].Vertices[0];
				Vertex westernVertex = builtPolygons[0].Vertices[0];


				for (int builtIndex = 0; builtIndex < builtPolygons.Length; builtIndex++) 
				{
					for (int i = 0; i < builtPolygons[builtIndex].Vertices.Length; i++) 
					{
						Vertex testVertex = builtPolygons[builtIndex].Vertices[i];

						float dotCurrent = Vector3.Dot(northernVertex.Position-polygonCenterWorld, worldVectorNorth);
						float dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, worldVectorNorth);
						if(dotTest > dotCurrent)
						{
							northernVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(southernVertex.Position-polygonCenterWorld, -worldVectorNorth);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, -worldVectorNorth);
						if(dotTest > dotCurrent)
						{
							southernVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(easternVertex.Position-polygonCenterWorld, worldVectorEast);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, worldVectorEast);
						if(dotTest > dotCurrent)
						{
							easternVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(westernVertex.Position-polygonCenterWorld, -worldVectorEast);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, -worldVectorEast);
						if(dotTest > dotCurrent)
						{
							westernVertex = testVertex;
						}
					}
				}

				float northernDistance = Vector3.Dot(northernVertex.Position - polygonCenterWorld, worldVectorNorth);
				float southernDistance = Vector3.Dot(southernVertex.Position - polygonCenterWorld, worldVectorNorth);

				float easternDistance = Vector3.Dot(easternVertex.Position - polygonCenterWorld, worldVectorEast);
				float westernDistance = Vector3.Dot(westernVertex.Position - polygonCenterWorld, worldVectorEast);

				// Update the source polygons
				for (int i = 0; i < polygon.Vertices.Length; i++) 
				{
					Vector3 localPosition = polygon.Vertices[i].Position;
					Vector3 worldPosition = brushTransform.TransformPoint(localPosition);

					float thisNorthDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorNorth);
					float thisEastDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorEast);

					Vector2 uv = new Vector2(MathHelper.InverseLerpNoClamp(westernDistance, easternDistance, thisEastDistance),
						MathHelper.InverseLerpNoClamp(southernDistance, northernDistance, thisNorthDistance));
					
					polygon.Vertices[i].UV = uv;
				}

				// Update the built polygons in case we need to use them for something else
				for (int builtPolygonIndex = 0; builtPolygonIndex < builtPolygons.Length; builtPolygonIndex++) 
				{
					Polygon builtPolygon = builtPolygons[builtPolygonIndex];
					for (int vertexIndex = 0; vertexIndex < builtPolygon.Vertices.Length; vertexIndex++) 
					{
						Vector3 worldPosition = builtPolygon.Vertices[vertexIndex].Position;

						float thisNorthDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorNorth);
						float thisEastDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorEast);

						Vector2 uv = new Vector2(MathHelper.InverseLerpNoClamp(westernDistance, easternDistance, thisEastDistance),
							MathHelper.InverseLerpNoClamp(southernDistance, northernDistance, thisNorthDistance));

						builtPolygon.Vertices[vertexIndex].UV = uv;
					}
				}

				// Update the actual built mesh
				Vector3[] vertices = mapping.Mesh.vertices;

				Vector2[] uvs = mapping.Mesh.uv;

				for (int i = 0; i < mapping.VertexIndices.Count; i++) 
				{
					Vector3 worldPosition = vertices[mapping.VertexIndices[i]];

					float thisNorthDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorNorth);
					float thisEastDistance = Vector3.Dot(worldPosition - polygonCenterWorld, worldVectorEast);

					Vector2 uv = new Vector2(MathHelper.InverseLerpNoClamp(westernDistance, easternDistance, thisEastDistance),
						MathHelper.InverseLerpNoClamp(southernDistance, northernDistance, thisNorthDistance));

					uvs[mapping.VertexIndices[i]] = uv;
				}
				mapping.Mesh.uv = uvs;
			}
		}

		void Align(AlignDirection direction)
		{
			for (int polygonIndex = 0; polygonIndex < selectedSourcePolygons.Count; polygonIndex++) 
			{
				Polygon polygon = selectedSourcePolygons[polygonIndex];

				Transform brushTransform = matchedBrushes[polygon].transform;

				UVOrientation worldOrientation = GetNorthEastVectors(polygon, brushTransform);

				Vector3 worldVectorNorth = worldOrientation.NorthVector;
				Vector3 worldVectorEast = worldOrientation.EastVector;

				// These are flipped for some reason
				worldVectorNorth *= -1;
				worldVectorEast *= -1;

				Vector3 polygonCenterLocal = polygon.GetCenterPoint();
				Vector3 polygonCenterWorld = brushTransform.TransformPoint(polygonCenterLocal);

				// World vertices
				Polygon[] builtPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);
				Vertex northernVertex = builtPolygons[0].Vertices[0];
				Vertex southernVertex = builtPolygons[0].Vertices[0];
				Vertex easternVertex = builtPolygons[0].Vertices[0];
				Vertex westernVertex = builtPolygons[0].Vertices[0];


				for (int builtIndex = 0; builtIndex < builtPolygons.Length; builtIndex++) 
				{
					for (int i = 0; i < builtPolygons[builtIndex].Vertices.Length; i++) 
					{
						Vertex testVertex = builtPolygons[builtIndex].Vertices[i];

						float dotCurrent = Vector3.Dot(northernVertex.Position-polygonCenterWorld, worldVectorNorth);
						float dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, worldVectorNorth);
						if(dotTest > dotCurrent)
						{
							northernVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(southernVertex.Position-polygonCenterWorld, -worldVectorNorth);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, -worldVectorNorth);
						if(dotTest > dotCurrent)
						{
							southernVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(easternVertex.Position-polygonCenterWorld, worldVectorEast);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, worldVectorEast);
						if(dotTest > dotCurrent)
						{
							easternVertex = testVertex;
						}

						dotCurrent = Vector3.Dot(westernVertex.Position-polygonCenterWorld, -worldVectorEast);
						dotTest = Vector3.Dot(testVertex.Position-polygonCenterWorld, -worldVectorEast);
						if(dotTest > dotCurrent)
						{
							westernVertex = testVertex;
						}
					}
				}

				Vector2 offset = new Vector2(0,0);

				if(direction == AlignDirection.Top)
				{
					offset.y = 1-northernVertex.UV.y;
				}
				else if(direction == AlignDirection.Bottom)
				{
					offset.y = 0-southernVertex.UV.y;
				}
				else if(direction == AlignDirection.Left)
				{
					offset.x = 0-westernVertex.UV.x;
				}
				else if(direction == AlignDirection.Right)
				{
					offset.x = 1-easternVertex.UV.x;
				}

				TransformUVs(polygon, UVUtility.TranslateUV, new UVUtility.TransformData(offset,0));
			}
		}

		void UserExcludePolygon(Polygon sourcePolygon)
		{
			Polygon[] builtRenderPolygons = csgModel.BuiltPolygonsByIndex(sourcePolygon.UniqueIndex);

			foreach (Polygon polygon in builtRenderPolygons) 
			{
				polygon.UserExcludeFromFinal = true;
			}

			Polygon[] builtCollisionPolygons = csgModel.BuiltCollisionPolygonsByIndex(sourcePolygon.UniqueIndex);

			foreach (Polygon polygon in builtCollisionPolygons) 
			{
				polygon.UserExcludeFromFinal = true;
			}

			csgModel.PolygonDirectory.RemoveAndUpdateMesh(sourcePolygon);
			csgModel.CollisionPolygonDirectory.RemoveAndUpdateMesh(sourcePolygon);

			// Mesh colliders need to be refreshed now that their collision meshes have changed
			csgModel.RefreshMeshGroup();

			csgModel.SetContextDirty();
		}


		void UserIncludePolygon(Polygon sourcePolygon)
		{
			Polygon[] builtRenderPolygons = csgModel.BuiltPolygonsByIndex(sourcePolygon.UniqueIndex);
			AddAndUpdateMesh(csgModel.PolygonDirectory, false, sourcePolygon, builtRenderPolygons);

			Polygon[] builtCollisionPolygons = csgModel.BuiltCollisionPolygonsByIndex(sourcePolygon.UniqueIndex);
			AddAndUpdateMesh(csgModel.CollisionPolygonDirectory, true, sourcePolygon, builtCollisionPolygons);

			// Mesh colliders need to be refreshed now that their collision meshes have changed
			csgModel.RefreshMeshGroup();

			csgModel.SetContextDirty();
		}

		public void AddAndUpdateMesh(PolygonDirectory polygonDirectory, bool isCollisionMesh, Polygon sourcePolygon, Polygon[] builtPolygons)
		{
			// Remove any old mapping
			polygonDirectory.RemoveMapping(sourcePolygon);

			foreach (Polygon polygon in builtPolygons) 
			{
				polygon.UserExcludeFromFinal = false;
				int verticesToAdd = polygon.Vertices.Length;

				Material searchMaterial = polygon.Material;
				if(searchMaterial == null)
				{
					searchMaterial = CSGModel.GetDefaultMaterial();
				}
				Mesh newMesh = null;
				if(isCollisionMesh)
				{
					newMesh = csgModel.GetMeshForCollision(verticesToAdd);
				}
				else
				{
					newMesh = csgModel.GetMeshForMaterial(searchMaterial, verticesToAdd);
				}

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

				int trianglesToAdd = verticesToAdd-2;

				List<int> newTriangles = new List<int>();

				List<int> triangleIndices = new List<int>();

				// Triangulate the n-sided polygon and allow vertex reuse by using indexed geometry
				for (int i = 2; i < verticesToAdd; i++)
				{
					int triangleStartIndex = newTriangles.Count + destTriangles.Length;

					newTriangles.Add(destVertexCount+0);
					newTriangles.Add(destVertexCount+(i-1));
					newTriangles.Add(destVertexCount+i);

					triangleIndices.Add(triangleStartIndex + 0);
					triangleIndices.Add(triangleStartIndex + 1);
					triangleIndices.Add(triangleStartIndex + 2);
				}

//				Vertex[] vertices = polygon.Vertices;

				List<int> vertexIndices = new List<int>();

				for (int i = 0; i < verticesToAdd; i++) 
				{
					destVertices[destVertexCount + i] = polygon.Vertices[i].Position;
					destUV[destVertexCount + i] = polygon.Vertices[i].UV;
					destNormals[destVertexCount + i] = polygon.Vertices[i].Normal;
					destColors[destVertexCount + i] = polygon.Vertices[i].Color;

					vertexIndices.Add(destVertexCount + i);
				}

				int destTrianglesCount = destTriangles.Length;

				System.Array.Resize(ref destTriangles, destTrianglesCount + trianglesToAdd * 3);

				for (int i = 0; i < trianglesToAdd * 3; i++) 
				{
					destTriangles[destTrianglesCount + i] = newTriangles[i]; //originalMapping.TriangleIndices[i];
				}


				newMesh.vertices = destVertices;
				newMesh.triangles = destTriangles;
				newMesh.uv = destUV;
				newMesh.colors32 = destColors;
				newMesh.normals = destNormals;

				polygonDirectory.AddMapping(sourcePolygon, newMesh, vertexIndices, triangleIndices);
			}
		}

		void TransferPolygon(Polygon polygon, Material destinationMaterial, Color32 destinationColor)
		{
			// Only attempt to transfer the polygon if it's to a different material!
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

			Polygon[] builtPolygons = csgModel.BuiltPolygonsByIndex(polygon.UniqueIndex);
			for (int i = 0; i < builtPolygons.Length; i++) 
			{
				builtPolygons[i].Material = destinationMaterial;
			}
			csgModel.SetContextDirty();
		}
    }
}
#endif