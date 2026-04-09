using UnityEngine;
using System.Collections;

public class Lunge : MonoBehaviour
{
    [Header("Параметры рывка")]
    public int lungeImpuls = 15;
    public float lungeDuration = 0.3f;
    public float lungeCooldown = 1f;
    public float invincibilityTime = 0.5f;
    public float colliderScale = 0.6f;
    public LayerMask enemyLayer;

    [Header("Компоненты")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Animator _anim;

    // 🔧 Ссылка на CharacterMovement для проверки земли
    [SerializeField] private CharacterMovement _movement;

    // 🔧 Событие: вызывается при начале/конце рывка (опционально)
    public System.Action<bool> OnLungeStateChanged;

    // Внутренние переменные
    private bool _isLunging = false;
    private bool _lockLunge = false;
    private bool _isInvincible = false;
    private float _invincibilityTimer = 0f;
    private Vector3 _originalColliderSize;
    private Vector3 _input;

    private void Start()
    {
        // Авто-поиск компонентов если не назначены
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
        if (_playerCollider == null) _playerCollider = GetComponent<Collider2D>();
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        if (_anim == null) _anim = GetComponent<Animator>();
        if (_movement == null) _movement = GetComponent<CharacterMovement>();

        // Сохраняем исходный размер коллайдера
        if (_playerCollider != null)
            _originalColliderSize = _playerCollider.bounds.size;
    }

    private void Update()
    {
        // 🔧 Обновление неуязвимости
        UpdateInvincibility();

        // 🔧 Проверка ввода (можно вынести в отдельный метод)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_lockLunge && !_isLunging)
        {
            // Проверяем что игрок на земле (если есть ссылка на movement)
            bool canLunge = _movement == null || _movement.IsGrounded();

            if (canLunge)
            {
                _input = new Vector2(Input.GetAxis("Horizontal"), 0);
                StartLunge();
            }
        }
    }

    // 🔧 Публичный метод для запуска рывка из других скриптов
    public void StartLunge()
    {
        if (_lockLunge || _isLunging) return;

        StartCoroutine(LungeCoroutine());
    }

    IEnumerator LungeCoroutine()
    {
        _lockLunge = true;
        _isLunging = true;

        // 🔧 Событие: рывок начался
        OnLungeStateChanged?.Invoke(true);

        // Направление
        float direction = _input.x != 0 ? Mathf.Sign(_input.x) : (_sprite.flipX ? -1f : 1f);
        if (_sprite != null)
            _sprite.flipX = direction < 0;

        // 🔧 Отключаем физику и коллайдер
        if (_rigidbody != null)
            _rigidbody.simulated = false;
        if (_playerCollider != null)
        {
            _playerCollider.enabled = false;
            SetColliderScale(colliderScale);
        }

        // Анимация
        if (_anim != null)
            _anim.Play("Roll");

        // Неуязвимость
        _isInvincible = true;
        _invincibilityTimer = invincibilityTime;

        // Движение
        float elapsed = 0f;
        Vector2 targetVelocity = new Vector2(direction * lungeImpuls, 0);

        while (elapsed < lungeDuration)
        {
            float t = elapsed / lungeDuration;
            transform.position += (Vector3)(targetVelocity * t * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🔧 Возвращаем физику
        if (_rigidbody != null)
            _rigidbody.simulated = true;
        if (_playerCollider != null)
        {
            _playerCollider.enabled = true;
            SetColliderScale(1f);
        }

        _isLunging = false;

        // 🔧 Событие: рывок закончился
        OnLungeStateChanged?.Invoke(false);

        // Кулдаун
        yield return new WaitForSeconds(lungeCooldown);
        _lockLunge = false;
    }

    // 🔧 Обновление неуязвимости (вызывать из Update)
    void UpdateInvincibility()
    {
        if (_isInvincible)
        {
            _invincibilityTimer -= Time.deltaTime;

            // Мигание спрайта
            if (_sprite != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                _sprite.color = new Color(1f, 1f, 1f, alpha);
            }

            if (_invincibilityTimer <= 0f)
            {
                _isInvincible = false;
                if (_sprite != null)
                    _sprite.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    // 🔧 Изменение размера коллайдера
    void SetColliderScale(float scale)
    {
        if (_playerCollider == null) return;

        Vector2 newSize = new Vector2(
            _originalColliderSize.x * scale,
            _originalColliderSize.y * scale
        );

        if (_playerCollider is BoxCollider2D box)
        {
            box.size = newSize;
        }
        else if (_playerCollider is CircleCollider2D circle)
        {
            circle.radius = (newSize.x + newSize.y) * 0.5f * 0.5f;
        }
    }

    // 🔧 Публичные геттеры для других скриптов
    public bool IsLunging() => _isLunging;
    public bool IsInvincible() => _isInvincible;

    // 🔧 Проверка можно ли делать рывок
    public bool CanLunge() => !_lockLunge && !_isLunging && (_movement == null || _movement.IsGrounded());
}