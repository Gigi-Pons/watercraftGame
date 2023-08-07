using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankVipers
{
    class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //SpriteBatch for drawing
        SpriteBatch spriteBatch;
        //One sprite for the player (watercraft)
        UserControlledSprite player;
        //A list of automated sprites (fish)
        List<Sprite> spriteList = new List<Sprite>();

        //variables for spawning new enemies
        int enemySpawnMinMilliseconds = 1000;
        int enemySpawnMaxMilliseconds = 2000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 10;
        int nextSpawnTime = 0;

        //chance of spawning different enemies
        int likelihoodOctopus = 33;
        int likelihoodUglyFish = 33;

        //scoring (this values are subtracted from player score)
        int octopusPointValue = 30;
        int uglyFishPointValue = 20;
        int longNoseFishPointValue = 10;

        //Lives 
        List<AutomatedSprite> livesList = new List<AutomatedSprite>();

        //spawn time variables
        int nextSpawnTimeChange = 5000;
        int timeSinceLastSpawnTimeChange = 0;

        //powerup/powerdown
        int powerUpExpiration = 0;

        public SpriteManager(Game game) : base(game) { }

        //for projectiles
        public KeyboardState kStateOld = Keyboard.GetState();

        public override void Initialize()
        {
            //initialize spawn time
            ResetSpawnTime();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            //Load the player sprite.  Call constructor
            player = new UserControlledSprite(
                Game.Content.Load<Texture2D>("tank2"), new Vector2(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2), new Point(109, 64), 10, new Point(0, 0),
                new Point(1, 0), new Vector2(6, 6), 1.0f);

            //load player lives list
            for(int i=0; i<((Game1)Game).NumberLivesRemaining; ++i)
            {
                //10 = number of pixels from left; 40 = number of pixels between sprites
                int offset = 10 + i * 40;
                livesList.Add(new AutomatedSprite(
                    Game.Content.Load<Texture2D>("tank2"), new Vector2(offset, 45),
                    new Point(109, 64), 10, new Point(0, 0), new Point(1, 0), Vector2.Zero, null, 0, .28f));
            }
 
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //decrement next spawn time
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if(nextSpawnTime < 0)
            {
                SpawnEnemy();
                ResetSpawnTime();
            }

            UpdateSprites(gameTime);
            AdjustSpawnTimes(gameTime);
            CheckPowerUpExpiration(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateSprites(GameTime gameTime)
        {
            //update player
            player.Update(gameTime, Game.Window.ClientBounds);

            //update all non-player sprites
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];

                s.Update(gameTime, Game.Window.ClientBounds);

                //check for collisions
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    //play collision sound
                    if (s.collisionCueName != null) ((Game1)Game).PlayCue(s.collisionCueName);

                    //if the sprite is an AutomatedSprite, it is an enemy (fish)
                    //If AutomatedSprite (fish), subtract a life from the player.
                    if(s is AutomatedSprite)
                    {
                        if(livesList.Count > 0)
                        {
                            livesList.RemoveAt(livesList.Count - 1);
                            --((Game1)Game).NumberLivesRemaining;
                        }

                    }
                    if (s.collisionCueName == "octopus")
                    {
                        //collided with octopus - start fish power-down - increases in size
                        powerUpExpiration = 3000;
                        player.ModifyScale(2);
                    }
                    //collided with uglyFish - start fish power-down - decreases in speed
                    else if(s.collisionCueName == "uglyFish")
                    {
                        powerUpExpiration = 3000;
                        player.ModifySpeed(.5f);
                    }

                    //remove collided sprite from the game
                    spriteList.RemoveAt(i);
                    --i;
                }

                //remove snake if it is out of bounds
                if (s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    //reduce player score if a snake gets away
                    ((Game1)Game).AddScore(-spriteList[i].scoreValue);
                    spriteList.RemoveAt(i);
                    //shift the list left
                    --i;
                }
            }

            KeyboardState kState = Keyboard.GetState();

            int score = ((Game1)Game).returnScore;
            //if score is greater than zero, projectile will be twice as big
            if(score > 0)
            {

               kState = Keyboard.GetState();
                if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space))
                {
                    //add new projectile to the projectiles list
                    Projectile.projectiles.Add(new Projectile(Game.Content.Load<Texture2D>("bullet25"),
                        new Vector2(player.GetPosition.X + 50, player.GetPosition.Y + 20),
                        new Point(25, 25), 10, new Point(0, 0), new Point(1, 0), new Vector2(50, 0), "projectileSound", 7, 2));

                    MySounds.projectileSound.Play(1f, 0.5f, 0f);

                }
            } 
            //otherwise, it will be regular size
            else
            {
                if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space))
                {
                    //add new projectile to the projectiles list
                    Projectile.projectiles.Add(new Projectile(Game.Content.Load<Texture2D>("bullet25"),
                        new Vector2(player.GetPosition.X + 50, player.GetPosition.Y + 20),
                        new Point(25, 25), 10, new Point(0, 0), new Point(1, 0), new Vector2(50, 0), "projectileSound", 7, 1));

                    MySounds.projectileSound.Play(1f, 0.5f, 0f);

                }
            }

            //update lives-list sprites.  sprite.Update() is called for each sprite to animate each sprite
            foreach(Sprite sprite in livesList) { sprite.Update(gameTime, Game.Window.ClientBounds); }

            //reset kStateOld to previous state
            kStateOld = kState;

            //call projectile.Update() for each projectile
            foreach(Projectile p in Projectile.projectiles)
            {
                p.Update(gameTime, Game.Window.ClientBounds);
            }

            //remove projectiles/fish combinations for collisions
            for(int i = 0; i < Projectile.projectiles.Count; ++i)
            {
                for(int j = 0; j < spriteList.Count; ++j)
                {
                    Sprite s = spriteList[j];

                    //bullet collided with a fish
                    if (s.collisionRect.Intersects(Projectile.projectiles[i].collisionRect))
                    {
                        //inrease the score due to a hit
                        ((Game1)Game).AddScore(spriteList[j].scoreValue);
                        //projectile marked for removal
                        Projectile.projectiles[i].collided = true;
                        //snake removed
                        spriteList.RemoveAt(j);
                        //play hit sound
                        MySounds.ballCollision.Play(1f, 1f, 0f);
                    }
                }
            }

            Projectile.projectiles.RemoveAll(p => p.collided);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            player.Draw(gameTime, spriteBatch);

            //Draw all sprites
            foreach(Sprite s in spriteList) { s.Draw(gameTime, spriteBatch);  }

            //draw all projectiles
            foreach(Projectile p in Projectile.projectiles) { p.Draw(gameTime, spriteBatch); }

            //draw player lives in top left corner of scene
            foreach(Sprite sprite in livesList) { sprite.Draw(gameTime, spriteBatch); }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ResetSpawnTime()
        {
            //set the next spawn time for an enemy to a random value
            nextSpawnTime = ((Game1)Game).rnd.Next(enemySpawnMinMilliseconds, enemySpawnMaxMilliseconds);
        }

        //called from Update()
        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;


            //default frame size
            Point frameSize = new Point(28, 28);

            //generate fish from right to left
            position = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ((Game1)Game).rnd.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight - frameSize.Y));

            //create speed in the negative X and 0 Y
            speed = new Vector2(-((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed), 0);

            //determine which fish to instantiate.
            //get random number between 0 and 99
            int random = ((Game1)Game).rnd.Next(100);
            //likelihoodOctopus is 33%
            if(random < likelihoodOctopus)
            {
                //create an octopus enemy, a little faster
                spriteList.Add( 
                    new AutomatedSprite(Game.Content.Load<Texture2D>("octopus"), position, new Point(50, 50), 10, new Point(0, 0),
                    new Point(6, 0), speed * 1.3f, "octopus", octopusPointValue));
            }
            //likelihoodUglyFish is 33%
            else if(random < likelihoodOctopus + likelihoodUglyFish)
            {
                //create a uglyFish enemy
                spriteList.Add( 
                    new AutomatedSprite(Game.Content.Load<Texture2D>("uglyFish"), position, new Point(50, 50), 10, new Point(0, 0),
                    new Point(6, 0), speed, "uglyFish", uglyFishPointValue));
            }
            //likelihood remaining is 34%
            else
            {
                //create a longNoseFish, a little slower
                spriteList.Add( 
                    new AutomatedSprite(Game.Content.Load<Texture2D>("longNoseFish"), position, new Point(50, 50), 10, new Point(0, 0),
                    new Point(4, 0), speed * 0.8f, "longNoseFish", longNoseFishPointValue));
            }

        }

        protected void AdjustSpawnTimes(GameTime gameTime)
        {
            //resetSpawnTime() above sets spawn time to: enemySpawnMinmilliseconds < nextSpawnTime < enemySpawnMaxMillisenconds
            //if the spawn max time is > 500 milliseconds decrease the spawn time if it is time to do so based on the
            //spawn-timer variables.  enemyspawnMaxMilliseconds starts at 2000; enemySpawnMinMilliseconds starts at 1000
            if(enemySpawnMaxMilliseconds > 500)
            {
                //timeSinceLastSpawntimeChange starts at 0; nextSpawnTimeChange always 5 seconds
                //every 5 seconds, enemySpawnMaxMilliseconds and enemySpawnMinMilliseconds.  max is down to a min of 500ms.
                timeSinceLastSpawnTimeChange += gameTime.ElapsedGameTime.Milliseconds;
                //will change the spawn time every 5 seconds
                if(timeSinceLastSpawnTimeChange > nextSpawnTimeChange)
                {
                    //reset to ~0
                    timeSinceLastSpawnTimeChange -= nextSpawnTimeChange;
                    //for the 1st second, larger reduction in spawn time bracket
                    if(enemySpawnMaxMilliseconds > 1000)
                    {

                        enemySpawnMaxMilliseconds -= 100;
                        enemySpawnMinMilliseconds -= 100;
                    }
                    //after 1st second, smaller reduction in spawn time bracket
                    else
                    {
                        enemySpawnMaxMilliseconds -= 10;
                        enemySpawnMinMilliseconds -= 10;
                    }
                }
            }
        }

        protected void CheckPowerUpExpiration(GameTime gameTime)
        {
            //powerUpExpiration set to 5000 if collided with plus, skull, or bolt
            //is a power-up active?
            if(powerUpExpiration > 0)
            {
                //decrement power-up timer
                powerUpExpiration -= gameTime.ElapsedGameTime.Milliseconds;
                if(powerUpExpiration <= 0)
                {
                    //power-up timer has expired, end all power-ups
                    powerUpExpiration = 0;
                    player.ResetScale();
                    player.ResetSpeed();
                }
            }
        }

    }
}
