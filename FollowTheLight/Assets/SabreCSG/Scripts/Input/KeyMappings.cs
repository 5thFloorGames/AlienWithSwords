#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	/// <summary>
	/// Central store for SabreCSG key mappings, change these constants to change the key bindings
	/// </summary>
	public static class KeyMappings
	{
		public const KeyCode ToggleMode = KeyCode.Space;

		// Main Toolbar
		public const KeyCode IncreasePosSnapping = KeyCode.Period;
		public const KeyCode DecreasePosSnapping = KeyCode.Comma;
		public const KeyCode TogglePosSnapping = KeyCode.Slash;

		// General
		public const KeyCode ChangeBrushToAdditive = KeyCode.A;
		public const KeyCode ChangeBrushToAdditive2 = KeyCode.KeypadPlus;

		public const KeyCode ChangeBrushToSubtractive = KeyCode.S;
		public const KeyCode ChangeBrushToSubtractive2 = KeyCode.KeypadMinus;

		// Clip Tool
		public const KeyCode ApplyClip = KeyCode.Return;
		public const KeyCode FlipPlane = KeyCode.R;

		// Resize Tool
		public const KeyCode CancelMove = KeyCode.Escape;
	}
}
#endif