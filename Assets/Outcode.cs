using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcode
{

    public bool up, down, left, right;

    public Outcode()
    {
        up = false;
        down = false;
        left = false;
        right = false;
    }

    public Outcode(Vector2 point)
    {
        up = point.y > 1;
        down = point.y < -1;
        right = point.x > 1;
        left = point.x < -1;
    }

    public Outcode(bool Up, bool Down, bool Left, bool Right)
    {
        up = Up;
        down = Down;
        left = Left;
        right = Right;
    }

    public string printOutcode()
    {
        String UDLR = "";

        if(up)
        {
            UDLR = UDLR + 1;
        }

        else
        {
            UDLR = UDLR + 0;
        }

        if (down)
        {
            UDLR = UDLR + 1;
        }

        else
        {
            UDLR = UDLR + 0;
        }

        if (left)
        {
            UDLR = UDLR + 1;
        }

        else
        {
            UDLR = UDLR + 0;
        }

        if (right)
        {
            UDLR = UDLR + 1;
        }

        else
        {
            UDLR = UDLR + 0;
        }

        return UDLR;
    }

    public static bool operator ==(Outcode a, Outcode b)
    {
        return (a.up == b.up) && (a.down == b.down) && (a.left == b.left) && (a.right == b.right);
    }

    public static bool operator !=(Outcode a, Outcode b)
    {
        return !(a == b);
    }

    public static Outcode operator +(Outcode a, Outcode b)
    {

        return new Outcode(a.up || b.up, a.down || b.down, a.left || b.left, a.right || b.right);

    }

    public static Outcode operator *(Outcode a, Outcode b)
    {
        return new Outcode(a.up && b.up, a.down && b.down, a.left && b.left, a.right && b.right);
        
    }

    

}
