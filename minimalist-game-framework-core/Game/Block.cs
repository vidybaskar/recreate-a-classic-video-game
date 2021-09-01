using System;
using System.Collections.Generic;
using System.Text;

class Block : Obstacle
{
    public Block(float xPos, float yPos, float xSize, float ySize,  int objSprite):base(xPos, yPos, xSize, ySize, objSprite)
    {

    }
    //use Obstacle superclass constructer


    public override void handleCollision(Player p)
    {

        Point[] corners = p.corners();

        Point[] coords = new Point[]{
            new Point(pos.X, pos.Y),
            new Point(pos.X+size.X, pos.Y),
            new Point(pos.X+size.X, pos.Y+size.Y),
            new Point(pos.X, pos.Y+size.Y)

        };
        //calculate points for rectangle

        bool collided = false;
        foreach (Point corner in corners)
        {
            if (corner.PointInShape(coords))
            {
                collided = true;
            }
        }
        //mark collided in player corner in block


        if (collided )
        {
            
            p.setGrounded(true);
            if (p.getPos().Y <= pos.Y - p.getSize()/2)
            {
                float target =(float)(Math.Round(p.getAngle() / 90) * 90);
                if (Math.Abs( target-p.getAngle()) > 5)
                {
                    int sign = Math.Sign(target - p.getAngle());
                    p.setRotation(sign);
                    p.setAirborne(true);

                    Point cur = corners[0];
                    for(int i = 1; i<4; i++)
                    {
                        if (corners[i].point.Y > cur.point.Y)
                        {
                            cur = corners[i];
                        }
                    }
                    p.setYPos(p.getPos().Y - (cur.point.Y - this.pos.Y));
                    if (p.vel() > 10)
                    {
                        p.setVel(10);
                    }

                }
                else
                {
                    p.setAngle((float)(Math.Round(p.getAngle() / 90) * 90));
                    p.setYPos(pos.Y - p.getSize());
                    p.setAirborne(false);
                    p.setRotation(1);
                    if (p.vel() > 0)
                    {
                        p.setVel(0);
                    }
                }
                

                if (p.vel()> 0)
                {
                    p.setVel(0);
                }
               
                p.setJump(true);
                //player must be certain height above platform to have landed on "top"
                //Allow jumping, stop rotation
            }
            else
            {
                p.setDead(true);
            }
            return;
            
        }

        for (int i = 0; i < 2; i++)
        {
            if (coords[i].PointInShape(corners))
            {
                float target = (float)(Math.Round(p.getAngle() / 90) * 90);
                if (Math.Abs(target - p.getAngle()) > 5)
                {
                    int sign = Math.Sign(target - p.getAngle());
                    p.setRotation(sign);
                    p.setAirborne(true);
                    if (p.vel() > 10)
                    {
                        p.setVel(10);
                    }
                }
                else
                {
                    p.setAngle((float)(Math.Round(p.getAngle() / 90) * 90));
                    p.setYPos(pos.Y - p.getSize());
                    p.setAirborne(false);
                    p.setRotation(1);
                    if (p.vel() > 0)
                    {
                        p.setVel(0);
                    }
                }
                
                p.setGrounded(true);
                p.setJump(true);
                //player must be certain height above platform to have landed on "top"
                //Allow jumping, stop rotation
                return;
            }
        }
        


        for (int i = 2; i<4; i++)
        {
            if (coords[i].PointInShape(corners))
            {
                p.setDead(true);

                return;
            }
        }

        //if player collides with bottom two corners, die

        



    }


}

