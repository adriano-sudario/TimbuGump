using Microsoft.Xna.Framework;

namespace TimbuGump.Interfaces
{
    public interface IInput
    {
        void Update();
        Vector2 DirectionalPressing();
        bool SpaceJustPressed();
        bool UpJustPressed();
        bool EnterJustPressed();
        bool BackspaceJustPressed();
    }
}
