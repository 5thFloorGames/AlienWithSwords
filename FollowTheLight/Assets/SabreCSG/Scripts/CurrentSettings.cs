#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Sabresaurus.SabreCSG
{
	public enum MainMode
	{
	    Free,
	    Resize,
	    Vertex,
		Face,
	    Clip
	};

	public enum GridMode
	{
		Unity,
		SabreCSG,
		None
	}

	[ExecuteInEditMode]
	public static class CurrentSettings
	{
	    static MainMode currentMode = MainMode.Free;
	    static MainMode lastMode = MainMode.Free;
		static bool brushesHidden = false;
		static bool meshHidden = false;

		static Material foregroundMaterial;
		static GridMode gridMode = GridMode.SabreCSG;

	    public static bool PositionSnappingEnabled
	    {
	        get
	        {
				return PlayerPrefs.GetInt("SabreCSGpositionSnappingEnabled", 1) != 0;
	        }
	        set
	        {
				PlayerPrefs.SetInt("SabreCSGpositionSnappingEnabled", value ? 1 : 0);
	        }
	    }

	    public static float PositionSnapDistance
	    {
	        get
	        {
				return PlayerPrefs.GetFloat("SabreCSGpositionSnapDistance", 1f);
	        }
	        set
	        {
				if(value > 0)
				{
					PlayerPrefs.SetFloat("SabreCSGpositionSnapDistance", value);
				}
	        }
		}
		
		public static bool AngleSnappingEnabled
		{
			get
			{
				return PlayerPrefs.GetInt("SabreCSGangleSnappingEnabled", 1) != 0;
			}
			set
			{
				PlayerPrefs.SetInt("SabreCSGangleSnappingEnabled", value ? 1 : 0);
			}
		}
		
		public static float AngleSnapDistance
		{
			get
			{
				return PlayerPrefs.GetFloat("SabreCSGangleSnapDistance", 15);
			}
			set
			{
				if(value > 0)
				{
					PlayerPrefs.SetFloat("SabreCSGangleSnapDistance", value);
				}
			}
		}

	    public static Material ForegroundMaterial
	    {
	        get
	        {
	            return foregroundMaterial;
	        }
	        set
	        {
	            foregroundMaterial = value;
	        }
		}

		public static GridMode GridMode 
		{
			get 
			{
				return gridMode;
			}
			set 
			{
				if(gridMode != value)
				{
					gridMode = value;

					if(gridMode == GridMode.Unity)
					{
						EditorHelper.ShowOrHideGrid(true);
					}
					else
					{
						EditorHelper.ShowOrHideGrid(false);
					}
				}
			}
		}

	    public static void ChangePosSnapDistance(float multiplier)
	    {
	        PositionSnapDistance *= multiplier;
	    }

		public static void ChangeAngSnapDistance(float multiplier)
		{
			AngleSnapDistance *= multiplier;
		}

		public static bool BrushesHidden
	    {
	        get
	        {
	            return brushesHidden;
	        }
	        set
	        {
	            brushesHidden = value;
	        }
	    }

		public static bool MeshHidden
		{
			get
			{
				return meshHidden;
			}
			set
			{
				meshHidden = value;
			}
		}

	    // TODO: This behaves differently to just !brushesHidden, need to make the two properties less ambiguous
		public static bool BrushesVisible
	    {
	        get
	        {
	            return currentMode != MainMode.Free && !brushesHidden;
	        }
	    }

		public static bool AllowMeshSelection
	    {
	        get
	        {
	            return currentMode == MainMode.Free;
	        }
	    }

		public static MainMode CurrentMode
	    {
	        get
	        {
	            return currentMode;
	        }
	        set
	        {
	            lastMode = currentMode;
	            currentMode = value;
	        }
	    }

		public static MainMode LastMode
	    {
	        get
	        {
	            return lastMode;
	        }
	    }
	}
}
#endif