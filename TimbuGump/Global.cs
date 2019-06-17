using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TimbuGump
{
    public static class Global
    {
        public enum HorizontalDirection { Left, Right }
        public enum VerticalDirection { Up, Down }
        public static float ScreenScale { get; set; }
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static Texture2D PlatformTexture { get; set; }
    }
}
