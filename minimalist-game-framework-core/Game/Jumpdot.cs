using System;
using System.Collections.Generic;
using System.Text;


class Jumpdot : Obstacle
{
    private float radius;
    private bool enabled;
    private bool soundEnabled = false;
    private static float frame;

    private static float frameRate = 3;

    public Jumpdot(float xPos, float yPos, float size, float radius,  int objSprite) : base(xPos, yPos, size, objSprite)
    {
        this.radius = radius;
        enabled = true;

        
    }

    //use Obstacle superclass constructer


    public override void handleCollision(Player p)
    {

        if ((p.center() - pos).Length() <= radius && enabled)
        {
            p.setJump(true);
            p.setJumpdot(this);
            soundEnabled = true;
        }
        if ((p.center() - pos).Length() > radius || !enabled)
        {
            soundEnabled = false;
        }

    }

    public override void update(float camshift)
    {
        base.update(camshift);
        frame = (frame + Engine.TimeDelta * frameRate) % 4.0f;


    }

    public override int getSprite()
    {
        return objSprite + (int)frame;
    }
    //allows player to jump if within certain radius of dot
    public void setEnabled(bool enabled)
    {
        this.enabled = false;
    }
    public bool isEnabled()
    {
        return enabled;
    }
    //Sound only plays once
    public void setsoundEnabled(bool soundEnabled)
    {
        this.soundEnabled = false;
    }
    public bool issoundEnabled()
    {
        return soundEnabled;
    }
    public override void reset()
    {
        base.reset();
        enabled = true;
        soundEnabled = false;
    }
}