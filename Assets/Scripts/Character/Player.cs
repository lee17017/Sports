﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    [Header("Movement")]

    [SerializeField]
    private float _liftConstant;


    [SerializeField]
    private float _gravityConstant = 2;

    [SerializeField]
    private float _autoMoveX = 0;
    public float AutoMoveX { get { return _autoMoveX; } }

    [SerializeField]
    private float _leanSpeed = 1;

    [SerializeField]
    private float _ySpeedLimit;
	
    [Header("Gameplay")]
    [SerializeField]
    private float _invincibleTime;

    [Header("Level Border"), Tooltip("Left and right border as distance to player")]
    [SerializeField]
    private float _borderLeft = 0;

    [SerializeField]
    private float _borderRight = 0;

    [SerializeField]
    private GameObject _projectilePrefab;

    private float _currentLeanSpeed;

    private float _currentPositionInBorder = 0;

    private Collider _collider;
    private Rigidbody _rig;

    private float _invincibleTimestamp = 0;

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private float _borderUp, _borderDown;
    // Use this for initialization
    private SkinnedMeshRenderer _skinRend;
    private int _animation = 100;
	void Start () {
        _borderUp = GameObject.Find("Main Camera").GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 1, 1)).y + transform.localScale.y / 4;
        _borderDown = GameObject.Find("Main Camera").GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, 1)).y - transform.localScale.y/2;
        //initialize gravity vector in -y direction
        Physics.gravity = new Vector3(0, -_gravityConstant, 0);
        _rig = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _rig.useGravity = false;
        _skinRend = GetComponentInChildren<SkinnedMeshRenderer>();
        //if border is not set, try get the camera view and calculate
        //this is more of a fallback and should not be used necessarily
        if (_borderLeft == 0 && _borderRight == 0) {
            float distance = Camera.main.transform.position.z;
            // the - is needed because the camera is looking along +z axis and not -z
            _borderLeft = -(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x
                + GetComponent<Collider>().bounds.max.x) + transform.position.x;
            _borderRight = (Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, distance)).x
                + GetComponent<Collider>().bounds.max.x/3) + transform.position.x;
        }
    }
	
    public void Activate() {
        _isActive = true;
        _rig.useGravity = true;
    }

    public void Deactivate() {
        _isActive = false;
        _rig.useGravity = false;
        _rig.velocity = new Vector3(0,0,0);
    }

    public void Damage(int damage) {
        /*
        if(_invincibleTimestamp != 0 && Time.time - _invincibleTimestamp > _invincibleTime) {
            GameManager.Instance.OnDamagePlayer(damage);
            _invincibleTimestamp = Time.time;
            StartCoroutine(InvincibleFlash());
        }
         * */
        GameManager.Instance.OnDamagePlayer(damage);
    }

    private IEnumerator flapAnimation() {
        if(_animation != 100)
        {
            Debug.Log("overlap!");
            yield break;
        }
        if(Time.timeScale == 0)
        {
            Debug.Log("paused");
            yield break;
        }
        for (int i = 0; i < 10; i++)
        {
            _animation = _animation - 11;
            _skinRend.SetBlendShapeWeight(0, _animation);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < 10; i++)
        {
            _animation = _animation + 11;
            _skinRend.SetBlendShapeWeight(0, _animation);
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator InvincibleFlash() {
        float step = .2f;
        bool flash = true;
        Color color = GetComponent<Renderer>().material.color;
        while(Time.time - _invincibleTimestamp - step > _invincibleTime) { 
            if (flash) {
                GetComponent<Renderer>().material.color = Color.white;
            } else {
                GetComponent<Renderer>().material.color = color;
            }
            flash = !flash;
            yield return new WaitForSeconds(step);
        }
        GetComponent<Renderer>().material.color = color;
    }

    //normalized value betweeen -1 and 1
    //with keybord we assume maximum values for now
    public void OnLean(float amount) {
        _currentLeanSpeed = amount * _leanSpeed;
    }

    // Update is called once per frame
    void Update() {
        if (_isActive) {
            if(transform.position.y > _borderUp){
                Damage(1);
                transform.position = new Vector3(transform.position.x, _borderUp-0.01f, transform.position.z);
                _rig.velocity = new Vector3(_rig.velocity.x, _rig.velocity.y * -0.5f, _rig.velocity.z);
                return;
            }
            if (transform.position.y < _borderDown)
            {
                Damage(1);
                transform.position = new Vector3(transform.position.x, _borderDown+0.05f, transform.position.z);
                _rig.velocity = new Vector3(_rig.velocity.x, 0.25f, _rig.velocity.z);
                return;
            }

            float x_vel = 0;

            //get y velocity from rigidbody
            float y_vel = _rig.velocity.y;

            if (y_vel > _ySpeedLimit)
                Physics.gravity = new Vector3(0, -4*_gravityConstant, 0);
            else
                Physics.gravity = new Vector3(0, -_gravityConstant, 0);

            if (GestureHandler.Instance.isKinectEnabled) {
                //calculate flap force
                GestureHandler.Instance.calcPositions();
                //detect if flap or shoot gesture occured
                float flap = GestureHandler.Instance.detectFlap();
                bool shoot = GestureHandler.Instance.detectShoot();

                if (flap > 0) {
                    //Debug.Log("Flap detected with force: " + flap + "  ,normalized from 0 to 1");
                    //if character is falling down reduce gravity on flap
                    //so it is a lot easier to recover from falling
                    StartCoroutine("flapAnimation");
                    if (y_vel < 0) {
                        _rig.velocity = new Vector3(0, y_vel / 2f, 0);
                    }
                    //add force
                        _rig.AddForce(new Vector3(0, GetFlapForce(flap), 0));
                }
                if (shoot) {
                    Shoot();
                }
                OnLean(GestureHandler.Instance.detectLean());
            }
            //for testing
            else {
                if (Input.GetKeyDown(KeyCode.UpArrow)) {

                    StartCoroutine("flapAnimation");
                    if (y_vel < 0) {
                        _rig.velocity = new Vector3(0, y_vel / 2f, 0);
                    }
                    //add force
                    _rig.AddForce(new Vector3(0, GetFlapForce(0.3f), 0));
                    //Debug.Log("Up Arrow Pressed");
                }
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Shoot();
                }
                if (Input.GetKey(KeyCode.LeftArrow)) {
                    OnLean(-1f);
                } else if (Input.GetKey(KeyCode.RightArrow)) {
                    OnLean(1f);
                } else {
                    OnLean(0f);
                }
            }

                        //calculate x velocity from time and fixed auto speed
            x_vel += _autoMoveX * Time.deltaTime;

            //leaning
            x_vel += _currentLeanSpeed * Time.deltaTime;

            //apply translation for constant speed in +x direction (right)
            gameObject.transform.Translate(new Vector3(x_vel, 0, 0));

            //keep player in bounds with leaning active
            _currentPositionInBorder += _currentLeanSpeed * Time.deltaTime;

            if (_currentPositionInBorder < _borderLeft) {
                transform.Translate((_borderLeft - _currentPositionInBorder), 0, 0);
                _currentPositionInBorder = _borderLeft;
            } else if (_currentPositionInBorder > _borderRight) {
                transform.Translate(-(_currentPositionInBorder - _borderRight), 0, 0);
                _currentPositionInBorder = _borderRight;
            }

            //update position in game manager in order to detect if the player finished the level
            GameManager.Instance.UpdatePlayerPosition(transform.position.x);
        }
    }

    

    //calculate the initial force which is then applied to the rigidbody
    private float GetFlapForce(float flapForce) {
        return flapForce * _liftConstant;
    }

    //TBD Offset
    private void Shoot() {
        Vector3 offset = new Vector3(0, 0, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        GameObject projectile = Instantiate(_projectilePrefab, transform.position + offset, rotation);
    }

#if UNITY_EDITOR
    //Gizmos for player movement borders
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //draw line for left border + line for bounding box max x bounds
        Gizmos.DrawLine(new Vector3(_borderLeft - _currentPositionInBorder, -10, 0) + transform.position,
            new Vector3(_borderLeft - _currentPositionInBorder, 10, 0) + transform.position);
        UnityEditor.Handles.Label(new Vector3(_borderLeft - _currentPositionInBorder, 1, 0) + transform.position, "Left Border");

        Gizmos.DrawLine(new Vector3(_borderLeft - _currentPositionInBorder, -10, 0) + transform.position - GetComponent<Collider>().bounds.max, 
            new Vector3(_borderLeft - _currentPositionInBorder, 10, 0) + transform.position - GetComponent<Collider>().bounds.max);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(_borderRight - _currentPositionInBorder, -10, 0) + transform.position,
            new Vector3(_borderRight - _currentPositionInBorder, 10, 0) + transform.position);
        UnityEditor.Handles.Label(new Vector3(_borderRight - _currentPositionInBorder, 1, 0) + transform.position, "Right border");
    }

#endif
}
