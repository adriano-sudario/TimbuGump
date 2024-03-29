﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TimbuGump.Interfaces;

namespace TimbuGump.Inputs
{
    public class KeyboardInput : IInput
    {
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        Keys leftKey;
        Keys rightKey;
        Keys upKey;
        Keys downKey;
        Keys spaceKey;
        Keys backspaceKey;

        public KeyboardInput()
        {
            SetDefaultKeysByIndex();
        }

        public void SetDefaultKeysByIndex()
        {
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            upKey = Keys.Up;
            downKey = Keys.Down;
            spaceKey = Keys.Space;
            backspaceKey = Keys.Back;
        }

        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public Vector2 DirectionalPressing()
        {
            float x = currentKeyboardState.IsKeyDown(leftKey) ? -1 : currentKeyboardState.IsKeyDown(rightKey) ? 1 : 0;
            float y = currentKeyboardState.IsKeyDown(upKey) ? -1 : currentKeyboardState.IsKeyDown(downKey) ? 1 : 0;
            return new Vector2(x, y);
        }

        public bool EnterJustPressed()
        {
            return previousKeyboardState.IsKeyUp(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter);
        }

        public bool BackspaceJustPressed()
        {
            return previousKeyboardState.IsKeyUp(backspaceKey) && currentKeyboardState.IsKeyDown(backspaceKey);
        }

        public bool SpaceJustPressed()
        {
            return previousKeyboardState.IsKeyUp(spaceKey) && currentKeyboardState.IsKeyDown(spaceKey);
        }

        public bool UpJustPressed()
        {
            return previousKeyboardState.IsKeyUp(upKey) && currentKeyboardState.IsKeyDown(upKey);
        }
    }
}
