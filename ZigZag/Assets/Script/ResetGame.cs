using UnityEngine;
using System.Collections;

public class ResetGame : MonoBehaviour {

    // Use this for initialization  
    void Start ()
    {

    }

    void OnClick ()
    {
        // 重新遊戲
        Application.LoadLevel(Application.loadedLevel);
    } 

}
