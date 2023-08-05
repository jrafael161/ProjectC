using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    TopView,
    FrontView
}

public class PinballManager : MonoBehaviour
{
    InputManager _inputManager;

    [SerializeField]
    Cinemachine.CinemachineVirtualCamera RolyPolyCameraTopDownView;
    [SerializeField]
    Cinemachine.CinemachineVirtualCamera RolyPolyCameraFrontFacingView;
    [SerializeField]
    public CameraMode cameraMode;
    [SerializeField]
    float _zoomingVelocity=1;

    [SerializeField]
    HingeJoint _leftHinge, _rightHinge;
    [SerializeField]
    GameObject _springBlock;
    [SerializeField]
    RolyPolyManager _rolyPoly;
    [SerializeField]
    GameObject _ejectionGate;
    [SerializeField]
    GameObject _leftEjectionGate;
    [SerializeField]
    GameObject _rightEjectionGate;
    [SerializeField]
    Transform _rolyPolySpawn;
    [SerializeField]
    float _redirectionForce = 1;
    [SerializeField]
    float _redirectionForceVis = 1;

    [SerializeField]
    float _restPosition = 0;
    [SerializeField]
    float _pressedPosition = 45;
    [SerializeField]
    float _hitStrenght = 10000;
    [SerializeField]
    float _flipperDamper = 150;
    [SerializeField]
    float _springForce = 15;
    [SerializeField]
    float _pushForce = 10;

    JointSpring _spring;
    bool _isPaused = false;
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    bool _visualHelp = false;
    Vector2 _initialCursorPos;
    Vector2 _currentCursorPos;
    Vector3 _rolyPolyPreviosVel;
    void Start()
    {
        _inputManager = GetComponent<InputManager>();
        _spring = new JointSpring();

        #if UNITY_EDITOR
                Application.targetFrameRate = 60;
        #else
                Application.targetFrameRate = -1;
        #endif

        StartPinballMachine();
    }

    void Update()
    {
        float delta = Time.deltaTime;
        _inputManager.TickInput(delta);
        HandleCameraZoomInZoomOut(delta);
    }

    private void FixedUpdate()
    {
        _inputManager.FixedTickInput(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        _inputManager.RB_Btn_Input = false;
        _inputManager.RT_Btn_Input = false;
        _inputManager.LB_Btn_Input = false;
        _inputManager.LT_Btn_Input = false;

        _inputManager.A_Btn_Input = false;
        _inputManager.B_Btn_Input = false;
        _inputManager.Y_Btn_Input = false;

        _inputManager.Start_btn_Input = false;
        _inputManager.Select_btn_Input = false;
    }

    private void StartPinballMachine()
    {
        _leftHinge.useSpring = true;
        _rightHinge.useSpring = true;
        _spring.spring = _hitStrenght;
        _spring.damper = _flipperDamper;
        cameraMode = CameraMode.TopView;
        _rolyPolyPreviosVel = _rolyPoly.GetComponent<Rigidbody>().velocity;
    }

    public void ActionateLeftFlipper(bool pressed)
    {
        if (pressed)
        {
            _spring.targetPosition = -_pressedPosition;
        }
        else
        {
            _spring.targetPosition = _restPosition;
        }
        _leftHinge.spring = _spring;
        _leftHinge.useLimits = true;
    }

    public void ActionateRightFlipper(bool pressed)
    {
        if (pressed)
        {
            _spring.targetPosition = _pressedPosition;
        }
        else
        {
            _spring.targetPosition = _restPosition;
        }
        _rightHinge.spring = _spring;
        _rightHinge.useLimits = true;
    }

    public void PullSpring(float delta)
    {
        _springBlock.GetComponent<Rigidbody>().velocity += -_springBlock.transform.up * _springForce * delta;
    }

    public void RedirectRolyPoly(bool startRedirection)
    {
        if (startRedirection)
        {
            _initialCursorPos = _inputManager.CursorPos;
            Time.timeScale = 0;
            _visualHelp = true;
        }

        if (!startRedirection)
        {
            _rolyPoly.GetComponent<Rigidbody>().velocity = Vector3.zero;

            _currentCursorPos = _inputManager.CursorPos;

            Vector2 movementVector = (_initialCursorPos - _currentCursorPos) * -1;
            Vector3 movementVector3 = Vector3.zero;

            if (cameraMode == CameraMode.TopView)
            {
                movementVector3 = new Vector3(movementVector.x, 0, movementVector.y);
            }
            else if (cameraMode == CameraMode.FrontView)
            {
                movementVector3 = new Vector3(movementVector.x, movementVector.y, 0);
            }

            Vector3 movementVector3OnPlane = Vector3.ProjectOnPlane(movementVector3, Camera.main.transform.forward);
            Vector3 redirectionVector = -movementVector3OnPlane;

            Debug.DrawLine(_rolyPoly.transform.position, (_rolyPoly.transform.position + redirectionVector * _redirectionForce) , Color.cyan, 5f);

            _rolyPoly.GetComponent<Rigidbody>().AddForce(redirectionVector * _redirectionForce, ForceMode.Impulse);
            _rolyPoly.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(redirectionVector * _redirectionForce);
                
            Time.timeScale = 1;
            _visualHelp = false;
        }
    }

    public void EnterDoor()
    {

    }

    public void ShakeTable()
    {

    }

    private void ChangeCameraToTopDownView()
    {
        RolyPolyCameraFrontFacingView.Priority = 9;
        RolyPolyCameraTopDownView.Priority = 11;
        cameraMode = CameraMode.TopView;
        _rolyPoly.transform.SetPositionAndRotation(_rolyPoly.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        _rolyPoly.RolyPolyModel.transform.localScale = new Vector3(1, 1, 1);
    }

    private void ChangeCameraToFrontView()
    {
        RolyPolyCameraFrontFacingView.Priority = 11;
        RolyPolyCameraTopDownView.Priority = 9;
        cameraMode = CameraMode.FrontView;
        _rolyPoly.transform.SetPositionAndRotation(_rolyPoly.transform.position, Quaternion.Euler(new Vector3(0,90,0)));
        _rolyPoly.RolyPolyModel.transform.localScale = new Vector3(1, 1, 1);
    }

    private void HandleCameraZoomInZoomOut(float delta)
    {
        Vector3 rolyPolyVel = _rolyPoly.GetComponent<Rigidbody>().velocity;
        Transform cameraTargetTransform = _rolyPoly.GetComponent<RolyPolyManager>().CameraTarget.transform;

        //Debug.Log(Mathf.Abs(rolyPolyVel.magnitude));

        if (cameraMode == CameraMode.TopView)
        {
            cameraTargetTransform.localPosition = new Vector3(0, Mathf.Lerp(0, 10, Mathf.Round(Mathf.Abs(rolyPolyVel.magnitude)) * _zoomingVelocity * delta), 0);
        }
        else if (cameraMode == CameraMode.FrontView)
        {
            cameraTargetTransform.localPosition = new Vector3(Mathf.Lerp(0, 10 * rolyPolyVel.normalized.x, Mathf.Round(Mathf.Abs(rolyPolyVel.magnitude)) * _zoomingVelocity * delta), 0, 0);
        }
        else
        {
            Debug.LogWarning("No camera mode defined");
        }
        _rolyPolyPreviosVel = rolyPolyVel;
    }

    public void ResetGame()
    {
        if (_rolyPoly != null)
        {
            StopAllCoroutines();

            ChangeCameraToTopDownView();
            _rolyPoly.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _springBlock.GetComponent<Rigidbody>().inertiaTensor = Vector3.zero;
            _springBlock.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _rolyPoly.transform.SetPositionAndRotation(_rolyPolySpawn.position, new Quaternion(0,0,0,0));
        }
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void PushBall(bool isLeft)
    {
        if (isLeft)
        {
            _leftEjectionGate.GetComponent<MeshCollider>().enabled = true;
            _leftEjectionGate.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            _rightEjectionGate.GetComponent<MeshCollider>().enabled = true;
            _rightEjectionGate.GetComponent<MeshRenderer>().enabled = true;
        }

        _rolyPoly.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _rolyPoly.GetComponent<Rigidbody>().velocity = _rolyPoly.transform.forward * _pushForce;
    }

    private void CloseGate()
    {
        _ejectionGate.GetComponent<MeshCollider>().enabled = true;
        _ejectionGate.GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnDrawGizmos()
    {
        float distanceFromCamera = -1;
        RaycastHit hitPoint;

        if (_inputManager != null)
        {
            _currentCursorPos = _inputManager.CursorPos;
        }

        //Gizmos.DrawRay(Camera.main.transform.position, Camera.main.ScreenPointToRay(currentCursorPos).direction * 100);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.ScreenPointToRay(_currentCursorPos).direction * 100, out hitPoint))
        {
            distanceFromCamera = Vector3.Distance(Camera.main.transform.position, hitPoint.point);
        }

        if (_visualHelp && distanceFromCamera > -1)
        {
            Vector2 movementVector = (_initialCursorPos - _currentCursorPos) * -1;
            Vector3 movementVector3 = Vector3.zero;
            if (Vector3.Dot(-Vector3.up, Camera.main.transform.forward) > 0)
            {
                movementVector3 = new Vector3(movementVector.x, 0, movementVector.y);
            }
            else if (Vector3.Dot(Vector3.forward, Camera.main.transform.forward) > 0)
            {
                movementVector3 = new Vector3(movementVector.x, movementVector.y, 0);
            }
            Vector3 movementVector3OnPlane = Vector3.ProjectOnPlane(movementVector3, Camera.main.transform.forward);

            Vector3 redirectionVector = movementVector3OnPlane;
            _rolyPoly.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(redirectionVector);

            Gizmos.color = Color.green;

            if (Mathf.Abs(redirectionVector.x) > 250 || Mathf.Abs(redirectionVector.y) > 250 || Mathf.Abs(redirectionVector.z) > 250)
            {
                Gizmos.color = Color.yellow;
            }

            if (Mathf.Abs(redirectionVector.x) >= 500 || Mathf.Abs(redirectionVector.y) >= 500 || Mathf.Abs(redirectionVector.z) >= 500)
            {
                Gizmos.color = Color.red;
            }

            //Gizmos.DrawSphere(_rolyPoly.transform.position - movementVector3OnPlane, .2f);
            //Gizmos.DrawLine(_rolyPoly.transform.position, _rolyPoly.transform.position - movementVector3OnPlane);

            Gizmos.DrawSphere(_rolyPoly.transform.position - (redirectionVector.normalized *  (redirectionVector.magnitude *_redirectionForceVis)), .2f);
            Gizmos.DrawLine(_rolyPoly.transform.position, _rolyPoly.transform.position - (redirectionVector.normalized * (redirectionVector.magnitude *_redirectionForceVis)));
        }
    }
}
