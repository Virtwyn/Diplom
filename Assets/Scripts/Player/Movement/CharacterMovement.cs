using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private LayerMask groundMask;

    private Vector3 _input;
    private bool _isMoving;
    private bool _isGrounded;

    private Rigidbody2D _rigidbody;
    private CharacterAnimations _animations;
    [SerializeField] private SpriteRenderer _characterSprite;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animations = GetComponentInChildren<CharacterAnimations>();
        _characterSprite = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        _animations.IsMoving = _isMoving;
        _animations.IsFlying = IsFlying();
    }

    private void Move()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), 0);
        _isMoving = _input.x != 0;
        _rigidbody.linearVelocity = new Vector2(_input.x * _speed, _rigidbody.linearVelocity.y);

        if (_isMoving)
        {
            _characterSprite.flipX = _input.x > 0 ? false : true;
        }
    }

    private bool IsFlying()
    {
        return !_isGrounded;
    }

    private void CheckGround()
    {
        float rayLength = 0.1f;
        Vector3 rayStartPosition = transform.position + _groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector3.down, rayLength, groundMask);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
        Color rayColor = _isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStartPosition, Vector3.down * rayLength, rayColor);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
            _animations.Jump();
        }
    }
}