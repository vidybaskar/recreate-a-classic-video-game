using System;
using System.Collections.Generic;
using System.Text;


static class Musica
{

    public static int musicIndex;
    public static float volume;
    //loads music and volume and plays when called
    public static void playMusic(Player p, float volume, int musicIndex)
    {
            // if not dead plays music and sets volume   
            if (p.isDead() == false)
             {
                
                Engine.MusicVolume(volume);
                Engine.PlayMusic(Engine.LoadMusic("./Music/music" + musicIndex + ".mp3"));

            }
            //stops music if on main menu
            if (p.hasWon()||Game.mainMenu)
            {
                Engine.StopMusic();
                
            }

    }
}