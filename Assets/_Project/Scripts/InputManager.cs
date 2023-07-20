using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControls _playerControls;
    PinballManager _pinballManager;
    RolyPolyManager _rolyPolyManager;

    [SerializeField]
    float _horizontal;
    [SerializeField]
    float _vertical;
    [SerializeField]
    float _moveAmount;
    [SerializeField]
    Vector2 _movementInput;
    Vector2 _cursorPos;
    Vector2 _cursorMovement;

    bool _a_Btn_Input;//A button by xbox standards
    bool _b_Btn_Input;//b button by xbox standards
    bool _x_Btn_Input;//x button by xbox standards
    bool _y_Btn_Input;//y button by xbox standards
    bool _rb_Btn_Input;//RB button by xbox standards
    bool _rt_Btn_Input;//RT button by xbox standards
    bool _lb_Btn_Input;//LB button by xbox standards
    bool _lt_Btn_Input;//LT button by xbox standards
    bool _Dpad_Up_Input;//Dpad UP button by xbox standards
    bool _Dpad_Down_Input;//Dpad Down button by xbox standards
    bool _Dpad_Left_Input;//Dpad Left button by xbox standards
    bool _Dpad_Right_Input;//Dpad Right button by xbox standards
    bool _start_btn_Input;//Start button by xbox standards
    bool _select_btn_Input;//Select button by xbox standards
    bool _RightJoystick_btn_Input;//Right Joystick button Input by xbox standards
    bool _LeftJoystick_btn_Input;//Right Joystick button Input by xbox standards

    public bool A_Btn_Input { get => _a_Btn_Input; set => _a_Btn_Input = value; }
    public bool Y_Btn_Input { get => _y_Btn_Input; set => _y_Btn_Input = value; }
    public bool RB_Btn_Input { get => _rb_Btn_Input; set => _rb_Btn_Input = value; }
    public bool RT_Btn_Input { get => _rt_Btn_Input; set => _rt_Btn_Input = value; }
    public bool LB_Btn_Input { get => _lb_Btn_Input; set => _lb_Btn_Input = value; }
    public bool LT_Btn_Input { get => _lt_Btn_Input; set => _lt_Btn_Input = value; }
    public bool Start_btn_Input { get => _start_btn_Input; set => _start_btn_Input = value; }
    public bool Select_btn_Input { get => _select_btn_Input; set => _select_btn_Input = value; }
    public bool B_Btn_Input { get => _b_Btn_Input; set => _b_Btn_Input = value; }
    public Vector2 CursorPos { get => _cursorPos; set => _cursorPos = value; }
    public Vector2 CursorMovement { get => _cursorMovement; set => _cursorMovement = value; }
    public float Horizontal { get => _horizontal; set => _horizontal = value; }
    public float Vertical { get => _vertical; set => _vertical = value; }
    public float MoveAmount { get => _moveAmount; set => _moveAmount = value; }
    public Vector2 MovementInput { get => _movementInput; set => _movementInput = value; }

    private void Start()
    {
        _pinballManager = GetComponent<PinballManager>();
        _rolyPolyManager = GetComponentInChildren<RolyPolyManager>();
    }

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.PinballControlls.LeftFlipper.performed += i => _lb_Btn_Input = true;
            _playerControls.PinballControlls.RightFlipper.performed += i => _rb_Btn_Input = true;
            _playerControls.PinballControlls.PullSpring.performed += i => _lt_Btn_Input = true;
            _playerControls.PinballControlls.PullSpring.performed += i => _rt_Btn_Input = true;
            _playerControls.PinballControlls.Redirect.performed += i => _b_Btn_Input = true;
            _playerControls.PinballControlls.ShakeTable.performed += i => _a_Btn_Input = true;
            _playerControls.PinballControlls.JumpSlam.performed += i => _y_Btn_Input = true;
            _playerControls.PinballControlls.Pause.performed += i => _start_btn_Input = true;
            _playerControls.PinballControlls.Reset.performed += i => _select_btn_Input = true;
            _playerControls.PinballControlls.CursorPos.performed += _playerControls => _cursorPos = _playerControls.ReadValue<Vector2>();
            _playerControls.PinballControlls.CursorMovement.performed += _playerControls => _cursorMovement = _playerControls.ReadValue<Vector2>();
            _playerControls.RolyPolyControls.Movement.performed += _playerControls => _movementInput = _playerControls.ReadValue<Vector2>();
        }
        _playerControls.PinballControlls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.PinballControlls.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMoveInput(delta);
        HandleJumpGroundSlam();
        HandleLeftFlipper();
        HandleRightFlipper();
        HandleTableShake();
        HandleRedirect();
        HandleReset();
        HandlePause();
    }

    public void FixedTickInput(float delta)
    {
        HandleSpringPull(delta);
    }

    private void HandleMoveInput(float delta)
    {
        _horizontal = _movementInput.x;
        _vertical = _movementInput.y;
        _moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));
    }

    public void HandleLeftFlipper()
    {
        if (_playerControls.PinballControlls.LeftFlipper.IsPressed())
        {
            _pinballManager.ActionateLeftFlipper(true);
        }
        else
        {
            _pinballManager.ActionateLeftFlipper(false);
        }
    }

    public void HandleRightFlipper()
    {
        if (_playerControls.PinballControlls.RightFlipper.IsPressed())
        {
            _pinballManager.ActionateRightFlipper(true);
        }
        else
        {
            _pinballManager.ActionateRightFlipper(false);
        }
    }

    public void HandleRedirect()
    {
        if (_b_Btn_Input == true)
        {
            _pinballManager.RedirectRolyPoly(true);
        }

        if (_playerControls.PinballControlls.Redirect.WasReleasedThisFrame())
        {
            _pinballManager.RedirectRolyPoly(false);
        }
    }

    public void HandleSpringPull(float delta)
    {
        if (_playerControls.PinballControlls.PullSpring.IsPressed())
        {
            _pinballManager.PullSpring(delta);
        }
    }

    public void HandleJumpGroundSlam()
    {
        if (!_y_Btn_Input)//Implement coyote time
            return;

        if (_rolyPolyManager.IsGrounded)
        {
            _rolyPolyManager.RolyPolyMovement.Jump();
        }
        else
        {
            if (Physics.Raycast(_rolyPolyManager.transform.position, _rolyPolyManager.RolyPolyMovement.GetRolyPolyDown(), out RaycastHit hitInfo, .5f, _rolyPolyManager.GroundLayer))
            {
                _rolyPolyManager.RolyPolyMovement.Jump();
            }
            else
            {
                _rolyPolyManager.RolyPolyMovement.GroundSlam();
            }
        }
    }

    public void HandleDoor()
    {
        if (_a_Btn_Input)
        {
            //if entering door
            //change control scheme to RolyPolyMovement
            //if exiting door
            //change control scheme to default controls
        }
    }

    public void HandleTableShake()
    {
        if (_y_Btn_Input)
        {
            //shake table
        }
    }

    public void HandleReset()
    {
        if (_select_btn_Input)
        {
            _pinballManager.ResetGame();
        }
    }

    public void HandlePause()
    {
        if (_start_btn_Input)
        {
            _pinballManager.PauseGame();
        }
    }

    [ContextMenu("ChangeToRolyPolyMovement")]
    public void ChangeToRolyPolyMovementControls()
    {
        OnDisable();
        EnableRolyPolyMovementControls();
        _rolyPolyManager.ChangeRolyPolyState(RolyPolyStates.Walking);
    }

    [ContextMenu("ChangePinballMovement")]
    public void ChangeToPinballControls()
    {
        DisableRolyPolyMovementControls();
        OnEnable();
        _rolyPolyManager.ChangeRolyPolyState(RolyPolyStates.Boling);
    }

    public void EnableRolyPolyMovementControls()
    {
        _playerControls.RolyPolyControls.Enable();
    }

    public void DisableRolyPolyMovementControls()
    {
        _playerControls.RolyPolyControls.Disable();
    }
}
