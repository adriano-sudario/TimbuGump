using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimbuGump.Abstracts;
using TimbuGump.Entities.Sprites;
using TimbuGump.Manipulators;
using System;
using static TimbuGump.Global;
using System.Collections.Generic;
using System.Diagnostics;

namespace TimbuGump.Entities
{
    public class Body : Cyclic
    {
        private Rectangle? customCollision;
        private Dictionary<string, Rectangle> hitAreas = new Dictionary<string, Rectangle>();

        protected bool isActive = true;

        private Fade fade = null;
        private Spin spin = null;
        private Size size = null;
        private Vector2 position;
        private float scale;

        public Sprite Sprite { get; private set; }

        public float Scale
        {
            get { return Sprite?.Scale ?? scale; }
            set
            {
                scale = value;
                if (Sprite != null)
                    Sprite.Scale = scale;
            }
        }

        public HorizontalDirection FacingDirection { get; set; }

        public virtual Vector2 Position
        {
            get { return Sprite?.Position ?? position; }
            protected set
            {
                position = value;
                if (Sprite != null)
                    Sprite.Position = position;
            }
        }

        public Rectangle Collision
        {
            get
            {
                Vector2 spriteSource = (Sprite?.Origin ?? Vector2.Zero) * (Scale * ScreenScale);
                Rectangle collision = new Rectangle((int)(Position.X - spriteSource.X), (int)(Position.Y - spriteSource.Y), Width, Height);

                if (customCollision != null)
                {
                    collision.X += customCollision.Value.X;
                    collision.Y += customCollision.Value.Y;
                    collision.Width = (int)(customCollision.Value.Width * (Scale * ScreenScale));
                    collision.Height = (int)(customCollision.Value.Height * (Scale * ScreenScale));
                }

                return collision;
            }
        }

        public float ScaleDefault { get; private set; }
        public virtual int Width { get { return (int)((Sprite?.Width ?? 0) * (Scale * ScreenScale)); } }
        public virtual int Height { get { return (int)((Sprite?.Height ?? 0) * (Scale * ScreenScale)); } }
        public bool IsFading { get { return fade != null; } }
        public bool IsSpinning { get; private set; }
        public bool IsVisible { get; set; }

        public Body(Vector2 position, Sprite sprite = null, HorizontalDirection facingDirection = HorizontalDirection.Right,
            float scale = 1f, Rectangle? customCollision = null)
        {
            IsVisible = true;
            Sprite = sprite;
            FacingDirection = facingDirection;
            Scale = scale;
            ScaleDefault = scale;
            this.customCollision = customCollision;
            MoveTo(position);
        }

        public void ReplaceSprite(Sprite sprite, Rectangle? customCollision = null)
        {
            Sprite = sprite;
            this.customCollision = customCollision;
        }

        private void UpdateHitAreas()
        {
            //for (int i = 0; i < hitAreas.Count; i++)
            //    hitAreas[hitAreas..Key] = new Rectangle(
            //        area.Value.X + (int)Position.X,
            //        area.Value.X + (int)Position.X,
            //        area.Value.Width,
            //        area.Value.Height);

            KeyValuePair<string, Rectangle>[] entries = new KeyValuePair<string, Rectangle>[hitAreas.Count];

            //foreach (KeyValuePair<string, Rectangle> area in hitAreas)
            //    hitAreas[area.Key] = new Rectangle(
            //        area.Value.X + (int)Position.X, 
            //        area.Value.X + (int)Position.X, 
            //        area.Value.Width, 
            //        area.Value.Height);

            int index = 0;

            foreach (KeyValuePair<string, Rectangle> area in hitAreas)
            {
                entries[index] = new KeyValuePair<string, Rectangle>(area.Key, new Rectangle(
                    area.Value.X + (int)Position.X,
                    area.Value.X + (int)Position.X,
                    area.Value.Width,
                    area.Value.Height));
                index++;
            }

            for (int i = 0; i < entries.Length; i++)
                hitAreas[entries[i].Key] = entries[i].Value;
        }

        public virtual void MoveTo(Vector2 position, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            if (Position.X != position.X && setFacingDirection)
            {
                float horizontalDifference = position.X - Position.X;
                FacingDirection = horizontalDifference < 0 ? HorizontalDirection.Left : HorizontalDirection.Right;
            }

            if (keepOnScreenBounds)
            {
                position.X = MathHelper.Clamp(position.X, 0, ScreenWidth - Width);
                position.Y = MathHelper.Clamp(position.Y, 0, ScreenHeight - Height);
            }
            
            Position = position;
            UpdateHitAreas();
        }

        public void MoveTo(int x, int y, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            MoveTo(new Vector2(x, y), setFacingDirection, keepOnScreenBounds);
        }

        public void MoveHorizontally(int x, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            MoveTo(new Vector2(x, Position.Y), setFacingDirection, keepOnScreenBounds);
        }

        public void MoveVertically(int y, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            MoveTo(new Vector2(Position.X, y), setFacingDirection, keepOnScreenBounds);
        }

        public void MoveAndSlide(int x, int y, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            MoveTo(new Vector2(Position.X + x, Position.Y + y), setFacingDirection, keepOnScreenBounds);
        }

        public void MoveAndSlide(Vector2 position, bool setFacingDirection = true, bool keepOnScreenBounds = false)
        {
            if (position != Vector2.Zero)
                MoveTo(Position + position, setFacingDirection, keepOnScreenBounds);
        }

        public void SetOrigin(float origin, bool keepInPlace = true)
        {
            float totalScale = (Scale * ScreenScale);
            Vector2 updatedOrigin = origin == 0 ? Vector2.Zero : new Vector2((Width * origin) / totalScale, (Height * origin) / totalScale);

            if (keepInPlace)
                MoveAndSlide((updatedOrigin * totalScale) - (Sprite.Origin * totalScale), false);

            Sprite.Origin = updatedOrigin;
        }

        public void SetOrigin(Vector2 origin, bool keepInPlace = true)
        {
            if (Sprite == null)
                return;

            float totalScale = (Scale * ScreenScale);
            Sprite.Origin = new Vector2((Width * origin.X) / totalScale, (Height * origin.Y) / totalScale) * -1;

            if (keepInPlace)
                MoveAndSlide(Sprite.Origin * totalScale);
        }

        public void FadeIn(float amount = .01f, EventHandler onFadeEnded = null)
        {
            Fade(Math.Abs(amount), 0, 1, onFadeEnded);
        }

        public void FadeOut(float amount = .01f, EventHandler onFadeEnded = null)
        {
            Fade(-Math.Abs(amount), 1, 0, onFadeEnded);
        }

        public void Fade(float amount, float from, float to, EventHandler onFadeEnded = null)
        {
            fade = new Fade(Sprite, amount, from, to, (sender, e) =>
            {
                StopFade();
                onFadeEnded?.Invoke(sender, EventArgs.Empty);
            });
        }

        public void StopFade()
        {
            fade = null;
        }

        public void StopSpin()
        {
            spin = null;
        }

        public void StopResize()
        {
            size = null;
        }

        public void Spin(HorizontalDirection direction, float amount = 5, bool autoSpin = true, EventHandler onCicleCompleted = null)
        {
            spin = new Spin(Sprite, amount, direction, autoSpin, onCicleCompleted);
        }

        public void Spin(float amount, bool autoSpin = true, EventHandler onCicleCompleted = null)
        {
            spin = new Spin(Sprite, amount, autoSpin, onCicleCompleted);
        }

        public void Grow(float amount = .01f, float percent = 100, bool goBackToOriginalSizeOnEnded = true, EventHandler onResizeEnded = null)
        {
            size = new Size(this);
            size.Grow(amount, percent, (sender, e) =>
            {
                StopResize();
                onResizeEnded?.Invoke(sender, EventArgs.Empty);
            });
        }

        public void Shrink(float amount = .01f, float percent = 100, EventHandler onResizeEnded = null)
        {
            size = new Size(this);
            size.Shrink(amount, percent, (sender, e) =>
            {
                size = null;
                onResizeEnded?.Invoke(sender, EventArgs.Empty);
            });
        }

        public void CustomizeCollision(Rectangle collision)
        {
            customCollision = collision;
        }

        public void ResetCollisionToSpriteBounds()
        {
            customCollision = null;
        }

        public void AddHitArea(string key, Rectangle area)
        {
            UpdateHitAreas();
            hitAreas.Add(key, area);
        }

        public bool CollidesWith(Body body)
        {
            return Collision.Intersects(body.Collision);
        }

        public bool Collides(string hitAreaKey, Rectangle area)
        {
            return hitAreas[hitAreaKey].Intersects(area);
        }

        public bool Collides(string hitAreaKey, Body body, string bodyHitAreaKey)
        {
            return hitAreas[hitAreaKey].Intersects(body.GetHitArea(bodyHitAreaKey));
        }

        public Rectangle GetHitArea(string key)
        {
            return hitAreas[key];
        }

        protected void UpdateSize(GameTime gameTime)
        {
            size?.Update(gameTime);
        }

        protected void UpdateFading(GameTime gameTime)
        {
            fade?.Update(gameTime);
        }

        protected void UpdateAnimation(GameTime gameTime)
        {
            if (Sprite != null && Sprite.GetType() == typeof(AnimatedSprite))
                (Sprite as AnimatedSprite).Update(gameTime);
        }

        protected void UpdateSpinning(GameTime gameTime)
        {
            spin?.Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!isActive)
                return;

            UpdateSize(gameTime);
            UpdateSpinning(gameTime);
            UpdateFading(gameTime);
            UpdateAnimation(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isActive || !IsVisible)
                return;

            Sprite?.Draw(spriteBatch, effect: FacingDirection == HorizontalDirection.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (Debugger.IsAttached)
            {
                Texture2D debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                debugTexture.SetData(new[] { new Color(154, 0, 0, 50) });

                if (hitAreas.Count > 0)
                    foreach (KeyValuePair<string, Rectangle> entry in hitAreas)
                        spriteBatch.Draw(debugTexture, entry.Value, Color.Red);
                else
                    spriteBatch.Draw(debugTexture, Collision, Color.Red);
            }
        }
    }
}
