using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public static bool playersTurn;
    public static GameObject activeCharacter;
	public static int amountOfCharacters;
	
	static int level = 1;
	static int lastLevel = 3;

	public static int GetLevel() {
		return level;
	}

	public static int GetLastLevel() {
		return lastLevel;
	}

	public static void SetLevel(int number) {
		level = number;
	}

	public static void LevelComplete(){
		Debug.Log ("completed level " + level);
		level++;
		if (level > lastLevel) {
			level = lastLevel;
		}
	}

	public static void Reset(){
		level = 1;
	}
}
