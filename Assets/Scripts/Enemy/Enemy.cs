using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Передвижение")]
    public float speed=4;
    public int positionOfPatrol;
    public float stoppingDistance;

    [Header("Обнаружение игрока")]
    public float maxVerticalDistance;

    public Transform point;
    bool moveingRight;
    [SerializeField] private SpriteRenderer _characterSprite;

    Transform player;

    bool chill = false;
    bool angry = false;
    bool goBack = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float verticalDiff = Mathf.Abs(transform.position.y - player.position.y);
        bool playerOnSameLevel = verticalDiff <= maxVerticalDistance;

        if (Vector2.Distance(transform.position, point.position) < positionOfPatrol && !angry)
        {
            chill = true;
        }

        if (Vector2.Distance(transform.position, player.position) < stoppingDistance && playerOnSameLevel)
        {
            angry = true;
            chill = false;
            goBack = false;
        }

        if (Vector2.Distance(transform.position, player.position) > stoppingDistance || !playerOnSameLevel)
        {
            goBack = true;
            angry = false;
        }

        if (chill)
        {
            Chill();
        }
        else if (angry)
        {
            Angry();
        }
        else if (goBack)
        {
            Goback();
        }
    }
    //Спокойное патрулирование
    void Chill()
    {
        if (transform.position.x > point.position.x + positionOfPatrol)
        {
            moveingRight = false;
            _characterSprite.flipX = false;
        }
        else if (transform.position.x < point.position.x - positionOfPatrol)
        {
            moveingRight = true;
            _characterSprite.flipX = true;
        }

        if (moveingRight)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }
    }
    //Бежит на врага
    void Angry()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // Поворот к игроку
        if (player.position.x < transform.position.x)
        {
            _characterSprite.flipX = false;
        }
        else if (player.position.x > transform.position.x)
        {
            _characterSprite.flipX = true;
        }
    }
    //Возвращение к точек если игрок далеко
    void Goback()
    {
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);

        // Поворот к точке
        if (point.position.x < transform.position.x)
        {
            _characterSprite.flipX = false;
            moveingRight = false;
        }
        else if (point.position.x > transform.position.x)
        {
            _characterSprite.flipX = true;
            moveingRight = true;
        }
    }
}