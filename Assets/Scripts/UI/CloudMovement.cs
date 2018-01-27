using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour {

    Transform cloud1;
    Transform cloud2;
	// Use this for initialization
	void Start () {
        cloud1 = transform.GetChild(0);
        cloud2 = transform.GetChild(1);
        cloud1.position = new Vector3(11.2f, 2, 0.5f);
        cloud2.position = new Vector3(-11.2f, 2, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        cloud1.Translate(new Vector3(-Time.deltaTime, 0, 0));
        cloud2.Translate(new Vector3(-Time.deltaTime, 0, 0));
        if (cloud1.position.x < -22.4f)
        {
            cloud1.position = new Vector3(22.391f, 2, 0.5f);
        }
        if (cloud2.position.x < -22.4f)
        {
            cloud2.position = new Vector3(22.391f, 2, 0.5f);
        }
	}
}
