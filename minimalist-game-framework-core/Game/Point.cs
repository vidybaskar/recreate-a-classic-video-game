using System;
using System.Collections.Generic;
using System.Text;

class Point
{
    public Vector2 point;
    //Utility class for dealing with shapes inside other shapes through vector math

    public Point(float x, float y)
    {
        point = new Vector2(x, y);
    }

    public Point(Vector2 point)
    {
        this.point = point;
    }


    public Point rotate(Vector2 origin, float rotation)
    {
        double rad = Math.PI * rotation / 180;
        double cosT = Math.Cos(rad);
        double sinT = Math.Sin(rad);


        float x = (float) (cosT * (point.X - origin.X) - sinT * (point.Y - origin.Y) + origin.X);

        float y = (float)
            (sinT * (point.X - origin.X) +
            cosT * (point.Y - origin.Y) + origin.Y);

        return new Point(x, y);
    }
    //used to rotate a point around an arbitrary origin for certain number of degrees

    private float isLeft(Vector2 P0, Vector2 P1, Vector2 P2)
    {
        return ((P0.X - P2.X) * (P1.Y - P2.Y) - (P1.X - P2.X) * (P0.Y - P2.Y));
    }

    public bool PointInShape(Point[]coords)
    {

        int last = Math.Sign(isLeft(coords[0].point, coords[1].point, point)); 
        for(int i = 1; i<coords.Length; i++)
        {
            Vector2 first = coords[i].point;
            Vector2 second = coords[(i+1)%coords.Length].point;

            int sign = Math.Sign(isLeft(point, first, second));

            if (last!=0 && sign != last)
            {
                return false;
            }
            last = sign;

        }
        return true;
    }
    //calculates if a point is within a convex polygon
    //points must be given in clockwise or counterclockwise order

    public bool PointInCircle(Point center, float radius)
    {
        return (point - center.point).Length()<radius;
    }
    //calculates if a point is within circule radius

}
