using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Vector3 _groundCheckOffset;
    private Vector3 _input;
    private bool _isMoving;
    private bool _isGround;

    private Rigidbody2D _rigidbody;
    private CharacterAnimations _animations;
    [SerializeField] private SpriteRenderer _characterSprite;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animations = GetComponentInChildren<CharacterAnimations>();
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();

        // ? ������ ������ �� Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            _animations.Jump();
        }

        _animations.IsMoving = _isMoving;
        _animations.IsFlying = IsFlying();
    }

    private void Move()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), 0);

        // ? �������� ����� Rigidbody (�� ������ ������)
        _rigidbody.linearVelocity = new Vector2(_input.x * _speed, _rigidbody.linearVelocity.y);

        _isMoving = _input.x != 0;

        if (_input.x != 0)
        {
            _characterSprite.flipX = _input.x > 0 ? false : true;
        }
    }

    private void CheckGround()
    {
        float rayLength = 0.3f;
        Vector3 rayStartPosition = transform.position + _groundCheckOffset;

        // ? ��������� Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector3.down, rayLength);

        if (hit.collider != null)
        {
            _isGround = hit.collider.CompareTag("Ground");
        }
        else
        {
            _isGround = false;
        }

        // ?? ��� ������� (������� ��� � Scene view)
        Debug.DrawRay(rayStartPosition, Vector3.down * rayLength, Color.red);
    }

    private bool IsFlying()
    {
        // ? � ������� = �� �� �����
        return !_isGround;
    }

    private void Jump()
    {
        // ? ������ ������ �� �����
        if (_isGround)
        {
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
}