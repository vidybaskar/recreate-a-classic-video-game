using Newtonsoft.Json;
using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

class Game
{
    public static readonly string Title = "Minimalist Game Framework";
    public static Vector2 Resolution = new Vector2(1200, 900);
    private static readonly int numLevels = 3;

    private static float unit = Resolution.X / 16 / 100;

    private static Player player;
    private static Player playerWin;
    private static Player playerSelect;
    public static Font arialSmall = Engine.LoadFont("arial.ttf", 50);
    public static Font arialLarge = Engine.LoadFont("arial.ttf", 100);

    private static List<Obstacle> obstacles;
    //all obstacles in current level

    private static List<Texture> playerSprites;
    private static List<Texture> obstacleSprites;

    private static List<Texture> UI;
    private static List<Button> buttonMain;
    private static List<Button> main1;
    private static List<Button> main2;
    private static List<Button> main3;
    //sprite lists to choose from

    private static List<LevelText> levelTexts;

    private static int left;
    private static int right;
    //current obstacles to draw and calc collisions

    private static float obstacleSpeed = -450;

    private static Obstacle ground;

    public static bool mainMenu = true;
    private static int screenIndex = 0;
    private static int curLevel = 0;

    //Repeat music if dead
    private static int musicRepeat = 0;
    private static int musicIndex;
    //Checks if allowed click on button 
    private static bool allowedClick = true;
    private static bool alloweddotSlider = false;
    //On settings screen or not
    private static bool screen3 = false;

    Texture volumeButton = Engine.LoadTexture("./Music/volume.png");
    Texture musicSlider = Engine.LoadTexture("./Music/music slider.png");
    Texture dotSlider = Engine.LoadTexture("./Music/dot slider.png");
    float musicSliderPosx;
    float musicSliderPosy;
    float dotSliderPosx;
    float dotSliderPosy;
    float tempPos = 0; 

    private static Settings settings = new Settings();

    private Button back = new Button(5f / 6, 0, 1 / 7.5f, 1f / 6, 0);

    private Button work = new Button(.85f, .9f, .12f, .08f, 8);
    private static bool hiding = false;


    public Game()
    {

        importSettings();

        loadUI();
        loadSprites();

        loadPlayers();

        //setting up game, could be moved to update() 
        //depending on whether level select is implemented
    }

    public void Update()
    {
        
        if (hiding)
        {
            Engine.DrawTexture(UI[7], new Vector2(0, 0), size: Resolution);
        }
        else {
            if (mainMenu)
            {
                getInputsMain();

                drawButtons(buttonMain);
                Engine.DrawString("Scuffed Geometry Dash", new Vector2(Resolution.X / 2, Resolution.Y / 15), Color.White, arialLarge, TextAlignment.Center);

                drawScreens();


            }
            else
            {
                handleMusic(); //all music functions

                Engine.DrawTexture(UI[0], new Vector2(Resolution.X * 5 / 6, 0), size: new Vector2(Resolution.X / 7.5f, Resolution.Y / 6));
                if (player.hasWon())
                {

                    playerWin.updateRot();
                    drawWin();

                    handleBack();

                    //win screen post-level

                }
                else if (!player.isDead())
                {

                    player.setAirborne(true);
                    player.setJump(false);
                    //set conditions to minimum abilities at the beginning


                    //get the amount that obstacle should shift by to simulate camera

                    updateBounds();
                    calcCollisions();

                    player.updateRot();

                    getInputsGame();
                    float camShift = player.updatePos(300, 900);
                    updateObstacles(camShift);
                    //shift screen by amount character is falling/jumping


                    drawPlayer();
                    drawObstacles();

                    handleBack();
                    //for writing text on top of everything else
                    debug();
                }

                else
                {
                    reset();

                    player.setDead(false);
                    //reset level on death
                }
            }
        }
        workButton();
        //pauses and hides game


    }


    public void importSettings()
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(settings.GetType());
        if (!File.Exists("Assets/settings.txt"))
        {
            settings = new Settings();
            settings.res = 1;
            settings.volume = 50;
            settings.playerSprite = 0;
            serialize();

        }
        else
        {
            string[] lines = File.ReadAllLines("Assets/settings.txt");
            settings = new Settings();
            settings.res = int.Parse(lines[2]);
            settings.volume = int.Parse(lines[1]); ;
            settings.playerSprite = int.Parse(lines[0]);
        }
        //import settings into relevant class from settings.txt



        if (settings.res == 0 && Resolution.X != 640)
        {
            changeRes(640, 480, 27, 53, 0);


        }

        else if (settings.res == 1 && Resolution.X != 1200)
        {
            changeRes(1200, 900, 50, 100, 1);
        }
        else if (settings.res == 2 && Resolution.X != 1600)
        {
            changeRes(1600, 1200, 67, 133, 2);
        }
        //close and reopen to saved resolution
    }

  
    public void loadObstacles(int x)
    {
        musicIndex = x;
        string[] lines = File.ReadAllLines("Assets/Levels/level"+x+".txt");
        

        obstacles = new List<Obstacle>();
        levelTexts = new List<LevelText>();

        Obstacle.setVel(obstacleSpeed);
        LevelText.setVel(obstacleSpeed);



        foreach (string line in lines)
        {

            string[] input = line.Split(" ");

            float xPos = float.Parse(input[1]);
            float yPos = float.Parse(input[2]);

            if (input[0]=="Ground")
            {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);
                ground = new Block(xPos, yPos, xSize, ySize,  sprite);
            }

            
            if (input[0] == "Block") {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);

                obstacles.Add(new Block(xPos, yPos, xSize, ySize,  sprite));
            }
            if (input[0] == "Spike")
            {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int rotation = int.Parse(input[5]);
                int sprite = int.Parse(input[6]);

                obstacles.Add(new Spike(xPos, yPos, xSize, ySize,  rotation, sprite));
            }
            if (input[0] == "Spikestrip")
            {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);

                obstacles.Add(new Spikestrip(xPos, yPos, xSize, ySize,  sprite));
            }
            if (input[0] == "Dot")
            {
                float size = float.Parse(input[3]);
                float radius = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);

                obstacles.Add(new Jumpdot(xPos, yPos, size, radius, sprite));
            }
            if (input[0] == "Bounce")
            {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);

                obstacles.Add(new Bounce(xPos, yPos, xSize, ySize, sprite));
            }
            if (input[0] == "GameEnd")
            {
                float xSize = float.Parse(input[3]);
                float ySize = float.Parse(input[4]);
                int sprite = int.Parse(input[5]);

                obstacles.Add(new GameEnd(xPos, yPos, xSize, ySize,  sprite));

            }

            if (input[0] == "Text")
            {
                levelTexts.Add(new LevelText(xPos, yPos, arialSmall, Color.White, input[3]));

            }
        }
        //loads all obstacles and text into obstacle list based on level .txt files

        
    }
    //NOTE: obstacle x positions must be from left to right across the level

    private void loadLevelAt(float levelX)
    {
        foreach (Obstacle o in obstacles)
        {
            o.setXPos(o.getPos().X - levelX);
        }
        ground.setXPos(ground.getPos().X - levelX);
        
    }
    //offset level based on where you want player to start
    private void loadSprites()
    {
        int index = 0;
        playerSprites = new List<Texture>();
        while (index < 100)
        {
            try
            {
                playerSprites.Add(Engine.LoadTexture("./Player Sprites/playerSprite" + index + ".png"));
                index++;
            }
            catch
            {
                break;
            }
        }
        //add all player sprites

        obstacleSprites = new List<Texture>();
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/platform.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/bounceBoost.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/jumpbox.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/jumpdot.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/jumpdot0.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/jumpdot1.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/jumpdot2.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/spike.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/spikeStrip.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/spike2.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/portal.png"));
        obstacleSprites.Add(Engine.LoadTexture("./Obstacles/ground.png"));
        //add all obstacle sprites
    }
    private void loadUI()
    {
        UI = new List<Texture>();
        UI.Add(Engine.LoadTexture("./UI/back.png"));
        UI.Add(Engine.LoadTexture("./UI/left.png"));
        UI.Add(Engine.LoadTexture("./UI/right.png"));
        UI.Add(Engine.LoadTexture("./UI/up.png"));
        UI.Add(Engine.LoadTexture("./UI/down.png"));
        UI.Add(Engine.LoadTexture("./UI/play.png"));
        
        UI.Add(Engine.LoadTexture("./UI/gray.png"));
        UI.Add(Engine.LoadTexture("./UI/workScreen.png"));
        UI.Add(Engine.LoadTexture("./UI/workScreenButton.png"));

        //load all UI sprites

        buttonMain = new List<Button>();
        main1 = new List<Button>();
        main2 = new List<Button>();
        main3 = new List<Button>();

        buttonMain.Add(new Button(1f / 20, 1f / 3, 1f / 15, 1f / 3, 1));
        buttonMain.Add(new Button(19f / 20f - 1f / 15f, 1f / 3, 1f / 15, 1f / 3, 2));

        main1.Add(new Button(3f/5, 1f / 3, 1f / 10, 1f / 20, 3));
        main1.Add(new Button(3f / 5, 2f / 3, 1f / 10, 1f / 20, 4));
        main1.Add(new Button(1f / 4, 3f/5, 1f / 5, 4f / 15, 5));

        main2.Add(new Button(3f / 5, 2f / 5, 1f / 10, 1f / 20, 3));
        main2.Add(new Button(3f / 5, 3f / 5, 1f / 10, 1f / 20, 4));

        main3.Add(new Button(1f / 5, .33f, 1f / 4, 1f / 6, 6));
        main3.Add(new Button(1f / 5, .53f, 1f / 4, 1f / 6, 6));
        main3.Add(new Button(1f / 5, .73f, 1f / 4, 1f / 6, 6));

        //screen buttons
    }

    public void loadPlayers()
    {
        playerWin = new Player(750, 600, 0, 100, 0, 10f);
        playerWin.setAirborne(true);

        playerSelect = new Player(1000, 575, 0, 100, 0, 5f);
        playerSelect.setAirborne(true);

        player = new Player(160, 500, 0, 100, 1800, 2.5f, settings.playerSprite);
    }
    //loads player instances for post-game, selecting player, and regular gameplay player

    private void handleMusic()
    {
        //sets local feilds for positions and sizes of all music related sprites
        float volumeButtonSize = (float)100 * unit;
        float volumeButtonPosx = (float)1200 * unit;
        float volumeButtonPosy = (float)33 * unit;
        float slideDown = (float)20 * unit;
        float slideDownSpeed = (float)2.666 * unit;

        //Changes positions and size for volume on settings screen
        if (screen3)
        {
            volumeButtonPosx = (float)900 * unit;
            volumeButtonPosy = (float)500 * unit;
            volumeButtonSize = 200 * unit;
            slideDown = 550 * unit;
            slideDownSpeed = 10 * unit;
        }
        //variables to calculate dot position 
        float dotSliderSize = (float)26.666 * unit;
        float centerx = volumeButtonPosx + volumeButtonSize / 2;
        float centery = volumeButtonPosy + volumeButtonSize / 2;
        float radius = (float)(volumeButtonSize / 2);
        float musicSliderSizex = (float)(333.5656) * unit;
        float musicSldierSizey = (float)0.3997204748 * musicSliderSizex;
        float dist = (float)Math.Sqrt(Math.Pow((centerx - Engine.MousePosition.X), 2) + Math.Pow((centery - Engine.MousePosition.Y), 2)); // distance from mouse and center of volume button
        
        //Checks if you've clicked on volume button
        if ((dist > radius || (!Engine.GetMouseButtonHeld(MouseButton.Left))) && allowedClick)
        {
            musicSliderPosx = 900 * unit;
            musicSliderPosy = -100 * unit;
            //sets position of dot slider based on current volume and current position/size of the slider 
            dotSliderPosx = (((musicSliderPosx + musicSliderSizex - (20 * unit)) - musicSliderPosx + 20 * unit) * (settings.volume / ((float)100.0))) + (musicSliderPosx + 20 * unit);
            //Ensures position of dotslider is same as last position 
            if (dotSliderPosx != tempPos && tempPos != 0)
            {
                dotSliderPosx = tempPos;
            }
            if (dotSliderPosx>(musicSliderPosx+289*unit))
            {
                dotSliderPosx = musicSliderPosx + (289 * unit);
            }
            else if(dotSliderPosx<(musicSliderPosx+20*unit))
            {
                dotSliderPosx = musicSliderPosx + 20 * unit;
            }
             
            dotSliderPosy = (float)0.46 * musicSliderPosy;
            //Draws volume button if not clicked
            Engine.DrawTexture(volumeButton, new Vector2(volumeButtonPosx, volumeButtonPosy), size: new Vector2(volumeButtonSize, volumeButtonSize));
        }
        //volume button has been clicked
        else
        {
            //not allowed to click button again 
            allowedClick = false;
            //draws music slider and dot out of frame
            Engine.DrawTexture(musicSlider, new Vector2(musicSliderPosx, musicSliderPosy), size: new Vector2(musicSliderSizex, musicSldierSizey));
            Engine.DrawTexture(dotSlider, new Vector2(dotSliderPosx, dotSliderPosy), size: new Vector2(dotSliderSize, dotSliderSize));
            //chekcs if user wants to put away slider
            if (Engine.GetMouseButtonDown(MouseButton.Right))
            {
                moveSliderup();
            }
            //slides the volume control down from the top 
            else if (musicSliderPosy <= slideDown)
            {
                musicSliderPosy += slideDownSpeed;
                dotSliderPosy += slideDownSpeed;
            }
            if (musicSliderPosy >= 1.333 * unit)
            {
                alloweddotSlider = true;
                float dotSlidercenterx = dotSliderPosx + dotSliderSize / 2;
                float dotSlidercentery = dotSliderPosy + dotSliderSize / 2;
                float distDotSlider = (float)Math.Sqrt(Math.Pow((dotSlidercenterx - Engine.MousePosition.X), 2) + Math.Pow((dotSlidercentery - Engine.MousePosition.Y), 2)); // distance from mouse position and dot 
                float dotSldierradius = (float)(dotSliderSize / 2);
                //Checks if interacting with dot
                if (Math.Abs(distDotSlider - dotSldierradius) <= 300 * unit && Engine.GetMouseButtonHeld(MouseButton.Left) && alloweddotSlider)
                {
                    //moves dot right or left depending on position of mouse
                    if (Engine.MousePosition.X > dotSliderPosx && dotSliderPosx < musicSliderPosx + (289 * unit))
                    {
                        dotSliderPosx += (float)5.333 * unit;
                    }
                    else if (Engine.MousePosition.X < dotSliderPosx && dotSliderPosx > musicSliderPosx + (20 * unit))
                    {
                        dotSliderPosx -= (float)5.333 * unit;
                    }
                    //calculates volume based on position of dot on the slider (100 volume scale)
                    settings.volume = (int)Math.Round((dotSliderPosx-(musicSliderPosx+20*unit))/((musicSliderPosx+289*unit)-(musicSliderPosx+20*unit))*100);
                    //Ensures volume cannot go over 100 or under 0 
                    if(settings.volume>100)
                    {
                        settings.volume = 100;
                    }
                    else if(settings.volume<0)
                    {
                        settings.volume = 0;
                    }
                    Engine.MusicVolume(settings.volume); //sets volume
                    serialize();


                }
            }


        }
        //music repeats if you die
        if (musicRepeat == 0)
        {
            Musica.playMusic(player, settings.volume, musicIndex);
            musicRepeat += 1;

            
        }
    }
    //animates the slider going back up once done using it 
    private void moveSliderup()
    {
        tempPos = dotSliderPosx;
        alloweddotSlider = false;
        //moves position up until out of frame
        while (musicSliderPosy > -100 * unit)
        {
            musicSliderPosy -= (float)2.666 * unit;
            dotSliderPosy -= (float)2.666 * unit;

        }
        if (musicSliderPosy <= -100 * unit)
        {
            allowedClick = true;
        }
    }

    private void drawWin()
    {
        Engine.DrawString("You Won!", new Vector2(Resolution.X / 2, Resolution.Y / 3), Color.White, arialSmall, TextAlignment.Center);
        Engine.DrawTexture(playerSprites[player.sprite()], playerWin.getPos() * unit, size: new Vector2(player.getSize(), player.getSize()) * unit, rotation: playerWin.getAngle());
    }
    //draws winning screen

    private void drawScreens()
    {
        if (screenIndex == 0)
        {

            Engine.DrawString("Select a Level:", new Vector2(Resolution.X / 3, Resolution.Y / 2), Color.White, arialSmall, TextAlignment.Center);
            Engine.DrawString("Level " + (curLevel + 1), new Vector2(Resolution.X * 13f / 20, Resolution.Y / 2), Color.White, arialSmall, TextAlignment.Center);

            drawButtons(main1);
            screen3 = false;
            moveSliderup(); //moves volume control back up if not on settings page
        }
        else if (screenIndex == 1)
        {
            Engine.DrawString("Select a Character:", new Vector2(Resolution.X * 2 / 5, Resolution.Y / 2), Color.White, arialSmall, TextAlignment.Center);
            playerSelect.updateRot();
            Engine.DrawTexture(playerSprites[player.sprite()], playerSelect.getPos() * unit, size: new Vector2(player.getSize(), player.getSize()) * unit, rotation: playerSelect.getAngle());

            drawButtons(main2);
            moveSliderup(); //moves volume control back up if not on settings page
        }
        else

        {
            drawButtons(main3);
            screen3 = true;
            Engine.DrawString("Resolution:", new Vector2(Resolution.X * .33f, Resolution.Y / 4), Color.White, arialSmall, TextAlignment.Center);

            if (settings.res == 0) Engine.DrawString("640x480", main3[0].getRelPos() + new Vector2(Resolution.X * .125f, Resolution.Y * .05f), Color.Red, arialSmall, TextAlignment.Center);
            else Engine.DrawString("640x480", main3[0].getRelPos() + new Vector2(Resolution.X * .125f, Resolution.Y * .05f), Color.White, arialSmall, TextAlignment.Center);

            if (settings.res == 1) Engine.DrawString("1200x900", main3[1].getRelPos() + new Vector2(Resolution.X * .12f, Resolution.Y * .05f), Color.Red, arialSmall, TextAlignment.Center);
            else Engine.DrawString("1200x900", main3[1].getRelPos() + new Vector2(Resolution.X * .125f, Resolution.Y * .05f), Color.White, arialSmall, TextAlignment.Center);

            if (settings.res == 2) Engine.DrawString("1600x1200", main3[2].getRelPos() + new Vector2(Resolution.X * .12f, Resolution.Y * .05f), Color.Red, arialSmall, TextAlignment.Center);
            else Engine.DrawString("1600x1200", main3[2].getRelPos() + new Vector2(Resolution.X * .125f, Resolution.Y * .05f), Color.White, arialSmall, TextAlignment.Center);

            //changes which resolution is highlighted based off of which one is currently selected

            handleMusic();
        }
    }

    private void updateBounds()
    {
        while(left<obstacles.Count && obstacles[left].getPos().X+obstacles[left].getSize().X < 0)
        {
            left++;
        }

        while (right < obstacles.Count && obstacles[right].getPos().X<1600 )
        {
            right++;
        }
        //update left and right bounds for drawing & calculating collisions for objects

        
    }
    //update the left and right bounds of which obstacles are on-screen

    private void updateObstacles(float camShift)
    {
        foreach(Obstacle o in obstacles)
        {
            o.update(camShift);
        }
        foreach(LevelText lt in levelTexts)
        {
            lt.update(camShift);
        }
        ground.update(camShift);


    }
    //move obstacles left, move down depending on if camera is shifting
    //updates level  text move as well

    private void getInputsGame()
    {
        
            
        if (Engine.GetKeyHeld(Key.Space) && player.getJump())
        {
            player.jump();
        }

        if (Engine.GetKeyDown(Key.R))
        {
            reset();
        }

        //gets jump input during game
            
        
    }

    private void handleBack()
    {
        if (back.getClicked())
        {
            mainMenu = true;
            musicRepeat = 0;
            Engine.StopMusic();
            allowedClick = true;
        }
    }
    //Space to jump, R to reset
    private void getInputsMain()
    {
        if (buttonMain[1].getClicked())
        {
            screenIndex = (screenIndex + 1) % 3;

            
        } else if (buttonMain[0].getClicked())
        {
            screenIndex--;
            if (screenIndex < 0) screenIndex += 3;
            
        }
        //change main menu screen

        if (screenIndex == 0)
        {
            if (main1[0].getClicked())
            {
                curLevel--;
                if (curLevel < 0) curLevel += numLevels;
            } 
            if (main1[1].getClicked())
            {
                curLevel = (curLevel+1) % numLevels;
            }
            if (main1[2].getClicked())
            {
                loadObstacles(curLevel+1);
                loadLevelAt(0);
                mainMenu = false;
                reset();
            }
            //selecting current level

        }
        else if(screenIndex==1)
        {
            if (main2[0].getClicked())
            {

                if (player.sprite() - 1 < 0) player.setSprite(playerSprites.Count - 1);
                else player.setSprite(player.sprite() - 1);
                settings.playerSprite = player.sprite();
                serialize();
            }
            if (main2[1].getClicked())
            {
                player.setSprite((player.sprite() + 1) % playerSprites.Count) ;
                settings.playerSprite = player.sprite();
                serialize();
            }

            //selecting player sprite, saves to settings
            
        }
        else
        {
            if (main3[0].getClicked())
            {
                moveSliderup();
                settings.res = 0;
                serialize();

                changeRes(640, 480, 27, 53, 0);
                
                

                screenIndex = 2;                
            }
            if (main3[1].getClicked())
            {
                moveSliderup();
                settings.res = 1;
                serialize();
                changeRes(1200, 900,50, 100, 1);

                screenIndex = 2;
            }
            if (main3[2].getClicked())
            {
                moveSliderup();
                settings.res = 2;
                serialize();
                changeRes(1600, 1200, 67, 133, 2);
               
                screenIndex = 2;
                
            }
            //changes resolution and relavant settings
            //reopens Game to current screen


        }
    }

    private void calcCollisions()
    {
        ground.handleCollision(player);
        for (int i = left; i < right && i<obstacles.Count; i++)
        {

            obstacles[i].handleCollision(player);
        }


        
    }
    //Calculate collisions between all on-screen objects and player
    private void drawPlayer()
    {
        Engine.DrawTexture(playerSprites[player.sprite()], player.getPos()*unit,size: new Vector2(player.getSize(), player.getSize()) * unit,
            rotation: player.getAngle());
        
    }
    //draw the player at given position and rotation
    private void drawObstacles()
    {
        Engine.DrawTexture(obstacleSprites[ground.getSprite()], ground.getPos()*unit, size: ground.getSize()*unit);
        for(int i = left; i< right && i < obstacles.Count; i++)
        {
            Obstacle o = obstacles[i];
            Engine.DrawTexture(obstacleSprites[o.getSprite()], o.getPos()*unit, size: o.getSize()*unit);

        }

    }
    //Draw all on-screen objects

    private void drawTexts()
    {
        for (int i = 0; i < levelTexts.Count; i++)
        {
            LevelText lt = levelTexts[i];

            if (lt.getPos().X <= Resolution.X || lt.getPos().X>-Resolution.X)
            {
                Engine.DrawString(lt.getText(), lt.getPos()*unit, lt.getColor(), lt.getFont());
            }


        }
    }
    //draw all text that moves with the level

    private void drawButtons(List<Button>arr)
    {
        foreach(Button b in arr)
        {
            Engine.DrawTexture(UI[b.sprite()], b.getRelPos(), 
                size: b.getRelSize());
        }
    }
    //utility method to draw a list of buttons on screen

    private void reset()
    {
        ground.reset();
        player.reset();
        foreach(Obstacle o in obstacles)
        {
            o.reset();
        }
        left = 0;
        right = 0;
        //restarts the music if player died
        musicRepeat = 0;
    }
    //restarts the level by resetting player and obstacle original positions

    private void changeRes(int x, int y, int small, int large, int selected)
    {

        settings.res = selected;
        arialSmall = Engine.LoadFont("arial.ttf", small);
        arialLarge = Engine.LoadFont("arial.ttf", large);
        //loading correct font sizes

        musicRepeat = 0;

        Resolution = new Vector2(x, y);
        unit = Resolution.X / 16 / 100;
        SDL.SDL_VideoQuit();
        //close sdl video

        Engine.Start();
        Engine.Run();
        //restart the engine with new settings

    }

    public void workButton()
    {
        Engine.DrawTexture(UI[work.sprite()], work.getRelPos(), size: work.getRelSize());
        if (work.getClicked())
        {
            if (hiding)
            {
                Engine.resumeMusic();
                hiding = false;
            }
            else
            {
                Engine.pauseMusic();
                hiding = true;
            }
            //music pauses when hiding game
        }
    }
    //used to hide game using work overlay

    private void serialize()
    {
        string[] lines = { settings.playerSprite.ToString(), settings.volume.ToString(), settings.res.ToString() };

        File.WriteAllLines("Assets/settings.txt", lines);
    }
    //serializes settings to settings.txt file

    private void debug()
    {
        Engine.DrawString("Space to Jump, R to reset", new Vector2(unit*100, unit*200), Color.White, arialSmall);
        
    }
    //printing over the entire screen for debugging


}
