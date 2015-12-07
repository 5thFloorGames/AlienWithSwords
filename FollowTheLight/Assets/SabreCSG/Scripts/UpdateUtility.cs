using UnityEngine;
using System.Collections;

public static class UpdateUtility
{
	public static void RunCleanup()
	{
		// As of 1.1 CurrentSettings is no longer a MonoBehaviour, so remove any existing objects
		CleanupOldSettings();
	}

	private static void CleanupOldSettings()
	{
		// As of 1.1 CurrentSettings is no longer a MonoBehaviour
		GameObject existingSettingsObject = GameObject.Find("CurrentSettings");
		if(existingSettingsObject != null) // Found an old style object
		{
			Component[] components = existingSettingsObject.GetComponents<Component>();

			// Should be a transform and a null
			if(components.Length == 2)
			{
				bool matched = true;
				for (int i = 0; i < components.Length; i++) 
				{
					if(components[i] != null && components[i].GetType() != typeof(Transform))
					{
						matched = false;
						break;
					}
				}

				if(matched)
				{
					GameObject.DestroyImmediate(existingSettingsObject);
				}
			}
		}
	}
}