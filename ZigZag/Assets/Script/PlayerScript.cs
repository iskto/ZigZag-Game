using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    // Player variable
    public float speed, tmpspeed, speedTime; 

    private string playState = "normal"; // initial state : normal

    private Vector3 dir;

    public bool isDead;

    // StartLabel "TAP TO PLAY" variable
    private float alpha = 255.0f, fadeState; // fadeState => 0:fadein, 1:fadeout.

    public GameObject StartLabel;

    // Diamond Particle variable
    public GameObject DiamondBreak, DiamondSUpBreak, DiamondSSlowBreak;

    private GameObject AddScoreText;

    private byte textAlpha = (byte)255.0f;

    // Button and Sheet variable
    public GameObject RetryButton, ExitButton, ScoreSheet;

    // The Tile Material and Color variable
    public Material TileMaterial;

    private Color newColor;

    // Score variable
    public int ScoreNum;
    private int tmpScoreNum, tmpScoreNumInNornal;

    private UILabel ScoreLabel;

    // Carema Background Color
    private Camera CaremaBgColor;

    // PlayerScript Instance
    private static PlayerScript instance;

    public static PlayerScript Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PlayerScript>();
            }

            return instance;
        }
    }

    /* Use this for initialization */
    void Start()
    {
        dir = Vector3.zero;
        speed = 14; // initial speed
        TileMaterial.color = new Color32(61, 155, 203, 255); // initial floor color
        newColor = TileMaterial.color;
        // inital Component
        CaremaBgColor = CameraMove.Instance.GetComponent<Camera>();
        ScoreLabel = GameObject.Find("score").GetComponentInChildren<UILabel>();

        Resources.UnloadUnusedAssets(); // clean memory

        GameObject.Find("highestScore").GetComponentInChildren<UILabel>().text = PlayerPrefs.GetInt("HighestScore").ToString(); 
    }

    /* Update is called once per frame */
    void Update()
    {
        // 主動回收垃圾
        if (Time.frameCount % 50 == 0)
        {
            System.GC.Collect();
        }

        // TAP TO PLAY的淡入淡出
        if (StartLabel.activeSelf == true)
        {
            float fadeSpeed = Time.deltaTime * 200; // 淡入淡出的速度
            if (fadeState == 0) // if fadeState = fadein
            {
                alpha -= fadeSpeed;
                if (alpha - fadeSpeed <= 0)
                {
                    alpha = 0;
                    fadeState = 1;
                }
            }
            else if (fadeState == 1) // if fadeState = fadeout
            {
                alpha += fadeSpeed;
                if (alpha + fadeSpeed >= 255)
                {
                    alpha = 255;
                    fadeState = 0;
                }
            }
            StartLabel.GetComponentInChildren<UILabel>().alpha = alpha / 255.0f;
        }
  
        // 點擊畫面時 
        if (Input.GetMouseButtonDown(0))
        {
            // TAP TO PLAY消失 
            StartLabel.SetActive(false);
            
            if (isDead == false) // 當Player還存活    
            {
                ScoreNum++; // 分數+1
                ScoreLabel.text = ScoreNum.ToString(); // 改變分數label   
                if (dir == Vector3.forward) // 若當前為向前則改向左
                {
                    dir = Vector3.left;
                }
                else // 若當前為向左則改向前
                {
                    dir = Vector3.forward;
                }
            }
        }
        // Player的移動(速度與方向)
        float amoutTomove = speed * Time.deltaTime;
        transform.Translate(dir * amoutTomove);

        // 碰到鑽石後的狀態
        CollierDiamondState();

        // +5分的Text向上移
        int smooth = 10;
        if (AddScoreText != null && AddScoreText.activeSelf == true)
        {
            if (AddScoreText.transform.position != new Vector3(AddScoreText.transform.position.x, 8.9f, AddScoreText.transform.position.z))
            {
                AddScoreText.transform.position = Vector3.Lerp(AddScoreText.transform.position, new Vector3(AddScoreText.transform.position.x, 8.9f, AddScoreText.transform.position.z), Time.deltaTime * smooth);
            }
            // 逐漸消失
            if (textAlpha - (Time.deltaTime * 400) > 0)
            {
                textAlpha -= (byte)(Time.deltaTime * 400);
            }
            AddScoreText.GetComponent<MeshRenderer>().material.color = new Color32(230, 0, 255, textAlpha);
        }

        // 每50分改變地板顏色 
        ResetColor();

        // 若分數超過最高分則最高分隨著改變
        if (ScoreNum > PlayerPrefs.GetInt("HighestScore"))
        {
            GameObject.Find("highestScore").GetComponentInChildren<UILabel>().text = ScoreNum.ToString();
        }

        // 死掉的時候出現menu 
        MenuButton();
    }

    // 當Player離開了地板 
    void OnTriggerExit(Collider other)
    {     
        if (other.tag == "Tile")
        {
            RaycastHit hit;
            Ray downRay = new Ray(transform.position, Vector3.down);
            
            if (!Physics.Raycast(downRay, out hit))
            {
                isDead = true; // 設定成死亡狀態

                dir = Vector3.down; // 設定成掉落

                // 當分數高於最高分時則最高分換成此次分數
                if (ScoreNum > PlayerPrefs.GetInt("HighestScore"))
                {
                    PlayerPrefs.SetInt("HighestScore", ScoreNum);   
                }

                // 死亡後紀錄分數的字消失
                if (GameObject.Find("scoreTitle") != null && GameObject.Find("highestScoreTitle") != null)
                {
                    GameObject.Find("scoreTitle").SetActive(false);
                    GameObject.Find("highestScoreTitle").SetActive(false);
                }

                // 死亡後顯示最高分與此次分數
                GameObject.Find("HighestInSheet").GetComponentInChildren<UILabel>().text = PlayerPrefs.GetInt("HighestScore").ToString();
                GameObject.Find("ScoreInSheet").GetComponentInChildren<UILabel>().text = ScoreNum.ToString();
            }
        }
    }

    // 碰撞到鑽石 
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Diamond") // 分數鑽石
        { 
            other.gameObject.SetActive(false);
            Instantiate(DiamondBreak, transform.position, Quaternion.Euler(275 + (transform.position.x % 24), 340, 115 + (transform.position.z % 24)));
            textAlpha = (byte)255.0f; // 回復alpha
            AddScoreText = other.gameObject.transform.parent.transform.FindChild("AddScoreText").gameObject;
            AddScoreText.SetActive(true); // 分數text出現
            ScoreNum += 5; // 分數加5
            ScoreLabel.text = ScoreNum.ToString();
            //消除陰影
            other.gameObject.transform.parent.transform.FindChild("BlobShadowProjector").gameObject.SetActive(false);
        }
        else if (other.tag == "DiamondSUp") // 加速鑽石
        {
            other.gameObject.SetActive(false);
            Instantiate(DiamondSUpBreak, transform.position, Quaternion.Euler(275 + (transform.position.x % 24), 340, 115 + (transform.position.z % 24)));  
            //消除陰影
            other.gameObject.transform.parent.transform.FindChild("BlobShadowProjector").gameObject.SetActive(false);
            if (playState == "speedup")
            {
                speedTime = Time.time;
            }
            playState = "speedup";
        }
        else if (other.tag == "DiamondSSlow") // 減速鑽石
        {
            other.gameObject.SetActive(false);
            Instantiate(DiamondSSlowBreak, transform.position, Quaternion.Euler(275 + (transform.position.x % 24), 340, 115 + (transform.position.z % 24)));
            //消除陰影
            other.gameObject.transform.parent.transform.FindChild("BlobShadowProjector").gameObject.SetActive(false);
            if (playState == "speedslow")
            {
                speedTime = Time.time;
            }
            playState = "speedslow";
        }
    }

    // 碰到鑽石後的狀態
    void CollierDiamondState()
    {
        int smooth = 1;
        // player在一般狀態下
        if (playState == "normal")
        {
            // 改變背景色
            if (CaremaBgColor.backgroundColor != new Color32(240, 240, 240, 5))
            {
                CaremaBgColor.backgroundColor = Color.Lerp(CameraMove.Instance.GetComponent<Camera>().backgroundColor, new Color32(240, 240, 240, 5), Time.deltaTime * smooth);
            }
            tmpScoreNumInNornal = tmpScoreNum; // 暫存tmpScoreNum
            tmpspeed = speed; // 暫存speed
            speedTime = Time.time; // 暫存未碰到(加/減)速鑽石的時間
        }
        // 吃到加速鑽石的加速狀態 
        if (playState == "speedup")
        {
            // 改變背景色
            if (CaremaBgColor.backgroundColor != new Color32(237, 249, 178, 5))
            {
                CaremaBgColor.backgroundColor = Color.Lerp(CameraMove.Instance.GetComponent<Camera>().backgroundColor, new Color32(237, 249, 178, 5), Time.deltaTime * smooth);
            }
         
            if (Time.time - speedTime > 3) // 持續3秒
            {
                if (tmpScoreNum > tmpScoreNumInNornal && tmpScoreNum % 2 == 0 && tmpspeed < 22)
                {
                    tmpspeed += 2;
                }
                speed = tmpspeed;
                playState = "normal";
            }
            else if (speed < tmpspeed + 4)
            {
                speed += 0.2f;
            }
        }
        // 吃到減速鑽石的減速狀態 
        if (playState == "speedslow")
        {
            // 改變背景色
            if (CaremaBgColor.backgroundColor != new Color32(172, 213, 246, 5))
            {
                CaremaBgColor.backgroundColor = Color.Lerp(CameraMove.Instance.GetComponent<Camera>().backgroundColor, new Color32(172, 213, 246, 5), Time.deltaTime * smooth);
            }
            
            if (Time.time - speedTime > 3) // 持續3秒
            {
                if (tmpScoreNum > tmpScoreNumInNornal && tmpScoreNum % 2 == 0 && tmpspeed < 22)
                {
                    tmpspeed += 2;
                }
                speed = tmpspeed;
                playState = "normal";
            }
            else if (speed > tmpspeed - 4)
            {
                speed -= 0.2f;
            }
        }
    }

    // 改變地板的顏色 
    void ResetColor()
    {
        int smooth = 2;
        if ((ScoreNum / 50) > tmpScoreNum)
        {
            int ColorR = Random.Range(100, 255);
            int ColorG = Random.Range(100, 255);
            int ColorB = Random.Range(100, 255);
            newColor = new Color32((byte)ColorR, (byte)ColorG, (byte)ColorB, 255);

            tmpScoreNum = (ScoreNum / 50);

            // 當速度<22並且分數是100的倍數的時候，速度+2 
            if (speed < 22 && tmpScoreNum % 2 == 0)
            {
                speed += 2;
            }
        }
        if (TileMaterial.color != newColor)
        {
            TileMaterial.color = Color.Lerp(TileMaterial.color, newColor, Time.deltaTime * smooth);
        }
    }

    // Menu的按鈕下降 
    void MenuButton()
    {
        int smooth = 3;
        if (isDead == true && RetryButton.transform.position != new Vector3(0, 0.05f, 0))
        {
            ScoreSheet.transform.position = Vector3.Lerp(ScoreSheet.transform.position, new Vector3(0, 0.372f, 0), Time.deltaTime * smooth);
            RetryButton.transform.position = Vector3.Lerp(RetryButton.transform.position, new Vector3(0, 0.02f, 0), Time.deltaTime * smooth);
            ExitButton.transform.position = Vector3.Lerp(ExitButton.transform.position, new Vector3(0, -0.186f, 0), Time.deltaTime * smooth);
        }
    }

}
