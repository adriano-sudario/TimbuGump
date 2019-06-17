using Microsoft.Xna.Framework;
using System;
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
            Random random = new Random();
            int[] possibleVerticalDistances = new int[] { 20, 30, 40, 50 };
            int verticalDistance = possibleVerticalDistances[random.Next(possibleVerticalDistances.Length)];

            int[] possibleHorizontalDistances = new int[(verticalDistance / 10) + 1];
            int minimumHorizontalDistance = 100;

            for (int i = 0; i < possibleHorizontalDistances.Length; i ++)
                possibleHorizontalDistances[i] = minimumHorizontalDistance + (i * 10);

            int horizontalDistance = possibleHorizontalDistances[random.Next(possibleHorizontalDistances.Length)];

            MoveTo(new Vector2(lastPlatform.Position.X + lastPlatform.Width + horizontalDistance, lastPlatform.Position.Y - verticalDistance));
        }

        public Platform(Vector2 position) : base(position, sprite: GetSprite(position), scale: 1f)
        {

        }

        private static Sprite GetSprite(Vector2 position)
        {
            Random random = new Random();
            return new Sprite(Global.PlatformTexture, new Rectangle((int)position.X, (int)position.Y, 400, 5));
        }

        public override void Update(GameTime gameTime)
        {
            MoveAndSlide(new Vector2(-speed, 0));
        }
    }
}
