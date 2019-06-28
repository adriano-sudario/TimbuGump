using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;

namespace TimbuGump.Entities.Obstacles
{
    class Guy : Body
    {
        private readonly float gravity = 0.2f;
        float forceApplied = -3f;

        public Guy(Vector2 position) : base(position, sprite: GetAnimationDefault(), scale: 5f)
        {

        }

        private static Sprite GetAnimationDefault()
        {
            Texture2D spriteSheet = Loader.LoadTexture("guy_on_stairs");
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            animationFrames.Add("guy", new Frame[]
            {
                new Frame() { Name = "guy_1", Source = new Rectangle(11, 7, 5, 9), Duration = 100 }
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            forceApplied += gravity;

            MoveAndSlide(new Vector2(3f, forceApplied));
        }
    }
}
