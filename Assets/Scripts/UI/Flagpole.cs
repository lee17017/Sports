using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flagpole : MonoBehaviour {

    public Sprite wonSprite;
    public Sprite notWonSprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changePole()
    {
        if (this.GetComponent<SpriteRenderer>().sprite == wonSprite)
        {
            this.GetComponent<SpriteRenderer>().sprite = notWonSprite;
        } else
        {
            this.GetComponent<SpriteRenderer>().sprite = wonSprite;
        }
    }
}
