using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Debug")]
    private bool _isKinectEnabled = false;

    [Header("Movement")]

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private float _lift;

    [SerializeField, Tooltip("Time until force in Y direction is 0 after wing beat")]
    private float _flapTime;

    [SerializeField]
    private float _autoMoveX = 0;
    public float AutoMoveX { get { return _autoMoveX; } }

    [SerializeField]
    private float _leanSpeed = 1;

    [Header("Level Border"), Tooltip("Left and right border as distance to player")]
    [SerializeField]
    private float _borderLeft = 0;

    [SerializeField]
    private float _borderRight = 0;


    private float _currentForceY = 0;
    private float _lastFlapStamp = 0;
    private float _currentLeanSpeed;
    private float _currentPositionInBorder = 0;

    private Collider _collider;

	// Use this for initialization
	void Start () {
        _collider = GetComponent<Collider>();

        //if border is not set, try get the camera view and calculate
        //this is more of a fallback and should not be used necessarily
        if(_borderLeft == 0 && _borderRight == 0) {
            float distance = Camera.main.transform.position.z;
            // the - is needed because the camera is looking along +z axis and not -z
            _borderLeft = -(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x
                + GetComponent<Collider>().bounds.max.x) + transform.position.x;
            _borderRight = (Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, distance)).x
                + GetComponent<Collider>().bounds.max.x/3) + transform.position.x;
        }
    }
	
    public void Damage(int damage) {
        GameManager.Instance.OnDamagePlayer(damage);
    }

    public void OnFlapWings() {
        Debug.Log("OnFLapWings");
        _lastFlapStamp = Time.realtimeSinceStartup;
    }

    //normalized value betweeen -1 and 1
    //with keybord we assume maximum values for now
    public void OnLean(float amount) {
        _currentLeanSpeed = amount * _leanSpeed;
    }

	// Update is called once per frame
	void Update () {
        float deltaX = 0, deltaY = 0;
        const float deltaZ = 0;

        deltaX += _autoMoveX * Time.deltaTime;
        deltaX += _currentLeanSpeed * Time.deltaTime;

        if (_isKinectEnabled) {
            //calculate flap force
            GestureHandler.Instance.calcPositions();
            float flap = GestureHandler.Instance.detectFlap();
            bool shoot = GestureHandler.Instance.detectShoot();

            _currentForceY += flap;
        } else {
            _currentForceY += GetFlapForce(Time.realtimeSinceStartup - _lastFlapStamp);
            _currentForceY -= GetGravity(Time.realtimeSinceStartup - _lastFlapStamp);
        }

        //apply translation
        gameObject.transform.Translate(new Vector3(deltaX, deltaY, deltaZ));

        //keep player in bounds
        _currentPositionInBorder += _currentLeanSpeed * Time.deltaTime;
        
        if (_currentPositionInBorder < _borderLeft) {
            transform.Translate((_borderLeft - _currentPositionInBorder),0,0);
            _currentPositionInBorder = _borderLeft;
        }else if (_currentPositionInBorder > _borderRight){
            transform.Translate(-(_currentPositionInBorder - _borderRight), 0, 0);
            _currentPositionInBorder = _borderRight;
        }

        //update position in game manager
        GameManager.Instance.UpdatePlayerPosition(transform.position.x);
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


    //Gizmos for player movement borders
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //draw line for left border + line for bounding box max x bounds
        Gizmos.DrawLine(new Vector3(_borderLeft - _currentPositionInBorder, -10, 0) + transform.position,
            new Vector3(_borderLeft - _currentPositionInBorder, 10, 0) + transform.position);
        Gizmos.DrawLine(new Vector3(_borderLeft - _currentPositionInBorder, -10, 0) + transform.position - GetComponent<Collider>().bounds.max, 
            new Vector3(_borderLeft - _currentPositionInBorder, 10, 0) + transform.position - GetComponent<Collider>().bounds.max);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(_borderRight - _currentPositionInBorder, -10, 0) + transform.position,
            new Vector3(_borderRight - _currentPositionInBorder, 10, 0) + transform.position);
    }

}
