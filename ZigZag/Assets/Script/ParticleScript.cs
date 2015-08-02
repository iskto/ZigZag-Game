using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {

    private ParticleSystem DiamondBreak;

	// Use this for initialization
	void Start () {

        DiamondBreak = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!DiamondBreak.isPlaying)
        {
            Destroy(gameObject);   
        }
	}
}
