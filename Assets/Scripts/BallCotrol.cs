using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum BallState{
	Wait,
	Clicked,
	Move
}

public class BallCotrol : MonoBehaviour {

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

	Rigidbody2D rb;
	SpriteRenderer sr;
	SpriteRenderer bgSr;

	Text clearText;

	// Use this for initialization
	void Start () {

		B_state = BallState.Wait;
		myTransform = this.gameObject.transform;
		rb = this.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();

		arrow = GameObject.Find ("ball/arrow");
		arrowTransform = arrow.transform;

		bg = GameObject.Find ("BG");
		bgSr = bg.GetComponent<SpriteRenderer> ();

		sw = 1;
		bump_cnt = 0;
		remain_cnt = 0;

		clearText = GameObject.Find ("Canvas/Text").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (B_state == BallState.Wait) {
			//init
			arrow.SetActive(true);
			arrowTransform.localScale = new Vector3(2.38f,1.19f,1.0f);
			sw = 1;
			//bump_cnt = 0;

			myTransform.Rotate( 0 , 0, -Time.deltaTime*150);
			if(Input.GetMouseButtonDown(0)){
				B_state = BallState.Clicked;
				direction = myTransform.rotation.z;
			}
		}

		if (B_state == BallState.Clicked){
			force = force + 2*sw;
			//Debug.Log(force);
			arrowTransform.localScale = new Vector3(1+force*0.015f, arrowTransform.localScale.y, arrowTransform.localScale.z);  
			if(force >= 100){
				sw = -1;
			} else if (force <=40){
				sw = 1;
			}
			if(Input.GetMouseButtonUp(0)){
				B_state = BallState.Move;
				//rb.velocity = new Vector2(1.0f, 1.0f);

				Vector3 velocity = myTransform.rotation * Vector3.right;
				rb.AddForce(velocity*force*5.0f);
				arrow.SetActive(false);
			}
		}

		if (B_state == BallState.Move) {
			rb.velocity = rb.velocity * 0.995f;
			//Debug.Log(rb.velocity);
			Invoke ("GetVelocity",0.5f);
		}

		if (remain_cnt == figures.Length) {
			Time.timeScale = 0;
			clearText.text = "Clear!!!";
			this.gameObject.SetActive(false);
		}
	}

	void GetVelocity(){
		if((rb.velocity.x < 0.6f && rb.velocity.x > -0.6f)||(rb.velocity.y < 0.6f && rb.velocity.y > -0.6f)){
			rb.velocity = new Vector2(0.0f, 0.0f);
			B_state = BallState.Wait;
		}
	}

	public void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag != this.gameObject.tag) {
			Debug.Log ("bump!!");
			bump_cnt ++;
			ColorChange ();
		} else if (other.gameObject.tag == this.gameObject.tag) {
			//ChangeBG
			bgSr.color = sr.color;

			bump_cnt ++;
			ColorChange ();

			sameColor = GameObject.FindGameObjectsWithTag(other.gameObject.tag);
			foreach (GameObject sc in sameColor) {
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
							Debug.Log(sc.gameObject.name);
							figures[i].GetComponent<Renderer>().sortingOrder--;
						}
					}

					sc.gameObject.SetActive(false);
				}
				rb.velocity = new Vector2(0.0f,0.0f);
				myTransform.position = new Vector3(0.0f, 0.0f, 0.0f);
				B_state = BallState.Wait;

			}

		}

	}

	void ColorChange(){
		bump_cnt = bump_cnt % 3;
		switch (bump_cnt) {
		case 0:
			sr.color = Color.blue;
			this.gameObject.tag = "Blue";
			break;
		case 1:
			sr.color = Color.red;
			this.gameObject.tag = "Red";
			break;
		case 2:
			sr.color = Color.green;
			this.gameObject.tag = "Green";
			break;
		}
	}
	
}

