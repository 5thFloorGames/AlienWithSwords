using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public static bool playersTurn;
    public static GameObject activeCharacter;
	public static int amountOfCharacters;
    public static List<GameObject> characters;
	
	static int level = 0;
	static int lastLevel = 4;

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
		level++;
		if (level > lastLevel) {
			level = lastLevel;
		}
	}

	public static void Reset(){
		level = 0;
	}

    void OnLevelWasLoaded(int appLevel) {
        StartCoroutine(DebugMessages());
    }

    IEnumerator DebugMessages() {
        yield return new WaitForSeconds(0.2f);
        //Debug.Log("Level: " + level);
        //Debug.Log("Amount of characters: " + amountOfCharacters);
    }
}
