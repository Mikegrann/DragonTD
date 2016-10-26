using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.UI
{
    partial class UI
    {
        class BuildWindow : Window
        {
            //pixels per second
            float ScrollOffscreenSpeed = 256f;
            float ScrollOnscreenSpeed = 512f;

            TowerContextMenu TCM;

            public BuildWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                TCM = new TowerContextMenu(game, parent);
                Background = game.Content.Load<Texture2D>("Textures/UI/Bottom Border");
                Button WallButton           = new Button("wall",            game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Wall"), null, null, null, new Rectangle(229, 8, 60, 60), null);
                Button BasicTowerButton     = new Button("basicTower",      game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Red"), null, null, null, new Rectangle(322, 8, 60, 60), null);
                Button PoisonTowerButton    = new Button("poisonTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Green"), null, null, null, new Rectangle(413, 8, 60, 60), null);
                Button PiercingTowerButton  = new Button("piercingTower",   game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Blue"), null, null, null, new Rectangle(506, 8, 60, 60), null);
                Button SniperTowerButton    = new Button("sniperTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Purple"), null, null, null, new Rectangle(598, 8, 60, 60), null);
                Button ExplosiveTowerButton = new Button("explosiveTower",  game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Black"), null, null, null, new Rectangle(691, 8, 60, 60), null);
                Button FreezeTowerButton    = new Button("freezeTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/White"), null, null, null, new Rectangle(783, 8, 60, 60), null);
                Button LightningTowerButton = new Button("lightningTower",  game, this, game.Content.Load<Texture2D>("Textures/UI/TowerIcons/Yellow"), null, null, null, new Rectangle(875, 8, 60, 60), null);

                WallButton.OnHover += TowerButton_OnHover;
                WallButton.OnClick += TowerButton_OnClick;
                WallButton.OnLeave += TowerButton_OnLeave;

                BasicTowerButton.OnHover += TowerButton_OnHover;
                BasicTowerButton.OnClick += TowerButton_OnClick;
                BasicTowerButton.OnLeave += TowerButton_OnLeave;

                PoisonTowerButton.OnHover += TowerButton_OnHover;
                PoisonTowerButton.OnClick += TowerButton_OnClick;
                PoisonTowerButton.OnLeave += TowerButton_OnLeave;

                PiercingTowerButton.OnHover += TowerButton_OnHover;
                PiercingTowerButton.OnClick += TowerButton_OnClick;
                PiercingTowerButton.OnLeave += TowerButton_OnLeave;

                SniperTowerButton.OnHover += TowerButton_OnHover;
                SniperTowerButton.OnClick += TowerButton_OnClick;
                SniperTowerButton.OnLeave += TowerButton_OnLeave;

                ExplosiveTowerButton.OnHover += TowerButton_OnHover;
                ExplosiveTowerButton.OnClick += TowerButton_OnClick;
                ExplosiveTowerButton.OnLeave += TowerButton_OnLeave;

                FreezeTowerButton.OnHover += TowerButton_OnHover;
                FreezeTowerButton.OnClick += TowerButton_OnClick;
                FreezeTowerButton.OnLeave += TowerButton_OnLeave;

                LightningTowerButton.OnHover += TowerButton_OnHover;
                LightningTowerButton.OnClick += TowerButton_OnClick;
                LightningTowerButton.OnLeave += TowerButton_OnLeave;

                components.Add(WallButton);
                components.Add(BasicTowerButton);
                components.Add(PoisonTowerButton);
                components.Add(PiercingTowerButton);
                components.Add(SniperTowerButton);
                components.Add(ExplosiveTowerButton);
                components.Add(FreezeTowerButton);
                components.Add(LightningTowerButton);
            }

            public enum WindowAnimState { Moving, Bounce, Paused }

            WindowAnimState animState = WindowAnimState.Paused;

            public override void Update(GameTime gameTime)
            {
                if (Enabled)
                {
                    if (animState == WindowAnimState.Paused && offsetLocation.Y > 0)
                        animState = WindowAnimState.Moving;
                    if (animState == WindowAnimState.Moving && offsetLocation.Y < 0)
                        animState = WindowAnimState.Bounce;
                    if (animState == WindowAnimState.Bounce && offsetLocation.Y >= 0)
                    {
                        animState = WindowAnimState.Paused;
                        offsetLocation.Y = 0;
                    }

                    switch (animState)
                    {
                        default:
                            break;
                        case WindowAnimState.Moving:
                            offsetLocation.Y -= ScrollOnscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                        case WindowAnimState.Bounce:
                            offsetLocation.Y += (ScrollOnscreenSpeed / 10f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            break;
                    }

                    TCM.Update(gameTime);
                    base.Update(gameTime);
                }
                else
                {
                    //we don't need a bounce. but still set animstate to paused
                    animState = WindowAnimState.Paused;
                    if (offsetLocation.Y < bounds.Height)
                        offsetLocation.Y += ScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (offsetLocation.Y > bounds.Height)
                        offsetLocation.Y = bounds.Height;
                }
            }

            public override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);
                TCM.Draw(gameTime);
            }

            public override void DrawRenderTargets(GameTime gameTime)
            {
                TCM.DrawRenderTargets(gameTime);
            }


            private void TowerButton_OnLeave(Button sender)
            {
                Console.WriteLine("button leave " + sender.Name);
                TCM.Hide();
            }

            private void TowerButton_OnClick(Button sender)
            {
                Console.WriteLine("button click " + sender.Name);
                switch(sender.Name)
                {
                    case "basicTower":
                        ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Basic);
                        break;
                    case "poisonTower": 
                        ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Poison);
                        break;
                    case "piercingTower":
                        ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Piercing);
                        break;
                    case "sniperTower":
                        ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Sniper);
                        break;
                    case "explosiveTower":
                        ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Explosive);
                        break;
                    case "freezeTower":
                        ui.level.Building = ui.building = new Tower.AoETower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Freeze);
                        break;
                    case "lightningTower":
                        ui.level.Building = ui.building = new Tower.AoETower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Lightning);
                        break;
                    case "wall":
                        ui.level.Building = ui.building = new Obstacle(Game, ui.level, new Point(-100, -100), Obstacle.ObstacleType.Wall);
                        break;
                    default:break;
                }
            }

            private void TowerButton_OnHover(Button sender)
            {
                Console.WriteLine("buton hover " + sender.Name);
                switch (sender.Name)
                {
                    case "basicTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Basic), sender.Bounds.Center - new Point(TCM.Size.X/2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "poisonTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Poison), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "piercingTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Piercing), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "sniperTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Sniper), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "explosiveTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Explosive), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "freezeTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Freeze), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "lightningTower":
                        TCM.Show(new Tower.ProjectileTower(Game, ui.level, new Point(-100, -100), Tower.TowerType.Lightning), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    case "wall":
                        TCM.Show(new Obstacle(Game, ui.level, new Point(-100, -100), Obstacle.ObstacleType.Wall), sender.Bounds.Center - new Point(TCM.Size.X / 2, TCM.Size.Y), TowerContextMenu.TargetPositionRelative.Center, true);
                        break;
                    default: break;
                }
            }
        }
    }
}