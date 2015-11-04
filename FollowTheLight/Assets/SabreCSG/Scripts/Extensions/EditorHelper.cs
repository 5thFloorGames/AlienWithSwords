#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Sabresaurus.SabreCSG
{
	public static class EditorHelper
	{
	    // Threshold for raycasting vertex clicks, in screen space (should match half the icon dimensions)
	    const float CLICK_THRESHOLD = 15;

	    // Used for offseting mouse position
	    const int TOOLBAR_HEIGHT = 37;

		public static bool HasDelegate (System.Delegate mainDelegate, System.Delegate targetListener)
		{
			if (mainDelegate != null)
			{
				if (mainDelegate.GetInvocationList().Contains(targetListener))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

	    public static bool SceneViewHasDelegate(SceneView.OnSceneFunc targetDelegate)
	    {
			return HasDelegate(SceneView.onSceneGUIDelegate, targetDelegate);
	    }

	    public enum SceneViewCamera { Top, Bottom, Left, Right, Front, Back, Other };

	    public static SceneViewCamera GetSceneViewCamera(SceneView sceneView)
	    {
	        Vector3 cameraForward = sceneView.camera.transform.forward;

	        if (cameraForward == new Vector3(0, -1, 0))
	        {
	            return SceneViewCamera.Top;
	        }
	        else if (cameraForward == new Vector3(0, 1, 0))
	        {
	            return SceneViewCamera.Bottom;
	        }
	        else if (cameraForward == new Vector3(1, 0, 0))
	        {
	            return SceneViewCamera.Left;
	        }
	        else if (cameraForward == new Vector3(-1, 0, 0))
	        {
	            return SceneViewCamera.Right;
	        }
	        else if (cameraForward == new Vector3(0, 0, -1))
	        {
	            return SceneViewCamera.Front;
	        }
	        else if (cameraForward == new Vector3(0, 0, 1))
	        {
	            return SceneViewCamera.Back;
	        }
	        else
	        {
	            return SceneViewCamera.Other;
	        }
	    }

	    /// <summary>
	    /// Whether the mouse position is within the bounds of the axis snapping gizmo that appears in the top right
	    /// </summary>
	    public static bool IsMousePositionNearSceneGizmo(Vector2 mousePosition)
	    {
	        mousePosition.x = Screen.width - mousePosition.x;
	        if (mousePosition.x > 14 && mousePosition.x < 89 && mousePosition.y > 14 && mousePosition.y < 105)
	        {
	            return true;
	        }
	        else
	        {
	            return false;
	        }
	    }

	    public static Vector2 ConvertMousePosition(Vector2 sourceMousePosition)
	    {
	        // Flip the direction of Y and remove the Scene View top toolbar's height
	        sourceMousePosition.y = Screen.height - sourceMousePosition.y - TOOLBAR_HEIGHT;
	        return sourceMousePosition;
	    }

	    public static bool InClickZone(Vector2 mousePosition, Vector3 worldPosition)
	    {
	        mousePosition = ConvertMousePosition(mousePosition);
	        Vector3 targetScreenPosition = Camera.current.WorldToScreenPoint(worldPosition);

	        if (targetScreenPosition.z < 0)
	        {
	            return false;
	        }

	        float distance = Vector2.Distance(mousePosition, targetScreenPosition);

	        if (distance <= CLICK_THRESHOLD)
	        {
	            return true;
	        }
	        else
	        {
	            return false;
	        }
	    }

	    public static Vector3 CalculateWorldPoint(SceneView sceneView, Vector3 screenPoint)
	    {
	        screenPoint = ConvertMousePosition(screenPoint);

	        return sceneView.camera.ScreenToWorldPoint(screenPoint);
	    }

		public static void ShowOrHideGrid(bool gridVisible)
		{
			// This code uses reflection to access and set the internal AnnotationUtility.showGrid property. 
			// As a result the internal structure could change, so the entire thing is wrapped in a try-catch
			try
			{
				Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(Editor));
				if(unityEditorAssembly != null)
				{
					System.Type type = unityEditorAssembly.GetType("UnityEditor.AnnotationUtility");
					if(type != null)
					{
						PropertyInfo property = type.GetProperty("showGrid", BindingFlags.Static | BindingFlags.NonPublic);
						if(property != null)
						{
							property.GetSetMethod(true).Invoke(null, new object[] { gridVisible } );
						}
					}
				}
			}
			catch
			{
				// Failed, suppress any errors and just do nothing
			}
		}

		public static string GetCurrentSceneGUID()
		{
			string currentScenePath = EditorApplication.currentScene;
			if(!string.IsNullOrEmpty(currentScenePath))
			{
				return AssetDatabase.AssetPathToGUID(currentScenePath);
			}
			else
			{
				// Scene hasn't been saved
				return null;
			}
		}

	    [MenuItem("Assets/Copy Path")]
	    public static void CopyPath()
	    {
	        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
	        EditorGUIUtility.systemCopyBuffer = path;
	    }

		[MenuItem("GameObject/Group")]
		public static void Group()
		{
			int count = Selection.transforms.Length;
			if(count > 0)
			{
				Transform groupParent = Selection.transforms[0].parent;

				GameObject newGameObject = new GameObject("Group");
				Undo.RegisterCreatedObjectUndo(newGameObject, "Group");
				Transform newTransform = newGameObject.transform;
				newTransform.parent = groupParent;

				System.Array.Sort(Selection.transforms, new TransformIndexComparer());
				for (int i = 0; i < Selection.transforms.Length; i++) 
				{
					Undo.SetTransformParent(Selection.transforms[i], newTransform, "Group");
				}
			}
		}

		public static void SetDirty(Object targetObject)
		{
			EditorUtility.SetDirty(targetObject);

	#if UNITY_5_0 || UNITY_5
			// As of Unity 5, SetDirty no longer marks the scene as dirty. Need to use the new call for that.
			EditorApplication.MarkSceneDirty();
	#endif
		}

		public class TransformIndexComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((Transform) x).GetSiblingIndex().CompareTo(((Transform) y).GetSiblingIndex());
			}
		}
	}
}
#endif