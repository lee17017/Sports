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

    [SerializeField]
    private float _autoMoveX = 0;

    [SerializeField]
    private int _maxLife;

    private float _currentForceY = 0;

    private float _lastFlapStamp = 0;

    private int _life;

	// Use this for initialization
	void Start () {
        _life = _maxLife;
	}
	
    public void Damage(int damage) {
        _life -= damage;
        if(_life <=0) {
            Die();
        }
    }

    public void Die() {
        //TODO: do whatever happens when the player dies
    }

    public void OnFlapWings() {
        Debug.Log("OnFLapWings");
        _lastFlapStamp = Time.realtimeSinceStartup;

        //defy gravity?
        //_currentForceY = 0;
        //a bit?
        _currentForceY /= 2.5f;
    }



	// Update is called once per frame
	void Update () {
        float deltaX = _autoMoveX, deltaY = 0;
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

    //physically correct gravity calculation for input time in seconds; not sure if works for the game
    private float GetGravity(float elapsedTime) {
        return (_gravity * (elapsedTime * 60) * (elapsedTime * 60)) / 2;
    }

}
