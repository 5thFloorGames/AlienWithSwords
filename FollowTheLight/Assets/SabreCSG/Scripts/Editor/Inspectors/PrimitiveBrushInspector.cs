using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	[CanEditMultipleObjects]
    [CustomEditor(typeof(PrimitiveBrush))]
    public class PrimitiveBrushInspector : Editor
    {
		float rescaleValue = 1f;
//		string rescaleValueString = "1";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

			if (GUILayout.Button("Recalculate Normals"))
			{
				foreach (var thisBrush in targets) 
				{
					((PrimitiveBrush)thisBrush).RecalculateNormals();
				}
			}

			GUILayout.BeginHorizontal();
			rescaleValue = EditorGUILayout.FloatField(rescaleValue);
			
			if(GUILayout.Button("Rescale"))
			{
				if(rescaleValue != 0)
				{
					foreach (var thisBrush in targets) 
					{
						((PrimitiveBrush)thisBrush).Rescale(rescaleValue);
					}
				}
			}

			if (GUILayout.Button("Reset Polygons"))
			{
				foreach (var thisBrush in targets) 
				{
					((PrimitiveBrush)thisBrush).ResetPolygons();
					((PrimitiveBrush)thisBrush).Invalidate();
				}
			}
			
			if (GUILayout.Button("Reset Pivot"))
			{
				foreach (var thisBrush in targets) 
				{
					((PrimitiveBrush)thisBrush).ResetPivot();
				}
			}

			GUILayout.EndHorizontal();

			if(targets.Length == 1)
			{
	            PrimitiveBrush thisBrush = ((PrimitiveBrush)target);
//	            CSGModel csgModel = thisBrush.GetCSGModel();

				if (GUILayout.Button("Send To Back"))
				{
					thisBrush.transform.SetAsFirstSibling();
				}

				if (GUILayout.Button("Send To Front"))
				{
					thisBrush.transform.SetAsLastSibling();
				}
				
				if (GUILayout.Button("Send Backward"))
				{
					int siblingIndex = thisBrush.transform.GetSiblingIndex();
					siblingIndex--;
					thisBrush.transform.SetSiblingIndex(siblingIndex);
				}

				if (GUILayout.Button("Send Forward"))
				{
					int siblingIndex = thisBrush.transform.GetSiblingIndex();
					siblingIndex++;
					thisBrush.transform.SetSiblingIndex(siblingIndex);
				}
			}
			else
			{

				GUILayout.Box("Additional options not supported in multi-select");
//				GUILayout.Label("Additional options not supported in multi-select");
			}

//            GUILayout.Label("UVs", EditorStyles.boldLabel);
//
//            if (GUILayout.Button("Flip XY"))
//            {
//                UVUtility.FlipUVsXY(thisBrush.Polygons);
//            }
//
//            GUILayout.BeginHorizontal();
//            if (GUILayout.Button("Flip X"))
//            {
//                UVUtility.FlipUVsX(thisBrush.Polygons);
//            }
//            if (GUILayout.Button("Flip Y"))
//            {
//                UVUtility.FlipUVsY(thisBrush.Polygons);
//            }
//            GUILayout.EndHorizontal();
//
//            GUILayout.BeginHorizontal();
//            if (GUILayout.Button("UVs x 2"))
//            {
//                UVUtility.ScaleUVs(thisBrush.Polygons, 2f);
//            }
//            if (GUILayout.Button("UVs / 2"))
//            {
//                UVUtility.ScaleUVs(thisBrush.Polygons, .5f);
//            }
//            GUILayout.EndHorizontal();
            // Ensure Edit Mode is on
//            csgModel.EditMode = true;
        }
    }
}