using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TimbuGump.Abstracts;
using TimbuGump.Entities;
using TimbuGump.Helpers;
using TimbuGump.Inputs;
using TimbuGump.Interfaces;
using TimbuGump.Sounds;

namespace TimbuGump.Scenes
{
    public class World : Cyclic
    {
        public List<Platform> Platforms { get; private set; }
        public Timbu Player { get; set; }
        public float Gravity { get; private set; } = 0.2f;
        public float Score { get; private set; }

        private readonly IInput input = new KeyboardInput();
        private readonly float maxGravityForce = 4.9f;
        private readonly float scoreIncrement = .2f;
        private string deathText;
        private Vector2 textPosition;

        public float ElapsedTime { get; set; }

        public void Initialize()
        {
            if (!SoundTrack.IsPlaying)
            {
                SoundTrack.Load(Loader.LoadSound("Soundtrack\\timbu_beat"));
                SoundTrack.Play();
            }
            Sfx.Load("pulo", Loader.LoadSound("SFX\\pulando"));
            Sfx.Load("pegar_ar", Loader.LoadSound("SFX\\pegando_ar"));
            Sfx.Load("afulibar_escada", Loader.LoadSound("SFX\\afulibando_escada"));
            Sfx.Load("rodada", Loader.LoadSound("SFX\\rodando"));

            Platforms = new List<Platform>();
            Platforms.Add(new Platform());
            AddPlatforms();
            Player = new Timbu(Vector2.Zero);
            Player.MoveTo(new Vector2(50, Player.Position.Y));
            StepPlayerOnPlatform(Platforms.First());
            Score = 0;
            ElapsedTime = 0;
            deathText = "";
        }

        private void SetDeathText(string text)
        {
            deathText = text;
            Vector2 textSize = Global.Monogram.MeasureString(text) * 2;
            textPosition = new Vector2(
                (Global.ScreenWidth * .5f) - (textSize.X * .5f),
                (Global.ScreenHeight * .8f) - (textSize.Y * .5f));
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            if (Player == null && input.BackspaceJustPressed())
                Initialize();

            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            foreach (Platform platform in Platforms)
                platform.Update(gameTime);

            UpdatePlayer(gameTime);
            RebuildPlatforms();

            if (Player != null)
                Score += scoreIncrement;
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            if (Player == null)
                return;

            Player.Update(gameTime);

            if (Player.Ground == null)
            {
                Player.ForceApplied += Gravity;

                if (Player.ForceApplied > maxGravityForce)
                    Player.ForceApplied = maxGravityForce;

                Player.MoveAndSlide(new Vector2(0, Player.ForceApplied));

                if (Player.Position.Y + Player.Height > Global.ScreenHeight)
                {
                    KillPlayer(false);
                    return;
                }
            }

            if (Player.Ground != null && !Player.Collides("foot_area", Player.Ground))
                Player.Ground = null;

            foreach (Platform platform in Platforms)
            {
                if (platform.Position.X > Player.Position.X + platform.Width)
                    break;

                if (platform.HasGuy)
                {
                    if (Player.HasTookAir &&
                        Player.Collides("front_area", platform.GuyOnStairs, "destroy_area"))
                        platform.GuyOnStairs.Destroy();
                    else if (!Player.IsAmbushed &&
                        Player.Collides("front_area", platform.GuyOnStairs, "catch_area"))
                        platform.GuyOnStairs.Ambush(Player);
                    else if (Player.IsAmbushed && platform.GuyOnStairs.HasAmbushedPlayer &&
                        Player.GetHitArea("front_area").Right >= platform.GuyOnStairs.GetHitArea("catch_area").Right)
                    {
                        platform.GuyOnStairs.CatchPlayer();
                        return;
                    }
                }

                if (platform == Player.Ground)
                    continue;

                if (Player.Collides("foot_area", platform))
                {
                    StepPlayerOnPlatform(platform);
                    break;
                }
            }
        }

        private void StepPlayerOnPlatform(Platform platform)
        {
            Player.Ground = platform;
            Player.ForceApplied = 0;
            Player.MoveTo(new Vector2(Player.Position.X, platform.Position.Y - Player.Height));
        }

        private void RebuildPlatforms()
        {
            CleanPlatforms();
            AddPlatforms();
        }

        public void KillPlayer(bool hasBeenCatched = true)
        {
            Player = null;
            Sfx.Play("rodada");
            SetDeathText(hasBeenCatched ? "you have been safely placed back into the wild" : "you have fallen");
        }

        private void CleanPlatforms()
        {
            Platforms.RemoveAll(p =>
            {
                if (p.HasGuy && p.GuyOnStairs.HasBeenDestroyed)
                    return p.GuyOnStairs.HasFallen && p.Position.X + p.Width < 0;

                return p.Position.X + p.Width < 0;
            });
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

            Player?.Draw(spriteBatch);
            spriteBatch.DrawString(Global.Monogram, $"score: {((int)Score).ToString()}", new Vector2(10, 5), 
                Color.Black, 0, Vector2.Zero, 2, SpriteEffects.None, 0);

            if (!string.IsNullOrEmpty(deathText))
                spriteBatch.DrawString(Global.Monogram, deathText, textPosition,
                Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
        }
    }
}
