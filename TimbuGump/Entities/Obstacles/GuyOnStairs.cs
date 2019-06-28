using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;

namespace TimbuGump.Entities.Obstacles
{
    public class GuyOnStairs : Body
    {
        Stair stair;
        Guy guy;
        Stick stick;
        Body fullBody;

        public override int Height => fullBody?.Height ?? (int)((stair.Position.Y + stair.Height - guy.Position.Y) * stair.Scale);
        public override int Width => fullBody?.Width ?? (int)((stair.Position.X + stair.Width - guy.Position.X) * stair.Scale);

        public GuyOnStairs(Vector2 position) : base(position)
        {
            fullBody = new Body(position, sprite: GetAnimationDefault(), scale: 5f);
            fullBody.AddHitArea("destroy_area",
                new Rectangle(
                    (int)(-2 * fullBody.Scale),
                    (int)(10 * fullBody.Scale),
                    (int)(5 * fullBody.Scale),
                    (int)(6 * fullBody.Scale)
                    ));

            fullBody.AddHitArea("catch_area",
                new Rectangle(
                    (int)(5 * fullBody.Scale),
                    (int)(11 * fullBody.Scale),
                    (int)(10 * fullBody.Scale),
                    (int)(5 * fullBody.Scale)
                    ));
        }

        private static Sprite GetAnimationDefault()
        {
            Texture2D spriteSheet = Loader.LoadTexture("guy_on_stairs");
            Dictionary<string, Frame[]> animationFrames = new Dictionary<string, Frame[]>();
            animationFrames.Add("idle", new Frame[]
            {
                new Frame() { Name = "idle_1", Source = new Rectangle(16, 0, 12, 16), Duration = 100 }
            });

            animationFrames.Add("catch", new Frame[]
            {
                new Frame() { Name = "catch_1", Source = new Rectangle(32, 0, 16, 16), Duration = 100 },
                new Frame() { Name = "catch_2", Source = new Rectangle(48, 0, 16, 16), Duration = 100 },
                new Frame() { Name = "catch_3", Source = new Rectangle(64, 0, 16, 16), Duration = 100 },
                new Frame() { Name = "catch_4", Source = new Rectangle(80, 0, 16, 16), Duration = 100 },
                new Frame() { Name = "catch_5", Source = new Rectangle(96, 0, 16, 16), Duration = 100 },
                new Frame() { Name = "catch_6", Source = new Rectangle(112, 0, 16, 16), Duration = 100 }
            });

            return new AnimatedSprite(spriteSheet, animationFrames);
        }

        private void SplitBodies()
        {
            stair = new Stair(Vector2.Zero);
            guy = new Guy(Vector2.Zero);
            stick = new Stick(Vector2.Zero);

            float platformVerticalPosition = fullBody.Position.Y + fullBody.Height;

            stair.MoveTo(new Vector2(fullBody.Position.X, platformVerticalPosition - stair.Height));
            guy.MoveTo(new Vector2(stair.Position.X + stair.Width - (1 * guy.Scale), stair.Position.Y - (5 * guy.Scale)));
            stick.MoveTo(new Vector2(guy.Position.X + guy.Width, stair.Position.Y));

            stair.SetOrigin(.5f);
            guy.SetOrigin(.5f);
            stick.SetOrigin(.5f);

            stair.Spin(Global.HorizontalDirection.Left, 2f);
            guy.Spin(Global.HorizontalDirection.Right);
            stick.Spin(Global.HorizontalDirection.Left, 7f);

            fullBody = null;
        }

        public override void MoveTo(Vector2 position, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            if (fullBody != null)
                fullBody.MoveTo(position, setFacingDirection, keepOnScreenBounds);
            else
                MoveSplitEntities(position);
        }

        private void MoveSplitEntities(Vector2 position, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            stair?.MoveTo(position, setFacingDirection, keepOnScreenBounds);
            guy?.MoveTo(position, setFacingDirection, keepOnScreenBounds);
            stick?.MoveTo(position, setFacingDirection, keepOnScreenBounds);
        }

        public override void Update(GameTime gameTime)
        {
            if (fullBody != null)
                fullBody.Update(gameTime);
            else
                UpdateSplitEntities(gameTime);
        }

        private void UpdateSplitEntities(GameTime gameTime)
        {
            stair.Update(gameTime);
            guy.Update(gameTime);
            stick.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (fullBody != null)
                fullBody.Draw(spriteBatch);
            else
                DrawSplitEntities(spriteBatch);
        }

        private void DrawSplitEntities(SpriteBatch spriteBatch)
        {
            stair.Draw(spriteBatch);
            guy.Draw(spriteBatch);
            stick.Draw(spriteBatch);
        }
    }
}
