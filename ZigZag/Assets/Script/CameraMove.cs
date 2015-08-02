using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

    private float baseWidth = 182;
    private float baseHeight = 323;
    private float baseOrthographicSize = 25.8f;

    private Vector3 dirCamera;

    private static CameraMove instance;

    public static CameraMove Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CameraMove>();
            }

            return instance;
        }
        set
        {
            instance = value; 
        }
    }

    void Awake()
    { 
        float newOrthographicSize = (float)Screen.height / (float)Screen.width * this.baseWidth / this.baseHeight * this.baseOrthographicSize;
        GetComponent<Camera>().orthographicSize = Mathf.Max(newOrthographicSize, this.baseOrthographicSize);
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
    void Update()
    {
        if (PlayerScript.Instance.isDead != true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dirCamera = Quaternion.Euler(-30f, 0f, 0f) * Vector3.forward;
            }

            float amoutTomove = PlayerScript.Instance.speed / Mathf.Sqrt(2) * Time.deltaTime;

            transform.Translate(dirCamera * amoutTomove);
        }
        else
        {
            dirCamera = Vector3.zero;
            transform.Translate(dirCamera * 0);
        }
   
	}
}
