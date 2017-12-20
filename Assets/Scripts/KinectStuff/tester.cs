using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GestureHandler.calcPositions();
        GestureHandler.detectFlap();
        GestureHandler.detectShoot();
	}
}
