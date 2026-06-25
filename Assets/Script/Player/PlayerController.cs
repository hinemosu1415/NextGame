using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerWeaponManager))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerAllyManager))]
[RequireComponent(typeof(PlayerBuildingManager))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private float _firstSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int _maxMultiJumpCount; //多段ジャンプできる最大回数
    [SerializeField] private float _jumpCutMultiplier; //ジャンプボタンを離したときに上昇速度へ掛ける倍率(可変ジャンプ用)       

    private Rigidbody2D _rigidbody;
    private PlayerAnimator _playerAnimator;
    private PlayerWeaponManager _weaponManager;
    private PlayerBuildingManager _structureManager;
    private PlayerAllyManager _allyManager;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _attackAction2;
    private InputAction _modeChange;
    private InputAction[] _slotSelectActions;
    private InputAction _spawnAllyAction;
    private float _moveInputX;
    private float _currentSpeed;
    private int _currentJumpCount = 0;

    public event Action<Mode> OnModeChanged;

    private const int MAX_SLOT_COUNT = 4;

    public enum Mode
    {
        Attack,
        Building
    }
    public Mode CurrentMode { get; private set; } = Mode.Attack;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _weaponManager = GetComponent<PlayerWeaponManager>();
        _structureManager = GetComponent<PlayerBuildingManager>();
        _allyManager = GetComponent<PlayerAllyManager>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _attackAction2 = InputSystem.actions.FindAction("Attack2");
        _modeChange = InputSystem.actions.FindAction("ModeChange");
        _slotSelectActions = new InputAction[MAX_SLOT_COUNT];
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            _slotSelectActions[i] = InputSystem.actions.FindAction($"Slot{i + 1}");
        }
        _spawnAllyAction = InputSystem.actions.FindAction("SpawnAlly");
    }

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
        _jumpAction.performed += Jump;
        _jumpAction.canceled += JumpCanceled;
        _attackAction.performed += PrimaryAttack;
        _attackAction2.performed += SecondaryAttack;
        _modeChange.performed += ChangeMode;
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            _slotSelectActions[i].performed += SelectSlot;
        }
        _spawnAllyAction.performed += SpawnAlly;
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
        _jumpAction.performed -= Jump;
        _jumpAction.canceled -= JumpCanceled;
        _attackAction.performed -= PrimaryAttack;
        _attackAction2.performed -= SecondaryAttack;
        _modeChange.performed -= ChangeMode;
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            _slotSelectActions[i].performed -= SelectSlot;
        }
        _spawnAllyAction.performed -= SpawnAlly;
    }

    private void Update()
    {
        _moveInputX = _moveAction.ReadValue<Vector2>().x;

        //武器使用中でなければ移動処理を行う
        if (_weaponManager.CurrentWeaponState != WeaponBase.WeaponState.Attacking)
        {
            UpdateRotation();
            _weaponManager.UnequipCurrentWeapon();

            //移動処理            
            if (_moveInputX != 0)
            {
                if (Mathf.Sign(_moveInputX) != Mathf.Sign(_currentSpeed))
                {
                    _currentSpeed = 0;
                }

                // 加速
                if (Mathf.Abs(_currentSpeed) < _firstSpeed)
                    _currentSpeed = _moveInputX * _firstSpeed;

                _currentSpeed = Mathf.MoveTowards(_currentSpeed, _moveInputX * _maxSpeed, _acceleration * Time.deltaTime);
            }
            else
            {
                _currentSpeed = 0f;
            }
        }

        //着地処理
        const float MIN_VELOCITY_Y = 0.01f;
        if (Mathf.Abs(_rigidbody.linearVelocityY) < MIN_VELOCITY_Y)
        {
            _currentJumpCount = 0;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocityX = _currentSpeed;
    }

    private void UpdateRotation()
    {
        //移動する向きによってキャラクターを反転させる
        if (_moveInputX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_moveInputX < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (_moveInputX * _currentSpeed < 0)
        {
            _currentSpeed = 0f;
        }

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_currentJumpCount >= _maxMultiJumpCount) return;

        _rigidbody.linearVelocityY = 0;
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);


        if (_weaponManager.CurrentWeaponState != WeaponBase.WeaponState.Attacking)
            _playerAnimator.JumpAnimation();

        _currentJumpCount++;
    }

    private void JumpCanceled(InputAction.CallbackContext context)
    {
        //ジャンプボタンが離されたときに上方向の速度を半減させることで小ジャンプを再現
        if (_rigidbody.linearVelocityY > 0)
        {
            _rigidbody.linearVelocityY = _rigidbody.linearVelocityY * _jumpCutMultiplier;
        }
    }

    private void PrimaryAttack(InputAction.CallbackContext context)
    {
        if (CurrentMode == Mode.Attack)
        {
            if (_weaponManager.TryUsePrimaryWeapon())
                AttackAnimation();
        }
        else if (CurrentMode == Mode.Building)
        {
            //建築モードのときは攻撃ボタンで建築配置
            _structureManager.TryPlaceStructure();

        }
    }

    private void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (CurrentMode == Mode.Attack)
        {
            if (_weaponManager.TryUseSecondaryWeapon())
                AttackAnimation();
        }
    }

    private void ChangeMode(InputAction.CallbackContext context)
    {
        Debug.Log("Mode Change");
        if (CurrentMode == Mode.Attack)
        {
            CurrentMode = Mode.Building;
            _structureManager.EnterBuildingMode();
        }
        else if (CurrentMode == Mode.Building)
        {
            CurrentMode = Mode.Attack;
            _structureManager.ExitBuildingMode();
        }

        OnModeChanged?.Invoke(CurrentMode);
    }

    private void SelectSlot(InputAction.CallbackContext context)
    {
        int slotIndex = context.action.name switch
        {
            "Slot1" => 0,
            "Slot2" => 1,
            "Slot3" => 2,
            "Slot4" => 3,
            _ => throw new System.NotImplementedException()
        };
        _structureManager.SelectStructure(slotIndex);
    }

    private void SpawnAlly(InputAction.CallbackContext context)
    {
        _allyManager.TrySpawnAlly();
    }

    private void AttackAnimation()
    {
        _currentSpeed = 0f;
        _playerAnimator.AttackAnimation(_weaponManager.GetCurrentWeaponName);
    }

    public void SetControlLock(bool lockState)
    {
        if (lockState)
        {
            _currentSpeed = 0f;
            _moveInputX = 0f;
            _rigidbody.linearVelocityX = 0f;

            _moveAction.Disable();
            _jumpAction.Disable();
            _attackAction.Disable();
            _attackAction.Disable();
            _modeChange.Disable();
        }
        else
        {
            _moveAction.Enable();
            _jumpAction.Enable();
            _attackAction.Enable();
            _attackAction.Enable();
            _modeChange.Enable();
        }

        //建築モード中に死んだとき、UIが表示されたままになるバグの仮修正
        CurrentMode = Mode.Attack;
        OnModeChanged?.Invoke(CurrentMode);
    }
}

