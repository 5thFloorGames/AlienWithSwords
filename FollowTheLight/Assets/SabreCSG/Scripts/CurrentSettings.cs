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
	public class CurrentSettings : MonoBehaviour
	{
	    #region Singleton
	    private static CurrentSettings instance = null;

	    public static CurrentSettings Instance
	    {
	        get
	        {
	            // If the instance is null, we need to make sure we have one
	            if (instance == null)
	            {
	                // First check if there's actually one in the scene already
	                CurrentSettings currentSettings = FindObjectOfType<CurrentSettings>();
	                if (currentSettings != null)
	                {
	                    // There was, so set instance to that
	                    instance = currentSettings;
	                }
	                else
	                {
	                    // Couldn't find one in the scene, let's create a new one
	                    GameObject spawnedObject = new GameObject("CurrentSettings", typeof(CurrentSettings));
	                    instance = spawnedObject.GetComponent<CurrentSettings>();
	                }
	            }

	            return instance;
	        }
	    }

	    void Awake()
	    {
	        if (instance != null && instance != this)
	        {
	            // Instance already exists other than this one, so destroy this object so we don't create a duplicate
	            Destroy(this.gameObject);
	        }
	        else
	        {
	            instance = this;
	        }
	    }
	    #endregion

	    MainMode currentMode = MainMode.Free;
	    MainMode lastMode = MainMode.Free;
		bool brushesHidden = false;
		bool meshHidden = false;

		Material foregroundMaterial;
		GridMode gridMode = GridMode.SabreCSG;

	    public bool PositionSnappingEnabled
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

	    public float PositionSnapDistance
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
		
//		public bool UVSnappingEnabled
//		{
//			get
//			{
//				return PlayerPrefs.GetInt("SabreCSGuvSnappingEnabled", 1) != 0;
//			}
//			set
//			{
//				PlayerPrefs.SetInt("SabreCSGuvSnappingEnabled", value ? 1 : 0);
//			}
//		}
		
		public float UVSnapDistance
		{
			get
			{
				return PlayerPrefs.GetFloat("SabreCSGuvSnapDistance", 0.1f);
			}
			set
			{
				if(value > 0)
				{
					PlayerPrefs.SetFloat("SabreCSGuvSnapDistance", value);
				}
			}
		}
		
		public bool AngleSnappingEnabled
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
		
		public float AngleSnapDistance
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

	    public Material ForegroundMaterial
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

		public GridMode GridMode 
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

	    public void ChangePosSnapDistance(float multiplier)
	    {
	        PositionSnapDistance *= multiplier;
	    }

		public void ChangeAngSnapDistance(float multiplier)
		{
			AngleSnapDistance *= multiplier;
		}

	    public bool BrushesHidden
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

		public bool MeshHidden
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
	    public bool BrushesVisible
	    {
	        get
	        {
	            return currentMode != MainMode.Free && !brushesHidden;
	        }
	    }

	    public bool AllowMeshSelection
	    {
	        get
	        {
	            return currentMode == MainMode.Free;
	        }
	    }

	    public MainMode CurrentMode
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

	    public MainMode LastMode
	    {
	        get
	        {
	            return lastMode;
	        }
	    }
	}
}
#endif