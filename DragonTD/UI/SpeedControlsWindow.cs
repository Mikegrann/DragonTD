using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.UI
{
    partial class UI
    {
        class SpeedControlsWindow : Window
        {
            //pixels per second
            float ScrollOffscreenSpeed = 128f;

            Button PausePlayButton, FastForwardButton;

            public SpeedControlsWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                // Pause/Play Button
                Texture2D pauseTexture = game.Content.Load<Texture2D>("textures/ui/buttons/pause");
                Point playPoint = new Point(10, 0);
                PausePlayButton = new Button("pausePlay", game, this, pauseTexture, pauseTexture, pauseTexture, pauseTexture,
                    new Rectangle(playPoint, new Point(pauseTexture.Width, pauseTexture.Height)), null);

                PausePlayButton.OnHover += PausePlayButton_OnHover;
                PausePlayButton.OnClick += PausePlayButton_OnClick;
                PausePlayButton.OnLeave += PausePlayButton_OnLeave;

                components.Add(PausePlayButton);

                // FastForward Button
                Texture2D forwardTexture = game.Content.Load<Texture2D>("textures/ui/buttons/fastforward");
                Point forwardPoint = new Point(10, 31);
                FastForwardButton = new Button("pausePlay", game, this, forwardTexture, forwardTexture, forwardTexture, forwardTexture,
                    new Rectangle(forwardPoint, new Point(forwardTexture.Width, forwardTexture.Height)), null);

                FastForwardButton.OnHover += FastForwardButton_OnHover;
                FastForwardButton.OnClick += FastForwardButton_OnClick;
                FastForwardButton.OnLeave += FastForwardButton_OnLeave;

                components.Add(FastForwardButton);
            }

            public override void Update(GameTime gameTime)
            {
                if (Enabled)
                {
                    if (offsetLocation.X > 0)
                        offsetLocation.X -= ScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (offsetLocation.X < 0)
                        offsetLocation.X = 0;
                    base.Update(gameTime);
                }
                else
                {
                    if (offsetLocation.X < bounds.Width)
                        offsetLocation.X += ScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (offsetLocation.X > bounds.Width)
                        offsetLocation.X = bounds.Width;
                }
            }

            private void PausePlayButton_OnLeave(Button sender)
            {
                Console.WriteLine("button leave " + sender.Name);
            }

            private void PausePlayButton_OnClick(Button sender)
            {
                Console.WriteLine("button click " + sender.Name);

                if (ui.level.SimSpeed > 0.0f)
                {
                    ui.level.SimSpeed = 0.0f; // Pause

                    FastForwardButton.Enabled = false;

                    Texture2D playTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/play");
                    sender.SetTextures(playTexture, playTexture, playTexture, playTexture);

                    Texture2D forwardTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/fastforward");
                    FastForwardButton.SetTextures(forwardTexture, forwardTexture, forwardTexture, forwardTexture);
                }
                else
                {
                    ui.level.SimSpeed = 1.0f; // Play at normal speed

                    FastForwardButton.Enabled = true;

                    Texture2D pauseTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/pause");
                    sender.SetTextures(pauseTexture, pauseTexture, pauseTexture, pauseTexture);
                }
            }

            private void PausePlayButton_OnHover(Button sender)
            {
                Console.WriteLine("buton hobver " + sender.Name);
            }

            private void FastForwardButton_OnLeave(Button sender)
            {
                Console.WriteLine("button leave " + sender.Name);
            }

            private void FastForwardButton_OnClick(Button sender)
            {
                Console.WriteLine("button click " + sender.Name);

                if (ui.level.SimSpeed == 1.0f)
                {
                    ui.level.SimSpeed = 2.0f; // Fast Forward

                    Texture2D playTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/play");
                    sender.SetTextures(playTexture, playTexture, playTexture, playTexture);

                    Texture2D pauseTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/pause");
                    PausePlayButton.SetTextures(pauseTexture, pauseTexture, pauseTexture, pauseTexture);
                }
                else
                {
                    ui.level.SimSpeed = 1.0f; // Play at normal speed

                    Texture2D forwardTexture = Game.Content.Load<Texture2D>("textures/ui/buttons/fastforward");
                    sender.SetTextures(forwardTexture, forwardTexture, forwardTexture, forwardTexture);
                }
            }

            private void FastForwardButton_OnHover(Button sender)
            {
                Console.WriteLine("buton hobver " + sender.Name);
            }
        }
    }
}
