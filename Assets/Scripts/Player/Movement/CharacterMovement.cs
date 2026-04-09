using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterMovement : Sounds
{
    [Header("Звуки")]
    public float walkSoundInterval = 0.4f;
    private float _walkSoundTimer = 0f;
    private bool _wasGrounded = false;
    [Header("Передвижение")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask groundMask;

    [Header("Рывок")]
    public int LungeImpuls = 15;
    public float LungeDuration = 0.3f;
    public float LungeCooldown = 1f;
    public float LungeInvincibilityTime = 0.5f;
    public LayerMask enemyLayer;

    [Header("Компоненты")]
    private Vector3 _input;
    private bool _isMoving;
    private bool _isGrounded;

    public Rigidbody2D _rigidbody;
    private CharacterAnimations _animations;
    [SerializeField] private SpriteRenderer _characterSprite;
    public Animator anim;
    [SerializeField] private HealthSystem playerHealth;
    private bool lockLunge = false;
    private bool isLunging = false;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private Collider2D _playerCollider;
    private Vector3 _originalColliderSize;
    private int _playerLayer;

    public bool IsAttacking { get; set; } = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animations = GetComponentInChildren<CharacterAnimations>();
        _characterSprite = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();
        _playerLayer = gameObject.layer;
        playerHealth = GetComponent<HealthSystem>();
    }

    private void FixedUpdate()
    {
        _wasGrounded = _isGrounded;

        CheckGround();
        Move();
    }

    private void Update()
    {
        CheckLanding();
        if (Input.GetKeyDown(KeyCode.LeftShift) && !lockLunge && !isLunging && _isGrounded)
        {
            StartCoroutine(LungeCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
            _animations.Jump();
        }

        UpdateFlyingState();
        UpdateInvincibility();

        _animations.IsMoving = _isMoving;
    }
    void UpdateInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                _characterSprite.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
    // Проверка в воздухе ли игрок для анимации
    private void UpdateFlyingState()
    {
        if (_isGrounded)
        {
            _animations.IsFlying = false;
        }
        else if (_rigidbody.linearVelocity.y < 0)
        {
            _animations.IsFlying = true;
        }
    }
    //Передвижение игрока
    public void Move()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), 0);
        _isMoving = _input.x != 0;
        if (_input.x != 0 && !isLunging)
        {
            _characterSprite.flipX = _input.x > 0 ? false : true;
        }
        if (!isLunging && !IsAttacking)
        {
            _rigidbody.linearVelocity = new Vector2(_input.x * _speed, _rigidbody.linearVelocity.y);
        }
        else if (IsAttacking && !isLunging)
        {
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
        }
        if (_isMoving && !isLunging && _isGrounded && !IsAttacking)
        {
            _walkSoundTimer += Time.deltaTime;

            if (_walkSoundTimer >= walkSoundInterval && !isLunging)
            {
                PlaySound(sounds[0]);
                _walkSoundTimer = 0f;
            }
        }
        else
        {
            _walkSoundTimer = 0.4f;
        }
    }
    // Проверка стоит ли игрок на земле
    public void CheckGround()
    {
        float rayLength = 0.1f;
        Vector3 rayStartPosition = transform.position + _groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector3.down, rayLength, groundMask);

        _isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");

        Color rayColor = _isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStartPosition, Vector3.down * rayLength, rayColor);    
    }
    //Прыжок игрока
    private void Jump()
    {
        if (_isGrounded)
        {
            PlaySound(sounds[1]);
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
    //Рывок
    IEnumerator LungeCoroutine()
    {
        lockLunge = true;
        isLunging = true;
        float direction = _input.x != 0 ? Mathf.Sign(_input.x) : (_characterSprite.flipX ? -1f : 1f);
        _rigidbody.simulated = false;
        if (_playerCollider != null)
            _playerCollider.enabled = false;
         _characterSprite.flipX = direction < 0;
       anim.Play("Roll");

        isInvincible = true;
        invincibilityTimer = LungeInvincibilityTime;

        float elapsedTime = 0f;
        Vector2 targetVelocity = new Vector2(direction * LungeImpuls, 0);

        while (elapsedTime < LungeDuration)
        {
            float t = elapsedTime / LungeDuration;
            transform.position += (Vector3)(targetVelocity * t * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _rigidbody.simulated = true;
        if (_playerCollider != null)
            _playerCollider.enabled = true;

        isLunging = false;
        yield return new WaitForSeconds(LungeCooldown);
        lockLunge = false;
    }
    public bool IsInvincible()
    {
        return isInvincible;
    }
    // Проверка приземление для звука
    void CheckLanding()
    {
        if (_isGrounded && !_wasGrounded)
        {
            PlaySound(sounds[2], 0.1f);
        }
    }
    public bool IsGrounded()
    {
        return _isGrounded;
    }
    public bool IsLunging()
    {
        return isLunging;
    }
}