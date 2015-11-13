using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
    [CustomEditor(typeof(CSGModel))]
    public class CSGModelInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
////			return;
            CSGModel csgModel = (CSGModel)target;

			if(GUILayout.Button("Export OBJ"))
			{
				csgModel.ExportOBJ();
			}
//			
//			EditorGUILayout.PropertyField(prop1);
//
//			if(GUI.changed)
//			{
//				serializedObject.ApplyModifiedProperties();
//			}
			BuildMetrics buildMetrics = csgModel.BuildMetrics;

			GUILayout.Label("Vertices: " + buildMetrics.TotalVertices);
			GUILayout.Label("Triangles: " + buildMetrics.TotalTriangles);
			GUILayout.Label("Meshes: " + buildMetrics.TotalMeshes);
			GUILayout.Label("Build Time: " + buildMetrics.BuildTime.ToString());
        }
    }
}