using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TimbuGump.Abstracts;
using TimbuGump.Entities;
using TimbuGump.Helpers;
using TimbuGump.Manipulators;
using TimbuGump.Sounds;

namespace TimbuGump.Scenes
{
    public class World : Cyclic
    {
        public List<Platform> Platforms { get; private set; }
        public Timbu Player { get; private set; }

        private float gravity = 0.2f;
        private readonly float maxGravityForce = 4.9f;

        public float ElapsedTime { get; set; }

        public World()
        {
            Initialize();
        }

        private void Initialize()
        {
            SoundTrack.Load(Loader.LoadSound("Soundtrack\\timbu_beat"));
            SoundTrack.Play();
            Sfx.Load("pulo", Loader.LoadSound("SFX\\pulando"));

            Platforms = new List<Platform>();
            Platforms.Add(new Platform());
            // AddPlatforms();
            Player = new Timbu(Vector2.Zero);
            Player.MoveTo(new Vector2(25, Platforms.First().Position.Y - Player.Height));
        }

        public override void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            foreach (Platform platform in Platforms)
                platform.Update(gameTime);

            Player.Update(gameTime);

            if (Player.Ground == null)
            {
                Player.ForceApplied += gravity;

                if (Player.ForceApplied > maxGravityForce)
                    Player.ForceApplied = maxGravityForce;

                Player.MoveAndSlide(new Vector2(0, Player.ForceApplied));
            }

            if (Player.ForceApplied >= 0)
            {
                if (Player.Ground != null && !Player.CollidesWith(Player.Ground))
                    Player.Ground = null;

                foreach (Platform platform in Platforms)
                {
                    if (platform == Player.Ground)
                        continue;

                    if (Player.CollidesWith(platform))
                    {
                        Player.Ground = platform;
                        Player.MoveTo(new Vector2(Player.Position.X, platform.Position.Y - Player.Height));
                        break;
                    }
                }
            }

            UpdatePlatforms();
        }

        private void UpdatePlatforms()
        {
            CleanPlatforms();
            AddPlatforms();
        }

        private void CleanPlatforms()
        {
            int platformsToRemove = 0;

            foreach (Platform platform in Platforms)
            {
                if (platform.Position.X + platform.Width < 0)
                    platformsToRemove++;
                else
                    break;
            }

            if (platformsToRemove > 0)
                Platforms.RemoveRange(0, platformsToRemove);
        }

        private void AddPlatforms()
        {
            Platform lastPlatform = Platforms.Last();

            while (lastPlatform.Position.X + lastPlatform.Width < Global.ScreenWidth * 2)
            {
                Platform newPlatform = new Platform(lastPlatform);
                Platforms.Add(newPlatform);
                lastPlatform = newPlatform;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            DrawEntities(spriteBatch);

            spriteBatch.End();
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (Platform platform in Platforms)
                platform.Draw(spriteBatch);

            Player.Draw(spriteBatch);
        }
    }
}
