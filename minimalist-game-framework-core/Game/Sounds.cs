using System;
using System.Collections.Generic;
using System.Text;


class Sounds
{
    //Loads and plays all sound affects when called
    public static void deadSound()
    {
        {
            Engine.PlaySound(Engine.LoadSound("./Sounds/Dead.mp3"));
        }

    }
    public static void bounceSound()
    {
        {
            Engine.PlaySound(Engine.LoadSound("./Sounds/Bounce.mp3"));
        }

    }
    public static void jumpdotSound()
    {
        {
            Engine.PlaySound(Engine.LoadSound("./Sounds/JumpDot.mp3"));
        }

    }
}