using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {

    private float fallDelay = 0.4f; // 從Player碰撞到掉落的延遲時間
    TileManager tileManager;

	// Use this for initialization
	void Start () {
        tileManager = TileManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            tileManager.SpawnTile();
            StartCoroutine(FallDown());
            // 起始的大地板掉落
            if (tileManager.startTile.GetComponent<Rigidbody>().isKinematic != false)
            {
                tileManager.startTile.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    IEnumerator FallDown()
    {
        yield return new WaitForSeconds(fallDelay);
        GetComponent<Rigidbody>().isKinematic = false;
        GameObject AddScoreText = gameObject.transform.FindChild("AddScoreText").gameObject;
        AddScoreText.SetActive(false); // +5分的Text消失
        AddScoreText.transform.position = new Vector3(AddScoreText.transform.position.x, 6.8f, AddScoreText.transform.position.z); // 回復原位
        
        yield return new WaitForSeconds(2);
        switch (gameObject.name)
        {
            case "LeftTile":
                tileManager.LeftTile.Push(gameObject);
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.SetActive(false);
                break;

            case "TopTile":
                tileManager.TopTile.Push(gameObject);
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.SetActive(false);
                break;
        }
    }

}
