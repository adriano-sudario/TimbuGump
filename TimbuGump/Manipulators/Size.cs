﻿using Microsoft.Xna.Framework;
using TimbuGump.Entities;
using System;

namespace TimbuGump.Manipulators
{
    public class Size
    {
        private float multiplier;
        private float amount;
        private float limit;
        private float startingScale;
        private Body affectedBody;

        private event EventHandler onResizeEnded;

        public bool IsResizing { get; private set; }

        public Size(Body body)
        {
            affectedBody = body;
            startingScale = affectedBody.Scale;
            IsResizing = false;
        }

        public void Grow(float amount = .01f, float percent = 100, EventHandler onResizeEnded = null)
        {
            Resize(Math.Abs(amount), Math.Abs(MathHelper.Clamp(percent, 0, 100)), onResizeEnded);
        }

        public void Shrink(float amount = .01f, float percent = 100, EventHandler onResizeEnded = null)
        {
            Resize(-Math.Abs(amount), -(Math.Abs(MathHelper.Clamp(percent, 0, 100))), onResizeEnded);
        }

        private void Resize(float amount, float percent, EventHandler onResizeEnded)
        {
            multiplier = startingScale / affectedBody.ScaleDefault;
            limit = multiplier + (percent / 100);
            this.amount = amount;
            this.onResizeEnded = onResizeEnded;
            IsResizing = true;
        }

        public void ReturnToStartingSize()
        {
            affectedBody.Scale = startingScale;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsResizing)
                return;

            multiplier += amount;
            affectedBody.Scale = affectedBody.ScaleDefault * multiplier;

            if ((Math.Sign(amount) < 0 && multiplier <= limit) || (Math.Sign(amount) > 0 && multiplier >= limit))
            {
                IsResizing = false;
                affectedBody.Scale = limit * affectedBody.ScaleDefault;
                onResizeEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
