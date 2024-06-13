using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    //Variables
    public float startX;
    public float startY;

    public Vector2 currentTile, nextTile;
    bool moving;
    public float speedMove;
    public LayerMask wallMask;
    public Image lifeBar;
    //private bool canMove = true;

    public Animator anim;
    [HideInInspector] public Directions dir;
    [SerializeField] private AudioSource footStepFX;


    // Start is called before the first frame update
    void Start()
    {
        PlayerValues.InitLife(150);

        transform.position = new Vector2(startX, startY);
        anim = GetComponent<Animator>();

        currentTile = transform.position;
        nextTile = transform.position;
        moving = false;
    }

    private void FixedUpdate()
    {
        anim.SetBool("Moving", moving);

        if (moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextTile, speedMove * Time.deltaTime);
            if ((Vector2)transform.position == nextTile)
            {
                currentTile = nextTile;
                moving = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            if (Input.GetKey(KeyCode.W)) nextTile = CheckNextTile(Directions.Up);
            if (Input.GetKey(KeyCode.A)) nextTile = CheckNextTile(Directions.Left);
            if (Input.GetKey(KeyCode.S)) nextTile = CheckNextTile(Directions.Down);
            if (Input.GetKey(KeyCode.D)) nextTile = CheckNextTile(Directions.Right);
            if (currentTile != nextTile)
            {
                moving = true;
                footStepFX.Play();
            }
        }
    }
    //Comprobar el siguiente Tile
    Vector2 CheckNextTile(Directions direction)
    {
        dir = direction;
        anim.SetFloat("Direction", (int)dir);
        Vector2 tempTile = currentTile;
        //Segun la direccion que me miras si hay una colision segun el Enum
        Vector2 rayDirection = Direction.GetDirection(direction);

        Collider2D checkColliders = Physics2D.OverlapCircle(tempTile + rayDirection, 0.4f, wallMask);
       //bool check = Physics2D.Raycast(tempTile, rayDirection, 1, wallMask);
        if (checkColliders == null)
        {
            tempTile += rayDirection;
        }

        return tempTile;
    }

    public void GetDamage(int damage)
    {
        PlayerValues.RemoveLife(damage);
        ReprintLifeBar();
    }

    public void ReprintLifeBar()
    {
        lifeBar.fillAmount = (float)PlayerValues.life / (float)PlayerValues.maxLife;
    }
}
