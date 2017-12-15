using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public class GestureHandler : MonoBehaviour {


    public BasicAvatarModel MoCapAvatar;

    //flap Gestic Variables:
    private enum handState {MID, UP, MIDtoDOWN, DOWN, MIDtoUP }; //maybe rename enum 
    private handState curHandState = handState.MID;
    private float handDetUpY = 0.4f;
    private float handDetDownY = -0.2f;
    private float handDetX = 0.4f; // 0.6 * cos(rot) change for more specific calculation to DO
    private float handDetZ = 0.3f;


    //shoot Gestic Variables:
    private enum shootState { NORM, SHOOT }; //I am terible at naming stuff
    private shootState curShootState = shootState.NORM;
    private float shootDetZ = 0.5f;
    private float shootBackDetZ = 0.3f;

    //testVariables:
    private float flapCnt = 0;

    private float rightMax=0, rightMin=20, leftMax=0, leftMin=20;
    private float rMaxCnt = 0, rMinCnt = 0, lMaxCnt = 0, lMinCnt = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 spineMid = MoCapAvatar.getRawWorldPosition(JointType.SpineMid);
        Vector3 handRightRel = MoCapAvatar.getRawWorldPosition(JointType.HandRight)-spineMid;
        Vector3 handLeftRel = MoCapAvatar.getRawWorldPosition(JointType.HandLeft)-spineMid;
        HandState h = MoCapAvatar.getRightHandState();
        detectFlap(handRightRel, handLeftRel);
        detectShoot(handRightRel, handLeftRel);
        detectRightHandState(h);
        //minMaxDetect(handRightRel.x, handLeftRel.x);
       // Debug.Log("r: " + handRightRel + " -------- l: " + handLeftRel);
	}

    void detectRightHandState(HandState h)
    {
        if (h == HandState.Closed)
        {
            Debug.Log("Closed");
        }
    }
    void detectShoot(Vector3 handRight, Vector3 handLeft)
    {
        switch (curShootState)
        {
            case shootState.NORM:
                if ((handRight.z > shootDetZ && handLeft.z < shootDetZ) || (handRight.z < shootDetZ && handLeft.z > shootDetZ))
                {
                    Debug.LogWarning("SHOOOOOOOOOOOOOOOOOOOT");
                    curShootState = shootState.SHOOT;
                }
                break;
            case shootState.SHOOT:
                if (handRight.z < shootBackDetZ && handLeft.z < shootBackDetZ)
                {
                    curShootState = shootState.NORM;
                    Debug.LogWarning("Back");
                }
                break;

            default: Debug.LogWarning("detectShoot no STATE"); break;
        }
    }
    void detectFlap(Vector3 handRight, Vector3 handLeft)
    {
        if (handRight.x < handDetX || handLeft.x > -handDetX || handLeft.z > handDetZ || handRight.z > handDetZ)
        {
            if (curHandState != handState.MID)
            {
                curHandState = handState.MID;
            }
            return;
        }
        
        switch (curHandState)
        {
            case handState.MIDtoUP:
                if (handRight.y > handDetUpY && handLeft.y > handDetUpY)
                    curHandState = handState.UP;
                break;
            case handState.UP:
                if (handRight.y < handDetUpY && handLeft.y < handDetUpY)
                    curHandState = handState.MIDtoDOWN;
                break;
            case handState.MIDtoDOWN:
                if (handRight.y < handDetDownY && handLeft.y < handDetDownY)
                {
                    curHandState = handState.DOWN;
                    flapCnt++;
                    Debug.LogWarning("Flap"+flapCnt);
                }
                break;
            case handState.DOWN:
                if (handRight.y > handDetDownY && handLeft.y > handDetDownY)
                    curHandState = handState.MIDtoUP;
                break;
            case handState.MID: //to do handle start flap
                if (handRight.y > handDetUpY && handLeft.y > handDetUpY)
                    curHandState = handState.UP;
                else if (handRight.y < handDetDownY && handLeft.y < handDetDownY)
                {
                    curHandState = handState.DOWN;
                    flapCnt+=0.5f;
                    Debug.LogWarning("Flap" + flapCnt);
                }
                break;
            default: Debug.LogWarning("detectFlap no STATE"); break;

        }
    }

    void minMaxDetect(float handRightRel, float handLeftRel)
    {
        if (handRightRel > rightMax)
        {
            rightMax = handRightRel;
            rMaxCnt = 1;
        }

        else if (handRightRel < rightMin)
        {
            rightMin = handRightRel;
            rMinCnt = 1;
        }
        if (handLeftRel > leftMax)
        {
            leftMax = handLeftRel;
            rMaxCnt = 1;
        }
        else if (handLeftRel < leftMin)
        {
            leftMin = handLeftRel;
            rMinCnt = 1;
        }
    }
    void OnApplicationQuit()
    {
        Debug.Log("rMax: " + rightMax);
        Debug.Log("rMin: " + rightMin);
        Debug.Log("lMax: " + leftMax);
        Debug.Log("lMin: " + leftMin);
    }
}
