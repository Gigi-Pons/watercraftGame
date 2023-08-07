using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TankVipers
{
    class Projectile : Sprite
    {
        public override Vector2 direction { get { return speed; } }

        //auto-implemented property
        public bool collided { get; set; }

        public static List<Projectile> projectiles = new List<Projectile>();

        public Projectile(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, string collisionCueName, int scoreValue, int scale)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, collisionCueName, scoreValue, scale) { }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //move sprite based on direction
            position += direction;

            base.Update(gameTime, clientBounds);
        }
    }
}
