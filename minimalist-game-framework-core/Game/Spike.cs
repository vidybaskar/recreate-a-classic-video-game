using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Testing Branch 
/// </summary>
class Spike: Obstacle
{
    private int rotation;
    public Spike(float xPos, float yPos, float xSize, float ySize, int rotation, int objSprite) : base(xPos, yPos, xSize, ySize, objSprite)
    {
        this.rotation = rotation;
    }
    //


    public override void handleCollision(Player p)
    {

        Point[] corners = p.corners();

        Point[] coords;
        if (rotation == 0)
        {
            coords = new Point[]{
            new Point(pos.X+size.X/2, pos.Y),
            new Point(pos.X, pos.Y+size.Y),
            new Point(pos.X+size.X, pos.Y+size.Y) };
        }
        else if (rotation == 1)
        {
            coords = new Point[]{
            new Point(pos.X, pos.Y),
            new Point(pos.X+size.X, pos.Y+size.Y/2),
            new Point(pos.X, pos.Y+size.Y)};
        } 
        else if (rotation ==2){
            coords = new Point[]{
            new Point(pos.X, pos.Y),
            new Point(pos.X+size.X, pos.Y),
            new Point(pos.X+size.X/2, pos.Y+size.Y)};
        }
        else
        {
            coords = new Point[]{
            new Point(pos.X+size.X, pos.Y),
            new Point(pos.X, pos.Y + size.Y / 2),
            new Point(pos.X+size.X, pos.Y + size.Y)};
    }
        
        
        //Points for equilateral triangle, rotated clockwise by 90 degrees
        //per rotation

        bool collided = false;
        foreach (Point corner in corners)
        {
            if (corner.PointInShape(coords))
            {
                collided = true;
            }
        }



        if (!collided)
        {
            foreach (Point coord in coords)
            {
                if (coord.PointInShape(corners))
                {
                    collided = true;
                }
            }
        }
        //calculate if player corner in triangle or the other way around

        if (collided)
        {
            p.setDead(true);
        }
    }
    //Player dies if collides with spike
}

