using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBackground : MonoBehaviour {

    public GameObject bImgPref;
    private float _offset = 21.43f;
    private GameObject[] bImg;
    private int next;
    // Use this for initialization
    void Start () {
        bImg = new GameObject[2];
		for(int i=0; i<2; i++)
        {
            bImg[i] = Instantiate(bImgPref);
            bImg[i].transform.position = new Vector3(transform.position.x + i * 21.43f, this.transform.position.y-1, 5);
        }
        next = 1;
	}
	
	// Update is called once per frame
	void Update () {
        if (bImg[next].transform.position.x - transform.position.x < 0)
        {
            next = ((next + 1) % 2);
            bImg[next].transform.position = new Vector3(bImg[(next+1)%2].transform.position.x + 21.43f, this.transform.position.y-1, 5);
        }
	}
}
