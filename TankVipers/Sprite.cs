using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankVipers
{
    abstract class Sprite
    {
        //For drawing the sprite
        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;

        protected float scale = 1;
        protected float originaScale = 1;
        Vector2 originalSpeed;

        //Collision data
        int collisionOffset;

        //Framerate data
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;

        //Movement data
        protected Vector2 speed;
        protected Vector2 position;

        //definition of collisionCueName property
        public string collisionCueName { get; private set; }

        //definition of direction property
        //abstract means it must be overridden in children
        public abstract Vector2 direction { get; }

        //get current position of the sprite
        public Vector2 GetPosition { get { return position; } }

        public int scoreValue { get; protected set; }

        //Gets the collision rect based on position, framesize, scale, and collision offset of the Sprite
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)(position.X + (collisionOffset * scale)),
                    (int)(position.Y + (collisionOffset  * scale)),
                    (int)((frameSize.X - (collisionOffset * 2)) * scale),
                    (int)((frameSize.Y - (collisionOffset * 2)) * scale));
            }
        }

        //calls the second constructor with defaulst MillisecondsPerFrame
        //if not supplied in the instantiation
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, 
            string collisionCueName, int scoreValue)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
                  sheetSize, speed, defaultMillisecondsPerFrame, collisionCueName,
                  scoreValue) { }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, string collisionCueName, int scoreValue)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            originalSpeed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.collisionCueName = collisionCueName;
            this.scoreValue = scoreValue;
        }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            string collisionCueName, int scoreValue, float scale)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
                  sheetSize, speed, defaultMillisecondsPerFrame, collisionCueName,
                  scoreValue)
        {
            this.scale = scale;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Update frame if time to do so based on framerate
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
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
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the sprite (called from SpriteManager.Draw())
            spriteBatch.Draw(textureImage, position,
                new Rectangle(currentFrame.X * frameSize.X,
                currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public bool IsOutOfBounds(Rectangle clientRec)
        {
            //Top left is the origin draw point (no offsets used here)
            if (position.X < -frameSize.X || position.X > clientRec.Width ||
                position.Y < -frameSize.Y || position.Y > clientRec.Height)
            {
                return true;
            }
            return false;


        }

        //scale and speed are modified by a factor when ModifyScale and ModifySpeed are called
        public void ModifyScale(float modifier) { scale *= modifier; }

        public void ResetScale() { scale = originaScale; }

        public void ModifySpeed(float modifier) { speed *= modifier; }

        public void ResetSpeed() { speed = originalSpeed; }
    }
}
