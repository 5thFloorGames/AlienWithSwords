#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	public class KeyMappings : ScriptableObject
	{
		[SerializeField]
		KeyCode toggleMode = KeyCode.Space;

//		[SerializeField]
//		KeyCode translate = KeyCode.W;
//
//		[SerializeField]
//		KeyCode rotate = KeyCode.E;
//
//		[SerializeField]
//		KeyCode toggleGrid = KeyCode.Slash;
//
//		[SerializeField]
//		KeyCode reduceGrid = KeyCode.Comma;
//
//		[SerializeField]
//		KeyCode increaseGrid = KeyCode.Period;
//		
//		[SerializeField]
//		KeyCode additive1 = KeyCode.A;
//		
//		[SerializeField]
//		KeyCode subtract1 = KeyCode.S;
//		
//		[SerializeField]
//		KeyCode additive2 = KeyCode.KeypadPlus;
//		
//		[SerializeField]
//		KeyCode subtract2 = KeyCode.KeypadMinus;
//
//		[SerializeField]
//		KeyCode applyClip = KeyCode.Return;
//
//		[SerializeField]
//		KeyCode cancel = KeyCode.Escape;


		public KeyCode ToggleMode {
			get {
				return toggleMode;
			}
		}

		private static KeyMappings instance = null;
		
		public static KeyMappings Instance
		{
			get
			{
				if (instance == null)
				{
					Object loadedObject = AssetDatabase.LoadMainAssetAtPath("Assets/SabreCSG/KeyMappings.asset");
					instance = (KeyMappings)loadedObject;
				}

				return instance;
			}
		}
	}
}
#endif