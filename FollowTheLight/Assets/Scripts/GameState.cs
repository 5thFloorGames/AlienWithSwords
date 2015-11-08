using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public static bool playersTurn;
	
	static int level = 1;
	static int lastLevel = 1;

	public static int GetLevel() {
		return level;
	}

	public static int GetLastLevel() {
		return lastLevel;
	}

	public static void LevelComplete(){
		level++;
		if (level > lastLevel) {
			level = lastLevel;
		}
	}

	public static void Reset(){
		level = 1;
	}
}
