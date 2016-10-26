using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DragonTD.UI
{
    partial class UI
    {
        class TowerContextMenu : Window
        {
            RenderTarget2D rt;
            HexEntity Target;
            bool ShowDescription;
            float transparency = 0;
            const float fadeTime = 0.05f;
            float fadeTimer = 0f;
            SpriteFont font;
            string[] towerInfo;
            Texture2D backImage;
            Texture2D pointer;

            public Point Size { get { return rt.Bounds.Size; } } 

            TargetPositionRelative targetPosRel = TargetPositionRelative.Left;

            public enum TargetPositionRelative { Left, Center, Right }

            Button UpgradeButton;
            Button SellButton;

            
            public TowerContextMenu(Game game, UI parent) : base(game, parent, Rectangle.Empty)
            {
                //<60|240|60>
                rt = new RenderTarget2D(game.GraphicsDevice, 360, 300);
                font = game.Content.Load<SpriteFont>("Fonts/Console");
                Visible = false;
                //offsetLocation.Y = -rt.Height / 2f;
                backImage = game.Content.Load<Texture2D>("Textures/UI/ContextMenu/test");
                pointer = game.Content.Load<Texture2D>("Textures/UI/ContextMenu/pointer");

                UpgradeButton = new Button("upgrade", game, this, game.Content.Load<Texture2D>("Textures/UI/ContextMenu/button"), null, null, null, new Rectangle(82, 151, 200, 35), Color.White, true);
                SellButton    = new Button("sell",    game, this, game.Content.Load<Texture2D>("Textures/UI/ContextMenu/button"), null, null, null, new Rectangle(82, 191, 200, 35), Color.White, true);

                UpgradeButton.OnClick += Button_OnClick;
                SellButton.OnClick += Button_OnClick;

                UpgradeButton.OnHover += Button_OnHover;
                SellButton.OnHover += Button_OnHover;

                UpgradeButton.OnLeave += Button_OnLeave;
                SellButton.OnLeave += Button_OnLeave;

                components.Add(UpgradeButton);
                components.Add(SellButton);
            }

            private void Button_OnHover(Button sender)
            {
                System.Console.WriteLine(sender.Name + " HOVERED");
            }

            private void Button_OnLeave(Button sender)
            {
                System.Console.WriteLine(sender.Name + " LEFT");
            }

            private void Button_OnClick(Button sender)
            {
                System.Console.WriteLine(sender.Name + " CLICKED");
                switch(sender.Name)
                {
                    case "upgrade":
                        if(Target.GetType().IsSubclassOf(typeof(Tower.Tower)))
                            ui.level.UpgradeTower((Tower.Tower)Target);
                        break;
                    case "sell":
                        ui.level.SellHex(Target);
                        break;

                    default:break;
                }
                UpdateTowerInfo();
            }

            public void Show(HexEntity target, Point ManualLocation, TargetPositionRelative tpr, bool showDescription = false)
            {
                //if we clicked on the same target again, close
                if (Target != null && Target.Position.Equals(target.Position))
                {
                    Hide();
                    return;
                }
                targetPosRel = tpr;
                Target = target;
                ShowDescription = showDescription;
                Enabled = true;
                bounds.Location = ManualLocation;
                bounds.Size = rt.Bounds.Size;
                UpdateTowerInfo();

                UpgradeButton.Enabled &= !ShowDescription;
                UpgradeButton.Visible &= !ShowDescription;
                SellButton.Enabled &= !ShowDescription;
                SellButton.Visible &= !ShowDescription;
            }
            public void Show(HexEntity target, bool showDescription = false)
            {
                Point TargetPos = Vector2.Transform(target.ScreenPosition, ui.ViewMatrix).ToPoint();
                if(ui.screenSize.X - TargetPos.X < rt.Width)
                    Show(target, TargetPos - new Point(rt.Width, rt.Height / 2), TargetPositionRelative.Right, showDescription);
                else
                    Show(target, TargetPos - new Point(0, rt.Height/2), TargetPositionRelative.Left, showDescription);
            }

            void UpdateTowerInfo()
            {
                if(!ShowDescription)
                    Target = ui.level.Map[Target.Position.Y, Target.Position.X];

                if (Target != null && Target.GetType().IsSubclassOf(typeof(Tower.Tower)))
                {
                    towerInfo = ((Tower.Tower)Target).GetTowerStatsStrings(null).ToArray();
                    UpgradeButton.Enabled = ((Tower.Tower)Target).CanUpgrade();
                    UpgradeButton.Visible = true;
                }
                else if (Target != null && Target.GetType() == typeof(Obstacle) && ((Obstacle)Target).Type == Obstacle.ObstacleType.Wall)
                {
                    towerInfo = new string[] { ui.Localization.Get("wall"), ui.Localization.Get("cost") + ": " + Target.Cost };
                    UpgradeButton.Enabled = false;
                    UpgradeButton.Visible = false;
                }
                else
                {
                    Hide();
                }
            }

            public void Hide()
            {
                Enabled = false;
                Target = null;
                //bounds = Rectangle.Empty;
            }


            public override void DrawRenderTargets(GameTime gameTime)
            {
                if (Visible && Target != null)
                {
                    ui.Game.GraphicsDevice.SetRenderTarget(rt);
                    ui.spriteBatch.Begin();

                    ui.spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                    if (targetPosRel == TargetPositionRelative.Left)
                        ui.spriteBatch.Draw(pointer, new Vector2(10, 0), Color.White);
                    else if (targetPosRel == TargetPositionRelative.Right)
                        ui.spriteBatch.Draw(pointer, new Vector2(300, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);

                    ui.spriteBatch.Draw(backImage, new Vector2(60, 0), Color.White);

                    ui.spriteBatch.DrawString(font, towerInfo[0], new Vector2(72, 10), Color.Black);

                    //cost is always second. only show cost to buy if showing description.
                    for (int i = (ShowDescription)?1:2; i < towerInfo.Length; i++)
                    {
                        ui.spriteBatch.DrawString(font, towerInfo[i], new Vector2(72, 14 + (i * 16)), Color.Black);
                    }

                    base.Draw(gameTime);

                    if(UpgradeButton.Visible && Target.GetType().IsSubclassOf(typeof(Tower.Tower)))
                        ui.spriteBatch.DrawString(font, ui.Localization.Get("upgrade") + ": $" + ((Tower.Tower)Target).CostToUpgrade, new Vector2(82, 151), Color.Black);
                    if(SellButton.Visible)
                        ui.spriteBatch.DrawString(font, ui.Localization.Get("sell") + ": $" + Target.SellPrice, new Vector2(82, 191), Color.Black);

                    ui.spriteBatch.End();
                    ui.Game.GraphicsDevice.SetRenderTarget(null);
                }
            }

            public override void Draw(GameTime gameTime)
            {
                if (Visible && Target != null)
                {
                    ui.spriteBatch.Draw(rt, bounds, Color.FromNonPremultiplied(255, 255, 255, (int)(transparency * 255f)));
                }
                //base.Draw(gameTime);
            }

            public override void Update(GameTime gameTime)
            {
                if (Target == null)
                    Hide();

                if (Enabled && fadeTimer < fadeTime)
                {
                    fadeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (fadeTimer > fadeTime)
                        fadeTimer = fadeTime;
                }
                else if (!Enabled && fadeTimer > 0)
                {
                    fadeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (fadeTimer < 0)
                    {
                        //once we are at 0, we can remove the target
                        Target = null;
                        fadeTimer = 0;
                        bounds = Rectangle.Empty;
                    }
                }
                transparency = fadeTimer / fadeTime;

                Visible = transparency > 0;

                if(Enabled)
                    base.Update(gameTime);
            }

        }
    }
}
