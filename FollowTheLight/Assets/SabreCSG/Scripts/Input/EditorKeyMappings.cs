#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Sabresaurus.SabreCSG
{
	/// <summary>
	/// Central store for SabreCSG key mappings, change these constants to change the key bindings
	/// </summary>
	public static class EditorKeyMappings
	{
		const string TOOL_VIEW_KEY = "Tools/View";
		const string TOOL_MOVE_KEY = "Tools/Move";
		const string TOOL_ROTATE_KEY = "Tools/Rotate";

		const string TOOL_VIEW_DEFAULT = "Q";
		const string TOOL_MOVE_DEFAULT = "W";
		const string TOOL_ROTATE_DEFAULT = "E";

		public static Event GetToolViewMapping()
		{
			return GetMapping(TOOL_VIEW_KEY) ?? Event.KeyboardEvent(TOOL_VIEW_DEFAULT);
		}

		public static Event GetToolMoveMapping()
		{
			return GetMapping(TOOL_MOVE_KEY) ?? Event.KeyboardEvent(TOOL_MOVE_DEFAULT);
		}

		public static Event GetToolRotateMapping()
		{
			return GetMapping(TOOL_ROTATE_KEY) ?? Event.KeyboardEvent(TOOL_ROTATE_DEFAULT);
		}

		private static Event GetMapping(string key)
		{
			string value = EditorPrefs.GetString(key);

			if(!string.IsNullOrEmpty(value))
			{
				string[] split = value.Split(';');
				if(split.Length == 2)
				{
					return Event.KeyboardEvent(split[1]);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
#endif
