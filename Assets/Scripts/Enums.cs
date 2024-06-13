using UnityEngine;


public enum Directions { Down, Up, Right, Left }

public class Direction
{
    //Sobrecarga de direcciones
    static public Vector2 GetDirection(Directions direction)
    {
        switch (direction)
        {
            case Directions.Down: return Vector2.down;
            case Directions.Up: return Vector2.up;
            case Directions.Right: return Vector2.right;
            case Directions.Left: return Vector2.left;
        }
        return Vector2.zero;
    }
    //Sobrecarga opuesta de direcciones
    static public Directions GetDirection(Vector2 direction)
    {
        Vector2 normalizedDir = direction.normalized;
        if (normalizedDir == Vector2.down) return Directions.Down;
        if (normalizedDir == Vector2.up) return Directions.Up;
        if (normalizedDir == Vector2.right) return Directions.Right;
        if (normalizedDir == Vector2.left) return Directions.Left;

        return Directions.Down;
    }
}


public enum BaseEnemyStates { Patrol, Follow, Attack }