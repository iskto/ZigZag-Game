using UnityEngine;
using System.Collections;

public class ExitGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnClick () {
        // 離開遊戲
        Application.Quit();
	}

}
