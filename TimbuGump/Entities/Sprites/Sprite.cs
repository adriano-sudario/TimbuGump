using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimbuGump.Interfaces;
using System;

namespace TimbuGump.Entities.Sprites
{
    public class Sprite : IVisual
    {
        private Texture2D spriteStrip;
        
        public virtual Rectangle Source { get; set; }
        public virtual int Width { get { return Source.Width; } }
        public virtual int Height { get { return Source.Height; } }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float Opacity { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; } = Color.White;

        public Sprite(Texture2D spriteStrip, Rectangle? source = null, float opacity = 1f, Vector2 origin = default(Vector2), float rotation = 0)
        {
            this.spriteStrip = spriteStrip;
            Source = source ?? new Rectangle(0, 0, spriteStrip.Width, spriteStrip.Height);
            Opacity = opacity;
            Origin = origin;
            Rotation = rotation;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects effect = SpriteEffects.None, float layerDepth = 0)
        {
            spriteBatch.Draw(spriteStrip, Position, Source, Color * Opacity, Rotation, Origin, Scale * Global.ScreenScale, effect, layerDepth);
        }
    }
}
