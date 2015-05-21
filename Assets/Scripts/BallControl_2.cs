using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BallState2
{
    Wait,
    Clicked,
    Move
}



public class BallControl_2 : MonoBehaviour
{

    public const int Color1 = 0;
    public const int Color2 = 1;
    public const int Color3 = 2;
    public const int Color4 = 3;
    public const int Color5 = 4;
    public const int Color6 = 5;
    public const int Color7 = 6;
    public const int Color8 = 7;
    public const int Color9 = 8;
    public const int Color10 = 9;
    public const int Color11 = 10;
    public const int Color12 = 11;
    public const int Color13 = 12;
    public const int Color14 = 13;

    public BallState2 B_state;

    GameObject arrow;
    GameObject bg;

    //----- 자연스럽게 밀리게 하기 - group으로 묶어서 rigidbody2D와 Tag정해주기 -----------
        //맵의 색 그룹 갯수만큼 
        public int figuresNum = 3; 
        public GameObject[] figures_group;
        //맵의 색 그룹 갯수만큼 child의 갯수도 정해줘야됨
        public GameObject[] figures_child1;
        public GameObject[] figures_child2;
        public GameObject[] figures_child3;

       // public GameObject[,] figures_child = new GameObject[,];
       // public List<int[]> figureList = new List<int[]>();
    
    //-----------------------------------------------------------------------------------

    public GameObject[] figures;
    public GameObject[] sameColor;

    public float direction = 0f;
    public float force = 40f;

    Transform myTransform;
    Transform arrowTransform;

    int sw;
    public int remain_cnt;
    public int bump_cnt;

    public Rigidbody2D rb;
    SpriteRenderer sr;
    SpriteRenderer bgSr;
    SpriteRenderer sr_ColorBar;

    public GameObject colorBarObj;

    public Vector2 rb_velocity;

    Vector3 velocity;

    Text clearText;

    // Resource Array
    Sprite[] player1;
    Sprite[] player2;   //14개
    Sprite[] background;    //14개
    Sprite[] circle;    //14개
    Sprite[] hexagon;   //14개
    Sprite[] triangle;  //14개
    Sprite[] square;    //14개
    Sprite[] star;  //14개
    Sprite[] gage;  //11개
    Sprite[] border; //14개
    public Sprite[] ColorBar; //14개

    void Awake()
    {
        figuresNum = 3;
    }

    void Start()
    {

        B_state = BallState2.Wait;
        myTransform = this.gameObject.transform;
        rb = this.GetComponent<Rigidbody2D>();
        sr = this.GetComponent<SpriteRenderer>();
        sr_ColorBar = GameObject.Find("ColorChangeBar").GetComponent<SpriteRenderer>();
        colorBarObj = GameObject.Find("ColorChangeBar");

        rb_velocity = rb.velocity;

        arrow = GameObject.Find("ball/arrow");
        arrowTransform = arrow.transform;

        bg = GameObject.Find("BG");
        bgSr = bg.GetComponent<SpriteRenderer>();


        sw = 1;
        bump_cnt = 0;
        remain_cnt = 0;

        clearText = GameObject.Find("Canvas/Text").GetComponent<Text>();

        //----- 자연스럽게 밀리게 하기 - group으로 묶어서 rigidbody2D와 Tag정해주기 ------------

            //figures group의 tag를 맨바깥쪽 object의 tag와 같게 초기화
            figures_group[0].tag = figures_child1[0].tag; // Color1
            figures_group[1].tag = figures_child2[0].tag; // Color1
            figures_group[2].tag = figures_child3[0].tag; // Color3
            //확인
            for(int i = 0 ; i < figures_group.Length ; i ++)
            {
                Debug.Log("figures_group["+i+"].tag = " + figures_group[i].tag);               
            }

        //-----------------------------------------------------------------------------------


        //Sprite Array 초기화
        player1 = Resources.LoadAll<Sprite>("player_mal2/player1"); //14개
        player2 = Resources.LoadAll<Sprite>("player_mal2/player2");   //14개
        background = Resources.LoadAll<Sprite>("background");    //14개
        circle = Resources.LoadAll<Sprite>("Circle");    //14개
        hexagon = Resources.LoadAll<Sprite>("hexagon");   //14개
        triangle = Resources.LoadAll<Sprite>("triangle");  //14개
        square = Resources.LoadAll<Sprite>("square");    //14개
        star = Resources.LoadAll<Sprite>("star");  //14개
        gage = Resources.LoadAll<Sprite>("gage");  //11개
        border = Resources.LoadAll<Sprite>("teduri"); //14개
        ColorBar = Resources.LoadAll<Sprite>("color_change_bar"); //14개

        Debug.Log(player1[1]);

      //test
      //  Debug.Log(figures_group[0]);
      //  Debug.Log(figures_group[0].GetComponentInChildren<SpriteRenderer>());

    

    }

    // Update is called once per frame
    void Update()
    {

        if (B_state == BallState2.Wait)
        {
            Debug.Log("BallState2. Wait 안에 들어옴");
            //init
            arrow.SetActive(true);
            arrowTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
            lookPos = lookPos - transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            myTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            if (Input.GetMouseButtonDown(0))
            {
                B_state = BallState2.Clicked;
                direction = myTransform.localRotation.z;
            }
        }

        if (B_state == BallState2.Clicked)
        {
            if (Input.GetMouseButtonUp(0))
            {

                force = 100;
                velocity = myTransform.localRotation * Vector2.right;
                Debug.Log(myTransform.localRotation * Vector2.right);
                rb.AddForce(velocity * force * 5.0f);
                arrow.SetActive(false);

                B_state = BallState2.Move;
            }
        }

        if (B_state == BallState2.Move)
        {

            rb.velocity = rb.velocity * 0.995f;
            Debug.Log("rb.velocity = " + rb.velocity);
            //Debug.Log(rb.velocity);
            Invoke("GetVelocity", 0.5f);
        }

        if (remain_cnt == figures.Length)
        {
            Debug.Log("Clear블럭에 들어옴");
            Time.timeScale = 0;
            Debug.Log(Time.timeScale);
            clearText.text = "Clear!!!";
            this.gameObject.SetActive(false);
        }
    }

    void GetVelocity()
    {
        if ((rb.velocity.x < 0.3f && rb.velocity.x > -0.3f) || (rb.velocity.y < 0.3f && rb.velocity.y > -0.3f))
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
            B_state = BallState2.Wait;
            Debug.Log("GetVelocity() 실행");
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("other.gameObject.name가 뭐냐하믄 " + other.gameObject.name);
        if (other.gameObject.tag != this.gameObject.tag)
        {
            Debug.Log("bump!!");
            bump_cnt++;
            ColorChange();
        }
        else if (other.gameObject.tag == this.gameObject.tag)
        {
           
            //ChangeBG////////////////////////////////////////////////////////////////////////
            ChangeBackground(other.gameObject.tag);

            

            bump_cnt++;
            ColorChange();

            //// 바뀐부분 -------- 부딪힌거 감지하기 : else if가 색깔 object 갯수만큼 있어야됨---------
            //if (other.gameObject.name == "F1_Group")
            //{
            //   // sameColor = figures_child1;
            //    for (int i = 0; i < figures_child1.Length; i++)
            //    {
            //        if(figures_child1[i].GetComponent<SpriteRenderer>().sortingOrder == 0)
            //        {
            //            figures_group[0].gameObject.tag = figures_child1[i].tag;
            //            Debug.Log("부딪힌 후 figures_group[0].gameObject.tag = "
            //                        + figures_group[0].gameObject.tag + " 로 바뀜");
            //        }
            //    }
                    
            //}
            //else if (other.gameObject.name == "F2_Group")
            //{
            //   // sameColor = figures_child2;
            //}
            //else if (other.gameObject.name == "F3_Group")
            //{
            //    //sameColor = figures_child3;
            //}

            //sameColor = GameObject.FindGameObjectsWithTag(other.gameObject.tag);
            //-----------------------------------------------------------------------------------


            
         // //  sameColor = GameObject.FindObjectOfType("figures");
            
            foreach (GameObject sc in sameColor)
        //    foreach (GameObject sc in figures)
            {
                //Destroy Same Colors
                if (sc/*.GetComponentInChildren<GameObject>()*/.GetComponent<Renderer>().sortingOrder == 0)
                {
                    for (int i = 0; i < figures.Length; i++)
                    {
                        if (figures[i].gameObject.name == sc.gameObject.name)
                        {
                            figures[i] = new GameObject("Destory");
                            figures[i].AddComponent<SpriteRenderer>();
                            remain_cnt++;
                        }
                    }

                    for (int i = 0; i < figures.Length; i++)
                    {
                        if (figures[i].GetComponent<Renderer>().sortingLayerID == sc.GetComponent<Renderer>().sortingLayerID)
                        {
                            Debug.Log(i + "sc.gameObject.name = " + sc.gameObject.name);
                            figures[i].GetComponent<Renderer>().sortingOrder--;
                            Debug.Log("figures[i] = " + figures[i]);
                        }
                    }

                    sc.gameObject.SetActive(false);
                }

                while(rb.velocity.x > 0.1f && rb.velocity.y > 0.1f)
                {
                    rb.velocity = rb.velocity * 0.995f;
                    Debug.Log("여기 실행됨");
                }
                
                rb.velocity = new Vector2(0.0f, 0.0f);

              //  myTransform.position = new Vector3(0.0f, 0.0f, 0.0f);
                B_state = BallState2.Wait;
            }

        }

    }

    void ChangeBackground(string tag)
    {
        if (tag == "Color1") { bgSr.sprite = background[Color1]; bg.gameObject.tag = "Color1"; }
        else if (tag == "Color2") { bgSr.sprite = background[Color2]; bg.gameObject.tag = "Color2"; }
        else if (tag == "Color3") { bgSr.sprite = background[Color3]; bg.gameObject.tag = "Color3"; }
        else if (tag == "Color4") { bgSr.sprite = background[Color4]; bg.gameObject.tag = "Color4"; }
        else if (tag == "Color5") { bgSr.sprite = background[Color5]; bg.gameObject.tag = "Color5"; }
        else if (tag == "Color6") { bgSr.sprite = background[Color6]; bg.gameObject.tag = "Color6"; }
        else if (tag == "Color7") { bgSr.sprite = background[Color7]; bg.gameObject.tag = "Color7"; }
        else if (tag == "Color8") { bgSr.sprite = background[Color8]; bg.gameObject.tag = "Color8"; }
        else if (tag == "Color9") { bgSr.sprite = background[Color9]; bg.gameObject.tag = "Color9"; }
        else if (tag == "Color10") { bgSr.sprite = background[Color10]; bg.gameObject.tag = "Color10"; }
        else if (tag == "Color11") { bgSr.sprite = background[Color11]; bg.gameObject.tag = "Color11"; }
        else if (tag == "Color12") { bgSr.sprite = background[Color12]; bg.gameObject.tag = "Color12"; }
        else if (tag == "Color13") { bgSr.sprite = background[Color13]; bg.gameObject.tag = "Color13"; }
        else if (tag == "Color14") { bgSr.sprite = background[Color14]; bg.gameObject.tag = "Color14"; }
    }

    // 공의 컬러와 컬러체인지바 같이 바꾸는 함수
    void ColorChange()
    {
        bump_cnt = bump_cnt % 5;
        switch (bump_cnt)
        {
            case 0:
                sr.sprite = player1[0];
                this.gameObject.tag = "Color1";

                colorBarObj.gameObject.tag = "Color1";
                sr_ColorBar.sprite = ColorBar[Color1];
                break;
            case 1:
                sr.sprite = player1[1];
                this.gameObject.tag = "Color2";

                colorBarObj.gameObject.tag = "Color2";
                sr_ColorBar.sprite = ColorBar[Color2];
                break;
            case 2:
                sr.sprite = player1[2];
                this.gameObject.tag = "Color3";

                colorBarObj.gameObject.tag = "Color3";
                sr_ColorBar.sprite = ColorBar[Color3];
                break;
            case 3:
                sr.sprite = player1[3];
                this.gameObject.tag = "Color4";

                colorBarObj.gameObject.tag = "Color4";
                sr_ColorBar.sprite = ColorBar[Color4];
                break;
            case 4:
                sr.sprite = player1[4];
                this.gameObject.tag = "Color5";

                colorBarObj.gameObject.tag = "Color5";
                sr_ColorBar.sprite = ColorBar[Color5];
                break;
            /*case 5:
                sr.sprite = player1[5];
                this.gameObject.tag = "Color6";

                colorBarObj.gameObject.tag = "Color6";
                sr_ColorBar.sprite = ColorBar[Color6];
                break;
            case 6:
                sr.sprite = player1[6];
                this.gameObject.tag = "Color7";

                colorBarObj.gameObject.tag = "Color7";
                sr_ColorBar.sprite = ColorBar[Color7];
                break;
            case 7:
                sr.sprite = player1[7];
                this.gameObject.tag = "Color8";

                colorBarObj.gameObject.tag = "Color8";
                sr_ColorBar.sprite = ColorBar[Color8];
                break;
            case 8:
                sr.sprite = player1[8];
                this.gameObject.tag = "Color9";

                colorBarObj.gameObject.tag = "Color9";
                sr_ColorBar.sprite = ColorBar[Color9];
                break;
            case 9:
                sr.sprite = player1[9];
                this.gameObject.tag = "Color10";

                colorBarObj.gameObject.tag = "Color10";
                sr_ColorBar.sprite = ColorBar[Color10];
                break;
            case 10:
                sr.sprite = player1[10];
                this.gameObject.tag = "Color11";

                colorBarObj.gameObject.tag = "Color11";
                sr_ColorBar.sprite = ColorBar[Color11];
                break;
            case 11:
                sr.sprite = player1[11];
                this.gameObject.tag = "Color12";

                colorBarObj.gameObject.tag = "Color12";
                sr_ColorBar.sprite = ColorBar[Color12];
                break;
            case 12:
                sr.sprite = player1[12];
                this.gameObject.tag = "Color13";

                colorBarObj.gameObject.tag = "Color13";
                sr_ColorBar.sprite = ColorBar[Color13];
                break;
            case 13:
                sr.sprite = player1[13];
                this.gameObject.tag = "Color14";

                colorBarObj.gameObject.tag = "Color14";
                sr_ColorBar.sprite = ColorBar[Color14];
                break;
			*/
        }
    }

}

