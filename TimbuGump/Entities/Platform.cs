using Microsoft.Xna.Framework;
using System;
using System.Threading;
using TimbuGump.Entities.Sprites;

namespace TimbuGump.Entities
{
    public class Platform : Body
    {
        private readonly float speed = 3f;

        public Platform() : this(Vector2.Zero)
        {
            MoveTo(new Vector2(0, (Global.ScreenHeight * .5f) - 2.5f));
        }

        public Platform(Platform lastPlatform) : this(Vector2.Zero)
        {
            MoveTo(GetPositionUp(lastPlatform));
        }

        public Platform(Vector2 position) : base(position, sprite: GetSprite(position), scale: 1f)
        {

        }

        private static Sprite GetSprite(Vector2 position)
        {
            Random random = new Random();
            return new Sprite(Global.PlatformTexture, new Rectangle((int)position.X, (int)position.Y, 400, 5));
        }

        private Vector2 GetPositionUp(Platform lastPlatform)
        {
            Random random = new Random();
            int[] possibleVerticalDistances = new int[] { 20, 30, 40, 50 };
            int verticalDistanceIndex = random.Next(possibleVerticalDistances.Length);
            verticalDistanceIndex = 0;
            int verticalDistance = possibleVerticalDistances[verticalDistanceIndex];

            if (lastPlatform.Position.Y - verticalDistance <= 100)
            {
                verticalDistanceIndex = 0;
                verticalDistance = 100 - (int)lastPlatform.Position.Y;
            }

            int[] possibleHorizontalDistances = new int[] { 100, 110, 120, 130, 140, 150, 160 };
            int horizontalDistanceMaximumIndex = possibleHorizontalDistances.Length - possibleVerticalDistances.Length + 
                (possibleVerticalDistances.Length - MathHelper.Clamp(verticalDistanceIndex - 1, 0, possibleVerticalDistances.Length));
            int horizontalDistance = possibleHorizontalDistances[random.Next(horizontalDistanceMaximumIndex)];

            return new Vector2(lastPlatform.Position.X + lastPlatform.Width + horizontalDistance, lastPlatform.Position.Y - verticalDistance);
        }

        public override void Update(GameTime gameTime)
        {
            MoveAndSlide(new Vector2(-speed, 0));
        }
    }
}
