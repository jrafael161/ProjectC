using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RolyPolyStates
{
    Boling,
    Walking
}

public class RolyPolyManager : MonoBehaviour
{
    InputManager _inputManager;
    PinballManager _pinballManager;
    RolyPolyMovement _rolyPolyMovement;

    [SerializeField]
    GameObject _cameraTarget;

    [SerializeField]
    float _speedLimit=1;
    
    [SerializeField]
    RolyPolyStates _state;
    [SerializeField]
    GameObject _rolyPolyModel;

    [SerializeField]
    Transform _groundCheckPos;
    [SerializeField]
    bool _isGrounded = false;
    [SerializeField]
    LayerMask _groundLayer;
    
    public RolyPolyMovement RolyPolyMovement { get => _rolyPolyMovement; }
    public InputManager InputManager { get => _inputManager; }
    public PinballManager PinballManager { get => _pinballManager; }
    public GameObject RolyPolyModel { get => _rolyPolyModel; }
    public GameObject CameraTarget { get => _cameraTarget; }
    public bool IsGrounded { get => _isGrounded; }
    public LayerMask GroundLayer { get => _groundLayer; }

    private void Start()
    {
        _rolyPolyMovement = GetComponent<RolyPolyMovement>();
        _inputManager = GetComponentInParent<InputManager>();
        _pinballManager = GetComponentInParent<PinballManager>();
        _state = RolyPolyStates.Boling;
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        GroundCheck();
        _rolyPolyMovement.HandleMovement(_state,delta);
    }

    public void ChangeRolyPolyState(RolyPolyStates rolyPolyState)
    {
        switch (rolyPolyState)
        {
            case RolyPolyStates.Boling:
                _state = RolyPolyStates.Boling;
                break;
            case RolyPolyStates.Walking:
                _state = RolyPolyStates.Walking; 
                break;
            default:
                break;
        }
    }

    public void GroundCheck()
    {
        _isGrounded = Physics.CheckSphere(_groundCheckPos.position, .2f, _groundLayer);
    }

    public void LimitVelocity(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > _speedLimit)
        {
            //Debug.Log("Before:" + velocity.x);
            if (Mathf.Sign(velocity.x) > 0)
            {
                GetComponent<Rigidbody>().velocity.Set(_speedLimit, velocity.y, velocity.z);
            }
            if (Mathf.Sign(velocity.x) < 0)
            {
                GetComponent<Rigidbody>().velocity.Set(-_speedLimit, velocity.y, velocity.z);
            }
            //Debug.Log("After:" + velocity.x);
        }

        if (Mathf.Abs(velocity.y) > _speedLimit)
        {
            //Debug.Log("Before:" + velocity.y);
            if (Mathf.Sign(velocity.y) > 0)
            {
                GetComponent<Rigidbody>().velocity.Set(velocity.x, _speedLimit, velocity.z);
            }
            if (Mathf.Sign(velocity.y) < 0)
            {
                GetComponent<Rigidbody>().velocity.Set(velocity.x, -_speedLimit, velocity.z);
            }
            //Debug.Log("After:" + velocity.y);
        }

        if (Mathf.Abs(velocity.z) > _speedLimit)
        {
            //Debug.Log("Before:" + velocity.z);
            if (Mathf.Sign(velocity.z) > 0)
            {
                GetComponent<Rigidbody>().velocity.Set(velocity.x, velocity.y, _speedLimit);
            }
            if (Mathf.Sign(velocity.z) < 0)
            {
                GetComponent<Rigidbody>().velocity.Set(velocity.x, velocity.y, -_speedLimit);
            }
            //Debug.Log("After:" + velocity.z);
        }
    }

    public bool CheckVelocity(Vector3 velocity)
    {
        if ((Mathf.Abs(velocity.x) > _speedLimit || Mathf.Abs(velocity.y) > _speedLimit) || Mathf.Abs(velocity.z) > _speedLimit)
        {
            LimitVelocity(velocity);
            return true;
        }

        return false;
    }


    //private void OnDrawGizmos()
    //{
    //    Vector3 rolyPolyVelocity = GetComponent<Rigidbody>().velocity;
    //    Debug.DrawLine(transform.position, transform.position + rolyPolyVelocity, Color.red, .1f);
    //    Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, .1f);
    //}
}
