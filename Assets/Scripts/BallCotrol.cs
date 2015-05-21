using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum BallState{
	Wait,
	Clicked,
	Move
}



public class BallCotrol : MonoBehaviour {

    public const int Color1 = 0;
    public const int Color3 = 1;
    public const int Color5 = 2;
    public const int Color7 = 3;
    public const int Color8 = 4;
    public const int Color10 = 5;
    public const int Color13 = 6;
    public const int Color14 = 7; 
    
	public int end_cnt;
    
    public BallState B_state;

	GameObject arrow;
	GameObject bg;
    

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
    Text endCnt;

    // Resource Array
    Sprite[] player1;
    Sprite[] player2;   //8개
    Sprite[] background;    //8개
    Sprite[] circle;    //8개
    Sprite[] hexagon;   //8개
    Sprite[] triangle;  //8개
    Sprite[] square;    //8개
    Sprite[] star;  //8개
    Sprite[] gage;  //11개
    Sprite[] border; //8개
    Sprite[] ColorBar; //8개

    public bool isCollided = false;
    public GameObject collidedObj;

	void Start () {

		B_state = BallState.Wait;
		myTransform = this.gameObject.transform;
		rb = this.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();
        sr_ColorBar = GameObject.Find("ColorChangeBar").GetComponent<SpriteRenderer>();
        colorBarObj = GameObject.Find("ColorChangeBar");

        rb_velocity = rb.velocity;

		arrow = GameObject.Find ("ball/arrow");
		arrowTransform = arrow.transform;

		bg = GameObject.Find ("BG");
		bgSr = bg.GetComponent<SpriteRenderer> ();
       

		sw = 1;
		bump_cnt = 0;
		remain_cnt = 0;

        clearText = GameObject.Find("Canvas/ResultText").GetComponent<Text>();
        endCnt = GameObject.Find("Canvas/EndCnt").GetComponent<Text>();
        endCnt.text = end_cnt.ToString();

        //Sprite Array  

        player1 = Resources.LoadAll<Sprite>("player_mal2/player1"); //8개
        player2 = Resources.LoadAll<Sprite>("player_mal2/player2");   //8개
        background = Resources.LoadAll<Sprite>("background") ;    //8개
        circle = Resources.LoadAll<Sprite>("Circle") ;    //8개
        hexagon = Resources.LoadAll<Sprite>("hexagon") ;   //8개
        triangle = Resources.LoadAll<Sprite>("triangle");  //8개
        square = Resources.LoadAll<Sprite>("square");    //8개
        star = Resources.LoadAll<Sprite>("star");  //8개
        gage = Resources.LoadAll<Sprite>("gage");  //11개
        border = Resources.LoadAll<Sprite>("teduri"); //8개
        ColorBar = Resources.LoadAll<Sprite>("color_change_bar"); //8개

        Debug.Log(player1[1]);
	}
	
	// Update is called once per frame
    void Update()
    {
        if (B_state == BallState.Wait)
        {
            if (end_cnt <= 0)
            {
                Time.timeScale = 0;
                clearText.text = "GameOver!";
                this.gameObject.SetActive(false);
            }
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
                B_state = BallState.Clicked;
                direction = myTransform.localRotation.z;
                end_cnt--;
                endCnt.text = end_cnt.ToString();
            }
        }

        if (B_state == BallState.Clicked)
        {
            if (Input.GetMouseButtonUp(0))
            {
                B_state = BallState.Move;

                force = 100;
                velocity = myTransform.localRotation * Vector2.right;
                rb.AddForce(velocity * force * 5.0f);
                arrow.SetActive(false);

                // B_state = BallState.Move;
            }
        }

        if (B_state == BallState.Move)
        {

            rb.velocity = rb.velocity * 0.995f;
            Debug.Log("rb.velocity = " + rb.velocity);
            //Debug.Log(rb.velocity);

            //if(rb.velocity.x < 0.5f && rb.velocity.y < 0.5f)
            Invoke("GetVelocity", 1.0f);
        }

        if (remain_cnt == figures.Length)
        {
            Time.timeScale = 0;
            clearText.text = "Clear!!!";
            this.gameObject.SetActive(false);
        }

        if (isCollided == true)//other.gameObject.transform.localScale.x < 9.9f)
        {
            collidedObj.gameObject.GetComponent<Collider2D>().enabled = false;
            collidedObj.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 0; //default
            collidedObj.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;

            collidedObj.gameObject.transform.localScale = Vector3.Lerp(collidedObj.gameObject.transform.localScale,
                                                        new Vector3(7.0f, 7.0f, 1.0f), Time.deltaTime * 8.0f);
            Debug.Log("들어옴");
           
            if (collidedObj.gameObject.transform.localScale.x >= 6.5f)
            {
                isCollided = false;
                ChangeBackground(collidedObj.gameObject.tag);
                SameColorProcess();
            }
        }


    }




	void GetVelocity(){
		if((rb.velocity.x < 0.3f && rb.velocity.x > -0.3f)||(rb.velocity.y < 0.3f && rb.velocity.y > -0.3f))
        {
			rb.velocity = new Vector2(0.0f, 0.0f);
			B_state = BallState.Wait;
            Debug.Log("GetVelocity() 실행");
		}
	}

	public void OnCollisionEnter2D(Collision2D other){

        collidedObj = other.gameObject;
        
       //


		if (other.gameObject.tag != this.gameObject.tag)
        {
           // isCollided = false;
			Debug.Log ("bump!!");
			bump_cnt ++;
			ColorChange ();
		}
        else if (other.gameObject.tag == this.gameObject.tag)
        {
            if(other.gameObject.tag != "Obstacle" && other.gameObject.tag != "Border" && other.gameObject.tag != "BlackHole")
            {
                isCollided = true;
            }
           
            Debug.Log("isCollided : " + isCollided + " 로 바뀜");
            
			//ChangeBG////////////////////////////////////////////////////////////////////////
            //ChangeBackground(other.gameObject.tag);

/*            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerID = 0; //default
            other.gameObject.GetComponent<CircleCollider2D>().enabled = false;

            isCollided = true;
           
            //while(isCollided == true)//other.gameObject.transform.localScale.x < 9.9f)
            //{
            //    other.gameObject.transform.localScale = Vector3.Lerp(other.gameObject.transform.localScale,
            //                                                new Vector3(10.0f, 10.0f, 1.0f), Time.deltaTime * 150 );
            //    Debug.Log("들어옴");
            //    if(other.gameObject.transform.localScale == new Vector3(10.0f, 10.0f, 1.0f))
            //    {
            //        isCollided = false;
            //    }
            //}
    
           // other.gameObject.transform.localScale.y += Time.deltaTime * 150;
*/


           
                
			bump_cnt ++;
			ColorChange ();

			sameColor = GameObject.FindGameObjectsWithTag(other.gameObject.tag);


/*			foreach (GameObject sc in sameColor) {
				//Destroy Same Colors
				if(sc.GetComponent<Renderer>().sortingOrder == 0){
					for(int i = 0; i < figures.Length; i++){
						if(figures[i].gameObject.name == sc.gameObject.name){
							figures[i] = new GameObject("Destory");
							figures[i].AddComponent<SpriteRenderer>();
							remain_cnt++;
						}
					}

					for(int i = 0; i < figures.Length; i++){
						if(figures[i].GetComponent<Renderer>().sortingLayerID == sc.GetComponent<Renderer>().sortingLayerID){
							Debug.Log(i + "sc.gameObject.name = " + sc.gameObject.name);
							figures[i].GetComponent<Renderer>().sortingOrder--;
                            Debug.Log("figures[i] = " + figures[i]);
						}
					}

					sc.gameObject.SetActive(false);
				}
			//	rb.velocity = new Vector2(0.0f,0.0f);
			//	myTransform.position = new Vector3(0.0f, 0.0f, 0.0f);
			//	B_state = BallState.Wait;
			}
 */    

		}

	}

    void SameColorProcess()
    {

        foreach (GameObject sc in sameColor)
        {
            //Destroy Same Colors
            if (sc.GetComponent<Renderer>().sortingOrder == 0)
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
            //	rb.velocity = new Vector2(0.0f,0.0f);
            //	myTransform.position = new Vector3(0.0f, 0.0f, 0.0f);
            //	B_state = BallState.Wait;
        }
    }
    void ChangeBackground(string tag)
    {
        if (tag == "Color1") { bgSr.sprite = background[Color1]; bg.gameObject.tag = "Color1"; }
        else if (tag == "Color3") { bgSr.sprite = background[Color3]; bg.gameObject.tag = "Color3"; } 
        else if (tag == "Color5") { bgSr.sprite = background[Color5]; bg.gameObject.tag = "Color5"; } 
        else if (tag == "Color7") { bgSr.sprite = background[Color7]; bg.gameObject.tag = "Color7"; } 
        else if (tag == "Color8") { bgSr.sprite = background[Color8]; bg.gameObject.tag = "Color8"; } 
        else if (tag == "Color10") {  bgSr.sprite = background[Color10]; bg.gameObject.tag = "Color10"; } 
        else if (tag == "Color13") { bgSr.sprite = background[Color13]; bg.gameObject.tag = "Color13"; }
        else if (tag == "Color14") { bgSr.sprite = background[Color14]; bg.gameObject.tag = "Color14"; } 
    }

    // 공의 컬러와 컬러체인지바 같이 바꾸는 함수
	void ColorChange(){
		bump_cnt = bump_cnt % 8;
		switch (bump_cnt)
        {
		    case 0:
                sr.sprite = player1[0];
			    this.gameObject.tag = "Color1";

                colorBarObj.gameObject.tag = "Color1";
                sr_ColorBar.sprite = ColorBar[Color1];
			    break;
            case 1:
                sr.sprite = player1[2];
                this.gameObject.tag = "Color3";

                colorBarObj.gameObject.tag = "Color3";
                sr_ColorBar.sprite = ColorBar[Color3];
                break;
            case 2:
                sr.sprite = player1[4];
                this.gameObject.tag = "Color5";

                colorBarObj.gameObject.tag = "Color5";
                sr_ColorBar.sprite = ColorBar[Color5];
                break;
            case 3:
                sr.sprite = player1[6];
                this.gameObject.tag = "Color7";

                colorBarObj.gameObject.tag = "Color7";
                sr_ColorBar.sprite = ColorBar[Color7];
                break;
            case 4:
                sr.sprite = player1[7];
                this.gameObject.tag = "Color8";

                colorBarObj.gameObject.tag = "Color8";
                sr_ColorBar.sprite = ColorBar[Color8];
                break;
            case 5:
                sr.sprite = player1[9];
                this.gameObject.tag = "Color10";

                colorBarObj.gameObject.tag = "Color10";
                sr_ColorBar.sprite = ColorBar[Color10];
                break;
            case 6:
                sr.sprite = player1[12];
                this.gameObject.tag = "Color13";

                colorBarObj.gameObject.tag = "Color13";
                sr_ColorBar.sprite = ColorBar[Color13];
                break;
            case 7:
                sr.sprite = player1[13];
                this.gameObject.tag = "Color14";

                colorBarObj.gameObject.tag = "Color14";
                sr_ColorBar.sprite = ColorBar[Color14];
                break;
			
		}
	}
	
}

