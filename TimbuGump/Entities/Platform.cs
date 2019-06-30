using Caieta;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using TimbuGump.Entities.Obstacles;
using TimbuGump.Entities.Sprites;
using TimbuGump.Helpers;
using TimbuGump.Scenes;

namespace TimbuGump.Entities
{
    public class Platform : Body
    {
        private readonly float speed = 4f;
        //private List<GuyOnStairs> guys = new List<GuyOnStairs>();
        public GuyOnStairs GuyOnStairs { get; private set; }
        public bool HasGuy => GuyOnStairs != null;

        public Platform() : base(Vector2.Zero, sprite: GetSprite(true))
        {
            MoveTo(new Vector2(0, Global.ScreenHeight * .5f));
        }

        public Platform(Platform lastPlatform) : base(Vector2.Zero, sprite: GetSprite())
        {
            int[] directions = new int[] { -2, -1, 0, 1, 2 };

            if (lastPlatform.Position.Y <= 100)
                directions = new int[] { -1, -2, 0 };
            else if (lastPlatform.Position.Y >= (Global.ScreenHeight * .5) + 100)
                directions = new int[] { 0, 1, 2 };

            int direction = directions[Calc.Random(directions.Length)];

            Vector2 position;

            if (direction < 0)
                position = GetPositionDown(lastPlatform);
            else if (direction > 0)
                position = GetPositionUp(lastPlatform);
            else
                position = GetPositionMiddle(lastPlatform);

            MoveTo(position);
            GenerateGuy();
        }

        private void GenerateGuy()
        {
            if (Calc.Random(2) == 0)
                return;
            
            GuyOnStairs = new GuyOnStairs(Vector2.Zero);
            GuyOnStairs.MoveTo(
                new Vector2(Position.X + Calc.Random(65, Sprite.Width - 81), Position.Y - GuyOnStairs.Height));
        }

        private static Sprite GetSprite(bool isFirst = false)
        {
            int width = 800;

            if (!isFirst)
            {
                int[] possibleWidths = new int[] { 200, 400, 600, 800 };
                width = possibleWidths[Calc.Random(possibleWidths.Length)];
            }
            
            return new Sprite(Global.PlatformTexture, new Rectangle(0, 0, width, 5));
        }

        private Vector2 GetPositionUp(Platform lastPlatform)
        {
            int[] possibleVerticalDistances = new int[] { 30, 40, 50, 60 };
            int verticalDistanceIndex = Calc.Random(possibleVerticalDistances.Length);
            verticalDistanceIndex = 0;
            int verticalDistance = possibleVerticalDistances[verticalDistanceIndex];

            if (lastPlatform.Position.Y - verticalDistance <= 100)
            {
                verticalDistanceIndex = 0;
                verticalDistance = 0;
            }

            int[] possibleHorizontalDistances = new int[] { 100, 110, 120, 130, 140, 150, 160 };
            int horizontalDistanceMaximumIndex = possibleHorizontalDistances.Length - possibleVerticalDistances.Length +
                (possibleVerticalDistances.Length - MathHelper.Clamp(verticalDistanceIndex - 1, 0, possibleVerticalDistances.Length));
            int horizontalDistance = possibleHorizontalDistances[Calc.Random(horizontalDistanceMaximumIndex)];
            float horizontalPosition = lastPlatform.Position.X + lastPlatform.Width + horizontalDistance;

            return new Vector2(horizontalPosition, lastPlatform.Position.Y - verticalDistance);
        }

        private Vector2 GetPositionMiddle(Platform lastPlatform)
        {
            int[] possibleHorizontalDistances = new int[] { 100, 110, 120, 130, 140, 150, 160 };
            int horizontalDistance = possibleHorizontalDistances[Calc.Random(possibleHorizontalDistances.Length)];
            float horizontalPosition = lastPlatform.Position.X + lastPlatform.Width + horizontalDistance;

            return new Vector2(horizontalPosition, lastPlatform.Position.Y);
        }

        private Vector2 GetPositionDown(Platform lastPlatform)
        {
            int[] possibleVerticalDistances = new int[] { 30, 40, 50, 60, 70, 80, 90, 100 };
            int verticalDistanceIndex = Calc.Random(possibleVerticalDistances.Length);
            verticalDistanceIndex = 0;
            int verticalDistance = possibleVerticalDistances[verticalDistanceIndex];
            float verticalPosition = lastPlatform.Position.Y + lastPlatform.Height + verticalDistance;

            if (verticalPosition >= (Global.ScreenHeight * .5) + 100)
                verticalPosition = (Global.ScreenHeight * .5f) + 100;

            int[] possibleHorizontalDistances = new int[] { 100, 110, 120, 130, 140, 150, 160, 170 };
            int horizontalDistance = possibleHorizontalDistances[Calc.Random(possibleHorizontalDistances.Length)];
            float horizontalPosition = lastPlatform.Position.X + lastPlatform.Width + horizontalDistance;

            return new Vector2(horizontalPosition, verticalPosition);
        }

        public override void Update(GameTime gameTime)
        {
            bool isMoving = (SceneManager.CurrentScene as World).Player != null;

            if (isMoving && Position.X + Width >= 0)
                MoveAndSlide(new Vector2(-speed * Scale, 0));

            if (HasGuy)
            {
                if (!GuyOnStairs.HasBeenDestroyed && isMoving)
                    GuyOnStairs.MoveAndSlide(new Vector2(-speed * Scale, 0), false);

                GuyOnStairs.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            GuyOnStairs?.Draw(spriteBatch);
        }
    }
}
