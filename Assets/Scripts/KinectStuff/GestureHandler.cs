using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public class GestureHandler : MonoBehaviour {

    private static GestureHandler _instance = null;
    public static GestureHandler Instance { get { return _instance; } }
 
    private BasicAvatarModel _moCapAvatar;
   
    //flap Gestic Variables:
    public enum handState {MID, UP, MIDtoDOWN, DOWN, MIDtoUP }; //maybe rename enum 
    private handState _curHandState = handState.MID;

    //position limits
    //private float _handDetX = 0.4f; // 0.6 * cos(rot) change for more specific calculation to DO
    private Vector3 _handRightRel;
    private Vector3 _handLeftRel;
    private float _handDetZ = 0.3f;

    private float _screenXDim = Screen.width;
    private float _screenYDim = Screen.height;
    

    //rotational informations
    //private float _shoulderRightRotZ, _shoulderLeftRotZ;
    private float _shoulderRightRotY, _shoulderLeftRotY;
    private float _maxRotY;
    private float _rotYOffset = 10;//offset to shift detectionarea upwards= ... _shoulderXRoty = originalRot + offset
    private float _detRotUpY = 0;
    private float _detRotDownY = -10;

    //shoot Gestic Variables:
    public enum shootState { NORM, SHOOT }; //I am terible at naming stuff
    private shootState _curShootState = shootState.NORM;
    private float _shootDetZ = 0.5f;
    private float _shootBackDetZ = 0.3f;

    //head Tilt Variables:
    private float _headRotY;
    private float _headDetY = 15 ;

    //testVariables:
    private float flapCnt = 0;

    private float rightMax=0, rightMin=20, leftMax=0, leftMin=20;
    private float rMaxCnt = 0, rMinCnt = 0, lMaxCnt = 0, lMinCnt = 0;
	// Use this for initialization
    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            _moCapAvatar = GetComponent<KinectPointManAvatarModel>();
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public bool detectPlayer() {
        return _moCapAvatar.detectPlayer();
    }


    public bool getRightHandState() //returns true if right Hand is closed
    { 
        bool temp = _moCapAvatar.getRightHandState() == HandState.Closed;
        return temp;
    }

    public void giveMeMyInfo() {
        Debug.Log("Info: ");
    }

    public void calcPositions() {//call this before retrieving detectInformations
        Vector3 spineShoulder = _moCapAvatar.getRawWorldPosition(JointType.SpineShoulder);
        _handRightRel = _moCapAvatar.getRawWorldPosition(JointType.HandRight) - spineShoulder;
        _handLeftRel = _moCapAvatar.getRawWorldPosition(JointType.HandLeft) - spineShoulder;

        _shoulderRightRotY = Mathf.Asin(_handRightRel.y / _handRightRel.magnitude) * 180 / Mathf.PI + _rotYOffset;
        //_shoulderRightRotZ = Mathf.Asin(_handRightRel.z / _handRightRel.magnitude) * 180 / Mathf.PI + _rotYOffset;

        _shoulderLeftRotY = Mathf.Asin(_handLeftRel.y / _handRightRel.magnitude) * 180 / Mathf.PI + _rotYOffset;
        //_shoulderLeftRotZ = Mathf.Asin(_handLeftRel.z / _handRightRel.magnitude) * 180 / Mathf.PI + _rotYOffset;

        Vector3 headBase = _moCapAvatar.getRawWorldPosition(JointType.SpineMid);
        Vector3 head = _moCapAvatar.getRawWorldPosition(JointType.Head);
        Vector3 headDir = head - headBase;
        _headRotY = Mathf.Asin(headDir.y/headDir.magnitude)*180/Mathf.PI;
        if (headDir.x > 0)
            _headRotY = 180 - _headRotY;
    }
    public Vector2 getMappedRightHandPosition()
    {
        Vector2 result;
        
        result.x = (_handRightRel.x - 0.1f)/0.6f;
        result.y = (_handRightRel.y + 0.2f)/0.4f;
        return result;

    }

    public int detectLean()
    {
        if (_headRotY < 90-_headDetY)
            return -1;
        else if (_headRotY < 90+_headDetY)
            return 0;
        else
            return 1;
    }

    public bool detectShoot() //returns true when shoot gesture is detected
    {
        switch (_curShootState)
        {
            case shootState.NORM:
                if(_handRightRel.z > _shootDetZ || _handLeftRel.z > _shootDetZ)
                //if ((handRightRel.z > shootDetZ && handLeftRel.z < shootDetZ) || (handRightRel.z < shootDetZ && handLeftRel.z > shootDetZ))
                {
                    _curShootState = shootState.SHOOT;
                    _curHandState = handState.MID;
                    _maxRotY = 0;
                    Debug.Log("PEEEEEEEEEEEEEEEEEEEEEEEEEEWWWWWWWWWWW");
                    return true;
                }
                break;
            case shootState.SHOOT:
                if (_handRightRel.z < _shootBackDetZ && _handLeftRel.z < _shootBackDetZ)
                {
                    _curShootState = shootState.NORM;
                }
                break;

            default: Debug.LogWarning("detectShoot no STATE"); break;
        }
        return false;
    }   

    public float detectFlap() //returns a float between 0 and 1 when flap is detected(1frame), else 0
    {
        if (_handRightRel.z > _handDetZ) {return 0.0f; }
        float avg = (_shoulderRightRotY + _shoulderLeftRotY) / 2;
        switch (_curHandState)
        {
            case handState.UP:
                if (avg > _maxRotY)
                    _maxRotY = avg;
                if (_shoulderRightRotY < _detRotUpY && _shoulderLeftRotY < _detRotUpY)
                    _curHandState = handState.MID;
                break;
            case handState.DOWN:
                if (_shoulderRightRotY > _detRotDownY && _shoulderLeftRotY > _detRotDownY)
                    _curHandState = handState.MID;
                break;
            case handState.MID: //to do handle start flap
                if (_shoulderRightRotY > _detRotUpY && _shoulderLeftRotY > _detRotUpY)
                    _curHandState = handState.UP;
                else if (_shoulderRightRotY < _detRotDownY && _shoulderLeftRotY < _detRotDownY) {
                    //Debug.Log("Flap: " + _maxRotY);
                    _curHandState = handState.DOWN;
                    float temp = _maxRotY;
                    _maxRotY = 0;
                    if(temp != 0)
                        return (temp-10)/80f;
                }
                break;
            default: Debug.LogWarning("detectFlap no STATE"); break;

        }
        return 0;
    }
    /*
    public bool detectFlap2() // detects flap with positions - probably obsolete
    {
        if (_handRightRel.x < _handDetX || _handLeftRel.x > -_handDetX || _handLeftRel.z > _handDetZ || _handRightRel.z > _handDetZ)
        {
            if (_curHandState != handState.MID)
            {
                _curHandState = handState.MID;
            }
            return false;
        }
        
        switch (_curHandState)
        {
            case handState.MIDtoUP:
                if (_handRightRel.y > _handDetUpY && _handLeftRel.y > _handDetUpY)
                    _curHandState = handState.UP;
                break;
            case handState.UP:
                if (_handRightRel.y > _max || _handLeftRel.y > _max)
                    _max = _handRightRel.y > _handLeftRel.y ? _handRightRel.y : _handLeftRel.y;

                if (_handRightRel.y < _handDetUpY && _handLeftRel.y < _handDetUpY)
                    _curHandState = handState.MIDtoDOWN;
                break;
            case handState.MIDtoDOWN:
                if (_handRightRel.y < _handDetDownY && _handLeftRel.y < _handDetDownY)
                {
                    _curHandState = handState.DOWN;
                    flapCnt++;
                    Debug.LogWarning("Flap"+flapCnt);
                    return true;
                }
                break;
            case handState.DOWN:
                if (_handRightRel.y < _min || _handLeftRel.y < _min)
                    _min = _handRightRel.y < _handLeftRel.y ? _handRightRel.y : _handLeftRel.y;

                if (_handRightRel.y > _handDetDownY && _handLeftRel.y > _handDetDownY)
                    _curHandState = handState.MIDtoUP;
                break;
            case handState.MID: //to do handle start flap
                if (_handRightRel.y > _handDetUpY && _handLeftRel.y > _handDetUpY)
                    _curHandState = handState.UP;
                else if (_handRightRel.y < _handDetDownY && _handLeftRel.y < _handDetDownY)
                {
                    _curHandState = handState.DOWN;
                    flapCnt+=0.5f;
                    Debug.LogWarning("Flap" + flapCnt);
                }
                break;
            default: Debug.LogWarning("detectFlap no STATE"); break;

        }
        return false;
    }
    */

    void OnApplicationQuit()
    {

    }
}
