using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimbuGump.Abstracts;
using TimbuGump.Helpers;
using TimbuGump.Inputs;
using TimbuGump.Interfaces;

namespace TimbuGump.Scenes
{
    public class Opening : Cyclic
    {
        private readonly IInput input = new KeyboardInput();
        private readonly string text = "PRESS START TO RUN";
        private Vector2 position;
        private float scale = 2f;

        public Opening()
        {
            Vector2 textSize = Global.Monogram.MeasureString(text) * scale;
            position = new Vector2(
                (Global.ScreenWidth * .5f) - (textSize.X * .5f), 
                (Global.ScreenHeight * .5f) - (textSize.Y * .5f));
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            if (input.EnterJustPressed())
            {
                SceneManager.ChangeScene("World");
                (SceneManager.CurrentScene as World).Initialize();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            spriteBatch.DrawString(Global.Monogram, text, position,
                Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
