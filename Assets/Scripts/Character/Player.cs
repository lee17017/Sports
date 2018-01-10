using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Debug")]
    private bool _isKinectEnabled = true;

    [Header("Movement")]

    [SerializeField]
    private float _liftConstant;

    [SerializeField]
    private float _gravityConstant;

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


    private float _currentLeanSpeed;
    private float _currentPositionInBorder = 0;

    private Collider _collider;
    private Rigidbody _rig;

	// Use this for initialization
	void Start () {
        //initialize gravity vector in -y direction
        Physics.gravity = new Vector3(0, -1f, 0);
        _rig = GetComponent<Rigidbody>();
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

    //normalized value betweeen -1 and 1
    //with keybord we assume maximum values for now
    public void OnLean(float amount) {
        _currentLeanSpeed = amount * _leanSpeed;
    }

	// Update is called once per frame
	void Update () {
        float x_vel = 0;

        //get y velocity from rigidbody
        float y_vel = _rig.velocity.y;

        //y velocity border cases
        if (y_vel < -5f)
        { 
            _rig.velocity = new Vector3(0, -1f, 0); 
            Debug.Log("a"); 
        }
        else if (y_vel > 5f)
        { _rig.velocity = new Vector3(0, 1f, 0); }

        //calculate x velocity from time and fixed auto speed
        x_vel += _autoMoveX * Time.deltaTime;

        //leaning
        x_vel += _currentLeanSpeed * Time.deltaTime;
        

        if (_isKinectEnabled) {
            //calculate flap force
            GestureHandler.Instance.calcPositions();
            //detect if flap or shoot gesture occured
            float flap = GestureHandler.Instance.detectFlap();
            bool shoot = GestureHandler.Instance.detectShoot();

            if (flap > 0)
            {
                Debug.Log("Flap detected with force: " + flap + "  ,normalized from 0 to 1");
                //if character is falling down reduce gravity on flap
                //so it is a lot easier to recover from falling
                if (y_vel < 0) {
                    _rig.velocity = new Vector3(0, y_vel / 2f, 0);
                }
                //add force
                _rig.AddForce(new Vector3(0, GetFlapForce(flap), 0));
            }
        } 
        //for testing
        else {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                if (y_vel < 0) {
                    _rig.velocity = new Vector3(0, y_vel / 2f, 0);
                }
                //add force
                _rig.AddForce(new Vector3(0, GetFlapForce(.8f), 0));
                Debug.Log("Up Arrow Pressed");
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                OnLean(-1f);
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                OnLean(1f);
            } else {
                OnLean(0f);
            }
        }

        //apply translation for constant speed in +x direction (right)
        gameObject.transform.Translate(new Vector3(x_vel, 0, 0));
        
        //keep player in bounds with leaning active
        _currentPositionInBorder += _currentLeanSpeed * Time.deltaTime;
        
        if (_currentPositionInBorder < _borderLeft) {
            transform.Translate((_borderLeft - _currentPositionInBorder),0,0);
            _currentPositionInBorder = _borderLeft;
        }else if (_currentPositionInBorder > _borderRight){
            transform.Translate(-(_currentPositionInBorder - _borderRight), 0, 0);
            _currentPositionInBorder = _borderRight;
        }
        
        //update position in game manager in order to detect if the player finished the level
        GameManager.Instance.UpdatePlayerPosition(transform.position.x);
    }

    //calculate the initial force which is then applied to the rigidbody
    private float GetFlapForce(float flapForce) {
        return flapForce * _liftConstant;
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
