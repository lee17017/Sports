using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private float _lift;
    
    [SerializeField, Tooltip("Time until force in Y direction is 0 after wing beat")]
    private float _flapTime;

    private float _currentForceY = 0;

    private float _lastFlapStamp = 0;

	// Use this for initialization
	void Start () {
		
	}
	

    public void OnFlapWings() {
        Debug.Log("OnFLapWings");
        _lastFlapStamp = Time.realtimeSinceStartup;

        //defy gravity?
        //_currentForceY = 0;
        //a bit?
        _currentForceY /= 3;
    }



	// Update is called once per frame
	void Update () {
        float deltaX = 0, deltaY = 0;
        const float deltaZ = 0;

        _currentForceY += GetFlapForce(Time.realtimeSinceStartup - _lastFlapStamp);
        _currentForceY -= _gravity * Time.deltaTime;

        deltaY = _currentForceY * Time.deltaTime;

        //apply translation
        gameObject.transform.Translate(new Vector3(deltaX, deltaY, deltaZ));
    }

    private float GetFlapForce(float elapsedTime) {
        //function to calculate the force after the player hit the wings
        //this basic function is linear, and should be more complex and realistic
        return Mathf.Max((_flapTime - elapsedTime) * _lift, 0);
    }

}
