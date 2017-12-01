using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class UCharacterAvatarController :  BasicAvatarController
{

	public override void Start(){

        // find transforms of model
        SpineBase = GameObject.Find("joint_HipMaster").transform;
        SpineMid = GameObject.Find("joint_TorsoA").transform;
        Neck = GameObject.Find("joint_Neck").transform;
        Head = GameObject.Find("joint_Head").transform;
        ShoulderLeft = GameObject.Find("joint_ShoulderLT").transform;
        ElbowLeft = GameObject.Find("joint_ElbowLT").transform;
        //WristLeft = GameObject.Find("joint_HandLT").transform;
        HandLeft = GameObject.Find("joint_HandLT").transform;
        ShoulderRight = GameObject.Find("joint_ShoulderRT").transform;
        ElbowRight = GameObject.Find("joint_ElbowRT").transform;
        //WristRight = GameObject.Find("joint_RightHand").transform;
        HandRight = GameObject.Find("joint_HandRT").transform;
        HipLeft = GameObject.Find("joint_HipLT").transform;
        KneeLeft = GameObject.Find("joint_KneeLT").transform;
        AnkleLeft = GameObject.Find("joint_FootLT").transform;
        FootLeft = GameObject.Find("joint_ToeLT").transform;
        HipRight = GameObject.Find("joint_HipRT").transform;
        KneeRight = GameObject.Find("joint_KneeRT").transform;
        AnkleRight = GameObject.Find("joint_FootRT").transform;
        FootRight = GameObject.Find("joint_ToeRT").transform;
        SpineShoulder = GameObject.Find("joint_TorsoB").transform;
        //HandTipLeft = GameObject.Find("joint_LeftHandIndex1").transform;
        //ThumbLeft = GameObject.Find("joint_LeftHandThumb1").transform;
        //HandTipRight = GameObject.Find("joint_RightHandIndex1").transform;
        //ThumbRight = GameObject.Find("joint_RightHandThumb1").transform;

        base.Start();
    }

    public virtual void Update()
    {
        base.Update();
    }
}
