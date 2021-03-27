using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;
using TimbuGump.Inputs;
using TimbuGump.Interfaces;
using TimbuGump.Scenes;
using TimbuGump.Sounds;

namespace TimbuGump.Entities
{
    public class Timbu : Body
    {
        private readonly float jumpForce = 1f;
        private readonly IInput input = new KeyboardInput();
        private readonly float takeAirTime = 600;
        private readonly float takeAirCooldown = 1000;

        private bool canTakeAir = true;
        private float takeAirElapsedTime = 0;

        public bool HasTookAir { get; private set; } = false;
        public bool IsAmbushed { get; set; } = false;
        public Platform Ground { get; set; }
        public float ForceApplied { get; set; }

        public Timbu(Vector2 position) : base(position, sprite: GetAnimationDefault(), scale: 5f, customCollision: new Rectangle(0, 0, 10, 6))
        {
            AddHitArea("foot_area", 
                new Rectangle((int)(3 * Scale), (int)(5 * Scale), (int)(3 * Scale), (int)(2 * Scale)));
            AddHitArea("front_area", 
                new Rectangle((int)(9 * Scale), (int)(1 * Scale), (int)(1 * Scale), (int)(5 * Scale)));
        }

        protected static AnimatedSprite GetAnimationDefault()
        {
            Texture2D spriteSheet = Loader.LoadTexture("8-bit_timbas");
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            
            animationFrames.Add("Run", new Frame[]
            {
                new Frame() { Name = "run_1", Source = new Rectangle(0, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "run_2", Source = new Rectangle(10, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "run_3", Source = new Rectangle(20, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "run_4", Source = new Rectangle(30, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "run_5", Source = new Rectangle(20, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "run_6", Source = new Rectangle(10, 0, 10, 6), Duration = 50 }
            });

            animationFrames.Add("TakeAir", new Frame[]
            {
                new Frame() { Name = "take_air_1", Source = new Rectangle(20, 0, 10, 6), Duration = 50 },
                new Frame() { Name = "take_air_2", Source = new Rectangle(40, 0, 10, 6), Duration = 50 },
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsAmbushed)
                return;

            input.Update();

            if (input.UpJustPressed() && Ground != null && ForceApplied >= 0)
            {
                Sfx.Play("pulo");
                Ground = null;
                ForceApplied = jumpForce * -Scale;
            }

            if (!canTakeAir)
            {
                takeAirElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                if (HasTookAir && takeAirElapsedTime >= takeAirTime)
                {
                    HasTookAir = false;
                    (Sprite as AnimatedSprite).Change("Run");
                    (Sprite as AnimatedSprite).JumpToFrame(2);
                }

                if (takeAirElapsedTime >= takeAirCooldown)
                {
                    takeAirElapsedTime = 0;
                    canTakeAir = true;
                }
            }

            if (input.SpaceJustPressed() && canTakeAir)
                TakeAir();
        }

        public void TakeAir()
        {
            canTakeAir = false;
            HasTookAir = true;
            (Sprite as AnimatedSprite).Change("TakeAir");
            Sfx.Play("pegar_ar");
        }
    }
}
