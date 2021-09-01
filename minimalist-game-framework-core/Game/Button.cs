using System;
using System.Collections.Generic;
using System.Text;


class Button
{
    private Vector2 pos;
    //current position vs. original position

    private readonly Vector2 size;
    private int objSprite;


    public Button(float xPos, float yPos, float xSize, float ySize,  int objSprite)
    {
        pos = new Vector2(xPos, yPos);
        size = new Vector2(xSize, ySize);

        this.objSprite = objSprite;

    }

    public Vector2 getPos()
    {
        return pos;
    }

    public Vector2 getRelPos()
    {
        return new Vector2(pos.X * Game.Resolution.X, pos.Y * Game.Resolution.Y);
    }

    public Vector2 getSize()
    {
        return size;

    }

    public Vector2 getRelSize()
    {
        return new Vector2(size.X * Game.Resolution.X, size.Y * Game.Resolution.Y);
    }

    public int sprite()
    {
        return objSprite;
    }

    public bool getClicked()
    {
        float x = Engine.MousePosition.X / Game.Resolution.X;
        float y = Engine.MousePosition.Y / Game.Resolution.Y;
        return x > pos.X && x < pos.X + size.X &&
            y > pos.Y && y < pos.Y + size.Y &&
            Engine.GetMouseButtonDown(MouseButton.Left);
    }


}

