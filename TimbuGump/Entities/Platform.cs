using Caieta;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using TimbuGump.Entities.Obstacles;
using TimbuGump.Entities.Sprites;

namespace TimbuGump.Entities
{
    public class Platform : Body
    {
        //private readonly float speed = 1f;
        private readonly float speed = 0;
        //private List<GuyOnStairs> guys = new List<GuyOnStairs>();
        private GuyOnStairs guyOnStairs;

        public Platform() : base(Vector2.Zero, sprite: GetSprite())
        {
            MoveTo(new Vector2(0, Global.ScreenHeight * .5f));
            guyOnStairs = new GuyOnStairs(Vector2.Zero);
            guyOnStairs.MoveTo(new Vector2(300, Position.Y - guyOnStairs.Height));
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
        }

        private static Sprite GetSprite()
        {
            int[] possibleWidths = new int[] { 200, 400, 600, 800 };
            //return new Sprite(Global.PlatformTexture, new Rectangle(0, 0, possibleWidths[Calc.Random(possibleWidths.Length)], 5));
            return new Sprite(Global.PlatformTexture, new Rectangle(0, 0, possibleWidths[3], 5));
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
            MoveAndSlide(new Vector2(-speed * Scale, 0));
            guyOnStairs?.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            guyOnStairs?.Draw(spriteBatch);
        }
    }
}
