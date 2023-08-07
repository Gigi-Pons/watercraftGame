//automatedSprites.cs

//Required for Vector2, Point, etc.
//Just scoreValue and scale changes for Chapter08
using Microsoft.Xna.Framework;
//required for Texture2D, etc.
using Microsoft.Xna.Framework.Graphics;

namespace TankVipers
{
    //the automated sprites are the fish
    class AutomatedSprite : Sprite
    {
        //Sprite is automated.  Direction is same as speed.
        //direction is abstract in base, must override in derived
        //speed is a Vector2 from Sprite class
        public override Vector2 direction
        {
            //speed is a Vector2 from Sprite class
            get { return speed;  }
        }

        public AutomatedSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, 
            string collisionCueName, int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
                  sheetSize, speed, collisionCueName, scoreValue) { }

        public AutomatedSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, string collisionCueName, int scoreValue)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
                  sheetSize, speed, millisecondsPerFrame, collisionCueName, scoreValue) { }

        //added constructor
        public AutomatedSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            string collisionCueName, int scoreValue, float scale)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
             sheetSize, speed, collisionCueName, scoreValue, scale) { }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //Move automated sprite based on direction
            position += direction;

            base.Update(gameTime, clientBounds);
        }
    }
}
