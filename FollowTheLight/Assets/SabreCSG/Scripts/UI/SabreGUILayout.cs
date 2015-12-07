#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Sabresaurus.SabreCSG
{
	public static class SabreGUILayout
	{
		public static bool Button(string title, GUIStyle style = null)
	    {
			return GUILayout.Button(title, style ?? EditorStyles.miniButton);
	    }

	    public static bool Toggle(bool value, string title, params GUILayoutOption[] options)
	    {
	        value = GUILayout.Toggle(value, title, EditorStyles.toolbarButton, options);
	        return value;
	    }

		public static GUIStyle GetLabelStyle()
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			if(EditorGUIUtility.isProSkin)
			{
				style.normal.textColor = Color.white;
			}
			else
			{
				style.normal.textColor = Color.black;
			}
			return style;
		}

		public static GUIStyle GetOverlayStyle()
		{
			GUIStyle style = new GUIStyle(GUI.skin.box);
			style.border = new RectOffset(22,22,22,22);
			style.normal.background = SabreGraphics.DialogOverlayTexture;
			style.fontSize = 20;
			style.padding = new RectOffset(15,15,15,15);
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.red;
			return style;
		}

		public static GUIStyle GetForeStyle()
		{
			if(EditorGUIUtility.isProSkin)
			{
				return FormatStyle(Color.white);
			}
			else
			{
				return FormatStyle(Color.black);
			}
		}

	    public static GUIStyle FormatStyle(Color textColor)
	    {
	        GUIStyle style = new GUIStyle(GUI.skin.label);
	        style.normal.textColor = textColor;
	        return style;
	    }

	    public static GUIStyle FormatStyle(Color textColor, int fontSize)
	    {
	        GUIStyle style = new GUIStyle(GUI.skin.label);
	        style.normal.textColor = textColor;
	        style.fontSize = fontSize;
	        return style;
	    }

		public static GUIStyle FormatStyle(Color textColor, int fontSize, TextAnchor alignment)
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = textColor;
			style.alignment = alignment;
			style.fontSize = fontSize;
			return style;
		}

		public static T? EnumPopupMixed<T> (string label, T[] selected, params GUILayoutOption[] options) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("EnumPopupMixed must be passed an enum");
			}

			string[] names = Enum.GetNames(typeof(T));

			int selectedIndex = (int)(object)selected[0];

			for (int i = 1; i < selected.Length; i++) 
			{
				if(selectedIndex != (int)(object)selected[i])
				{
					selectedIndex = names.Length;

					break;
				}
			}

			// Mixed selection, add a name entry for "Mixed"
			if(selectedIndex == names.Length)
			{
				Array.Resize(ref names, names.Length+1);
				
				int mixedIndex = names.Length-1;
				names[mixedIndex] = "Mixed";
			}

			int newIndex = EditorGUILayout.Popup(label, selectedIndex, names, options);

			if(newIndex == names.Length-1)
			{
				return null;
			}
			else
			{
				return (T)Enum.ToObject(typeof(T), newIndex);
			}
		}

	    public static T DrawEnumGrid<T>(T value, params GUILayoutOption[] options) where T : struct, IConvertible
	    {
	        if (!typeof(T).IsEnum)
	        {
	            throw new ArgumentException("DrawEnumGrid must be passed an enum");
	        }
	        GUILayout.BeginHorizontal();

	        string[] names = Enum.GetNames(value.GetType());
	        for (int i = 0; i < names.Length; i++)
	        {
	            GUIStyle activeStyle;
	            if (names.Length == 1) // Only one button
	            {
	                activeStyle = EditorStyles.miniButton;
	            }
	            else if (i == 0) // Left-most button
	            {
	                activeStyle = EditorStyles.miniButtonLeft;
	            }
	            else if (i == names.Length - 1) // Right-most button
	            {
	                activeStyle = EditorStyles.miniButtonRight;
	            }
	            else // Normal mid button
	            {
	                activeStyle = EditorStyles.miniButtonMid;
	            }

	            bool isActive = (Convert.ToInt32(value) == i);
	            if (GUILayout.Toggle(isActive, names[i], activeStyle, options))
	            {
	                value = (T)Enum.ToObject(typeof(T), i);
	            }
	        }

	        GUILayout.EndHorizontal();
	        return value;
	    }
	}
}
#endif