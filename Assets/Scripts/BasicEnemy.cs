using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicEnemy : EnemyManager
{
    [Header("FX Audio")]
    [SerializeField] private AudioSource attackFX;
    [SerializeField] private AudioSource failAttackFX;

    [Header("Movement Settings")]
    public Vector2 moveArea;
    public float limitArea;
    public float areaDetect;
    public int percentValueAttack;
    public Transform obstacleTile;

    Collider2D player;
    BaseEnemyStates state;
    
    Directions moveDirection;
    public Animator anim;


    void Start()
    {
        maxLife = life;
        state = BaseEnemyStates.Patrol;
        transform.position = initPosition;
        currentTile = transform.position;
    }

    void Update()
    {
        switch (state)
        {
            case BaseEnemyStates.Patrol:
                PatrolUpdate();
                break;

            case BaseEnemyStates.Follow:
                FollowUpdate();
                break;

            case BaseEnemyStates.Attack:
                AttackUpdate();
                break;
        }
    }


    void PatrolUpdate()
    {
        if (move)
        {
            if (stepTile != currentTile)
            {
                transform.position = Vector2.MoveTowards(transform.position, stepTile, speedMove * Time.deltaTime);
                if ((Vector2)transform.position == stepTile)
                {
                    currentTile = transform.position;
                    //compureba si el Player esta dentro del area de ataque
                    CheckPlayerInArea();
                }
            }
            else
            {
                if (currentTile == nextTile) move = false;
                else stepTile = GetNextStepTile();
            }
        }
        else
        {
            SetNextRandomTile();
        }
    }

    void FollowUpdate()
    {
        if (stepTile != currentTile)
        {
            transform.position = Vector2.MoveTowards(transform.position, stepTile, speedMove * Time.deltaTime);
            if ((Vector2)transform.position == stepTile)
            {
                currentTile = transform.position;

                if (Vector2.Distance(currentTile, player.transform.position) <= 1)
                {
                    state = BaseEnemyStates.Attack;
                }
                else
                {
                    CheckPlayerInArea();
                }
            }
        }
        else
        {
            if (currentTile == nextTile)
            {
                moveDirection = Direction.GetDirection(player.transform.position - transform.position);
                anim.SetFloat("Dir", (int)moveDirection);
                move = false;
            }
            else
            {
                stepTile = GetNextStepTile();
                //Debug.Log(stepTile);
            }
        }
    }

    void AttackUpdate()
    {
        rateTime += Time.deltaTime;
        if (rateTime >= rateAttack)
        {
            rateTime = 0;
            Debug.Log("Esta atacando");
            if (Vector2.Distance(initPosition, player.transform.position) > limitArea)
            {
                CheckPlayerInArea();
            }
            else
            {
                if (Vector2.Distance(transform.position, player.transform.position) > 1.2f)
                {
                    state = BaseEnemyStates.Follow;
                    CheckPlayerInArea() ;
                }
                else
                {
                    //Particulas de ataque

                    //ataca al player con daño aleatorio
                    bool canMiss = Random.Range(0, percentValueAttack) == 0; //0, (1,2,3)
                    if (canMiss)
                    {
                        //TODO: crear particulas y sonido cuando falla el golpe
                        failAttackFX.Play();
                    }
                    else
                    {
                        //TODO: crear particulas y sonido cuando golpea
                        attackFX.Play();
                        moveDirection = Direction.GetDirection(player.transform.position - transform.position);
                        anim.SetFloat("Dir", (int)moveDirection);
                        int randomDamage = Random.Range(atackDamage.x, atackDamage.y);
                        player.GetComponent<PlayerControl>().GetDamage(randomDamage);
                    }
                }
            }
        }
    }


    void CheckPlayerInArea()
    {
        player = Physics2D.OverlapCircle(transform.position, areaDetect, (1 << 3));
        if (player != null)
        {
            if (Vector2.Distance(initPosition, player.transform.position) < limitArea)
            {
                nextTile = player.transform.position;
                stepTile = GetNextStepTile();
                state = BaseEnemyStates.Follow;
            }
            else
            {
                SetNextRandomTile();
                state = BaseEnemyStates.Patrol;
            }
        }
        else
        {
            SetNextRandomTile();
            state = BaseEnemyStates.Patrol;
        }
    }

    //Mira el tile mas cercano
    Vector2 GetNextStepTile()
    {
        List<Vector2> dirTiles = new List<Vector2>();
        dirTiles.Add(currentTile + Vector2.down);
        dirTiles.Add(currentTile + Vector2.up);
        dirTiles.Add(currentTile + Vector2.right);
        dirTiles.Add(currentTile + Vector2.left);

        float minDistance = Mathf.Infinity;
        int index = -1;
        for (int i = 0; i < dirTiles.Count; i++)
        {
            float dist = Vector2.Distance(dirTiles[i], nextTile);
            if (dist < minDistance && Physics2D.OverlapCircle(dirTiles[i], 0.4f) == null)
            {
                minDistance = dist;
                index = i;
            }
        }
        moveDirection = (Directions)index;
        anim.SetFloat("Dir", (int)moveDirection);
        obstacleTile.transform.position = dirTiles[index];
        return dirTiles[index];
    }

    void SetNextRandomTile()
    {
        //futura posicion
        Vector2 newPosition = new Vector2((int)Random.Range(-moveArea.x, moveArea.x), (int)Random.Range(-moveArea.y, moveArea.y));
        nextTile = initPosition + newPosition;

        //recalcular en caso de que este ocupado el tile
        while (Physics2D.OverlapCircle(nextTile, 0.4f) != null)
        {
            //Debug.Log("Estoy encima de una colision, recalculo una nueva posicion");
            newPosition = new Vector2((int)Random.Range(-moveArea.x, moveArea.x), (int)Random.Range(-moveArea.y, moveArea.y));
            nextTile = initPosition + newPosition;
        }
        //Debug.Log(nextTile);
        stepTile = GetNextStepTile();
        obstacleTile.transform.position = stepTile;
        move = true;
    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);
    }
}
