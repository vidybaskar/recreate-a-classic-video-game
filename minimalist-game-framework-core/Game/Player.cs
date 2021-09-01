using System;
using System.Collections.Generic;
using System.Text;


class Player
{
    private int spriteUse;
    private float yVel;
    //player does not have velocity , objects move toward player

    private readonly float grav;
    private float rotation;
    //acceleration

    private Vector2 pos;
    private Vector2 ogPos;
    //original position vs. current position

    private float angle;
    private float size;

    private bool grounded;
    private bool airborne;
    private bool won;

    private bool canJump;

    private bool dead;

    public Jumpdot dot;

    public Player(float xPos, float yPos, float angle, float size, float grav, float rotation, int spriteUse = 0)
    {
        this.pos = new Vector2(xPos, yPos);
        this.ogPos = new Vector2(xPos, yPos);
        this.size = size;

        this.grav = grav;

        yVel = 0;

        this.spriteUse = spriteUse;
        this.angle = angle;
        this.rotation = rotation;

        dead = false;
        airborne = false;
        canJump = true;
        won = false;
        grounded = false;
    }

    public void jump()
    {
        yVel = -grav * 0.5f;
        setRotation(1);
        grounded = false;
        
        if (dot != null && !grounded)
        {
            dot.setEnabled(false);
            //plays sound if hasn't been already played
            if (dot.issoundEnabled() == true)
            {
                Sounds.jumpdotSound();
                dot.setsoundEnabled(false);
            }
        }
    }
    //accelerate upwards a certain amount

    public float updatePos(float lowerBound, float upperBound)
    {
        if (airborne)
        {
            yVel += grav * Engine.TimeDelta;
        }



        if (((center().Y >= lowerBound && yVel < 0) || (center().Y <= upperBound && yVel > 0)))
        {
            pos.Y += yVel * Engine.TimeDelta;
            return 0;
        }

        return yVel * Engine.TimeDelta;

    }
    //player stops moving at 1/4 and 3/4 of screen resolution, moves obstacles to simulate camera
    public void updateRot()
    {
        if (airborne)
        {
            angle += rotation;
            angle = angle % 360f;
        }

    }
    //update current rotation post-collision

    public void setRotation(int sign)
    {
        rotation = Math.Abs(rotation) * sign;
    }

    public void setJumpdot(Jumpdot dot)
    {
        this.dot = dot;
    }


    public void setYPos(float y)
    {
        pos.Y = y;
    }

    public void setVel(float v)
    {
        yVel = v;
    }

    public void setAngle(float angle)
    {
        this.angle = angle;
    }

    public void setDead(bool dead)
    {
        this.dead = dead;
        Sounds.deadSound(); //play death sound
    }

    public void setAirborne(bool airborne)
    {
        this.airborne = airborne;
    }

    public void setGrounded(bool grounded)
    {
        this.grounded = grounded;
    }

    public void setWon(bool won)
    {
        this.won = won;
    }

    public void setJump(bool jump)
    {
        this.canJump = jump;
    }
    //setter methods

    public void setSprite(int sprite)
    {
        this.spriteUse = sprite;
    }
    public void reset()
    {
        this.pos.X = ogPos.X;
        this.pos.Y = ogPos.Y;
        this.rotation = Math.Abs(rotation);
        angle = 0;
        yVel = 0;
        setJumpdot(null);
        this.grounded = false;
        this.dead = false;
        this.won = false;
        setRotation(1);
    }
    //reset to 0 velocity and original position

    public Vector2 getPos()
    {
        return pos;
    }

    public float getAngle()
    {
        return angle;
    }

    public float getSize()
    {
        return size;
    }

    public float getGrav()
    {
        return grav;
    }

    public float vel()
    {
        return yVel;
    }
    //getter methods

    public Vector2 center()
    {
        return pos+(new Vector2(size, size)/2);
    }
    //calculate center of square

    public Point[] corners()

    {
        Point[] ret = new Point[4];
        ret[0] = new Point(pos).rotate(center(), angle);
        ret[1] = new Point(pos + new Vector2(0, size)).rotate(center(), angle);
        ret[2] = new Point(pos + new Vector2(size, size)).rotate(center(), angle);
        ret[3] = new Point(pos + new Vector2(size, 0)).rotate(center(), angle);

        return ret;

    }
    //calculate corners of square rotated to a given angle

    public float rotVel()
    {
        return rotation;
    }
    public bool isDead()
    {
        return dead;
    }

    public bool isAirborne()
    {
        return airborne;
    }

    public bool hasWon()
    {
        return won;
    }

    public bool getJump()
    {
        return canJump;
    }

    public bool getGrounded()
    {
        return grounded;
    }
    public int sprite()
    {
        return spriteUse;
    }
    //getter methods
}

