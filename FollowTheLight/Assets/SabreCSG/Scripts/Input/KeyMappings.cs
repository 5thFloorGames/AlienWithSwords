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

		// Used in UtilityShortcuts.cs with MenuItem attribute
		// See http://unity3d.com/support/documentation/ScriptReference/MenuItem.html for advice on changing this
		public const string Rebuild = "%#r";

		/// <summary>
		/// Helper method to determine if two keyboard events match
		/// </summary>
		/// <returns><c>true</c>, if matched, <c>false</c> otherwise.</returns>
		public static bool EventsMatch(Event event1, Event event2)
		{
			if(event1.keyCode != event2.keyCode)
			{
				return false;
			}

			if(event1.modifiers != event2.modifiers)
			{
				return false;
			}

			return true;
		}
	}
}
#endif