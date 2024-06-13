using UnityEngine;

public class PlayerValues
{
    static public int life;
    static public int maxLife;
    

    static public void InitLife(int value)
    {
        life = value;
        maxLife = value;
    }

    static public void RemoveLife(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            life = 0;
            Debug.Log("Has muerto");
            //cambio de escena: Has muerto
        }
    }

    static public void AddLife(int value)
    {
        life += value;
    }
}
