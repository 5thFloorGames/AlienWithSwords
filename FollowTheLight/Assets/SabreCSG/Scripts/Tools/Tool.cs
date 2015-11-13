#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sabresaurus.SabreCSG
{
    public abstract class Tool
    {
        protected CSGModel csgModel;

        // Used so we can reset some of the tool if the selected brush changes
        protected PrimitiveBrush targetBrush;
		protected Transform targetBrushTransform;

		bool panInProgress = false;

		public CSGModel CSGModel {
			get {
				return csgModel;
			}
			set {
				csgModel = value;
			}
		}

        public PrimitiveBrush TargetBrush
        {
            get
            {
                return targetBrush;
            }
            set
            {

                if (targetBrush != value)
                {
					SelectionAboutToChange();

					targetBrush = value;

					ResetTool();

                    if (targetBrush != null)
                    {
                        
						targetBrushTransform = targetBrush.transform;
                    }
					else
					{
						targetBrushTransform = null;
					}
                }
            }
        }

		protected bool CameraPanInProgress
		{
			get
			{
				if(Tools.viewTool == ViewTool.Orbit)
				{
					return true;
				}
				else if(Tools.viewTool == ViewTool.FPS)
				{
					return true;
				}
				else if(Tools.viewTool == ViewTool.Zoom)
				{
					return true;
				}
				else if(Tools.viewTool == ViewTool.Pan)
				{
					// Unity will often report panning in progress even when it's not, so use a separate check
					return panInProgress;
				}
				else
				{
					return false;
				}
			}
		}

        public virtual void OnSceneGUI(SceneView sceneView, Event e)
		{
			if(e.type == EventType.MouseDown)
			{
				// You can use ctrl-alt and left drag on PC for pan. On OSX it's cmd-alt and left drag
				// Unfortunately even when you're not panning Unity will usually default to saying the viewTool is
				// still pan, so it's necessary to do a more substantial event driven check to see if panning is actually
				// in progress
#if UNITY_EDITOR_OSX
				if(e.command && e.alt)
				{
					panInProgress = true;
				}
#else
				if(e.control && e.alt)
				{
					panInProgress = true;
				}
#endif
			}
			else if(e.type == EventType.MouseUp)
			{
				panInProgress = false;
			}
		}

		// Called when the selected brush is about to change, to give the tool a last chance to cleanup anything it 
		// needs to on the previous brush
		public virtual void SelectionAboutToChange() {}

		// Called when the selected brush changes
		public abstract void ResetTool();

		public abstract void Deactivated();

		public virtual bool BrushesHandleDrawing
		{
			get
			{
				return true;
			}
		}
    }
}
#endif