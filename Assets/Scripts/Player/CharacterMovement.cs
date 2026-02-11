using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private Vector3 _groundCheckOffset;

    private Vector3 _input;
    private bool _isMoving;
    private bool _isGrounded;
    private bool _isFlying;

    private Rigidbody2D _rigidbody;
    private CharacterAnimation _animations;
    [SerializeField] private SpriteRenderer _characterSprite;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animations = GetComponent<CharacterAnimation>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag == "Enemy")
       //{
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}
    }

    void Update()
    {
        Move();
        CheckGround();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded)
            {
                Jump();
                _animations.Jump();
            }
        }
        _animations.IsMoving = _isMoving;
        _animations.IsFlying = IsFlying();
    }

    private bool IsFlying()
    {
        if (_rigidbody.linearVelocity.y < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckGround()
    {
        float rayLength = 0.5f;
        Vector3 rayStartPosition = transform.position + _groundCheckOffset;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector2.down, rayLength); 

        if (hit.collider != null)
        {
            _isGrounded = hit.collider.CompareTag("Ground");
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void Move()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), 0);
        transform.position += _input * _speed * Time.deltaTime;
        _isMoving=_input.x !=0 ? true : false;

        if(_isMoving)
        {
            _characterSprite.flipX = _input.x > 0 ? false : true;
        }

        _animations.IsMoving = _isMoving;
    }
    private void Jump()
    {
        _rigidbody.AddForce(transform.up * _jumpForce,ForceMode2D.Impulse);
    }
}
