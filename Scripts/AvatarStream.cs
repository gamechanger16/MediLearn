// Pedro Regalado
// Ph.D
// Spring 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Linq;

public class AvatarStream : MonoBehaviour
{
    public TrackerHandlerStream KinectDevice;
    Dictionary<int, Quaternion> absoluteOffsetMap;
    Animator AvatarAnimator;
    public GameObject RootPosition;
    public Transform CharacterRootTransform;
    public float OffsetY;
    public float OffsetZ;
    public Dictionary<int, string> indexJointMap;

    public string indexJointMapping(int index)
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

        return indexJointMap[index];
    }

    private static HumanBodyBones MapKinectJoint(string joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case "Pelvis": return HumanBodyBones.Hips;
            case "Spine navel": return HumanBodyBones.Spine;
            case "Spine chest": return HumanBodyBones.Chest;
            case "Neck": return HumanBodyBones.Neck;
            case "Head": return HumanBodyBones.Head;
            case "Left hip": return HumanBodyBones.LeftUpperLeg;
            case "Left knee": return HumanBodyBones.LeftLowerLeg;
            case "Left ankle": return HumanBodyBones.LeftFoot;
            case "Left foot": return HumanBodyBones.LeftToes;
            case "Right hip": return HumanBodyBones.RightUpperLeg;
            case "Right knee": return HumanBodyBones.RightLowerLeg;
            case "Right ankle": return HumanBodyBones.RightFoot;
            case "Right foot": return HumanBodyBones.RightToes;
            case "Left clavicle": return HumanBodyBones.LeftShoulder;
            case "Left shoulder": return HumanBodyBones.LeftUpperArm;
            case "Left elbow": return HumanBodyBones.LeftLowerArm;
            case "Left wrist": return HumanBodyBones.LeftHand;
            case "Right clavicle": return HumanBodyBones.RightShoulder;
            case "Right shoulder": return HumanBodyBones.RightUpperArm;
            case "Right elbow": return HumanBodyBones.RightLowerArm;
            case "Right wrist": return HumanBodyBones.RightHand;
            // TODO: Add all 32 joints to get a better smoother avatar moving eyes, ears, etc... [PEDRO]
            default: return HumanBodyBones.LastBone;
        }
    }
    private void Start()
    {
        AvatarAnimator = GetComponent<Animator>();
        Transform _rootJointTransform = CharacterRootTransform;

        absoluteOffsetMap = new Dictionary<int, Quaternion>();

        for (int i = 0; i < 32; i++) // TODO: change the 32 for something more dynamic once you convert the functions from other class
        {
            HumanBodyBones hbb = MapKinectJoint(indexJointMapping(i));
            if (hbb != HumanBodyBones.LastBone)
            {
                Transform transform = AvatarAnimator.GetBoneTransform(hbb);
                Quaternion absOffset = GetSkeletonBone(AvatarAnimator, transform.name).rotation;

                // find the absolute offset for the T-Pose
                while (!ReferenceEquals(transform, _rootJointTransform))
                {
                    transform = transform.parent;
                    absOffset = GetSkeletonBone(AvatarAnimator, transform.name).rotation * absOffset;
                }
                absoluteOffsetMap[indexJointMap.Values.ToList().IndexOf(indexJointMapping((i)))] = absOffset;
            }
        }
    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
    {
        int count = 0;
        StringBuilder cloneName = new StringBuilder(boneName);
        cloneName.Append("(Clone)");

        foreach (SkeletonBone sb in animator.avatar.humanDescription.skeleton)
        {
            if (sb.name == boneName || sb.name == cloneName.ToString())
            {
                return animator.avatar.humanDescription.skeleton[count];
            }
            count++;
        }
        return new SkeletonBone();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        for (int j = 0; j < 32; j++) // TODO: change the 32 for something more dynamic once you convert the functions from other class
        {
            if (MapKinectJoint(indexJointMapping(j)) != HumanBodyBones.LastBone && absoluteOffsetMap.ContainsKey(indexJointMap.Values.ToList().IndexOf(indexJointMapping((j)))))
            {
                // get the absolute offset
                Quaternion absOffset = absoluteOffsetMap[indexJointMap.Values.ToList().IndexOf(indexJointMapping((j)))];
                Transform finalJoint = AvatarAnimator.GetBoneTransform(MapKinectJoint(indexJointMapping(j)));
                finalJoint.rotation = absOffset * Quaternion.Inverse(absOffset) * KinectDevice.absoluteJointRotations[j] * absOffset;

                if (j == 0)
                {
                    // character root plus translation reading from the kinect, plus the offset from the script public variables
                    finalJoint.position = CharacterRootTransform.position + new Vector3(RootPosition.transform.localPosition.x, RootPosition.transform.localPosition.y + OffsetY, RootPosition.transform.localPosition.z - OffsetZ);
                }
            }
        }
    }
}
