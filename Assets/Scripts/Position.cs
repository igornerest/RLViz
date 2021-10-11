using UnityEngine;

public class Position
{
    public int X { get;  }
    public int Y { get;  }

    public Position(int xPos, int yPos)
    {
        this.X = xPos;
        this.Y = yPos;
    }

    public Vector3 getWorldPosition()
    {
        return new Vector3(this.X, 0, this.Y);
    }

    public Position MoveWithAction(Action action)
    {
        switch (action)
        {
            case Action.UP:     return new Position(X, Y + 1);
            case Action.RIGHT:  return new Position(X + 1, Y);
            case Action.DOWN:   return new Position(X, Y - 1);
            case Action.LEFT:   return new Position(X - 1, Y);
            default:            return this;
        }
    }

    public override bool Equals(object obj)
    {
        Position position = obj as Position;

        if (position == null)
        {
            return false;
        }

        return position.X == this.X && position.Y == this.Y;
    }

    /* Bijective function for hashing 2D coordinates
     * based on https://www.cs.upc.edu/~alvarez/calculabilitat/enumerabilitat.pdf 
     */
    public override int GetHashCode()
    {
        int tmp = (this.Y + ((this.X + 1) / 2));
        return this.X + (tmp * tmp);
    }

    public override string ToString()
    {
        return string.Format("Position (X: {0}, Y: {1})", X, Y);
    }
}
