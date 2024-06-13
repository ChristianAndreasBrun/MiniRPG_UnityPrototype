using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{

    public Vector2 initPosition;
    public int life;
    public int speedMove;
    public float rateAttack;
    public Vector2Int atackDamage;
    public Image lifeBar;
    public GameObject destroyDead;
    public GameObject drop;
    public GameObject deathParticles;
    [SerializeField] private AudioSource deathFX;
    public Directions dir;

    protected Vector2 currentTile, stepTile, nextTile;
    protected bool move;
    protected float rateTime;
    protected int maxLife;


    public virtual void GetDamage(int damage)
    {
        life -= damage;
        if (life < 0)
        {
            deathFX.Play(); 
            Destroy((destroyDead != null) ? destroyDead : gameObject);
            Instantiate(deathParticles, transform.position, transform.rotation);


            if (drop != null)
            {
                Vector2 dropPosition = (Vector2)transform.position - Direction.GetDirection(dir);
                Instantiate(drop, dropPosition, Quaternion.identity);
            }
        }
        else
        {
            //Aqui recibe daño. TODO: Crear particulas y sonido
        }
        lifeBar.fillAmount = (float)life / (float)maxLife;
    }

    public void GetDirection(Directions directions)
    {
        dir = directions;
    }
}
