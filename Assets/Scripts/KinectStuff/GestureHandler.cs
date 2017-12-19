using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public class GestureHandler : MonoBehaviour {

    public static GestureHandler instance;
 
    public static BasicAvatarModel MoCapAvatar;
    //flap Gestic Variables:
    public enum handState {MID, UP, MIDtoDOWN, DOWN, MIDtoUP }; //maybe rename enum 
    private static handState curHandState = handState.MID;

    private static Vector3 handRightRel;
    private static Vector3 handLeftRel;


    //position limits - delete maybe
    private static float handDetUpY = 0.4f;
    private static float handDetDownY = -0.2f;
    private static float handDetX = 0.4f; // 0.6 * cos(rot) change for more specific calculation to DO
    private static float handDetZ = 0.3f;
    private static float max=0, min=0;

    //rotational informations
    private static float shoulderRightRotY, shoulderLeftRotY;
    private static float shoulderRightRotZ, shoulderLeftRotZ;
    private static float maxRotY;
    private static float rotYOffset = 10;
    private static float detRotUpY = 0;
    private static float detRotDownY = -10;

    //shoot Gestic Variables:
    public enum shootState { NORM, SHOOT }; //I am terible at naming stuff
    private static shootState curShootState = shootState.NORM;
    private static float shootDetZ = 0.5f;
    private static float shootBackDetZ = 0.3f;

    //testVariables:
    private static float flapCnt = 0;

    private static float rightMax=0, rightMin=20, leftMax=0, leftMin=20;
    private static float rMaxCnt = 0, rMinCnt = 0, lMaxCnt = 0, lMinCnt = 0;
	// Use this for initialization
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            MoCapAvatar = GetComponent<KinectPointManAvatarModel>();
            DontDestroyOnLoad(this.gameObject);
        }
    }

	// Update is called once per frame
	void Update () {
     /*   Vector3 spineMid = MoCapAvatar.getRawWorldPosition(JointType.SpineMid);
        Vector3 handRightRel = MoCapAvatar.getRawWorldPosition(JointType.HandRight)-spineMid;
        Vector3 handLeftRel = MoCapAvatar.getRawWorldPosition(JointType.HandLeft)-spineMid;
        HandState h = MoCapAvatar.getRightHandState();
        detectFlap(handRightRel, handLeftRel);
        detectShoot(handRightRel, handLeftRel);
        detectRightHandState(h);
        //minMaxDetect(handRightRel.x, handLeftRel.x);
       // Debug.Log("r: " + handRightRel + " -------- l: " + handLeftRel);*/
	}


    public static bool getRightHandState()
    { 
        bool temp = MoCapAvatar.getRightHandState() == HandState.Closed;
        return temp;
    }

    public static void giveMeMyInfo() {
        Debug.Log(shoulderRightRotZ);
    }
    public static void calcPositions() {
        Vector3 spineShoulder = MoCapAvatar.getRawWorldPosition(JointType.SpineShoulder);
        handRightRel = MoCapAvatar.getRawWorldPosition(JointType.HandRight) - spineShoulder;
        handLeftRel = MoCapAvatar.getRawWorldPosition(JointType.HandLeft) - spineShoulder;

        shoulderRightRotY = Mathf.Asin(handRightRel.y / handRightRel.magnitude) * 180 / Mathf.PI + rotYOffset;
        shoulderRightRotZ = Mathf.Asin(handRightRel.z / handRightRel.magnitude) * 180 / Mathf.PI + rotYOffset;

        shoulderLeftRotY = Mathf.Asin(handLeftRel.y / handRightRel.magnitude) * 180 / Mathf.PI + rotYOffset;
        shoulderLeftRotZ = Mathf.Asin(handLeftRel.z / handRightRel.magnitude) * 180 / Mathf.PI + rotYOffset;
    }
    public static bool detectShoot()
    {
        switch (curShootState)
        {
            case shootState.NORM:
                if(handRightRel.z > shootDetZ || handLeftRel.z > shootDetZ)
                //if ((handRightRel.z > shootDetZ && handLeftRel.z < shootDetZ) || (handRightRel.z < shootDetZ && handLeftRel.z > shootDetZ))
                {
                    curShootState = shootState.SHOOT;
                    curHandState = handState.MID;
                    maxRotY = 0;
                    Debug.Log("PEEEEEEEEEEEEEEEEEEEEEEEEEEWWWWWWWWWWW");
                    return true;
                }
                break;
            case shootState.SHOOT:
                if (handRightRel.z < shootBackDetZ && handLeftRel.z < shootBackDetZ)
                {
                    curShootState = shootState.NORM;
                }
                break;

            default: Debug.LogWarning("detectShoot no STATE"); break;
        }
        return false;
    }   

    public static float detectFlap()
    {
        if (handRightRel.z > handDetZ) { return 0.0f; }
        float avg = (shoulderRightRotY + shoulderLeftRotY) / 2;
        switch (curHandState)
        {
            case handState.UP:
                if (avg > maxRotY)
                    maxRotY = avg;
                if (shoulderRightRotY < detRotUpY && shoulderLeftRotY < detRotUpY)
                    curHandState = handState.MID;
                break;
            case handState.DOWN:
                if (shoulderRightRotY > detRotDownY && shoulderLeftRotY > detRotDownY)
                    curHandState = handState.MID;
                break;
            case handState.MID: //to do handle start flap
                if (shoulderRightRotY > detRotUpY && shoulderLeftRotY > detRotUpY)
                    curHandState = handState.UP;
                else if (shoulderRightRotY < detRotDownY && shoulderLeftRotY < detRotDownY) {
                    Debug.Log("Flap: " + maxRotY);
                    curHandState = handState.DOWN;
                    float temp = maxRotY;
                    maxRotY = 0;
                    return temp;
                }
                break;
            default: Debug.LogWarning("detectFlap no STATE"); break;

        }
        return 0;
    }
    public static bool detectFlap2() // change to float
    {
        if (handRightRel.x < handDetX || handLeftRel.x > -handDetX || handLeftRel.z > handDetZ || handRightRel.z > handDetZ)
        {
            if (curHandState != handState.MID)
            {
                curHandState = handState.MID;
            }
            return false;
        }
        
        switch (curHandState)
        {
            case handState.MIDtoUP:
                if (handRightRel.y > handDetUpY && handLeftRel.y > handDetUpY)
                    curHandState = handState.UP;
                break;
            case handState.UP:
                if (handRightRel.y > max || handLeftRel.y > max)
                    max = handRightRel.y > handLeftRel.y ? handRightRel.y : handLeftRel.y;

                if (handRightRel.y < handDetUpY && handLeftRel.y < handDetUpY)
                    curHandState = handState.MIDtoDOWN;
                break;
            case handState.MIDtoDOWN:
                if (handRightRel.y < handDetDownY && handLeftRel.y < handDetDownY)
                {
                    curHandState = handState.DOWN;
                    flapCnt++;
                    Debug.LogWarning("Flap"+flapCnt);
                    return true;
                }
                break;
            case handState.DOWN:
                if (handRightRel.y < min || handLeftRel.y < min)
                    min = handRightRel.y < handLeftRel.y ? handRightRel.y : handLeftRel.y;

                if (handRightRel.y > handDetDownY && handLeftRel.y > handDetDownY)
                    curHandState = handState.MIDtoUP;
                break;
            case handState.MID: //to do handle start flap
                if (handRightRel.y > handDetUpY && handLeftRel.y > handDetUpY)
                    curHandState = handState.UP;
                else if (handRightRel.y < handDetDownY && handLeftRel.y < handDetDownY)
                {
                    curHandState = handState.DOWN;
                    flapCnt+=0.5f;
                    Debug.LogWarning("Flap" + flapCnt);
                }
                break;
            default: Debug.LogWarning("detectFlap no STATE"); break;

        }
        return false;
    }


    void OnApplicationQuit()
    {

    }
}
