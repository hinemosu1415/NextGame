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
    private InputAction _equipPrimaryAction;
    private InputAction _equipSecondaryAction;
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
        _equipPrimaryAction = InputSystem.actions.FindAction("EquipPrimary");
        _equipSecondaryAction = InputSystem.actions.FindAction("EquipSecondary");
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
        if (_equipPrimaryAction != null) _equipPrimaryAction.performed += EquipPrimary;
        if (_equipSecondaryAction != null) _equipSecondaryAction.performed += EquipSecondary;
        // Mode change (f) disabled per design
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
        if (_equipPrimaryAction != null) _equipPrimaryAction.performed -= EquipPrimary;
        if (_equipSecondaryAction != null) _equipSecondaryAction.performed -= EquipSecondary;
        // Mode change (f) disabled per design
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
            if (_weaponManager.TryUseSelectedWeapon())
                AttackAnimation();
        }
        else if (CurrentMode == Mode.Building)
        {
            // 建築モードでは攻撃ボタンで配置を実行（モードは維持）
            _structureManager.TryPlaceStructure();
        }
    }

    private void SecondaryAttack(InputAction.CallbackContext context)
    {
        if (CurrentMode == Mode.Attack)
        {
            if (_weaponManager.TryUseSelectedWeapon())
                AttackAnimation();
        }
        else if (CurrentMode == Mode.Building)
        {
            // 建築モードでのセカンダリ攻撃は特に動作させない（設置はプライマリで行う）
        }
    }

    private void EquipPrimary(InputAction.CallbackContext context)
    {
        // E: 武器の選択だけ行い、表示は攻撃時に限定する
        if (CurrentMode == Mode.Building)
        {
            _structureManager.ExitBuildingMode();
            _weaponManager.ExitBuildingMode();
        }

        CurrentMode = Mode.Attack;
        _weaponManager.SelectWeapon(0);
        OnModeChanged?.Invoke(CurrentMode);
    }

    private void EquipSecondary(InputAction.CallbackContext context)
    {
        // R: 武器の選択だけ行い、表示は攻撃時に限定する
        if (CurrentMode == Mode.Building)
        {
            _structureManager.ExitBuildingMode();
            _weaponManager.ExitBuildingMode();
        }

        CurrentMode = Mode.Attack;
        _weaponManager.SelectWeapon(1);
        OnModeChanged?.Invoke(CurrentMode);
    }

    // ModeChange (f) disabled per request; method removed.

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

        // 常に建築モードに入る（既に建築モードなら選択のみ）
        if (CurrentMode != Mode.Building)
        {
            CurrentMode = Mode.Building;
            _structureManager.EnterBuildingMode();
            _weaponManager.EnterBuildingMode();
            OnModeChanged?.Invoke(CurrentMode);
        }
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
        }
        else
        {
            _moveAction.Enable();
            _jumpAction.Enable();
            _attackAction.Enable();
            _attackAction.Enable();

        }

        //建築モード中に死んだとき、UIが表示されたままになるバグの仮修正
        CurrentMode = Mode.Attack;
        OnModeChanged?.Invoke(CurrentMode);
    }
}

