using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Coordinate
{
    private int X;
    private int Y;

    public Coordinate(int posX, int posY)
    {
        this.x = posX;
        this.y = posY;
    }

    public int x
    {
        get
        {
            return X;
        }

        set
        {
            X = value;
        }
    }

    public int y
    {
        get
        {
            return Y;
        }

        set
        {
            Y = value;
        }
    }

}
