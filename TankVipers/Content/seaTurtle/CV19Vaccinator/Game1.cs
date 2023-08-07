using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CV19Vaccinator
{
    public class Game1 : Game
    {
        //to interact with our window (graphics device)
        GraphicsDeviceManager graphics;
        //to draw images and text on the screen
        SpriteBatch spriteBatch;

        Texture2D bubble_Sprite;

        Texture2D turtle_Sprite;

        Texture2D background_Sprite;

        SpriteFont gameFont;
        SpriteFont gameFont50;

        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 150;
        Point frameSize = new Point(143, 116);
        Point currentFrame = new Point(50, 0);
        Point sheetSize = new Point(6, 1);

        //starting position of target is the center of the background
        //Vector2 targetPosition = new Vector2(960, 540)

        Vector2 targetPosition = new Vector2(894, 376);

        //both image radii are used to offset drawing of those images.
        //the top left of images is where drawing begins.  Therefore, move the location
        //where drawing begins up and to the left half of the image size.  That way,
        //the center of the image will be located at the center of the drawing position.
        const int BUTTERFLY_OFFSET = 45;
        const int CAT_OFFSET = 75;

        //for mouseposition and buttons state
        MouseState mState;

        //Mouse button is initially released
        bool mReleased = true;

        //stores distance between mouse and target to determine if a hit
        float mouseTargetDist;

        //try to get high score in 20 seconds
        int score = 0;

        //play the game for 20 seconds
        float timer = 20f;
        
        //flag prevents you from adding more seconds to the game
        bool flag = true;

        //instantiate a Random object for target placement
        Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            //Width and height has been decided to where it fits the screen and the x button can be pressed on window
            //set width of game window
            graphics.PreferredBackBufferWidth = 1880;
            //set height of game window
            graphics.PreferredBackBufferHeight = 980;
            //apply the game window size changes
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //SpriteBatch is a class used to draw images and text to the screen.
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load sprites
            bubble_Sprite = Content.Load<Texture2D>("Bubble35");
            turtle_Sprite = Content.Load<Texture2D>("turtle140");
            background_Sprite = Content.Load<Texture2D>("underwater");

            //Load font
            gameFont = Content.Load<SpriteFont>("Arial25");
            gameFont50 = Content.Load<SpriteFont>("Arial50");

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                    {
                        currentFrame.Y = 0;
                    }
                }
            }

            //Frame rate of 60/sec TotalSeconds will equal 1/60 (time since last game loop).
            //so, 1/60 sec (16.7ms) will be subtracted from the timer 60 times per second.
            //Therefore, our timer will countdown once per second.
           
            if (timer > 0) 
            { 
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds; 
                
                //Add 10 additional seconds only if a score higher or equal to than 9 is met
                //and the timer is still greater than 10
                if(timer >= 10 && score > 9 && flag)
                {
                    timer += 10;
                    flag = false;
                }
            }

            //get current mouse position and button state;
            mState = Mouse.GetState();

            //mouseTargetDist is Distance() between target and mouse position
            mouseTargetDist = Vector2.Distance(targetPosition, new Vector2(mState.X, mState.Y));

            //if mouse LeftButton is pressed but that hit has not been previously recorded (mReleased == true)
            if(mState.LeftButton == ButtonState.Pressed && mReleased == true)
            {
                //if mouseTargetDist < BUTTERFLY_OFFSET that means it is a hit!
                if(mouseTargetDist < BUTTERFLY_OFFSET && timer > 0)
                {
                    score++;

                    //arguments are min and max values; min is inclusive and max is exclusive
                    //(+1 is for extra pixel on the edge since max is exclusive)
                    targetPosition.X = rand.Next(BUTTERFLY_OFFSET, graphics.PreferredBackBufferWidth - BUTTERFLY_OFFSET + 1);
                    targetPosition.Y = rand.Next(BUTTERFLY_OFFSET, graphics.PreferredBackBufferHeight - BUTTERFLY_OFFSET + 1);
                }

                //set this to only record one hit per left mouse press
                mReleased = false;
            }

            //if mouse leftButton is released then set mRelleased = true to enable new hit recording
            if(mState.LeftButton == ButtonState.Released) { mReleased = true; }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            //bakcground is drawn first.  Butterfly and cat are drawn over background.
            //tinted with white does not alter color.
            spriteBatch.Draw(background_Sprite, new Vector2(0, 0), Color.White);

            //time ramaining in the game
            if(timer > 0)
            {
                //draw so the center of the target is at the center of the position
                spriteBatch.Draw(bubble_Sprite, new Vector2(targetPosition.X - BUTTERFLY_OFFSET, targetPosition.Y -
                    BUTTERFLY_OFFSET), Color.White);

                //Keep drawing the score and text as long as the timer is not zero
                spriteBatch.DrawString(gameFont, "Score: " + score.ToString(), new Vector2(4, 8), Color.Black);

                //draw the timer value.   .Ceiling() rounds up to the nearest integer
                spriteBatch.DrawString(gameFont, "Time: " + Math.Ceiling(timer).ToString(), new Vector2(4, 44), Color.Black);
            }

            if(timer <= 0)
            {
                spriteBatch.Draw(background_Sprite, new Vector2(0, 0), Color.CornflowerBlue);
                spriteBatch.DrawString(gameFont50, "GAME OVER", new Vector2(780, 490), Color.Black);
                spriteBatch.DrawString(gameFont50, "Score: " + score.ToString(), new Vector2(790, 590), Color.Black);
            }

            //draw so the center of the cat is at the center of the position
            spriteBatch.Draw(turtle_Sprite,
                new Vector2(mState.X - CAT_OFFSET, mState.Y - CAT_OFFSET),
                new Rectangle(currentFrame.X * frameSize.X,
                currentFrame.Y * frameSize.Y,
                frameSize.X,
                frameSize.Y),
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

/*
 * This game was the most fun to make.  We were able to use the images we want to and add
 * functionality of our own.  One thing I am liking about the class is that we are using 
 * skills from all of our previous projects and that makes me feel like I am expanding my 
 * knowledge.  The semester is going by so fast and I am learning very much as we go.
 */
