using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMovement : MonoBehaviour
{
    [SerializeField]
    Transform[] _bodySegments;
    [SerializeField]
    Transform[] _bodyBones;
    [SerializeField]
    Transform[] _bodyTargetSegments;
    [SerializeField]
    Transform[] _leftFeetTargets;
    [SerializeField]
    Transform[] _rightFeetTarget;

    [SerializeField]
    float _maxDistanceBetweenSegments = 1;
    [SerializeField]
    float _maxRotationBetweenSegments = 45;
    [SerializeField]
    float _followSpeed = 1;
    [SerializeField]
    float _segmentRotationFollowSpeed = 1;

    public void MoveSegments(float delta)
    {
        for (int i = 0; i < _bodyBones.Length; i++)
        {
            if (Vector3.Distance(_bodyBones[i].position, _bodyTargetSegments[i].position) > _maxDistanceBetweenSegments)
            {
                float height = _bodyBones[i].position.y;
                _bodyBones[i].position = Vector3.Lerp(_bodyBones[i].position, _bodyTargetSegments[i].position, _followSpeed * delta);
                if (Quaternion.Angle(_bodyBones[i].rotation, _bodyTargetSegments[i].rotation) > _maxRotationBetweenSegments * i)
                {
                    _bodyBones[i].rotation = Quaternion.Lerp(_bodyBones[i].rotation, _bodyTargetSegments[i].rotation, _segmentRotationFollowSpeed * delta);
                }
            }
        }
    }

    public void RotateSegmentToFeetHeight(float delta)
    {
        //Cant rotate bones because Im rotating them in othe functions for example the head bone is being rotated to follow the target in that case i need to implement there the rotation or change how it works
        //For the other segments their rotation is handled in here in the MoveSegments function so i would need to override the rotation after the vertical rotation is set

        //for (int i = 0; i < _bodySegments.Length; i++)
        //{
        //    //I need to move the bones heigher or lower to follow the feet height therefore ->
        //    //Maybe i will need to move the body bone to adjust the body also change back to 1 the target rotation weight because it seems to behave better when the body bone is rotated
        //    //Set the rotation based on the feet height diference instead of getting the slope
        //    if (i == _bodySegments.Length-1)//Given the tail doesnt have legs its rotation is set to the segement before it
        //    {
        //        _bodySegments[i].Rotate(Vector3.forward, (Mathf.LerpAngle(_bodySegments[i].rotation.z, _bodySegments[i-1].rotation.z, delta)));
        //    }
        //    else
        //    {
        //        float heightDif = _leftFeetTargets[i].position.y - _rightFeetTarget[i].position.y;//if the height dif is negative it means the right foot is higher and viceversa
        //        float angleOfInclinationRad = Mathf.Asin(Mathf.Abs(heightDif) / Vector3.Distance(_rightFeetTarget[i].position, _leftFeetTargets[i].position));
        //        float angleOfInclinationDeg = angleOfInclinationRad * 180 / Mathf.PI;
        //        if (i==0)
        //        {
        //            Debug.Log(angleOfInclinationDeg);
        //        }
        //        if (Mathf.Abs(Mathf.Round(heightDif)) > 0)
        //        {
        //            _bodySegments[i].Rotate(Vector3.forward, (Mathf.LerpAngle(_bodySegments[i].rotation.z, angleOfInclinationDeg, delta)));
        //        }
        //        else
        //        {
        //            _bodySegments[i].Rotate(Vector3.forward, (Mathf.LerpAngle(_bodySegments[i].rotation.z, 0, delta)));
        //        }
        //    }
        //}
    }
}
