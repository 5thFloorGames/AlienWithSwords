using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	public class UtilityShortcuts : MonoBehaviour
	{
		[MenuItem("SabreCSG/Create New CSG")]
		static void CreateNewCSGObject()
		{
			// Create objects to hold the CSG Model and Work Brush (with associated scripts attached)
			GameObject rootGameObject = new GameObject("CSGModel", typeof(CSGModel));
			
			Undo.RegisterCreatedObjectUndo (rootGameObject, "Create New CSG Model");
			
			// Set the user's selection to the new CSG Model, so that they can start working with it
			Selection.activeGameObject = rootGameObject;

			// The automatic lightmapping conflicts when dealing with small brush counts, so default to user baking
			// The user can change this back to Auto if they want, but generally that'll only be an issue when they've
			// got a few brushes.
			Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
		}
		
		[MenuItem("SabreCSG/Rebuild #r")]
		static void Rebuild()
		{
			Object[] csgModels = GameObject.FindObjectsOfType(typeof(CSGModel));
			
			if(csgModels.Length > 0)
			{
				(csgModels[0] as CSGModel).Build ();
			}
		}

		[MenuItem("SabreCSG/About")]
		static void ShowAboutDialog()
		{
			string message = "Version " + CSGModel.VERSION_STRING + 
				"\nhttp://www.sabrecsg.com";
			EditorUtility.DisplayDialog("SabreCSG", message, "Close");
		}
		
		[MenuItem("SabreCSG/Utilities/Reset Camera")]
		static void ResetSceneViewCamera()
		{
			// Sometimes have issues with the camera locking up, resetting both current tool and the view tool seems
			// to fix the issue. Generally this seems to be due to not consuming events correctly
			Tools.viewTool = ViewTool.None;
			Tools.current = UnityEditor.Tool.None;
		}

	    /// <summary>
	    /// Toggles the editor scene camera between orthographic and perspective
	    /// </summary>
		[MenuItem("SabreCSG/Utilities/Toggle Camera Mode %#c")]
	    static void ToggleCameraMode()
	    {
	        if (SceneView.lastActiveSceneView != null)
	        {
	            SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
	            SceneView.lastActiveSceneView.Repaint();
	        }
	    }
	}
}