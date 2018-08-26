using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct GridCoord : IEquatable<GridCoord>
{
    public int x;
    public int z;
    public GridCoord(int x, int z) { this.x = x; this.z = z; }

    public GridCoord GetNewCoordFromDirection(Direction dir)
    {
        GridCoord newCoord = new GridCoord(x, z);
        switch (dir)
        {
            case Direction.Forward: newCoord.z++; break;
            case Direction.Back: newCoord.z--; break;
            case Direction.Left: newCoord.x--; break;
            case Direction.Right: newCoord.x++; break;
        }
        return newCoord;
    }

    public override bool Equals(object obj)
    {
        return this.Equals((GridCoord)obj);
    }

    public bool Equals(GridCoord v)
    {
        if (System.Object.ReferenceEquals(v, null))
        {
            return false;
        }
        if (System.Object.ReferenceEquals(this, v))
        {
            return true;
        }
        if (this.GetType() != v.GetType())
        {
            return false;
        }
        return (this.x == v.x) && (this.z == v.z);
    }

    public override int GetHashCode()
    {
        return this.x * 0x00010000 + this.z;
    }
    public static bool operator ==(GridCoord lhs, GridCoord rhs)
    {
        if (System.Object.ReferenceEquals(lhs, null))
        {
            if (System.Object.ReferenceEquals(rhs, null))
            {
                return true;
            }
            return false;
        }
        return lhs.Equals(rhs);
    }

    public static bool operator !=(GridCoord lhs, GridCoord rhs)
    {
        return !(lhs == rhs);
    }

}

public enum TypeCell
{
    Wall = 0,
    Free = 1,
    EnemySpawn = 2,
    PlayerSpawn = 3
}

public enum Direction
{
    Forward = 0,
    Back = 1,
    Left = 2,
    Right = 3
}

public enum StateEnemy
{
    Normal = 0,
    Afraid = 1,
    Death = 2,
    Chase = 3
}