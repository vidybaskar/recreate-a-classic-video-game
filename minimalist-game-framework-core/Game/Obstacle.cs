using System;
using System.Collections.Generic;
using System.Text;

abstract class Obstacle
{
    protected Vector2 ogPos;
    protected Vector2 pos;
    //current position vs. original position

    protected readonly Vector2 size;
    protected static float xVel;
    protected int objSprite;

    public Obstacle(float xPos, float yPos, float xSize, float ySize,  int objSprite )
    {
        pos = new Vector2(xPos, yPos);
        ogPos = new Vector2(xPos, yPos);
        size = new Vector2(xSize, ySize);

        this.objSprite = objSprite;


    }

    public Obstacle(float xPos, float yPos, float size,  int objSprite)
    {
        pos = new Vector2(xPos, yPos);
        ogPos = new Vector2(xPos, yPos);
        this.size = new Vector2(size, size);

        this.objSprite = objSprite;

    }
    //for objects which 2 dimensions of size not necessary for def

    public abstract void handleCollision(Player p);
    //Implemented by subclasses to handle interaction with player

    public virtual void reset()
    {
        pos.X = ogPos.X;
        pos.Y = ogPos.Y;
    }
    //reset to original position

    public virtual void update(float camShift)
    {
        pos.X+=xVel*Engine.TimeDelta;
        pos.Y -= camShift;
    }
    //move left, move down if camera is moving

    public static void setVel(float vel)
    {
        xVel = vel;
    }
    public void setXPos(float x)
    {
        pos.X = x;
    }
    //for starting level at a certain amount of progress, sets X Position for object
    public Vector2 getPos()
    {
        return pos;
    }

    public Vector2 getSize()
    {
        return size;
    }

    public float vel()
    {
        return xVel;
    }

    public virtual int getSprite()
    {
        return objSprite;
    }
    //getter methods

       

        
}

