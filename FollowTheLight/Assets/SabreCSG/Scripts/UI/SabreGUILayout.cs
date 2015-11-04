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