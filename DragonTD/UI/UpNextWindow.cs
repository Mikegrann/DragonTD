using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DragonTD.UI
{
    partial class UI
    {
        class UpNextWindow : Window
        {
            public enum WindowAnimState { Moving, Bounce, Paused, Button, ButtonBounce, Wait }

            WindowAnimState animState = WindowAnimState.Paused;
            //pixels per second
            float WindowScrollOffscreenSpeed = 327f;
            float WindowScrollOnscreenSpeed = 654f;

            float ButtonScrollOffscreenSpeed = 180f;
            float ButtonScrollOnscreenSpeed = 90f;

            const float TimeBetweenAnimations = 0.25f;
            float TimerBetweenAnimations = 0;

            Button BeginNextWaveButton;

            public UpNextWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                UIComponent backgroundImage = new UIComponent("background", game, this, game.Content.Load<Texture2D>("Textures/UI/Top Border"), bounds, null);
                //Background = game.Content.Load<Texture2D>("Textures/UI/Top Border");

                BeginNextWaveButton = new Button("NextWave", game, this, game.Content.Load<Texture2D>("Textures/UI/DropButton"), null, null, null, new Rectangle(434, 87, 370, 60), null);

                BeginNextWaveButton.OnClick += BeginNextWaveButton_OnClick;

                components.Add(BeginNextWaveButton);
                components.Add(backgroundImage);//310 12
                components.Add(new EnemyIconList("eil", game, this, new Rectangle(310, 12, 780, 60)));
            }

            public override void Update(GameTime gameTime)
            {
                UpdateAnimation(gameTime);

                if (ui.level.CurrentWaveNumber >= ui.level.WaveCount)
                    BeginNextWaveButton.Enabled = false;
                else
                    BeginNextWaveButton.Enabled = true;

                base.Update(gameTime);
            }

            class EnemyIconList : UIComponent
            {
                EnemyWave[] EnemyDescriptions = new EnemyWave[0];

                Texture2D[] EnemyIcons;
                SpriteFont Font;

                string TexDir = "Textures/UI/UpNextIcons/";
                int waveNumber = -1;

                Point IconSize = new Point(60, 60);

                public EnemyIconList(string name, Game game, Window parent, Rectangle bounds) : base(name, game, parent, null, bounds, null)
                {
                    EnemyIcons = new Texture2D[] { game.Content.Load<Texture2D>(TexDir+"Trash"), game.Content.Load<Texture2D>(TexDir + "Basic"), game.Content.Load<Texture2D>(TexDir + "Flying"), game.Content.Load<Texture2D>(TexDir + "Fast"), game.Content.Load<Texture2D>(TexDir + "Mid"), game.Content.Load<Texture2D>(TexDir + "Heavy"), game.Content.Load<Texture2D>(TexDir + "Buff") };
                    Font = game.Content.Load<SpriteFont>("Fonts/console");
                    EnemyDescriptions = parentWindow.ui.level.NextWave.ToArray();
                }

                public override void Update(GameTime gameTime)
                {
                    if (waveNumber != parentWindow.ui.level.CurrentWaveNumber && !parentWindow.Visible)
                    {
                        if (parentWindow.ui.level.NextWave != null)
                            EnemyDescriptions = parentWindow.ui.level.NextWave.ToArray();
                        else
                            EnemyDescriptions = new EnemyWave[0];
                            waveNumber = parentWindow.ui.level.CurrentWaveNumber;

                    }
                    base.Update(gameTime);
                }

                public override void Draw(GameTime gameTime)
                {
                    for(int i = 0; i < EnemyDescriptions.Length; i++)
                    {
                        Rectangle rect = new Rectangle( new Point(i * 80, 0) + Bounds.Location, IconSize);
                        spriteBatch.Draw(EnemyIcons[(int)EnemyDescriptions[i].Type], rect, Color.White);
                        spriteBatch.DrawString(Font, EnemyDescriptions[i].Count.ToString(), rect.Location.ToVector2(), Color.White);
                    }
                }
            }

            private void UpdateAnimation(GameTime gameTime)
            {
                if (Enabled)
                {
                    if (animState == WindowAnimState.Paused && offsetLocation.Y < 0)
                    {
                        Visible = true;
                        animState = WindowAnimState.Moving;
                        BeginNextWaveButton.offsetLocation = new Vector2(0, -39);
                    }
                    if (animState == WindowAnimState.Moving && offsetLocation.Y > 0)
                        animState = WindowAnimState.Bounce;
                    if (animState == WindowAnimState.Bounce && offsetLocation.Y <= 0)
                    {
                        animState = WindowAnimState.Wait;
                        offsetLocation.Y = 0;
                    }
                    if (animState == WindowAnimState.Wait)
                    {
                        TimerBetweenAnimations += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (TimerBetweenAnimations > TimeBetweenAnimations)
                        {
                            TimerBetweenAnimations = 0;
                            animState = WindowAnimState.Button;
                        }
                    }
                    if (animState == WindowAnimState.Button && BeginNextWaveButton.offsetLocation.Y > 0)
                        animState = WindowAnimState.ButtonBounce;
                    if (animState == WindowAnimState.ButtonBounce && BeginNextWaveButton.offsetLocation.Y <= 0)
                    {
                        animState = WindowAnimState.Paused;
                        BeginNextWaveButton.offsetLocation.Y = 0;
                        
                    }

                    switch (animState)
                    {
                        default: break;
                        case WindowAnimState.Moving:
                            offsetLocation.Y += WindowScrollOnscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                        case WindowAnimState.Bounce:
                            offsetLocation.Y -= (WindowScrollOnscreenSpeed / 10f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                        case WindowAnimState.Button:
                            BeginNextWaveButton.offsetLocation.Y += ButtonScrollOnscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                        case WindowAnimState.ButtonBounce:
                            BeginNextWaveButton.offsetLocation.Y -= (ButtonScrollOnscreenSpeed / 10f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                    }

                }
                else //-138 main
                {
                    if (animState == WindowAnimState.Paused && BeginNextWaveButton.offsetLocation.Y > -39)
                    {
                        animState = WindowAnimState.Button;
                    }
                    if (animState == WindowAnimState.Button && BeginNextWaveButton.offsetLocation.Y < -39)
                    {
                        BeginNextWaveButton.offsetLocation.Y = -39;
                        animState = WindowAnimState.Wait;
                    }
                    if (animState == WindowAnimState.Wait)
                    {
                        TimerBetweenAnimations += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (TimerBetweenAnimations >= TimeBetweenAnimations)
                        {
                            TimerBetweenAnimations = 0f;
                            animState = WindowAnimState.Moving;
                        }
                    }
                    if (animState == WindowAnimState.Moving && offsetLocation.Y < -138)
                    {
                        offsetLocation.Y = -138;
                        animState = WindowAnimState.Paused;
                        Visible = false;
                    }

                    switch (animState)
                    {
                        default: break;
                        case WindowAnimState.Button:
                            BeginNextWaveButton.offsetLocation.Y -= ButtonScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                        case WindowAnimState.Moving:
                            offsetLocation.Y -= WindowScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                    }
                }
            }

            private void BeginNextWaveButton_OnClick(Button sender)
            {
                Console.WriteLine("BeginNextButton Clicked");

                // Prevent launching a wave while a building is in hand
                if (ui.building == null)
                {
                    ui.level.StartNextWave();
                }
            }
        }
    }
}
