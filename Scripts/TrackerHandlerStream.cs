// Pedro Regalado
// Spring 2023

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TrackerHandlerStream : MonoBehaviour
{
    public Dictionary<string, string> parentJointMap;
    Dictionary<string, Quaternion> basisJointMap;
    public Quaternion[] absoluteJointRotations = new Quaternion[32];
    public bool drawSkeletons = true;
    Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public Dictionary<int, string> indexJointMap;

    // Start is called before the first frame update
    void Awake()
    {
        indexJointMap = new Dictionary<int, string>();

        indexJointMap[0] = "Pelvis";
        indexJointMap[1] = "Spine navel";
        indexJointMap[2] = "Spine chest";
        indexJointMap[3] = "Neck";
        indexJointMap[4] = "Left clavicle";
        indexJointMap[5] = "Left shoulder";
        indexJointMap[6] = "Left elbow";
        indexJointMap[7] = "Left wrist";
        indexJointMap[8] = "Left hand";
        indexJointMap[9] = "Left hand tip";
        indexJointMap[10] = "Left thumb";
        indexJointMap[11] = "Right clavicle";
        indexJointMap[12] = "Right shoulder";
        indexJointMap[13] = "Right elbow";
        indexJointMap[14] = "Right wrist";
        indexJointMap[15] = "Right hand";
        indexJointMap[16] = "Right hand tip";
        indexJointMap[17] = "Right thumb";
        indexJointMap[18] = "Left hip";
        indexJointMap[19] = "Left knee";
        indexJointMap[20] = "Left ankle";
        indexJointMap[21] = "Left foot";
        indexJointMap[22] = "Right hip";
        indexJointMap[23] = "Right knee";
        indexJointMap[24] = "Right ankle";
        indexJointMap[25] = "Right foot";
        indexJointMap[26] = "Head";
        indexJointMap[27] = "Nose";
        indexJointMap[28] = "Left eye";
        indexJointMap[29] = "Left ear";
        indexJointMap[30] = "Right eye";
        indexJointMap[31] = "Right ear";
        indexJointMap[32] = "Count";

        parentJointMap = new Dictionary<string, string>();

        // pelvis has no parent so set to count
        parentJointMap["Pelvis"] = "Count"; 
        parentJointMap["Spine navel"] = "Pelvis";
        parentJointMap["Spine chest"] = "Spine navel";
        parentJointMap["Neck"] = "Spine chest";
        parentJointMap["Left clavicle"] = "Spine chest";
        parentJointMap["Left shoulder"] = "Left clavicle";
        parentJointMap["Left elbow"] = "Left shoulder";
        parentJointMap["Left wrist"] = "Left elbow";
        parentJointMap["Left hand"] = "Left wrist";
        parentJointMap["Left hand tip"] = "Left hand";
        parentJointMap["Left thumb"] = "Left hand";
        parentJointMap["Right clavicle"] = "Spine chest";
        parentJointMap["Right shoulder"] = "Right clavicle";
        parentJointMap["Right elbow"] = "Right shoulder";
        parentJointMap["Right wrist"] = "Right elbow";
        parentJointMap["Right hand"] = "Right wrist";
        parentJointMap["Right hand tip"] = "Right hand";
        parentJointMap["Right thumb"] = "Right hand";
        parentJointMap["Left hip"] = "Spine navel";
        parentJointMap["Left knee"] = "Left hip";
        parentJointMap["Left ankle"] = "Left knee";
        parentJointMap["Left foot"] = "Left ankle";
        parentJointMap["Right hip"] = "Spine navel";
        parentJointMap["Right knee"] = "Right hip";
        parentJointMap["Right ankle"] = "Right knee";
        parentJointMap["Right foot"] = "Right ankle";
        parentJointMap["Head"] = "Pelvis";
        parentJointMap["Nose"] = "Head";
        parentJointMap["Left eye"] = "Head";
        parentJointMap["Left ear"] = "Head";
        parentJointMap["Right eye"] = "Head";
        parentJointMap["Right ear"] = "Head";

        Vector3 xpositive = Vector3.right;
        Vector3 ypositive = Vector3.up;
        Vector3 zpositive = Vector3.forward;

        // spine and left hip are the same
        Quaternion leftHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion spineHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
        Quaternion rightHipBasis = Quaternion.LookRotation(xpositive, zpositive);

        // arms and thumbs share the same basis
        Quaternion leftArmBasis = Quaternion.LookRotation(ypositive, -zpositive);
        Quaternion rightArmBasis = Quaternion.LookRotation(-ypositive, zpositive);
        Quaternion leftHandBasis = Quaternion.LookRotation(-zpositive, -ypositive);
        Quaternion rightHandBasis = Quaternion.identity;
        Quaternion leftFootBasis = Quaternion.LookRotation(xpositive, ypositive);
        Quaternion rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive);

        basisJointMap = new Dictionary<string, Quaternion>();

        // pelvis has no parent so set to count
        basisJointMap["Pelvis"] = spineHipBasis;
        basisJointMap["Spine navel"] = spineHipBasis;
        basisJointMap["Spine chest"] = spineHipBasis;
        basisJointMap["Neck"] = spineHipBasis;
        basisJointMap["Left clavicle"] = leftArmBasis;
        basisJointMap["Left shoulder"] = leftArmBasis;
        basisJointMap["Left elbow"] = leftArmBasis;
        basisJointMap["Left wrist"] = leftHandBasis;
        basisJointMap["Left hand"] = leftHandBasis;
        basisJointMap["Left hand tip"] = leftHandBasis;
        basisJointMap["Left thumb"] = leftArmBasis;
        basisJointMap["Right clavicle"] = rightArmBasis;
        basisJointMap["Right shoulder"] = rightArmBasis;
        basisJointMap["Right elbow"] = rightArmBasis;
        basisJointMap["Right wrist"] = rightHandBasis;
        basisJointMap["Right hand"] = rightHandBasis;
        basisJointMap["Right hand tip"] = rightHandBasis;
        basisJointMap["Right thumb"] = rightArmBasis;
        basisJointMap["Left hip"] = leftHipBasis;
        basisJointMap["Left knee"] = leftHipBasis;
        basisJointMap["Left ankle"] = leftHipBasis;
        basisJointMap["Left foot"] = leftFootBasis;
        basisJointMap["Right hip"] = rightHipBasis;
        basisJointMap["Right knee"] = rightHipBasis;
        basisJointMap["Right ankle"] = rightHipBasis;
        basisJointMap["Right foot"] = rightFootBasis;
        basisJointMap["Head"] = spineHipBasis;
        basisJointMap["Nose"] = spineHipBasis;
        basisJointMap["Left eye"] = spineHipBasis;
        basisJointMap["Left ear"] = spineHipBasis;
        basisJointMap["Right eye"] = spineHipBasis;
        basisJointMap["Right ear"] = spineHipBasis;
    }
    public void renderSkeleton(Dictionary<int, Tuple<Vector3, Quaternion>> streamedJoints , int skeletonNumber)
    {
        for (int jointNum = 0; jointNum < streamedJoints.Count; jointNum++)
        {
            Vector3 jointPos = new Vector3(streamedJoints[jointNum].Item1.x, -streamedJoints[jointNum].Item1.y, streamedJoints[jointNum].Item1.z);
            Vector3 offsetPosition = transform.rotation * jointPos;
            Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;

            // If I put this jointRot quaternion, then I do the work on client side; [but I'd rather do work in server side]
            Quaternion jointRot = Y_180_FLIP * new Quaternion(streamedJoints[jointNum].Item2.x, streamedJoints[jointNum].Item2.y,
                streamedJoints[jointNum].Item2.z, streamedJoints[jointNum].Item2.w) * Quaternion.Inverse(basisJointMap[indexJointMap[jointNum]]);

            //UPDATED: I let server do all the work and then just pass on values after multiplied times 180 and by the inverse of basis jointmap
            //Quaternion jointRot = new Quaternion(streamedJoints[jointNum].Item2.x, streamedJoints[jointNum].Item2.y,
            //   streamedJoints[jointNum].Item2.z, streamedJoints[jointNum].Item2.w);

            absoluteJointRotations[jointNum] = jointRot;

            // these are absolute body space because each joint has the body root for a parent in the scene graph
            transform.GetChild(skeletonNumber).GetChild(jointNum).localPosition = jointPos;
            transform.GetChild(skeletonNumber).GetChild(jointNum).localRotation = jointRot;

            const int boneChildNum = 0;

            if (parentJointMap[indexJointMap[jointNum]] != "Head" && parentJointMap[indexJointMap[jointNum]] != "Count")
            {
                Debug.Log("went into if statement");

                Vector3 parentTrackerSpacePosition = new Vector3(streamedJoints[indexJointMap.Values.ToList().IndexOf(parentJointMap[indexJointMap[jointNum]])].Item1.x,
                                                                 -streamedJoints[indexJointMap.Values.ToList().IndexOf(parentJointMap[indexJointMap[jointNum]])].Item1.y,
                                                                 streamedJoints[indexJointMap.Values.ToList().IndexOf(parentJointMap[indexJointMap[jointNum]])].Item1.z); 
                
                Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                
                Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
            }
            else  
            {
                Debug.Log("went into else statement");
                transform.GetChild(skeletonNumber).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
            }
        }
    }
}
