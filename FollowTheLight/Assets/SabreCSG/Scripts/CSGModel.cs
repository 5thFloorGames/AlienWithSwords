#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Sabresaurus.SabreCSG
{
    [ExecuteInEditMode, RequireComponent(typeof(CSGModelRuntime))]
    public class CSGModel : MonoBehaviour
    {
		public const string VERSION_STRING = "1.0.0";

		// Limit to how many vertices a Unity mesh can hold, before it must be split into a second mesh (just under 2^16)
		const int MESH_VERTEX_LIMIT = 65500; 

        [SerializeField,HideInInspector] 
		List<Brush> brushes = new List<Brush>(); // Store the sequence of brushes and their operation (e.g. add, subtract)

		[SerializeField,HideInInspector]
		List<Brush> builtBrushes = new List<Brush>();

		[SerializeField,HideInInspector]
		MaterialMeshDictionary materialMeshDictionary;

		[SerializeField]
		CSGBuildSettings buildSettings = new CSGBuildSettings();

		bool editMode = false;
		
		bool mouseIsDragging = false;
		bool mouseIsHeld = false;

		// A reference to a component which holds a lot of build time data that helps change built geometry on the fly
		// This is used by the surface tools heavily.
		CSGBuildContext.BuildContext buildContext;

        // Tools
		Tool activeTool = null;

		// Used to track what objects have been previously clicked on, so that the user can cycle click through objects
		// on the same (or similar) ray cast
		List<GameObject> previousHits = new List<GameObject>();

		Dictionary<MainMode, Tool> tools = new Dictionary<MainMode, Tool>()
		{
			{ MainMode.Resize, new ResizeEditor() },
			{ MainMode.Vertex, new VertexEditor() },
			{ MainMode.Face, new SurfaceEditor() },
			{ MainMode.Clip, new ClipEditor() },
		};

		public BuildMetrics BuildMetrics
		{
			get
			{
				return buildContext.buildMetrics;
			}
		}

		public int BrushCount 
		{
			get
			{
				return brushes.Count;
			}
		}

		// Used by the face editor to determine where in the built meshes a source polygon has ended up
		public PolygonDirectory PolygonDirectory 
		{
			get 
			{
				return buildContext.polygonDirectory;
			}
		}

        public bool EditMode
        {
            get
            {
                return this.editMode;
            }
            set
            {
				// Has edit mode changed
                if (editMode != value)
                {
                    if (value == true) // Edit mode enabled
                    {
						// Default to resize mode
                        SetCurrentMode(MainMode.Resize);

						// Bind listeners
						EditorApplication.update += OnEditorUpdate;
						EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
                        SceneView.onSceneGUIDelegate += OnSceneGUI;

						// Force the scene views to repaint (shows our own UI)
                        SceneView.RepaintAll();
                    }
                    else // Edit mode disabled
                    {
                        SetCurrentMode(MainMode.Free);

						// Unbind listeners
						EditorApplication.update -= OnEditorUpdate;
						EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
                        SceneView.onSceneGUIDelegate -= OnSceneGUI;

						// Force the scene views to repaint (hides our own UI)
						SceneView.RepaintAll();
                        HandleUtility.Repaint();
                    }
                }

                editMode = value;
            }
        }

		public bool MouseIsDragging {
			get {
				return mouseIsDragging;
			}
		}

        void Start()
        {
			CSGModel[] csgModels = FindObjectsOfType<CSGModel>();
			if(csgModels.Length > 1)
			{
				Debug.LogError("SabreCSG currently supports 1 CSGModel per scene.");
				DestroyImmediate (this.gameObject);
				return;
			}

			// Get a reference to the build context (which holds post build helper data)
			CSGBuildContext csgBuildContext = this.AddOrGetComponent<CSGBuildContext>();
			buildContext = csgBuildContext.GetBuildContext();

			// Make sure editing is turned on
			EditMode = true;
        }

		public void Build ()
		{
			brushes = new List<Brush>(transform.GetComponentsInChildren<Brush>(false));

			CSGFactory.Build(brushes, buildSettings, buildContext, this.transform, ref materialMeshDictionary);

			UpdateBrushVisibility();

//			Debug.Log("Model build complete.");
//			Debug.Log("Total Meshes: " + buildContext.buildMetrics.TotalMeshes);
//			Debug.Log("Total Vertices: " + buildContext.buildMetrics.TotalVertices);
//			Debug.Log("Total Triangles: " + buildContext.buildMetrics.TotalTriangles);
			Debug.Log("Build Complete. Time Taken: " + buildContext.buildMetrics.BuildTime.ToString() + " - Total Triangles: " + buildContext.buildMetrics.TotalTriangles);

			// Mark the brushes that have been built (so we can differentiate later if new brushes are built or not)
			builtBrushes.Clear();
			builtBrushes.AddRange(brushes);

			EditorHelper.SetDirty(this);
		}

		public Mesh GetMeshForMaterial(Material sourceMaterial, int fitVertices = 0)
		{
			if(materialMeshDictionary.Contains(sourceMaterial))
			{
				Mesh lastMesh = materialMeshDictionary[sourceMaterial].Last();
				if(lastMesh.vertices.Length + fitVertices < MESH_VERTEX_LIMIT)
				{
					return lastMesh;
				}
			}

			Mesh mesh = new Mesh();
			materialMeshDictionary.Add(sourceMaterial, mesh);
			CSGFactory.CreateMaterialMesh(this.transform, sourceMaterial,mesh);
			return mesh;
		}

        /// <summary>
        /// Called to alert the CSG Model that a new brush has been created
        /// </summary>
        public bool TrackBrush(Brush brush)
        {
            // If we don't already know about the brush, add it
            if (!brushes.Contains(brush))
            {
                brushes.Add(brush);
				return true;
            }
			else
			{
				return false;
			}
        }

		float currentFrameTimestamp = 0;
		float currentFrameDelta = 0;

		public float CurrentFrameDelta {
			get {
				return currentFrameDelta;
			}
		}

        public void OnSceneGUI(SceneView sceneView)
        {
			Event e = Event.current;

			// Frame rate tracking
			if(e.type == EventType.Repaint)
			{
				currentFrameDelta = Time.realtimeSinceStartup - currentFrameTimestamp;
				currentFrameTimestamp = Time.realtimeSinceStartup;
			}

			// Raw checks for tracking mouse events (use raw so that consumed events are not ignored)
			if (e.rawType == EventType.MouseDown)
			{
				mouseIsDragging = false;
				mouseIsHeld = true;

				if(e.button == 0 && GUIUtility.hotControl == 0 )
				{
					GUIUtility.keyboardControl = 0;
				}

			}
			else if (e.rawType == EventType.MouseDrag)
			{
				mouseIsDragging = true;
			}
			else if (e.rawType == EventType.MouseUp)
			{
				mouseIsHeld = false;
			}

			if (e.type == EventType.Repaint)
			{
				if(CurrentSettings.Instance.GridMode == GridMode.SabreCSG)
				{
					CSGGrid.Draw();
				}
			}

            if (CurrentSettings.Instance.BrushesVisible)
            {
                // No idea what this line of code means, but it seems to stop normal mouse selection
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (CurrentSettings.Instance.CurrentMode != MainMode.Free)
            {
                // In CSG mode, prevent the normal tools, so that the user must use our tools instead
                Tools.current = UnityEditor.Tool.None;
            }

			int concaveBrushCount = 0;
			for (int i = 0; i < brushes.Count; i++) 
			{
				if(brushes[i] != null && !brushes[i].IsBrushConvex)
				{
					concaveBrushCount++;
				}
			}
			if(concaveBrushCount > 0)
			{
				Toolbar.WarningMessage = concaveBrushCount + " Concave Brush" + (concaveBrushCount > 1 ? "es" : "") + " Detected";
			}
			else
			{
				Toolbar.WarningMessage = "";
			}

			Toolbar.CSGModel = this;
			Toolbar.OnSceneGUI(sceneView, e);

            if (e.type == EventType.Repaint)// || e.type == EventType.Layout)
            {
				if (CurrentSettings.Instance.BrushesVisible && tools[CurrentSettings.Instance.CurrentMode].BrushesHandleDrawing)
                {
					SabreGraphics.GetSelectedBrushMaterial().SetPass(0);
					// Selection
					GL.Begin(GL.LINES);
					Color outlineColor = Color.blue;

					for (int brushIndex = 0; brushIndex < brushes.Count; brushIndex++) 
					{
						Brush brush = brushes[brushIndex];
						if(brush == null)
						{
							continue;
						}
						GameObject brushGameObject = brush.gameObject;

						if(!brushGameObject.activeInHierarchy)
						{
							continue;
						}

						if (Selection.Contains(brushGameObject))
						{
							if (brushes[brushIndex].Mode == CSGMode.Add)
							{
								outlineColor = Color.cyan;
							}
							else
							{
								outlineColor = Color.yellow;
							}
						}
						else
						{
							if (brushes[brushIndex].Mode == CSGMode.Add)
							{
								outlineColor = Color.blue;
							}
							else
							{
								outlineColor = new Color32(255, 130, 0, 255);
							}
						}
						
						GL.Color(outlineColor);

						Polygon[] polygons = brush.GetPolygons();
						Transform brushTransform = brush.transform;

						// Brush Outline
						for (int i = 0; i < polygons.Length; i++)
						{
							Polygon polygon = polygons[i];
							
							for (int j = 0; j < polygon.Vertices.Length; j++)
							{
								Vector3 position = brushTransform.TransformPoint(polygon.Vertices[j].Position);
								GL.Vertex(position);
								
								if (j < polygon.Vertices.Length - 1)
								{
									Vector3 position2 = brushTransform.TransformPoint(polygon.Vertices[j + 1].Position);
									GL.Vertex(position2);
								}
								else
								{
									Vector3 position2 = brushTransform.TransformPoint(polygon.Vertices[0].Position);
									GL.Vertex(position2);
								}
							}
						}
					}

					GL.End();

                    for (int i = 0; i < brushes.Count; i++)
                    {
                        if (brushes[i] is PrimitiveBrush && brushes[i] != null && brushes[i].gameObject.activeInHierarchy)
                        {
                            ((PrimitiveBrush)brushes[i]).OnRepaint(sceneView, e);
                        }
                    }
                }
            }

            if (e.type == EventType.Repaint)
            {
                Rect rect = new Rect(0, 0, Screen.width, Screen.height);
                EditorGUIUtility.AddCursorRect(rect, SabreMouse.ActiveCursor);
            }
            //

            //		int hotControl = GUIUtility.hotControl;
            //		if(hotControl != 0)
            //			Debug.Log (hotControl);
            //		Tools.viewTool = ViewTool.None;

			PrimitiveBrush primitiveBrush = null;
			if(Selection.activeGameObject != null)
			{
				primitiveBrush = Selection.activeGameObject.GetComponent<PrimitiveBrush>();
			}

			Tool lastTool = activeTool;

			if(tools.ContainsKey(CurrentSettings.Instance.CurrentMode))
			{
				activeTool = tools[CurrentSettings.Instance.CurrentMode];
			}
			else
			{
				activeTool = null;
			}

			if(activeTool != null)
			{
				activeTool.CSGModel = this;
				activeTool.TargetBrush = primitiveBrush;
				activeTool.OnSceneGUI(sceneView, e);

				if(activeTool != lastTool)
				{
					if(lastTool != null)
					{
						lastTool.Deactivated();
					}
					activeTool.ResetTool();
				}
			}

			if(e.type == EventType.DragPerform)
			{
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				
				RaycastHit hit = new RaycastHit();
				
				int layerMask = 1 << LayerMask.NameToLayer("CSGMesh");
				// Invert the layer mask
				layerMask = ~layerMask;
				
				// Shift mode means only add what they click (clicking nothing does nothing)
				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask))
				{
//					OnDragDrop(hit.collider.gameObject);
				}
			}

            if (e.type == EventType.MouseDown)
            {
            }
            else if (e.type == EventType.MouseDrag)
            {
            }
            else if (e.type == EventType.MouseUp)
            {
                OnMouseUp(sceneView, e);
                SabreMouse.ResetCursor();
            }
            else if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                OnKeyAction(sceneView, e);
            }
        }

        void OnMouseUp(SceneView sceneView, Event e)
        {
            if (mouseIsDragging || CurrentSettings.Instance.CurrentMode == MainMode.Free || EditorHelper.IsMousePositionNearSceneGizmo(e.mousePosition))
            {
                return;
            }

            // Left click - select
            if (e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                RaycastHit hit = new RaycastHit();

                int layerMask = 1 << LayerMask.NameToLayer("CSGMesh");
                // Invert the layer mask
                layerMask = ~layerMask;

                if (EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift)
                   || EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control)
                   || EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Command))
                {
                    // Shift mode means only add what they click (clicking nothing does nothing)
                    if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask))
                    {
                        if (Selection.objects == null || Selection.objects.Length == 0)
                        {
                            // Didn't already have a selection, so short cut via activeTransform
                            Selection.activeObject = hit.transform.gameObject;
                        }
                        else
                        {
                            List<UnityEngine.Object> objects = new List<UnityEngine.Object>(Selection.objects);

                            if (objects.Contains(hit.transform.gameObject))
                            {
                                objects.Remove(hit.transform.gameObject);
                            }
                            else
                            {
                                objects.Add(hit.transform.gameObject);
                            }
                            Selection.objects = objects.ToArray();
                        }
                    }
                }
                else
                {
					List<RaycastHit> hits = RaycastBrushesAll(ray);

					GameObject selectedObject = null;

					if(hits.Count == 0) // Didn't hit anything, blank the selection
					{
						previousHits.Clear();
					}
					else if(hits.Count == 1) // Only hit one thing, no ambiguity, this is what is selected
					{
						selectedObject = hits[0].collider.gameObject;
						previousHits.Clear();
					}
					else
					{
						// First try and select anything other than what has been previously hit
						for (int i = 0; i < hits.Count; i++) 
						{
							if(!previousHits.Contains(hits[i].collider.gameObject))
							{
								selectedObject = hits[i].collider.gameObject;
								break;
							}
						}

						// Only found previously hit objects
						if(selectedObject == null)
						{
							// Walk backwards to find the oldest previous hit that has been hit by this ray
							for (int i = previousHits.Count-1; i >= 0 && selectedObject == null; i--) 
							{
								for (int j = 0; j < hits.Count; j++) 
								{
									if(hits[j].collider.gameObject == previousHits[i])
									{
										selectedObject = previousHits[i];
										break;
									}
								}
							}
						}
					}

					Selection.activeGameObject = selectedObject;
					if(selectedObject != null)
					{
						previousHits.Remove(selectedObject);
						// Most recent hit
						previousHits.Insert(0, selectedObject);
					}
                }
                e.Use();
            }
        }

		public Polygon RaycastBuiltPolygons(Ray ray)
		{
			if(buildContext.visualPolygons != null)
			{
				float distance = 0;
				return GeometryHelper.RaycastPolygons(buildContext.visualPolygons, ray, out distance);
			}
			else
			{
				return null;
			}
		}

		public Brush FindBrushFromPolygon(Polygon sourcePolygon)
		{
			// Find which brush contains the source polygon
			for (int i = 0; i < brushes.Count; i++) 
			{
				if(brushes[i].GetPolygonIDs().Contains(sourcePolygon.UniqueIndex))
				{
					return brushes[i];
				}
			}

			// None found
			return null;
		}

		// Consider getting rid of this accessor!
		public List<Polygon> VisualPolygons
		{
			get
			{
				return buildContext.visualPolygons;
			}
		}

		public Polygon[] BuiltPolygonsByIndex(int uniquePolygonIndex)
		{
			return buildContext.visualPolygons.Where(poly => (poly.UniqueIndex == uniquePolygonIndex && !poly.ExcludeFromFinal)).ToArray();
		}

		public List<RaycastHit> RaycastBrushesAll(Ray ray)
		{
			int layerMask = 1 << LayerMask.NameToLayer("CSGMesh");
			// Invert the layer mask
			layerMask = ~layerMask;

			List<RaycastHit> hits = Physics.RaycastAll(ray, float.PositiveInfinity, layerMask).ToList();

			// Trim out any calculated collision meshes that have been hit
			for (int i = 0; i < hits.Count; i++) 
			{
				if(hits[i].collider.name == "CollisionMesh")
				{
					hits.RemoveAt (i);
					i--;
				}
			}

			// Trim out duplicate colliders on the same game object
			for (int j = 0; j < hits.Count; j++) 
			{
				for (int i = 0; i < hits.Count; i++) 
				{
					if(i != j)
					{
						if(hits[i].collider.gameObject == hits[j].collider.gameObject)
						{
							hits.RemoveAt (j);
							j--;
							break;
						}
					}
				}
			}

			hits.Sort((x,y) => x.distance.CompareTo(y.distance));
			return hits;
		}

        /// <summary>
        /// Subscribes to both KeyDown and KeyUp events from the SceneView delegate. This allows us to easily store key
        /// events in one place and mark them as used as necessary (for example to prevent error sounds on key down)
        /// </summary>
        void OnKeyAction(SceneView sceneView, Event e)
        {
            if (e.keyCode == KeyMappings.Instance.ToggleMode
			    && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Control)
               && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Alt)
               && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Command)
               && !EnumHelper.IsFlagSet(e.modifiers, EventModifiers.FunctionKey)
//			    && GUIUtility.keyboardControl == 0
               )
            {
                // Toggle mode - immediately (key down)
                if (e.type == EventType.KeyDown)
                {
                    int currentModeInt = (int)CurrentSettings.Instance.CurrentMode;
                    int count = Enum.GetNames(typeof(MainMode)).Length;

                    if (EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift))
                    {
                        currentModeInt--;
                    }
                    else
                    {
                        currentModeInt++;
                    }

                    if (currentModeInt >= count)
                    {
                        currentModeInt = 0;
                    }
                    else if (currentModeInt < 0)
                    {
                        currentModeInt = count - 1;
                    }
                    SetCurrentMode((MainMode)currentModeInt);

                    SceneView.RepaintAll();
                }
                e.Use();
            }
//            else if (e.keyCode == KeyCode.R && EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift)) // Rebuild
//            {
//                if (e.type == EventType.KeyUp)
//                {
//                    Build();
//                }
//                e.Use();
//            }
            else if (e.keyCode == KeyCode.Period)
            {
                if (e.type == EventType.KeyUp)
                {
                    CurrentSettings.Instance.ChangePosSnapDistance(2f);
                }
                e.Use();
            }
            else if (e.keyCode == KeyCode.Comma)
            {
                if (e.type == EventType.KeyUp)
                {
					CurrentSettings.Instance.ChangePosSnapDistance(.5f);
                }
                e.Use();
            }
            else if (e.keyCode == KeyCode.Slash)
            {
                if (e.type == EventType.KeyUp)
                {
					CurrentSettings.Instance.PositionSnappingEnabled = !CurrentSettings.Instance.PositionSnappingEnabled;
                }
                e.Use();
			}
			else if(!mouseIsHeld && e.modifiers == EventModifiers.None && (e.keyCode == KeyCode.KeypadPlus || e.keyCode == KeyCode.A))
			{
				if (e.type == EventType.KeyDown)
				{
					if (Selection.activeGameObject != null)
					{
						Brush brush = Selection.activeGameObject.GetComponent<Brush>();
						if (brush != null)
						{
							Undo.RecordObject(brush, "Change Brush To Add");
							brush.Mode = CSGMode.Add;
							// Need to update the icon for the csg mode in the hierarchy
							EditorApplication.RepaintHierarchyWindow();
						}
					}
				}
				e.Use();
			}
			else if(!mouseIsHeld && e.modifiers == EventModifiers.None && (e.keyCode == KeyCode.KeypadMinus || e.keyCode == KeyCode.S))
			{
				if (e.type == EventType.KeyDown)
				{
					if (Selection.activeGameObject != null)
					{
						Brush brush = Selection.activeGameObject.GetComponent<Brush>();
						if (brush != null)
						{
							Undo.RecordObject(brush, "Change Brush To Subtract");
							brush.Mode = CSGMode.Subtract;
							// Need to update the icon for the csg mode in the hierarchy
							EditorApplication.RepaintHierarchyWindow();
						}
					}
				}
				e.Use();
			}
//			else if(e.keyCode == KeyCode.R && EnumHelper.IsFlagSet(e.modifiers, EventModifiers.Shift))
//			{
//				if (e.type == EventType.KeyUp)
//				{
//					Build ();
//				}
//				e.Use();
//			}	
        }

        public void SetCurrentMode(MainMode newMode)
        {
            if (newMode != CurrentSettings.Instance.CurrentMode)
            {
                CurrentSettings.Instance.CurrentMode = newMode;

                UpdateBrushVisibility();
            }
        }

        public void UpdateBrushVisibility()
        {
            for (int i = 0; i < brushes.Count; i++)
            {
                if (brushes[i] != null)
                {
					brushes[i].SetVisible(CurrentSettings.Instance.BrushesVisible && tools[CurrentSettings.Instance.CurrentMode].BrushesHandleDrawing);
                }
            }

			Transform meshGroup = transform.FindChild("MeshGroup");

			if(meshGroup != null)
			{
				meshGroup.gameObject.SetActive(!CurrentSettings.Instance.MeshHidden);
			}
        }

		void OnProjectItemGUI (string guid, Rect selectionRect)
		{
//										Debug.Log(Event.current.type.ToString());
			/*
			if (Event.current.type == EventType.MouseDrag)
			{
				if(selectionRect.Contains(Event.current.mousePosition))
				{
					//					Debug.Log(Event.current.type.ToString());
					string path = AssetDatabase.GUIDToAssetPath (guid);
					if(!string.IsNullOrEmpty(path))
					{
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.paths = new string[] { path };

						DragAndDrop.StartDrag ("Dragging material");
						
						// Make sure no one else uses this event
						Event.current.Use();
					}
				}
			}
			*/
		}

		void OnDragDrop(GameObject gameObject)
		{
//			PrimitiveBrush brush = gameObject.GetComponent<PrimitiveBrush>();
//			
//			if(brush != null)
//			{
//				if(DragAndDrop.objectReferences.Length == 1)
//				{
//					if(DragAndDrop.objectReferences[0] is Material)
//					{
//						brush.Material = (Material)DragAndDrop.objectReferences[0];
//						DragAndDrop.AcceptDrag();
//						Event.current.Use();
//					}
//				}
//			}
		}

		void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
		{
			GameObject gameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;

			if(Event.current.type == EventType.DragPerform)
			{
				if(selectionRect.Contains(Event.current.mousePosition))
				{
					if(gameObject != null)
					{
						OnDragDrop(gameObject);
					}
				}
			}


			if(gameObject != null)
			{
				Brush brush = gameObject.GetComponent<Brush>();
				if(brush != null)
				{
					selectionRect.xMax -= 2;
					selectionRect.xMin = selectionRect.xMax - 16;
					selectionRect.height = 16;

					if(brush.Mode == CSGMode.Add)
					{
						GUI.DrawTexture(selectionRect, SabreGraphics.AddIconTexture);
					}
					else
					{
						GUI.DrawTexture(selectionRect, SabreGraphics.SubtractIconTexture);
					}
				}
			}
		}

		void OnEditorUpdate ()
		{

		}

        void Update()
        {
            if (EditMode)
            {
				// Make sure the events we need to listen for are all bound (recompilation removes listeners, so it is
				// necessary to rebind dynamically)
				if(!EditorHelper.SceneViewHasDelegate(OnSceneGUI))
				{
	                // Then resubscribe and repaint
	                SceneView.onSceneGUIDelegate += OnSceneGUI;
	                SceneView.RepaintAll();
				}

				if(!EditorHelper.HasDelegate(EditorApplication.hierarchyWindowItemOnGUI, (EditorApplication.HierarchyWindowItemCallback)OnHierarchyItemGUI))
				{
					EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
				}

				if(!EditorHelper.HasDelegate(EditorApplication.projectWindowItemOnGUI, (EditorApplication.ProjectWindowItemCallback)OnProjectItemGUI))
				{
					EditorApplication.projectWindowItemOnGUI += OnProjectItemGUI;
				}

				if(!EditorHelper.HasDelegate(Undo.undoRedoPerformed, (Undo.UndoRedoCallback)OnUndoRedoPerformed))
				{
					Undo.undoRedoPerformed += OnUndoRedoPerformed;					
				}
            }

			// Track whether all the brushes have been destroyed
			bool anyBrushes = false;

			if(brushes != null)
			{
				for (int i = 0; i < brushes.Count; i++) 
				{
					if(brushes[i] != null)
					{
						anyBrushes = true;
						break;
					}
				}
			}

			// All the brushes have been destroyed so add a default cube brush
			if(!Application.isPlaying && !anyBrushes)
			{
				// Create the default brush
				CreateBrush(PrimitiveBrushType.Cube);
			}
        }

		public bool HasBrushBeenBuilt(Brush candidateBrush)
		{
			return builtBrushes.Contains(candidateBrush);
		}

        public void OnDrawGizmosSelected()
        {
            // Ensure Edit Mode is on
            EditMode = true;
        }

		public void OnUndoRedoPerformed()
		{
			for (int i = 0; i < brushes.Count; i++) 
			{
				if(brushes[i] != null)
				{
					brushes[i].OnUndoRedoPerformed();
				}
			}
		}

        void OnDestroy()
        {
			EditorApplication.update -= OnEditorUpdate;
			EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

		public void CreateBrush(PrimitiveBrushType brushType)
		{
			GameObject brushObject = new GameObject("AppliedBrush");
			brushObject.transform.parent = this.transform;
			PrimitiveBrush primitiveBrush = brushObject.AddComponent<PrimitiveBrush>();
			primitiveBrush.BrushType = brushType;
			primitiveBrush.ResetPolygons();
			Selection.activeGameObject = brushObject;
//			primitiveBrush.Material = AssetDatabase.LoadMainAssetAtPath("Assets/Textures/AngryBotsTextures/Materials/WallPanel_E.mat") as Material;
		}

		public Polygon GetSourcePolygon(int uniqueIndex)
		{
			for (int i = 0; i < brushes.Count; i++) 
			{
				if(brushes[i] != null)
				{
					Polygon[] polygons = brushes[i].GetPolygons();
					for (int j = 0; j < polygons.Length; j++) 
					{
						if(polygons[j].UniqueIndex == uniqueIndex)
						{
							return brushes[i].GetPolygons()[j];
						}
					}
				}
			}

			// None found
			return null;
		}
		
		public void ExportOBJ ()
		{
			string path = EditorUtility.SaveFilePanel("Save Geometry As OBJ", "Assets", this.name + ".obj", "obj");
			if(!string.IsNullOrEmpty(path))
			{
				OBJFactory.ExportToFile(path, buildContext.visualPolygons, null);
				AssetDatabase.Refresh();
			}
		}

        static CSGModel FindCSGModel()
        {
            CSGModel[] models = UnityEngine.Object.FindObjectsOfType<CSGModel>();
            if (models.Length > 0)
            {
                if (models.Length > 1)
                {
                    Debug.LogWarning("Multiple CSGModels detected in scene");
                }
                return models[0];
            }
            else
            {
                Debug.LogError("Couldn't find a CSGModel in the scene");
                return null;
            }
        }

		public class RayHitComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((RaycastHit) x).distance.CompareTo(((RaycastHit) y).distance);
			}
		}
    }
}
#endif