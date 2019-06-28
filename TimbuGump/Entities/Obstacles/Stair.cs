using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;

namespace TimbuGump.Entities.Obstacles
{
    class Stair : Body
    {
        public Stair(Vector2 position) : base(position, sprite: GetAnimationDefault(), scale: 5f)
        {

        }

        private static Sprite GetAnimationDefault()
        {
            Texture2D spriteSheet = Loader.LoadTexture("guy_on_stairs");
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            animationFrames.Add("stair", new Frame[]
            {
                new Frame() { Name = "stair_1", Source = new Rectangle(0, 5, 5, 11), Duration = 100 }
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }
    }
}
