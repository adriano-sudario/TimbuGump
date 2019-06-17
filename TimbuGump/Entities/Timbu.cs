using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;
using TimbuGump.Inputs;
using TimbuGump.Interfaces;

namespace TimbuGump.Entities
{
    public class Timbu : Body
    {
        private readonly float jumpForce = 5f;
        private readonly IInput input = new KeyboardInput();

        public Platform Ground { get; set; }
        public float ForceApplied { get; set; }

        public Timbu(Vector2 position) : base(position, sprite: GetAnimationDefault(), scale: 5f, customCollision: new Rectangle(0, 0, 10, 6))
        {

        }

        protected static AnimatedSprite GetAnimationDefault()
        {
            Texture2D spriteSheet = Loader.LoadTexture("8-bit_timbas");
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            animationFrames.Add("Run", new Frame[]
            {
                new Frame() { Name = "run_1", Source = new Rectangle(0, 0, 10, 5), Duration = 50 },
                new Frame() { Name = "run_2", Source = new Rectangle(10, 0, 10, 5), Duration = 50 },
                new Frame() { Name = "run_3", Source = new Rectangle(20, 0, 10, 5), Duration = 50 },
                new Frame() { Name = "run_4", Source = new Rectangle(30, 0, 10, 5), Duration = 50 },
                new Frame() { Name = "run_5", Source = new Rectangle(20, 0, 10, 5), Duration = 50 },
                new Frame() { Name = "run_6", Source = new Rectangle(10, 0, 10, 5), Duration = 50 }
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            input.Update();

            if (input.InteractionJustPressed() && Ground != null && ForceApplied >= 0)
            {
                Ground = null;
                ForceApplied = jumpForce * -1;
            }
        }
    }
}
