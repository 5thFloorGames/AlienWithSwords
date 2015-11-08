using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	
	public static int level = 1;
	public static int maxLevel = 1;
	
	public static void Reset(){
		level = 1;
	}

	public static void LevelUp(){
		level++;
		if (level > maxLevel) {
			level = maxLevel;
		}
	}
}
