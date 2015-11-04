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
            GUILayout.Window(0, rectangle, OnBottomToolbarGUI, "", style);//, EditorStyles.textField);

            GUI.backgroundColor = Color.clear;
            rectangle = new Rect(0, 20, 320, 50);
            GUILayout.Window(1, rectangle, OnTopToolbarGUI, "", style);

			if(!string.IsNullOrEmpty(warningMessage))
			{
				GUI.backgroundColor = Color.clear;
				rectangle = new Rect(0, sceneView.position.height - BOTTOM_TOOLBAR_HEIGHT - 40, sceneView.position.width, 40);
				GUILayout.Window(2, rectangle, OnWarningToolbar, "", style);
			}
        }

        private static void OnTopToolbarGUI(int windowID)
        {
            csgModel.SetCurrentMode(SabreGUILayout.DrawEnumGrid(CurrentSettings.Instance.CurrentMode, GUILayout.Width(50)));
        }

		private static void OnWarningToolbar(int windowID)
		{
			GUIStyle style = SabreGUILayout.FormatStyle(Color.black, 20, TextAnchor.MiddleCenter);
			GUILayout.Label(warningMessage, style);
			Rect rect = GUILayoutUtility.GetLastRect();
			rect.center -= Vector2.one;
			style.normal.textColor = Color.red;
			GUI.Label(rect, warningMessage, style);
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
				CurrentSettings.Instance.GridMode = (GridMode)userData;
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
					menu.AddItem (new GUIContent (names[i]), false, OnSelectedCreateOption, Enum.Parse(typeof(PrimitiveBrushType), names[i]));
				}

				Rect rect = GUILayoutUtility.GetLastRect();
				rect.width = 100;

				menu.DropDown(rect);
			}

            if (SabreGUILayout.Button("Rebuild"))
            {
                csgModel.Build();
            }

			// DISABLED as no longer necessary - CurrentSettings.Instance.Optimizations = (Optimizations)EditorGUILayout.EnumMaskField(CurrentSettings.Instance.Optimizations, GUILayout.Width(100));

            bool lastBrushesHidden = CurrentSettings.Instance.BrushesHidden;
			if(lastBrushesHidden)
			{
				GUI.color = Color.red;
			}
            CurrentSettings.Instance.BrushesHidden = SabreGUILayout.Toggle(CurrentSettings.Instance.BrushesHidden, "Brushes Hidden");
            if (CurrentSettings.Instance.BrushesHidden != lastBrushesHidden)
            {
                // Has changed
                csgModel.UpdateBrushVisibility();
                SceneView.RepaintAll();
            }


			GUI.color = Color.white;
			bool lastMeshHidden = CurrentSettings.Instance.MeshHidden;
			if(lastMeshHidden)
			{
				GUI.color = Color.red;
			}
			CurrentSettings.Instance.MeshHidden = SabreGUILayout.Toggle(CurrentSettings.Instance.MeshHidden, "Mesh Hidden");
			if (CurrentSettings.Instance.MeshHidden != lastMeshHidden)
			{
				// Has changed
				csgModel.UpdateBrushVisibility();
				SceneView.RepaintAll();
			}

			GUI.color = Color.white;

			
			if(GUILayout.Button("Grid " + CurrentSettings.Instance.GridMode.ToString(), EditorStyles.toolbarDropDown, GUILayout.Width(90)))
			{
				GenericMenu menu = new GenericMenu ();
				
				string[] names = Enum.GetNames(typeof(GridMode));
				
				for (int i = 0; i < names.Length; i++) 
				{
					GridMode value = (GridMode)Enum.Parse(typeof(GridMode),names[i]);
					bool selected = false;
					if(CurrentSettings.Instance.GridMode == value)
					{
						selected = true;
					}
					menu.AddItem (new GUIContent (names[i]), selected, OnSelectedGridOption, value);
				}
				
				Rect rect = GUILayoutUtility.GetLastRect();
				rect.width = 100;
				
				menu.DropDown(rect);
			}

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Line Two
            GUILayout.BeginHorizontal();

			// Display brush count
			GUILayout.Label(csgModel.BrushCount.ToStringWithSuffix(" brush", " brushes"), SabreGUILayout.GetLabelStyle());
//			CurrentSettings.Instance.GridMode = (GridMode)EditorGUILayout.EnumPopup(CurrentSettings.Instance.GridMode, EditorStyles.toolbarPopup, GUILayout.Width(80));

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
			CurrentSettings.Instance.PositionSnappingEnabled = SabreGUILayout.Toggle(CurrentSettings.Instance.PositionSnappingEnabled, "Pos Snapping");
			CurrentSettings.Instance.PositionSnapDistance = EditorGUILayout.FloatField(CurrentSettings.Instance.PositionSnapDistance, GUILayout.Width(50));
			
			if (SabreGUILayout.Button("-", EditorStyles.miniButtonLeft))
			{
				CurrentSettings.Instance.ChangePosSnapDistance(.5f);
			}
			if (SabreGUILayout.Button("+", EditorStyles.miniButtonRight))
			{
				CurrentSettings.Instance.ChangePosSnapDistance(2f);
			}

			// Rotation snapping UI
			CurrentSettings.Instance.AngleSnappingEnabled = SabreGUILayout.Toggle(CurrentSettings.Instance.AngleSnappingEnabled, "Ang Snapping");
			CurrentSettings.Instance.AngleSnapDistance = EditorGUILayout.FloatField(CurrentSettings.Instance.AngleSnapDistance, GUILayout.Width(50));

			if (SabreGUILayout.Button("-", EditorStyles.miniButtonLeft))
			{
				CurrentSettings.Instance.ChangeAngSnapDistance(.5f);
			}
			if (SabreGUILayout.Button("+", EditorStyles.miniButtonRight))
			{
				CurrentSettings.Instance.ChangeAngSnapDistance(2f);
			}

// Disabled test build options
//			CurrentSettings.Instance.RestoreOriginalPolygons = SabreGUILayout.Toggle(CurrentSettings.Instance.RestoreOriginalPolygons, "Restore Original Polygons", GUILayout.Width(153));
//			CurrentSettings.Instance.RemoveHiddenGeometry = SabreGUILayout.Toggle(CurrentSettings.Instance.RemoveHiddenGeometry, "Remove Hidden Geometry", GUILayout.Width(153));

			GUILayout.FlexibleSpace();

            if (CurrentSettings.Instance.CurrentMode != MainMode.Free)
			{
				if( Tools.current == UnityEditor.Tool.View && Tools.viewTool == ViewTool.Pan)
				{
					GUI.color = Color.yellow;
				}
			}

			if (SabreGUILayout.Button("Disable"))
			{
				csgModel.EditMode = false;
			}

            GUILayout.EndHorizontal();
        }
    }
}
#endif