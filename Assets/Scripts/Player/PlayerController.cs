using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private CharacterController _characterController;
    private Animator _animator;

    [Header("Movement Settings")]
    [SerializeField]
    private float walkingSpeed = 3f;
    [SerializeField]
    private float walkingBackwardSpeed = 2f;
    [SerializeField]
    private float runningSpeed = 6f;
    [SerializeField]
    private float crouchingSpeed = 1.5f;
    [SerializeField]
    private float jumpForce = 1f;
    [SerializeField]
    private float gravity = 20f;
    [SerializeField]
    private float attackAnimLength = 1.3f;


    [Header("Unity links")]
    [SerializeField]
    GroundCheck gc;
    [SerializeField]
    private Transform maincamer;
    [SerializeField]
    private LayerMask layerMask;

    private Vector3 _moveDirection = Vector3.zero;
    [SerializeField]
    private float raycastDistance = 0.1f;
    private RigBuilder _rigBuilder;
    public bool isAttacking = false;
    private bool canAttack = true;

    private List<RigLayer> _layers = new List<RigLayer>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _rigBuilder = GetComponent<RigBuilder>();
        _layers = _rigBuilder.layers;
        InvokeRepeating("Log", 0f, 1f); 

        HandleAnimLayers(0, true);
        HandleAnimLayers(1, false);
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimations();
        HandleAttacks();
        _animator.SetBool("isGrounded", IsGrounded());
    }
    private IEnumerator HandleJumps()
    {
        float jumpStart = Time.time;
        while (Time.time - jumpStart < 0.5f)
        {
            Vector3 _vector3 = new Vector3(_moveDirection.x * 0.4f, 1f, _moveDirection.z * 0.4f);
            _characterController.Move(_vector3 * jumpForce * Time.deltaTime);
            yield return new WaitForSeconds(.02f);
        }
        _animator.SetBool("isGrounded", true);
    }

    private void HandleMovement()
    {
        if (IsGrounded())
        {
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 _localMoveDirection = _moveDirection;
            _moveDirection = transform.TransformDirection(_moveDirection);
            bool isCrouching = false;

            // Определение скорости
            if (Input.GetKey(KeyCode.LeftShift) && _localMoveDirection == Vector3.forward)
            {
                _moveDirection *= runningSpeed;
            }
            else if (_localMoveDirection == Vector3.back)
            {
                _moveDirection *= walkingBackwardSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                _moveDirection *= crouchingSpeed;
                isCrouching = true;
            }
            else if (_localMoveDirection == new Vector3(1, 0, -1) || _localMoveDirection == new Vector3(-1, 0, -1))
            {
                _moveDirection *= walkingBackwardSpeed * 0.7f;
            }
            else if (_localMoveDirection == new Vector3(1, 0, 1) || _localMoveDirection == new Vector3(-1, 0, 1))
            {
                _moveDirection *= walkingSpeed * 0.7f;
            }
            else
            {
                _moveDirection *= walkingSpeed;
            }
            // Прыжок
            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("Jumped");
                // _characterController.SimpleMove(Vector3.up*jumpForce);
                _animator.SetTrigger("jump"); // Переход в состояние Jumping Up
                StartCoroutine(HandleJumps());
            }
        }

        // Применение гравитации
        _moveDirection.y -= gravity * Time.deltaTime;

        // Движение персонажа
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, layerMask);
    }
    private void Log()
    {
        //Debug.Log(IsGrounded());
    }
    private void HandleAnimations()
    {
        // Получаем значения осей
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Передаем параметры в Blend Tree
        _animator.SetFloat("horSpeed", horizontalInput);
        _animator.SetFloat("vertSpeed", verticalInput);

        _animator.SetBool("forward", (horizontalInput > 0.1 || horizontalInput < -0.1 || verticalInput > 0.1) && !(verticalInput < -0.1));

        // Условие для перехода в Run
        // Этот код предполагает, что вы хотите бегать только вперед.
        // Если вам нужно бегать в других направлениях, Blend Tree тоже поможет.
        _animator.SetBool("run", Input.GetKey(KeyCode.LeftShift) && verticalInput > 0 && horizontalInput == 0);

        // Условие для перехода из Jumping Up обратно в Idle, Walking или Run
        _animator.SetBool("isGrounded", IsGrounded());

        _animator.SetBool("crouching", Input.GetKey(KeyCode.LeftControl));
        if (horizontalInput < -0.1 && Mathf.Abs(verticalInput) < 0.1)
        {
            _animator.SetBool("walkLeft", true);
            _animator.SetBool("forward", false);
        }
        else if (horizontalInput > 0.1 && Mathf.Abs(verticalInput) < 0.1)
        {
            _animator.SetBool("walkRight", true);
            _animator.SetBool("forward", false);
        }
        if (horizontalInput == 0)
        {
            _animator.SetBool("walkRight", false);
            _animator.SetBool("walkLeft", false);
        }
    }

    private void HandleAttacks()
    {
        if (canAttack)
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking) // Левая кнопка мыши
            {
                _animator.SetTrigger("attack"); // Переход в состояние Standing Melee Kick
                HandleAnimLayers(0, false);
                HandleAnimLayers(1, true);
                StartCoroutine(AttackDelay());
                isAttacking = true;
            }
        }
    }
    public void EnableDisableAttacking(bool enabled)
    {
        canAttack = enabled;
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackAnimLength);
        HandleAnimLayers(0, true);
        HandleAnimLayers(1, false);
        isAttacking = false;
    }
    private void HandleAnimLayers(int layerIndex, bool Active)
    {
        _rigBuilder.layers[layerIndex].active = Active;
    }

}