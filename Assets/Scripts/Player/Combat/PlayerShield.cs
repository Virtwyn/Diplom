using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    private const int BlockMouseButton = 1;
    private const string BlockTriggerParameter = "Block";
    private const string IdleBlockBoolParameter = "IdleBlock";
    private const string BlockStartStateName = "Block";
    [SerializeField, Range(0f, 1f)] private float blockedDamageMultiplier = 0f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private CharacterMovement movement;

    private int _blockTriggerHash;
    private int _idleBlockBoolHash;
    private bool _blockStartPlayed;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (characterSprite == null)
        {
            characterSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (movement == null)
        {
            movement = GetComponent<CharacterMovement>();
        }

        _blockTriggerHash = Animator.StringToHash(BlockTriggerParameter);
        _idleBlockBoolHash = Animator.StringToHash(IdleBlockBoolParameter);
    }

    private void Update()
    {
        bool canBlockNow = movement != null && movement.IsGrounded() && !movement.IsLunging();

        if (!canBlockNow)
        {
            IsBlocking = false;
        }
        else if (Input.GetMouseButtonDown(BlockMouseButton))
        {
            IsBlocking = true;
            _blockStartPlayed = false;

            if (animator != null)
            {
                animator.SetBool(_idleBlockBoolHash, false);
                animator.ResetTrigger(_blockTriggerHash);
                animator.SetTrigger(_blockTriggerHash);
                _blockStartPlayed = true;
            }
        }
        else if (Input.GetMouseButtonUp(BlockMouseButton))
        {
            IsBlocking = false;
        }

        if (animator == null)
        {
            return;
        }

        SetMovementLocks(IsBlocking);

        if (IsBlocking)
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            // После стартовой анимации блока включаем IdleBlock bool для loop-анимации.
            bool onBlockState = currentState.IsName(BlockStartStateName);
            bool inTransition = animator.IsInTransition(0);
            if (_blockStartPlayed && onBlockState && !inTransition && currentState.normalizedTime >= 1f)
            {
                animator.SetBool(_idleBlockBoolHash, true);
                _blockStartPlayed = false;
            }
        }
        else
        {
            animator.SetBool(_idleBlockBoolHash, false);
            animator.ResetTrigger(_blockTriggerHash);
            _blockStartPlayed = false;
        }
    }

    private void OnDisable()
    {
        IsBlocking = false;
        _blockStartPlayed = false;
        SetMovementLocks(false);

        if (animator != null)
        {
            animator.SetBool(_idleBlockBoolHash, false);
            animator.ResetTrigger(_blockTriggerHash);
        }
    }

    public int GetModifiedDamage(int incomingDamage, Transform attacker)
    {
        if (!CanBlockAttack(attacker))
        {
            return incomingDamage;
        }

        int reducedDamage = Mathf.RoundToInt(incomingDamage * blockedDamageMultiplier);
        return Mathf.Max(0, reducedDamage);
    }

    private bool CanBlockAttack(Transform attacker)
    {
        if (!IsBlocking)
        {
            return false;
        }

        // Блок всегда работает только от удара спереди.
        if (attacker == null || characterSprite == null)
        {
            return true;
        }

        bool isFacingRight = !characterSprite.flipX;
        bool attackerIsOnRight = attacker.position.x > transform.position.x;

        return isFacingRight == attackerIsOnRight;
    }

    private void SetMovementLocks(bool isLocked)
    {
        if (movement == null)
        {
            return;
        }

        movement.SetExternalMovementLock(isLocked);
        movement.SetExternalLungeLock(isLocked);
        movement.SetExternalJumpLock(isLocked);
    }
}
