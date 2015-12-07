#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Sabresaurus.SabreCSG
{
	public static class CSGGrid
	{
		const int WIDTH = 100;
		const int LINE_DISTRIBUTION = 8;

		public static void Draw()
		{
			SceneView sceneView = SceneView.currentDrawingSceneView;
			EditorHelper.SceneViewCamera camera = EditorHelper.GetSceneViewCamera(sceneView);
			float snapDistance = CurrentSettings.PositionSnapDistance;

			Vector3 cameraPosition = sceneView.camera.transform.position;

			Vector3 offset = cameraPosition;
			offset.y = 0;
			offset = MathHelper.RoundVector3(offset, snapDistance);

			SabreGraphics.GetSelectedBrushMaterial().SetPass(0);
			GL.Begin(GL.LINES);
			GL.Color(new Color32(200,200,200,128));

			Vector3 right;
			Vector3 forward;

			if(camera == EditorHelper.SceneViewCamera.Other)
			{
				right = Vector3.right;
				forward = Vector3.forward;
			}
			else
			{
				right = sceneView.camera.transform.right;
				forward = sceneView.camera.transform.up;
			}



			int xStart = (int)((-100 + offset.x)/snapDistance);
			int xEnd = (int)((100 + offset.x)/snapDistance);

			for (int i = xStart; i < xEnd; i++) 
			{
				Vector3 position1 = right * i * snapDistance;

				if(position1.x == 0) // Black
				{
					GL.Color(new Color32(0,0,0,128));
				}
				else if(position1.x % LINE_DISTRIBUTION == 0)// Grey
				{
					GL.Color(new Color32(128,128,128,128));
				}

				GL.Vertex(position1 + forward * (-WIDTH + offset.z));
				GL.Vertex(position1 + forward * (WIDTH + offset.z));

				if(position1.x % LINE_DISTRIBUTION == 0)
				{
					// Reset back
					GL.Color(new Color32(200,200,200,128));
				}
			}

			int yStart = (int)((-100 + offset.z) / snapDistance);
			int yEnd = (int)((100 + offset.z) / snapDistance);

			for (int i = yStart; i < yEnd; i++) 
			{
				Vector3 position2 = forward * i * snapDistance;
				
				if(position2.z == 0) // Black
				{
					GL.Color(new Color32(0,0,0,128));
				}
				else if(position2.z % LINE_DISTRIBUTION == 0)// Grey
				{
					GL.Color(new Color32(128,128,128,128));
				}
				
				GL.Vertex(position2 + right * (-WIDTH + offset.x));
				GL.Vertex(position2 + right * (WIDTH + offset.x));

				if(position2.z % LINE_DISTRIBUTION == 0)
				{
					// Reset back
					GL.Color(new Color32(200,200,200,128));
				}
			}

			GL.End();
		}
	}
}
#endif