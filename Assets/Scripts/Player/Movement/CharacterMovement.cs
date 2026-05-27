using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterMovement : MonoBehaviour
{
    [Header("Звуки")]
    public float walkSoundInterval = 0.4f; // интервал между звуками шагов в секундах
    private float _walkSoundTimer = 0f; // таймер который накапливается и сравнивается с интервалом

    [Header("Передвижение")]
    [SerializeField] private float _speed; // скорость передвижения игрока
    [SerializeField] private float _jumpForce; // сила прыжка (импульс вверх)
    [SerializeField] private float _fallGravityMultiplier = 2.5f; // множитель гравитации при падении вниз
    [SerializeField] private float _lowJumpGravityMultiplier = 2f; // множитель гравитации если отпустил пробел во время прыжка
    [SerializeField] private float _coyoteTime = 0.12f; // максимальное время в секундах после схода с платформы когда ещё можно прыгнуть
    [SerializeField] private Vector3 _groundCheckOffset; // смещение точки откуда идёт луч проверки земли относительно центра игрока
    [SerializeField] private LayerMask groundMask; // маска слоёв которые считаются землёй

    [Header("Рывок")]
    public int LungeImpuls = 15; // скорость рывка по горизонтали
    public float LungeDuration = 0.3f; // сколько секунд длится рывок
    public float LungeCooldown = 1f; // сколько секунд ждать до следующего рывка
    public float LungeInvincibilityTime = 0.5f; // сколько секунд неуязвимости даёт рывок
    public LayerMask enemyLayer; // маска слоёв врагов — нужна чтобы отключить коллизию с ними во время рывка

    [Header("Компоненты")]
    private Vector3 _input; // хранит текущий ввод игрока по горизонтали (от -1 до 1)
    private bool _isMoving; // true если игрок сейчас движется
    private bool _isGrounded; // true если игрок стоит на земле
    private float _coyoteTimeCounter; // текущее значение таймера coyote time — убывает когда игрок в воздухе
    private float _landingBuffer = 0f; // буфер времени после приземления чтобы анимация полёта не мигала
    private const float LANDING_BUFFER_TIME = 0.1f; // константа — сколько длится буфер приземления

    public Rigidbody2D _rigidbody; // физический компонент игрока
    private CharacterAnimations _animations; // скрипт анимаций — управляет IsMoving и IsFlying
    [SerializeField] private SpriteRenderer _characterSprite; // спрайт игрока — нужен для flipX (поворот)
    public Animator anim; // аниматор — для прямого вызова anim.Play()
    [SerializeField] private HealthSystem playerHealth; // система здоровья — нужна для передачи неуязвимости при рывке
    private bool lockLunge = false; // true во время кулдауна рывка — блокирует новый рывок
    private bool isLunging = false; // true пока идёт рывок
    private bool _externalMovementLock = false; // блокировка движения извне (например во время атаки или блока)
    private bool _externalLungeLock = false; // блокировка рывка извне
    private bool _externalJumpLock = false; // блокировка прыжка извне
    private bool isInvincible = false; // true если игрок неуязвим (во время рывка)
    private float invincibilityTimer = 0f; // таймер неуязвимости — убывает каждый кадр
    private Collider2D _playerCollider; // коллайдер игрока
    private Vector3 _originalColliderSize; // оригинальный размер коллайдера (зарезервировано)
    private int _playerLayer; // номер слоя игрока — нужен для IgnoreLayerCollision
    private PlatformEffector2D _currentPlatform; // платформа на которой стоит игрок прямо сейчас
    private PlatformEffector2D _droppingFromPlatform; // платформа с которой игрок прыгает вниз — отдельная ссылка чтобы не потерять после OnCollisionExit2D

    [SerializeField] private AudioClip MoveSoundClip;
    [SerializeField] private AudioClip JumpSoundClip;
    [SerializeField] private AudioClip LandingSoundClip;

    public bool IsAttacking { get; set; } = false; // свойство — устанавливается из PlayerCombat чтобы заблокировать движение во время атаки

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animations = GetComponentInChildren<CharacterAnimations>(); // ищем в дочерних объектах так как аниматор на дочернем объекте
        _characterSprite = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();
        _playerLayer = gameObject.layer; // запоминаем слой игрока для IgnoreLayerCollision
        playerHealth = GetComponent<HealthSystem>();
    }

    private void FixedUpdate()
    {
        // FixedUpdate используется для физики — вызывается с фиксированным интервалом независимо от FPS
        bool wasGrounded = _isGrounded; // запоминаем предыдущее состояние земли — нужно для CheckLanding
        CheckGround();
        UpdateCoyoteTime();
        CheckLanding(wasGrounded); // передаём предыдущее состояние чтобы определить момент приземления
        ApplyVariableJumpGravity();
        Move();
    }

    private void Update()
    {
        // Спрыгивание с платформы — S + Пробел и только если стоим на платформе с PlatformEffector2D
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.S) && _currentPlatform != null)
        {
            StartCoroutine(DropThroughPlatform());
        }

        // Рывок — Shift, только если нет кулдауна, нет внешней блокировки, не в рывке и на земле
        if (Input.GetKeyDown(KeyCode.LeftShift) && !lockLunge && !_externalLungeLock && !isLunging && _isGrounded)
        {
            StartCoroutine(LungeCoroutine());
        }
        // Прыжок — Пробел, только если не зажата S (чтобы не конфликтовать со спрыгиванием)
        else if (Input.GetKeyDown(KeyCode.Space) && CanJumpNow() && !Input.GetKey(KeyCode.S))
        {
            Jump();
            _animations.Jump();
        }

        UpdateFlyingState();
        UpdateInvincibility();

        _animations.IsMoving = _isMoving; // передаём состояние движения в скрипт анимаций каждый кадр
    }

    void UpdateInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime; // убываем таймер каждый кадр

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                _characterSprite.color = new Color(1f, 1f, 1f, 1f); // восстанавливаем полную непрозрачность спрайта
            }
        }
    }

    private void UpdateFlyingState()
    {
        if (_isGrounded)
        {
            _landingBuffer = LANDING_BUFFER_TIME; // при приземлении запускаем буфер — даём 0.1 секунды до смены анимации
            _animations.IsFlying = false;
        }
        else
        {
            _landingBuffer -= Time.deltaTime; // буфер убывает пока в воздухе

            if (_rigidbody.linearVelocity.y < 0 && _landingBuffer <= 0f)
            {
                // Включаем анимацию полёта только если падаем вниз И буфер истёк — предотвращает мигание при прыжке
                _animations.IsFlying = true;
            }
            else if (_rigidbody.linearVelocity.y >= 0)
            {
                // Если летим вверх — анимация полёта выключена (играет анимация прыжка)
                _animations.IsFlying = false;
            }
        }
    }

    public void Move()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), 0); // получаем горизонтальный ввод (от -1 до 1)
        _isMoving = _input.x != 0; // true если есть горизонтальный ввод

        if (_externalMovementLock && !isLunging)
        {
            // Если движение заблокировано извне (атака/блок) и не в рывке — останавливаем игрока
            _isMoving = false;
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y); // обнуляем только горизонтальную скорость, вертикальную не трогаем
            _walkSoundTimer = walkSoundInterval; // сбрасываем таймер звука шагов
            return;
        }

        if (_input.x != 0 && !isLunging)
        {
            // Поворачиваем спрайт в сторону движения — если вправо то flipX = false, если влево то true
            _characterSprite.flipX = _input.x > 0 ? false : true;
        }

        if (!isLunging && !IsAttacking)
        {
            // Обычное движение — устанавливаем горизонтальную скорость, вертикальную не трогаем (гравитация)
            _rigidbody.linearVelocity = new Vector2(_input.x * _speed, _rigidbody.linearVelocity.y);
        }
        else if (IsAttacking && !isLunging)
        {
            // Во время атаки останавливаем горизонтальное движение
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
        }

        if (_isMoving && !isLunging && _isGrounded && !IsAttacking)
        {
            _walkSoundTimer += Time.deltaTime; // накапливаем таймер пока идём

            if (_walkSoundTimer >= walkSoundInterval && !isLunging)
            {
                SoundFXManager.instance.PlaySoundFXClip(MoveSoundClip, transform, 1f);
                _walkSoundTimer = 0f; // сбрасываем таймер после воспроизведения звука
            }
        }
        else
        {
            _walkSoundTimer = 0.4f; // если не идём — ставим таймер близко к интервалу чтобы первый шаг после остановки не задерживался
        }
    }

    public void CheckGround()
    {
        float rayLength = 0.1f; // длина луча — короткий чтобы не задевать лишнее
        Vector3 rayStartPosition = transform.position + _groundCheckOffset; // точка старта луча со смещением
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector3.down, rayLength, groundMask); // луч вниз по маске земли

        _isGrounded = hit.collider != null && hit.collider.CompareTag("Ground"); // земля только если попали в коллайдер с тегом Ground

        Color rayColor = _isGrounded ? Color.green : Color.red; // зелёный если на земле красный если нет — для отладки
        Debug.DrawRay(rayStartPosition, Vector3.down * rayLength, rayColor);
    }

    private void Jump()
    {
        if (CanJumpNow())
        {
            SoundFXManager.instance.PlaySoundFXClip(JumpSoundClip, transform, 1f);
            _coyoteTimeCounter = 0f; // обнуляем coyote time чтобы нельзя было прыгнуть дважды
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse); // добавляем импульс вверх
        }
    }

    private bool CanJumpNow()
    {
        // Можно прыгнуть если: coyote time больше нуля И не атакуем И не в рывке И нет внешней блокировки
        return _coyoteTimeCounter > 0f && !IsAttacking && !isLunging && !_externalJumpLock;
    }

    private void UpdateCoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime; // на земле — держим таймер полным
        }
        else
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime; // в воздухе — убываем таймер
        }
    }

    private void ApplyVariableJumpGravity()
    {
        if (_isGrounded || isLunging)
        {
            return; // на земле или в рывке — не применяем дополнительную гравитацию
        }

        float verticalVelocity = _rigidbody.linearVelocity.y;
        Vector2 extraGravity = Vector2.zero;

        if (verticalVelocity < 0f)
        {
            // Падаем вниз — добавляем дополнительную гравитацию для быстрого падения
            extraGravity = Vector2.up * Physics2D.gravity.y * (_fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (verticalVelocity > 0f && !Input.GetKey(KeyCode.Space))
        {
            // Летим вверх но пробел не зажат — добавляем гравитацию чтобы срезать прыжок
            extraGravity = Vector2.up * Physics2D.gravity.y * (_lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        _rigidbody.linearVelocity += extraGravity; // прибавляем дополнительную гравитацию к текущей скорости
    }

    IEnumerator LungeCoroutine()
    {
        lockLunge = true; // блокируем новый рывок на время кулдауна
        isLunging = true; // помечаем что рывок активен

        // Определяем направление рывка:
        // если есть горизонтальный ввод — берём его знак (+1 или -1)
        // если ввода нет — смотрим куда повёрнут спрайт (flipX = true значит влево = -1)
        float direction = _input.x != 0 ? Mathf.Sign(_input.x) : (_characterSprite.flipX ? -1f : 1f);

        _characterSprite.flipX = direction < 0; // поворачиваем спрайт в сторону рывка (влево = true, вправо = false)

        anim.Play("Roll");

        SetEnemyCollisionIgnored(true); // отключаем коллизию с врагами — игрок проходит сквозь них

        // Включаем неуязвимость в обоих местах:
        isInvincible = true; // в CharacterMovement — для визуального эффекта
        invincibilityTimer = LungeInvincibilityTime;
        playerHealth.SetInvincible(LungeInvincibilityTime, true); // в HealthSystem — чтобы не получать урон, true = без мигания

        float elapsedTime = 0f; // счётчик времени рывка

        while (elapsedTime < LungeDuration) // крутимся пока не истечёт время рывка
        {
            // Каждый кадр устанавливаем горизонтальную скорость рывка, вертикальную не трогаем (гравитация работает)
            _rigidbody.linearVelocity = new Vector2(direction * LungeImpuls, _rigidbody.linearVelocity.y);
            elapsedTime += Time.deltaTime; // увеличиваем счётчик на время прошедшее с прошлого кадра
            yield return null; // ждём следующий кадр
        }

        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y); // останавливаем горизонтальное движение после рывка
        SetEnemyCollisionIgnored(false); // возвращаем коллизию с врагами

        isLunging = false; // рывок завершён
        yield return new WaitForSeconds(LungeCooldown); // ждём кулдаун
        lockLunge = false; // разблокируем рывок
    }

    private void SetEnemyCollisionIgnored(bool ignoreCollision)
    {
        int enemyMaskValue = enemyLayer.value; // получаем числовое значение маски слоёв врагов

        for (int layer = 0; layer < 32; layer++) // перебираем все 32 возможных слоя Unity
        {
            // Битовая операция — проверяем входит ли текущий слой в маску врагов
            bool layerInMask = (enemyMaskValue & (1 << layer)) != 0;
            if (layerInMask)
            {
                // Если слой входит в маску — включаем или отключаем коллизию между слоем игрока и этим слоем
                Physics2D.IgnoreLayerCollision(_playerLayer, layer, ignoreCollision);
            }
        }
    }

    public bool IsInvincible()
    {
        return isInvincible; // публичный геттер — другие скрипты могут проверить неуязвим ли игрок
    }

    void CheckLanding(bool wasGrounded)
    {
        if (_isGrounded && !wasGrounded)
        {
            // Только что приземлились — в прошлом кадре был в воздухе а сейчас на земле
            SoundFXManager.instance.PlaySoundFXClip(LandingSoundClip, transform, 0.5f);
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

    // Три метода для внешней блокировки — вызываются из PlayerCombat и PlayerShield
    public void SetExternalMovementLock(bool isLocked)
    {
        _externalMovementLock = isLocked;
    }

    public void SetExternalLungeLock(bool isLocked)
    {
        _externalLungeLock = isLocked;
    }

    public void SetExternalJumpLock(bool isLocked)
    {
        _externalJumpLock = isLocked;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlatformEffector2D effector = collision.gameObject.GetComponent<PlatformEffector2D>();
        if (effector != null)
            _currentPlatform = effector; // запоминаем платформу на которую встали
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlatformEffector2D effector = collision.gameObject.GetComponent<PlatformEffector2D>();
        if (effector != null && _currentPlatform == effector)
            _currentPlatform = null; // забываем платформу когда ушли с неё
    }

    private IEnumerator DropThroughPlatform()
    {
        _droppingFromPlatform = _currentPlatform; // сохраняем отдельно — _currentPlatform обнулится через OnCollisionExit2D пока ждём

        _droppingFromPlatform.rotationalOffset = 180f; // разворачиваем PlatformEffector2D на 180 градусов — теперь платформа пропускает сверху вниз

        yield return new WaitForSeconds(0.5f); // ждём пока игрок провалится

        if (_droppingFromPlatform != null)
            _droppingFromPlatform.rotationalOffset = 0f; // возвращаем платформу в нормальное состояние

        _droppingFromPlatform = null; // очищаем ссылку
    }
}