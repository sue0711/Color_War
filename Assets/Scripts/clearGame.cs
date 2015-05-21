using UnityEngine;
using System.Collections;

public class clearGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RetryClick()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void NextClick()
    {
        Application.LoadLevel(Application.loadedLevel+1);
    }
}
