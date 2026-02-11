using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _characterSprite;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float startMovementTime;
    private float movementTime;

    void Start()
    {
        movementTime = startMovementTime / 2;
    }
    void Update()
    {
        if (movementTime > 0f)
        { 
            movementTime -= Time.deltaTime;
        }
        else
        {
            movementSpeed = -movementSpeed;
            movementTime = startMovementTime;
            _characterSprite.flipX = !_characterSprite.flipX;
        }
        Vector2 movementVec = new Vector2(1f, 0f);
        transform.Translate(movementVec * movementSpeed * Time.deltaTime);
    }
}