using System;
using System.Collections.Generic;
using System.Text;


class Spikestrip:Obstacle
{
    public Spikestrip(float xPos, float yPos, float xSize, float ySize,  int objSprite) : base(xPos, yPos, xSize, ySize, objSprite)
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
        //rectangle coordinates

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
        //calculate if player corner in spikes or the other way around



        if (collided)
        {
            p.setDead(true);

        }







    }
}

