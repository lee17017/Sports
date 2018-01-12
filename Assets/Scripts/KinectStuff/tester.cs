using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour {
    public float interactionTimer;
    private float _timer = 0;
	private float _timer2 = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
      
        if (GestureHandler.Instance.detectPlayer()) {

            GestureHandler.Instance.calcPositions();
            //Debug.Log(GestureHandler.Instance.detectHeadTilt());
            //GestureHandler.Instance.getRightHandState();
            Debug.Log(GestureHandler.Instance.getMappedRightHandPosition());
            //Überprüfen, ob man die rechte Hand zuhält
            if (GestureHandler.Instance.getRightHandState())
            {
                //Start Timer:
                _timer += Time.deltaTime;
                _timer2 += Time.deltaTime;
                if (_timer2 >= 1)
                {
                    Debug.Log(_timer);
                    _timer2--;
                }
                if (_timer >= interactionTimer)
                {
                    // Interaction complete:
                    _timer = 0;
                    _timer2 = 0;
                }
            }
            else
            {
                //Stopping Timer:
                _timer = 0;
                _timer2 = 0;
               // Debug.Log("Rip");
            }
            //  GestureHandler.Instance.detectFlap();
            //  GestureHandler.Instance.detectShoot();
        }


    }
}
