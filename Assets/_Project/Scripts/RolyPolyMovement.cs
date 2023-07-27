using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolyPolyMovement : MonoBehaviour
{
    RolyPolyManager _rolyPolyManager;

    //General movement
    [SerializeField]
    GameObject _rolyPolyHeadPos;
    [SerializeField]
    GameObject _headingTarget;
    [SerializeField]
    float _rolyPolyMovementSpeed = 1;
    [SerializeField]
    float _rolyPolyRotationSpeed = 1;
    [SerializeField]
    float _rolyPolyBallRotationSpeed = 1;
    [SerializeField]
    float _rolyPolyJumpStrenght = 1;
    [SerializeField]
    float _rolyPolySlamStrenght = 1;
    [SerializeField]
    float _rolyPolySlamRange = 1;

    //Segment movement
    [SerializeField]
    Transform[] _bodyBones;
    [SerializeField]
    Transform[] _bodyTargetSegments;
    [SerializeField]
    Transform[] _leftFeetTargets;
    [SerializeField]
    Transform[] _rightFeetTarget;
    float[] _bodyBonesTargetRotations;
    float[] _bodyBonesStartingRotations;
    float[] _bodyBonesStartingHeightDifference;

    [SerializeField]
    float _scaleDamping = 1;
    [SerializeField]
    float _maxDistanceBetweenSegments = 1;
    [SerializeField]
    float _maxRotationBetweenSegments = 45;
    [SerializeField]
    float _followSpeed = 1;
    [SerializeField]
    float _segmentRotationFollowSpeed = 1;
    [SerializeField]
    float _segmentFeetPosRotationSpeed = 1;
    [SerializeField]
    float _feetBoneHeightRelationOffset = 1;
    [SerializeField]
    float _maxHeightfeetBoneDistanceDifference = 1;

    [SerializeField]
    float _stickiness = 1;

    bool _stickToGround = false;

    [SerializeField]
    bool _currentDirection = false;
    [SerializeField]
    bool _previousDirection = false;//left=false,right=true

    private void Start()
    {
        _rolyPolyManager = GetComponent<RolyPolyManager>();
        _bodyBonesTargetRotations = new float[_bodyBones.Length];
        _bodyBonesStartingRotations = new float[_bodyBones.Length];
        _bodyBonesStartingHeightDifference = new float[_bodyBones.Length];
        GetStartingBoneRotations();
        GetStartingFeetToSegmentBoneHeightDifference();
    }

    private void Update()
    {
        StickToTheGround(Time.deltaTime);
    }

    public void HandleMovement(RolyPolyStates state,float delta)
    {

        if (state == RolyPolyStates.Boling)
        {
            SpinRolyPoly(delta);
            SquashRolyPoly(delta);
        }
        else if (state == RolyPolyStates.Walking)
        {
            if (_rolyPolyManager.InputManager.MovementInput.y > 0 && _rolyPolyManager.InputManager.MovementInput.x == 0)
            {
                _rolyPolyHeadPos.transform.position = Vector3.MoveTowards(_rolyPolyHeadPos.transform.position, _headingTarget.transform.position, _rolyPolyMovementSpeed * delta);
            }

            if (Vector3.ProjectOnPlane(new Vector3(0, 0, _rolyPolyManager.InputManager.MovementInput.x), Vector3.up).normalized != -_rolyPolyHeadPos.transform.up)
                HandleHeadDirection(delta);

            AdjustSegmentHeight(delta);
            MoveSegments(delta);
            //RotateSegmentToFeetHeight(delta);
        }
        else
        {
            //NA
        }
    }

    public void HandleHeadDirection(float delta)
    {
        float rotationAmountX = _rolyPolyManager.InputManager.MovementInput.x * _rolyPolyRotationSpeed * Time.deltaTime;
        _rolyPolyHeadPos.transform.Rotate(0, 0, rotationAmountX);
        if (Mathf.Abs(rotationAmountX) > _maxRotationBetweenSegments)
        {
            _rolyPolyHeadPos.transform.position = Vector3.MoveTowards(_rolyPolyHeadPos.transform.position, _headingTarget.transform.position, _rolyPolyMovementSpeed * delta);
        }
    }

    public void MoveSegments(float delta)
    {
        for (int i = 1; i < _bodyBones.Length; i++)//Starts at 1 to skip the head
        {
            if (Vector3.Distance(_bodyBones[i].position, _bodyTargetSegments[i-1].position) > _maxDistanceBetweenSegments)
            {
                float height = _bodyBones[i].position.y;
                _bodyBones[i].position = Vector3.Lerp(_bodyBones[i].position, _bodyTargetSegments[i-1].position, _followSpeed * delta);//taking away 1 from the targets because the bodyBones array is bigger by 1
                if (Quaternion.Angle(_bodyBones[i].rotation, _bodyTargetSegments[i - 1].rotation) > _maxRotationBetweenSegments * i)
                {
                    _bodyBones[i].rotation = Quaternion.Lerp(_bodyBones[i].rotation, _bodyTargetSegments[i - 1].rotation, _segmentRotationFollowSpeed * delta);
                }
            }
        }
    }

    private void GetStartingFeetToSegmentBoneHeightDifference()
    {
        for (int i = 0; i < _bodyBones.Length; i++)
        {
            if (i == _bodyBones.Length - 1)
                break;

            _bodyBonesStartingHeightDifference[i] = _bodyBones[i].position.y - _rightFeetTarget[i].position.y;
            //if (i == 0)
            //{
            //    Debug.Log("Height difference at start:" + _bodyBonesStartingHeightDifference[i]);
            //}
        }
    }

    public void AdjustSegmentHeight(float delta)
    {
        for (int i = 0; i < _bodyBones.Length; i++)//Starts at 1 to skip the head
        {
            if (i ==_bodyBones.Length -1)//Tail has no legs so we skip that segment
                break;

            float heightDif = _bodyBones[i].position.y - (_rightFeetTarget[i].position.y + _leftFeetTargets[i].position.y) / 2;
            if (heightDif > _bodyBonesStartingHeightDifference[i] + _maxHeightfeetBoneDistanceDifference)
            {
                _bodyBones[i].position = Vector3.MoveTowards(_bodyBones[i].position, new Vector3(_bodyBones[i].position.x, _bodyBones[i].position.y - _feetBoneHeightRelationOffset, _bodyBones[i].position.z), delta);
            }
            else if(heightDif < _bodyBonesStartingHeightDifference[i] - _maxHeightfeetBoneDistanceDifference)
            {
                _bodyBones[i].position = Vector3.MoveTowards(_bodyBones[i].position, new Vector3(_bodyBones[i].position.x, _bodyBones[i].position.y + _feetBoneHeightRelationOffset, _bodyBones[i].position.z), delta);
            }
            else
            {
                //reached desired height
            }
        }
    }

    private void GetStartingBoneRotations()
    {
        for (int i = 0; i < _bodyBones.Length; i++)
        {
            _bodyBonesStartingRotations[i] = Mathf.Round(_bodyBones[i].localRotation.eulerAngles.x);
            if (i == 0)
            {
                Debug.Log("Bone rotation at start:" + _bodyBonesStartingRotations[i]);
            }
        }
    }

    public void RotateSegmentToFeetHeight(float delta)
    {
        for (int i = 0; i < _bodyBones.Length; i++)
        {
            if (i == _bodyBones.Length - 1)//Skips the tail
                return;

            float heightDif = _rightFeetTarget[i].position.y - _leftFeetTargets[i].position.y;
            float angleOfInclinationRad = Mathf.Asin(Mathf.Abs(heightDif) / Vector3.Distance(_rightFeetTarget[i].position, _leftFeetTargets[i].position));
            float angleOfInclinationDeg = angleOfInclinationRad * 180 / Mathf.PI;
            float currentBoneRotation = _bodyBones[i].localRotation.eulerAngles.x;

            if (Mathf.Round(heightDif * 100) < 0)
            {
                if (_bodyBonesTargetRotations[i] != Mathf.Round(_bodyBonesStartingRotations[i]) - Mathf.Round(angleOfInclinationDeg))
                {
                    _bodyBonesTargetRotations[i] = Mathf.Round(_bodyBonesStartingRotations[i]) - Mathf.Round(angleOfInclinationDeg);
                    if (i == 0)
                    {
                        Debug.Log("Changed target rotation:" + _bodyBonesTargetRotations[i]);
                    }
                }
            }
            else if (Mathf.Round(heightDif * 100) > 0)
            {
                if (_bodyBonesTargetRotations[i] != Mathf.Round(_bodyBonesStartingRotations[i]) + Mathf.Round(angleOfInclinationDeg))
                {
                    _bodyBonesTargetRotations[i] = Mathf.Round(_bodyBonesStartingRotations[i]) + Mathf.Round(angleOfInclinationDeg);
                    if (i == 0)
                    {
                        Debug.Log("Changed target rotation:" + _bodyBonesTargetRotations[i]);
                    }
                }
            }
            else
            {
                //After all I need to do something when the height difference is zero
                if (Mathf.Round(currentBoneRotation) < _bodyBonesStartingRotations[i])
                {
                    _bodyBones[i].Rotate(_bodyBones[i].forward, _segmentFeetPosRotationSpeed * delta);
                }
                else if (Mathf.Round(currentBoneRotation) > _bodyBonesStartingRotations[i])
                {
                    _bodyBones[i].Rotate(_bodyBones[i].forward, -_segmentFeetPosRotationSpeed * delta);
                }
                else
                {
                    //Reached original rotation
                    break;
                }
            }

            if (i == 0)
            {
                Debug.Log("height diff:" + Mathf.Round(heightDif));
                Debug.Log("Feet angle:" + Mathf.Round(angleOfInclinationDeg));
                Debug.Log("Bone current angle:" + Mathf.Round(currentBoneRotation));
            }

            if (Mathf.Round(currentBoneRotation) < Mathf.Round(_bodyBonesTargetRotations[i]))
            {
                _bodyBones[i].Rotate(_bodyBones[i].forward, _segmentFeetPosRotationSpeed * delta);
            }
            else if (Mathf.Round(currentBoneRotation) > Mathf.Round(_bodyBonesTargetRotations[i]))//This should be the correct target rotation
            {
                if (Mathf.Round(currentBoneRotation) < Mathf.Round(_bodyBonesStartingRotations[i]) + Mathf.Round(angleOfInclinationDeg))//But instead the rotation is increasing instead of decreasing soo i check against the value of the add between the original rotation and the target rotation
                {
                    _bodyBones[i].Rotate(_bodyBones[i].forward, -_segmentFeetPosRotationSpeed * delta);//Even though the rotation direction should be wrong, it is right
                }
            }
            else
            {
                //reached target rotation
                break;
            }
        }
    }


    private void SpinRolyPoly(float delta)
    {
        Vector3 rolyPolyVelocity = GetComponent<Rigidbody>().velocity;

        if (Mathf.Round(rolyPolyVelocity.magnitude) <= 0)//Avoids the roly poly rotating uncontrolably when the velocity is very small
            return;
        
        if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.TopView)
        {
            transform.LookAt(transform.position + Vector3.ProjectOnPlane(rolyPolyVelocity.normalized, transform.up), transform.up);
            //transform.LookAt(transform.position + Vector3.ProjectOnPlane(new Vector3(Mathf.Round(rolyPolyVelocity.normalized.x), 0, Mathf.Round(rolyPolyVelocity.normalized.z)), transform.up), transform.up);//Makes the look at direction a 8 way movement, I guess by discretizing the velocity, maybe?

            _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * _rolyPolyBallRotationSpeed * (Mathf.Abs(rolyPolyVelocity.z) + Mathf.Abs(rolyPolyVelocity.x) + Mathf.Abs(rolyPolyVelocity.y)), 0, 0, Space.Self);
        }
        if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.FrontView)
        {
            if (Mathf.Round(rolyPolyVelocity.x) > 0)
                _currentDirection = true;
            else
                _currentDirection = false;

            if (_previousDirection != _currentDirection)
            {
                //transform.LookAt(transform.position + Vector3.ProjectOnPlane(rolyPolyVelocity.normalized, transform.right), transform.up);
                if (_currentDirection)
                    transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, 90, 0)));
                else
                    transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, 270, 0)));

                _previousDirection = _currentDirection;
            }

            _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * _rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.x)) + (Mathf.Round(Mathf.Abs(rolyPolyVelocity.y)))), 0, 0, Space.Self);

            //if ((Vector3.Dot(transform.up, Vector3.up) > 0))//I need to check the orientation of the up vector because sometimes it gets flipped messing up the calculation of the look at, thats why depending the orientation it uses the transform.up or down, this approach didnt work, update: since the Y axis is no longer changing direction this evaluation is not needed anymore
            //{
            //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * _rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.x)) + (Mathf.Round(Mathf.Abs(rolyPolyVelocity.y)))), 0, 0, Space.Self);

            //    //if (Mathf.Abs(Mathf.Round(rolyPolyVelocity.x)) > Mathf.Abs(Mathf.Round(rolyPolyVelocity.y)))
            //    //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * _rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.x))), 0, 0, Space.Self);
            //    //else
            //    //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * _rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.y))), 0, 0, Space.Self);
            //}
            //else
            //{
            //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * -_rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.x)) + Mathf.Round(Mathf.Abs(rolyPolyVelocity.y))), 0, 0, Space.Self);

            //    //if (Mathf.Abs(Mathf.Round(rolyPolyVelocity.x)) > Mathf.Abs(Mathf.Round(rolyPolyVelocity.y)))
            //    //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * -_rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.x))), 0, 0, Space.Self);
            //    //else
            //    //    _rolyPolyManager.RolyPolyModel.transform.Rotate(delta * -_rolyPolyBallRotationSpeed * (Mathf.Round(Mathf.Abs(rolyPolyVelocity.y))), 0, 0, Space.Self);
            //}
        }
    }

    private float ConvertAngles(float angle)
    {
        float convertedAngle = angle;
        if (angle < 0)
            convertedAngle = 360 - angle;

        return convertedAngle;
    }

    private void SquashRolyPoly(float delta)
    {
        Vector3 rolyPolyVelocity = GetComponent<Rigidbody>().velocity;

        if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.TopView)
        {
            if (Mathf.Abs(rolyPolyVelocity.z) > 0 || Mathf.Abs(rolyPolyVelocity.x) > 0)
            {
                if (Mathf.Abs(rolyPolyVelocity.z) > Mathf.Abs(rolyPolyVelocity.x))
                {
                    _rolyPolyManager.RolyPolyModel.transform.localScale = new Vector3(Mathf.Lerp(1, .5f, Mathf.Abs(Mathf.Round(rolyPolyVelocity.magnitude)) * delta * _scaleDamping), 1, 1);//still wobly, update: since the velocity magnitude has been rounded its no longer wobly 
                }
                else
                {
                    _rolyPolyManager.RolyPolyModel.transform.localScale = new Vector3(Mathf.Lerp(1, .5f, Mathf.Abs(Mathf.Round(rolyPolyVelocity.magnitude)) * delta * _scaleDamping), 1, 1);
                }
            }
        }

        //if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.FrontView)//I dont like how the squash looks from the side
        //{
        //    if (Mathf.Abs(rolyPolyVelocity.x) > 0 || Mathf.Abs(rolyPolyVelocity.y) > 0)
        //        _rolyPolyManager.transform.localScale = new Vector3(1, Mathf.Lerp(1, .5f, Mathf.Abs(Mathf.Round(rolyPolyVelocity.magnitude)) * delta * _scaleDamping), 1);//Correct squash
        //        //_rolyPolyManager.RolyPolyModel.transform.localScale = new Vector3(Mathf.Lerp(1, .5f, (Mathf.Abs(Mathf.Round(rolyPolyVelocity.x)) + Mathf.Round(Mathf.Abs(rolyPolyVelocity.y))) * delta * _scaleDamping), 1, 1);//Incorrect squash
        //}
    }

    public void Jump()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * _rolyPolyJumpStrenght, ForceMode.Impulse);
    }
    
    public void GroundSlam()
    {
        //Add a vertical squash to the ground slam
        GetComponent<Rigidbody>().AddForce(new Vector3(-GetComponent<Rigidbody>().velocity.x, -GetComponent<Rigidbody>().velocity.y, -GetComponent<Rigidbody>().velocity.z), ForceMode.VelocityChange);
        if (!_stickToGround)
        {
            if (Physics.Raycast(transform.position, GetRolyPolyDown(), out RaycastHit hitInfo, _rolyPolySlamRange, _rolyPolyManager.GroundLayer))
            {
                if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Bouncer"))
                {
                    _stickToGround = true;
                    StartCoroutine("Unstick");
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(GetRolyPolyDown() * _rolyPolySlamStrenght, ForceMode.Impulse);
                }
            }
        }
    }

    public Vector3 GetRolyPolyDown()
    {
        Vector3 downDirection = Vector3.zero;
        if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.TopView)
        {
            downDirection = -transform.up;
        }
        else if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.FrontView)
        {
            downDirection = -Vector3.up;
        }
        return downDirection;
    }

    private void StickToTheGround(float delta)
    {
        if (_stickToGround)
        {
            if (Physics.Raycast(transform.position, GetRolyPolyDown(), out RaycastHit hitInfo, _rolyPolySlamRange, _rolyPolyManager.GroundLayer))
            {
                Debug.DrawRay(transform.position, GetRolyPolyDown());
                transform.SetPositionAndRotation(Vector3.Lerp(transform.position, hitInfo.point, delta * _stickiness), transform.rotation);
            }
        }
    }

    IEnumerator Unstick()
    {
        float startTime = Time.time;
        while (Time.time < startTime + .5f)
        {
            //if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.TopView)
            //{
            //    GetComponent<Rigidbody>().AddForce(-transform.up * _stickiness, ForceMode.Acceleration);
            //}
            //else if (_rolyPolyManager.PinballManager.cameraMode == CameraMode.FrontView)
            //{
            //    GetComponent<Rigidbody>().AddForce(-Vector3.up * _stickiness, ForceMode.Acceleration);
            //}
            yield return null;
            GetComponent<Rigidbody>().drag = _stickiness;
        }
        _stickToGround = false;
        GetComponent<Rigidbody>().drag = .2f;
    }
}
