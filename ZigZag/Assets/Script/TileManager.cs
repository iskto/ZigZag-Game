using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;

    public string[] tileChild = { "Tile/LeftAttachPoint", "Tile/TopAttachPoint"};

    public GameObject currentTile, startTile;

    public Material DiamondMaterial;

    private Stack<GameObject> leftTile = new Stack<GameObject>();

    public Stack<GameObject> LeftTile
    {
        get { return leftTile; }
        set { leftTile = value; }
    }

    private Stack<GameObject> topTile = new Stack<GameObject>();

    public Stack<GameObject> TopTile
    {
        get { return topTile; }
        set { topTile = value; }
    }

    private static TileManager instance;

    public static TileManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TileManager>();
            }

            return instance;
        }
    }

    private int LeftTileNum, TopTileNum;

    // Use this for initialization
    void Start()
    {
        LeftTileNum = 0; TopTileNum = 0;

        createTile(30);

        for (int i = 0; i < 30; i++)
        {
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createTile(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            leftTile.Push(Instantiate(tilePrefabs[0]));
            topTile.Push(Instantiate(tilePrefabs[1]));
            leftTile.Peek().name = "LeftTile";
            leftTile.Peek().SetActive(false);
            topTile.Peek().name = "TopTile";
            topTile.Peek().SetActive(false);
        }
    }

    public void SpawnTile()
    {
        if (leftTile.Count == 0 || topTile.Count == 0)
        {
            createTile(10);
        }

        int randomIndex = Random.Range(0, 2);

        int maxTileNum = 5;  // must less than 5, and must more than 3
        if ((randomIndex == 0 && LeftTileNum < maxTileNum) || TopTileNum >= maxTileNum-2)
        {
            GameObject tmp = leftTile.Pop();
            tmp.SetActive(true);
            tmp.transform.position = currentTile.transform.FindChild(tileChild[0]).position;
            currentTile = tmp;
            if (TopTileNum > 0)
            {
                TopTileNum--;
            }
            else
            {
                LeftTileNum++;
            } 
        }
        else
        {
            GameObject tmp = topTile.Pop();
            tmp.SetActive(true);
            tmp.transform.position = currentTile.transform.FindChild(tileChild[1]).position;
            currentTile = tmp;
            if (LeftTileNum > 0)
            {
                LeftTileNum--;
            }
            else
            {
                TopTileNum++;
            }        
        }
        
        // 產生鑽石
        int spawnDiamond = Random.Range(0, 5);
        if (spawnDiamond == 0)
        {
            int DiamondType = Random.Range(0, 15);
            if (DiamondType == 0 || DiamondType == 1) // 加速度的鑽石
            {
                if (PlayerScript.Instance.ScoreNum >= 600)
                {
                    currentTile.transform.FindChild("DiamondSUp").gameObject.SetActive(true);
                    currentTile.transform.FindChild("BlobShadowProjector").gameObject.SetActive(true);
                }
            }
            else if (DiamondType == 2) // 減速的鑽石
            {
                if (PlayerScript.Instance.ScoreNum >= 300)
                {
                    currentTile.transform.FindChild("DiamondSSlow").gameObject.SetActive(true);
                    currentTile.transform.FindChild("BlobShadowProjector").gameObject.SetActive(true);
                }
            }
            else // 加分的鑽石
            {
                
                currentTile.transform.FindChild("Diamond").gameObject.SetActive(true);          
                currentTile.transform.FindChild("BlobShadowProjector").gameObject.SetActive(true);
            } 
        }   

    }

}
