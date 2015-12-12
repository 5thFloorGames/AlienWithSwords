using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class LoadTextToTextFromFile : MonoBehaviour {

    public string filename;

    Text text;

	void Start () {
        if (filename != "" && System.IO.File.Exists(filename)) {
            text = GetComponent<Text>();
            text.text = System.IO.File.ReadAllText(filename);
        }
	}

	void Update () {
	
	}
}
