using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;

namespace TankVipers
{
    //Added for MonoGame sound
    //For data available across classes
    public static class MySounds
    {
        public static SoundEffect startSound;
        public static Song backgroundtrack;
        public static SoundEffect ballCollision;
        public static SoundEffect projectileSound;
        public static SoundEffect octopus;
        public static SoundEffect uglyFish;
        public static SoundEffect longNoseFish;
    }

    //used by projectile
    enum Dir { Down, Up, Left, Right }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteManager spriteManager;
        //random number generator
        public Random rnd { get; private set; }

        //score data 
        int currentScore = 0;
        public int returnScore { get { return currentScore; }
            private set { } }
        SpriteFont scoreFont;
        SpriteFont scoreFont2;

        //Background
        Texture2D backgroundTexture;
        Texture2D openingScene;
        Texture2D closingScene;

        //currentGameState is used to track game state.  Set to:
        //start by default below.
        //InGame in Update().  Will be the state while numberLivesRemaining > 0.
        //GameOver in NumberLivesRemaining property below which is decremented by AutomatedSprite collision.
        //Game states are checked in Update()
        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        //Lives remaining 
        //numberLivesRemaining is called a 'backing field'
        int numberLivesRemaining = 3;

        //numberLivesRemaining is the property that gets/sets the backing field
        //numberLivesRemaining is decremented in SpriteManager with each AutomatedSprite collision
        public int NumberLivesRemaining
        {
            get { return numberLivesRemaining; }
            set
            {

                numberLivesRemaining = value; //subtracts 1 from current lives remaining
                if(numberLivesRemaining <= 0)
                {
                    currentGameState = GameState.GameOver;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }

        //Game1() constructor instantiates GraphicsDeviceManager, rnd, and sets window size
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rnd = new Random();

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Initialize()
        {
            //NOTE: by making SpriteManager a component, its Update() and Draw() methods
            //are called automatically.

            Components.Add(spriteManager = new SpriteManager(this));

            //determines if spriteManager.Update() is called
            spriteManager.Enabled = false; //disable spriteManager
            spriteManager.Visible = false; //hide the spriteManager

            base.Initialize();
        }

        protected override void LoadContent() 
        {
            //create a new SpriteBatch which can be used to draw textures.
            //we don't use this here.  Drawing happens from SpriteManager and then Sprite
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Added for Monogame sound
            MySounds.startSound = Content.Load<SoundEffect>("Audio/bonus");
            MySounds.ballCollision = Content.Load<SoundEffect>("Audio/points");
            MySounds.octopus = Content.Load<SoundEffect>("Audio/shotDead");
            MySounds.uglyFish = Content.Load<SoundEffect>("Audio/shotDead");
            MySounds.longNoseFish = Content.Load<SoundEffect>("Audio/shotDead");
            MySounds.projectileSound = Content.Load<SoundEffect>("Audio/explosion");

            MySounds.backgroundtrack = Content.Load<Song>("Audio/track");

            //play start sound once when game loads
            MySounds.startSound.Play(0.5f, 0f, 0f); //(vol, picth(in octaves), pan)
            //added for monogame sound
            MediaPlayer.Play(MySounds.backgroundtrack); //Also .Stop, .Pause, .Resume
            //loop the backgroundTrack
            MediaPlayer.IsRepeating = true;

            //load font
            scoreFont = Content.Load<SpriteFont>("Arial50");
            scoreFont2 = Content.Load<SpriteFont>("Arial25");

            //load background image
            openingScene = Content.Load<Texture2D>("skullBackground");
            backgroundTexture = Content.Load<Texture2D>("skullBackground");
            closingScene = Content.Load<Texture2D>("skullBackground");

            base.LoadContent();
        }
        protected override void UnloadContent() {}
        protected override void Update(GameTime gameTime)
        {
            //perform actions based on the current game state
            switch(currentGameState)
            {
                case GameState.Start:
                    if(Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }
                    break;
                //playing the game
                case GameState.InGame:
                    break;
                //--NumberLivesRemaining on collisoins with AutomatedSprites in
                //SpriteManager.UpdateSprites()
                //GameOver set in NumberLivesRemaining property
                case GameState.GameOver:
                    if(Keyboard.GetState().IsKeyDown(Keys.Enter)) { Exit(); }
                    break;
              }

            //Exit game (also exits when player lives depleted and Enter is pressed
            if(Keyboard.GetState().IsKeyDown(Keys.Escape)) { this.Exit(); }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {

            //draw text based on the current game state
            //all text below is drawn in the center of the screen
            switch(currentGameState)
            {
                case GameState.Start:

                    spriteBatch.Begin();

                    spriteBatch.Draw(openingScene, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null,
                        Color.Gray, 0, Vector2.Zero, SpriteEffects.None, 0);

                    string text = "Water enemies are mean!";
                    spriteBatch.DrawString(scoreFont, text, new Vector2((Window.ClientBounds.Width / 2) -
                        (scoreFont.MeasureString(text).X / 1.3f),
                        (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString(text).Y / 2) - 100), Color.White, 0, Vector2.Zero,
                        1.5f, SpriteEffects.None, 1);

                    //draw text for intro splash screen (scale = 1.5f)
                    //+80 places this line below the one above
                    text = "(Press any key to begin)";
                    spriteBatch.DrawString(scoreFont, text, new Vector2((Window.ClientBounds.Width / 2) -
                        (scoreFont.MeasureString(text).X / 1.3f),
                        (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString(text).Y / 2) + 40), Color.White, 0, Vector2.Zero,
                        1.5f, SpriteEffects.None, 1);

                    spriteBatch.End();
                    break;

                case GameState.InGame:
                    GraphicsDevice.Clear(Color.White);
                    spriteBatch.Begin();

                    //draw background image at 0,0
                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, Window.ClientBounds.Width,
                        Window.ClientBounds.Height), null,
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

                    //draw score at 10, 5
                    spriteBatch.DrawString(scoreFont2, "Score: " + currentScore, new Vector2(10, 5), Color.White, 0,
                        Vector2.Zero,
                        1, SpriteEffects.None, 1);

                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.Aquamarine);

                    spriteBatch.Begin();

                    spriteBatch.Draw(closingScene, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null,
                        Color.Gray, 0, Vector2.Zero, SpriteEffects.None, 0);

                    //-60 places this line 
                    string gameover = "Game Over! Water enemies bite again!";
                    spriteBatch.DrawString(scoreFont, gameover, new Vector2((Window.ClientBounds.Width / 2) -
                        (scoreFont.MeasureString(gameover).X / 2), (Window.ClientBounds.Height / 2) -
                        (scoreFont.MeasureString(gameover).Y / 2)-
                        60), Color.White);

                    //+30 places this line below the one above
                    gameover = "Your score: " + currentScore;
                    spriteBatch.DrawString(scoreFont, gameover, new Vector2((Window.ClientBounds.Width / 2) -
                        (scoreFont.MeasureString(gameover).X / 2), (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString
                        (gameover).Y / 2) + 40), Color.WhiteSmoke);

                    //+140 places this line below the one above
                    gameover = "(Press ENTER to exit)";
                    spriteBatch.DrawString(scoreFont, gameover, new Vector2((Window.ClientBounds.Width / 2) -
                        (scoreFont.MeasureString(gameover).X / 2), (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString
                        (gameover).Y / 2) + 140), Color.White);

                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        public void PlayCue(string cueName)
        {

            if(cueName == "octopus") { MySounds.octopus.Play(1f, 0.5f, 0f); }
            else if (cueName == "uglyFish") { MySounds.uglyFish.Play(1f, 0.5f, 0f); }
            else if (cueName == "longNoseFish") { MySounds.longNoseFish.Play(1f, 0.5f, 0f); }
            else if(cueName == "skullball") { MySounds.ballCollision.Play(1f, 0.5f, 0f); }
            else if(cueName == "projectileSound") { MySounds.projectileSound.Play(1f, 5f, 0f); }
        }

        public void AddScore(int score) { currentScore += score; }
    }
}



