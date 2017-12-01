using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public abstract class BasicAvatarModel : MonoBehaviour {


    public abstract Quaternion applyRelativeRotationChange(JointType jt, Quaternion initialModelJointRotation, int player);

    public abstract Quaternion getRawWorldRotation(JointType jt, int player);

    public abstract Vector3 getRawWorldPosition(JointType jt, int player);
}
