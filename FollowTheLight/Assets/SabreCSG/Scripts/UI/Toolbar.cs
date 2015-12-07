#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Sabresaurus.SabreCSG
{
    public static class Toolbar
    {
        const int BOTTOM_TOOLBAR_HEIGHT = 40;

        static CSGModel csgModel;

		static string warningMessage = "Concave brushes detected";

		static Rect createRect;
		static Rect gridRect;

        public static CSGModel CSGModel
        {
            get
            {
                return csgModel;
            }
            set
            {
                csgModel = value;
            }
        }

		public static string WarningMessage {
			get {
				return warningMessage;
			}
			set {
				warningMessage = value;
			}
		}

		public static void OnSceneGUI (SceneView sceneView, Event e)
		{
			if (e.type == EventType.Repaint || e.type == EventType.Layout)
			{
				OnRepaint(sceneView, e);
			}
		}

		private static void OnRepaint(SceneView sceneView, Event e)
        {
            Rect rectangle = new Rect(0, sceneView.position.height - BOTTOM_TOOLBAR_HEIGHT, sceneView.position.width, BOTTOM_TOOLBAR_HEIGHT);

            GUIStyle style = new GUIStyle(EditorStyles.toolbar);

            style.fixedHeight = BOTTOM_TOOLBAR_HEIGHT;
			GUILayout.Window(140003, rectangle, OnBottomToolbarGUI, "", style);//, EditorStyles.textField);

			style = new GUIStyle(EditorStyles.toolbar);

			style.normal.background = SabreGraphics.ClearTexture;
			rectangle = new Rect(0, 20, 320, 50);
			GUILayout.Window(140004, rectangle, OnTopToolbarGUI, "", style);

			if(!string.IsNullOrEmpty(warningMessage))
			{				
				style.fixedHeight = 70;
				rectangle = new Rect(0, sceneView.position.height - BOTTOM_TOOLBAR_HEIGHT - style.fixedHeight, sceneView.position.width, style.fixedHeight);
				GUILayout.Window(140005, rectangle, OnWarningToolbar, "", style);
			}
            
        }

        private static void OnTopToolbarGUI(int windowID)
        {
            csgModel.SetCurrentMode(SabreGUILayout.DrawEnumGrid(CurrentSettings.CurrentMode, GUILayout.Width(50)));
        }

		private static void OnWarningToolbar(int windowID)
		{
			GUIStyle style = SabreGUILayout.GetOverlayStyle();
			Vector2 size = style.CalcSize(new GUIContent(warningMessage));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Box(warningMessage, style, GUILayout.Width(size.x));//, GUILayout.Width(size.x + 30), GUILayout.Height(50));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
//			GUIStyle style = SabreGUILayout.FormatStyle(Color.black, 20, TextAnchor.MiddleCenter);
//			GUILayout.Label(warningMessage, style);
//			Rect rect = GUILayoutUtility.GetLastRect();
//			rect.center -= Vector2.one;
//			style.normal.textColor = Color.red;
//
//			GUI.Label(rect, warningMessage, style);
		}

		static void OnSelectedCreateOption(object userData)
		{
			if(userData.GetType() == typeof(PrimitiveBrushType))
			{
				csgModel.CreateBrush((PrimitiveBrushType)userData);
			}
		}

		static void OnSelectedGridOption(object userData)
		{
			if(userData.GetType() == typeof(GridMode))
			{
				CurrentSettings.GridMode = (GridMode)userData;
			}
		}

        private static void OnBottomToolbarGUI(int windowID)
        {
            GUILayout.BeginHorizontal();

			// For debugging frame rate
//			GUILayout.Label(((int)(1 / csgModel.CurrentFrameDelta)).ToString(), SabreGUILayout.GetLabelStyle());

			if(GUILayout.Button("Create", EditorStyles.toolbarDropDown))
			{
				GenericMenu menu = new GenericMenu ();

				string[] names = Enum.GetNames(typeof(PrimitiveBrushType));

				for (int i = 0; i < names.Length; i++) 
				{
					if(names[i] != "Custom")
					{
						menu.AddItem (new GUIContent (names[i]), false, OnSelectedCreateOption, Enum.Parse(typeof(PrimitiveBrushType), names[i]));
					}
				}

				menu.DropDown(createRect);
			}

			if (Event.current.type == EventType.Repaint)
			{
				createRect = GUILayoutUtility.GetLastRect();
				createRect.width = 100;
			}


            if (SabreGUILayout.Button("Rebuild"))
            {
				csgModel.Build();
            }

			// DISABLED as no longer necessary - CurrentSettings.Optimizations = (Optimizations)EditorGUILayout.EnumMaskField(CurrentSettings.Optimizations, GUILayout.Width(100));

            bool lastBrushesHidden = CurrentSettings.BrushesHidden;
			if(lastBrushesHidden)
			{
				GUI.color = Color.red;
			}
            CurrentSettings.BrushesHidden = SabreGUILayout.Toggle(CurrentSettings.BrushesHidden, "Brushes Hidden");
            if (CurrentSettings.BrushesHidden != lastBrushesHidden)
            {
                // Has changed
                csgModel.UpdateBrushVisibility();
                SceneView.RepaintAll();
            }


			GUI.color = Color.white;
			bool lastMeshHidden = CurrentSettings.MeshHidden;
			if(lastMeshHidden)
			{
				GUI.color = Color.red;
			}
			CurrentSettings.MeshHidden = SabreGUILayout.Toggle(CurrentSettings.MeshHidden, "Mesh Hidden");
			if (CurrentSettings.MeshHidden != lastMeshHidden)
			{
				// Has changed
				csgModel.UpdateBrushVisibility();
				SceneView.RepaintAll();
			}

			GUI.color = Color.white;

			
			if(GUILayout.Button("Grid " + CurrentSettings.GridMode.ToString(), EditorStyles.toolbarDropDown, GUILayout.Width(90)))
			{
				GenericMenu menu = new GenericMenu ();
				
				string[] names = Enum.GetNames(typeof(GridMode));
				
				for (int i = 0; i < names.Length; i++) 
				{
					GridMode value = (GridMode)Enum.Parse(typeof(GridMode),names[i]);
					bool selected = false;
					if(CurrentSettings.GridMode == value)
					{
						selected = true;
					}
					menu.AddItem (new GUIContent (names[i]), selected, OnSelectedGridOption, value);
				}
				
				menu.DropDown(gridRect);
			}

			if (Event.current.type == EventType.Repaint)
			{
				gridRect = GUILayoutUtility.GetLastRect();
				gridRect.width = 100;
			}

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Line Two
            GUILayout.BeginHorizontal();

			// Display brush count
			GUILayout.Label(csgModel.BrushCount.ToStringWithSuffix(" brush", " brushes"), SabreGUILayout.GetLabelStyle());
//			CurrentSettings.GridMode = (GridMode)EditorGUILayout.EnumPopup(CurrentSettings.GridMode, EditorStyles.toolbarPopup, GUILayout.Width(80));

            if (Selection.activeGameObject != null)
            {
                Brush brush = Selection.activeGameObject.GetComponent<Brush>();
                if (brush != null)
                {
					CSGMode brushMode = (CSGMode)EditorGUILayout.EnumPopup(brush.Mode, EditorStyles.toolbarPopup, GUILayout.Width(80));
					if(brushMode != brush.Mode)
					{
						Undo.RecordObject(brush, "Change Brush Mode To " + brushMode);
						
						brush.Mode = brushMode;
					}

					bool isDetailBrush = SabreGUILayout.Toggle(brush.IsDetailBrush, "Detail", GUILayout.Width(53));
					bool hasCollision = SabreGUILayout.Toggle(brush.HasCollision, "Collision", GUILayout.Width(53));
					bool isVisible = SabreGUILayout.Toggle(brush.IsVisible, "Visible", GUILayout.Width(53));

					if(brush.IsDetailBrush != isDetailBrush)
					{
						Undo.RecordObject(brush, "Change Brush Detail Mode");
						brush.IsDetailBrush = isDetailBrush;
					}
					if(brush.HasCollision != hasCollision)
					{
						Undo.RecordObject(brush, "Change Brush Collision Mode");
						brush.HasCollision = hasCollision;
					}
					if(brush.IsVisible != isVisible)
					{
						Undo.RecordObject(brush, "Change Brush Visible Mode");
						brush.IsVisible = isVisible;
					}
                }
            }

			GUILayout.Space(10);

			// Position snapping UI
			CurrentSettings.PositionSnappingEnabled = SabreGUILayout.Toggle(CurrentSettings.PositionSnappingEnabled, "Pos Snapping");
			CurrentSettings.PositionSnapDistance = EditorGUILayout.FloatField(CurrentSettings.PositionSnapDistance, GUILayout.Width(50));
			
			if (SabreGUILayout.Button("-", EditorStyles.miniButtonLeft))
			{
				CurrentSettings.ChangePosSnapDistance(.5f);
			}
			if (SabreGUILayout.Button("+", EditorStyles.miniButtonRight))
			{
				CurrentSettings.ChangePosSnapDistance(2f);
			}

			// Rotation snapping UI
			CurrentSettings.AngleSnappingEnabled = SabreGUILayout.Toggle(CurrentSettings.AngleSnappingEnabled, "Ang Snapping");
			CurrentSettings.AngleSnapDistance = EditorGUILayout.FloatField(CurrentSettings.AngleSnapDistance, GUILayout.Width(50));

			if (SabreGUILayout.Button("-", EditorStyles.miniButtonLeft))
			{
				CurrentSettings.ChangeAngSnapDistance(.5f);
			}
			if (SabreGUILayout.Button("+", EditorStyles.miniButtonRight))
			{
				CurrentSettings.ChangeAngSnapDistance(2f);
			}

// Disabled test build options
//			CurrentSettings.RestoreOriginalPolygons = SabreGUILayout.Toggle(CurrentSettings.RestoreOriginalPolygons, "Restore Original Polygons", GUILayout.Width(153));
//			CurrentSettings.RemoveHiddenGeometry = SabreGUILayout.Toggle(CurrentSettings.RemoveHiddenGeometry, "Remove Hidden Geometry", GUILayout.Width(153));

			GUILayout.FlexibleSpace();

            if (CurrentSettings.CurrentMode != MainMode.Free)
			{
				if( Tools.current == UnityEditor.Tool.View && Tools.viewTool == ViewTool.Pan)
				{
					GUI.color = Color.yellow;
				}
			}

			if (SabreGUILayout.Button("Disable"))
			{
				Selection.activeGameObject = null;
				csgModel.EditMode = false;
			}

            GUILayout.EndHorizontal();
        }
    }
}
#endif