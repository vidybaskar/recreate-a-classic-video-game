using System;
using System.Collections.Generic;
using System.Text;


class LevelText
{
    private Vector2 ogPos;
    private Vector2 pos;
    private Font f;
    private Color c;

    private static float xVel;
    private string text;

    public LevelText(float xPos, float yPos, Font f, Color c, string text)
    {
        pos = new Vector2(xPos, yPos);
        ogPos = new Vector2(xPos, yPos);
        this.f = f;


        this.c = c;

        this.text = text;
    }
   

    public void reset()
    {
        pos.X = ogPos.X;
        pos.Y = ogPos.Y;
    }
    //reset to original position

    public void update(float camShift)
    {
        pos.X += xVel * Engine.TimeDelta;
        pos.Y -= camShift;
    }
    //move left, move down if camera is moving

    public void setXPos(float x)
    {
        pos.X = x;
    }
    //for starting level at a certain amount of progress, sets X Position for object


    public static void setVel (float vel)
    {
        xVel = vel;
    }
    public Vector2 getPos()
    {
        return pos;
    }

    public string getText()
    {
        return text;
    }

    public float vel()
    {
        return xVel;
    }

    public Font getFont()
    {
        return f;
    }

    public Color getColor()
    {
        return c;
    }
    //getter methods
}

