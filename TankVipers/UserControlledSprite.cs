using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankVipers
{
    //The user controlled sprite is the watercraft 
    class UserControlledSprite : Sprite
    {

        //direction property
        //Get direction of sprite based on player input and speed
        //No need for mouse movement here.  That is in Update();
        public override Vector2 direction
        {
            get 
            {
                Vector2 inputDirection = Vector2.Zero;

                //Keyboard arrows
                ///If player pressed arrow keys, move the sprite
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A)) inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D)) inputDirection.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W)) inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S)) inputDirection.Y += 1;

                return inputDirection * speed;
            }
        }

        //pass values to the base constructor
        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, null, 0) { }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, float scale)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, null, 0, scale) { }

        //Pass values to the base constructor
        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, 
            Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, 
                  sheetSize, speed, millisecondsPerFrame, null, 0) { }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //move the sprite based on direction
            position += direction;

            //if sprite is off the screen, move it back within the game window
            if (position.X < 0) position.X = 0;
            if (position.Y < 0) position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X) position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y) position.Y = clientBounds.Height - frameSize.Y;

            base.Update(gameTime, clientBounds);
        }
    }
}
