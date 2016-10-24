using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DragonTD
{
    class UI : DrawableGameComponent
    {
        //good fucking luck m8

        SpriteBatch spriteBatch;
        Level level;
        Point screenSize;
        Point centerScreen;
        InputStates inputStates = new InputStates();

        BuildWindow buildWindow;
        UpNextWindow upNextWindow;
        SpeedControlsWindow speedControlWindow;

        HexEntity building;

        Matrix ViewMatrix;

        Texture2D LeftBorder;
        Texture2D RightBorder;

        public UI(DragonTDGame game, Level level) : base(game)
        {
            screenSize = game.GraphicsDevice.Viewport.Bounds.Size;
            centerScreen = new Point(screenSize.X / 2, screenSize.Y / 2);
            spriteBatch = game.Services.GetService<SpriteBatch>();
            this.level = level;
            buildWindow = new BuildWindow(game, this, new Rectangle(0, 613, screenSize.X, 107));
            upNextWindow = new UpNextWindow(game, this, new Rectangle(0, 0, screenSize.X, 138));
            speedControlWindow = new SpeedControlsWindow(game, this, new Rectangle(screenSize.X-65, screenSize.Y-69, 64, 66));
            speedControlWindow.Enabled = false;
            ViewMatrix = game.ViewMatrix;
            LeftBorder = game.Content.Load<Texture2D>("Textures/UI/Left Border");
            RightBorder = game.Content.Load<Texture2D>("Textures/UI/Right Border");
        }

        public override void Update(GameTime gameTime)
        {
            inputStates.Update();
            Vector2 pointer = (Vector2.Transform(inputStates.CurrentMouse.Position.ToVector2(), Matrix.Invert(ViewMatrix)));

            if(building != null)
            {
                // Draw temporary building on hover
                Vector2 p = pointer - new Vector2(64, 0);
                building.Position = HexEntity.PixelToHex(p);
                building.ScreenPosition = HexEntity.CalculateScreenPosition(building.Position);

                if (level.IsPlaceable(building))
                {
                    building.Color = new Color(128, 128, 128, 128);

                    //after pressing LMB, place building
                    if (inputStates.CurrentMouse.LeftButton == ButtonState.Released && inputStates.LastMouse.LeftButton == ButtonState.Pressed)
                    {

                        Console.WriteLine("screen:{0} world:{1} hex:{2}", inputStates.CurrentMouse.Position, p, building.Position);
                        if (level.PlaceHexEntity(building))
                        {
                            level.Money -= building.Cost;
                            building.Color = Color.White;
                            level.Building = building = null;
                        }
                    }
                }
                else
                {
                    building.Color = new Color(64, 64, 64, 192);
                }
            }

            buildWindow.Update(gameTime);
            upNextWindow.Update(gameTime);
            speedControlWindow.Update(gameTime);

            /*if (inputStates.CurrentKey.IsKeyUp(Keys.Q) && inputStates.LastKey.IsKeyDown(Keys.Q))
            {
                buildWindow.Enabled = !buildWindow.Enabled;
                speedControlWindow.Enabled = !speedControlWindow.Enabled;
                upNextWindow.Enabled = !upNextWindow.Enabled;
            }       */   
            if(level.IsWaveOngoing())
            {
                buildWindow.Enabled = false;
                upNextWindow.Enabled = false;
                speedControlWindow.Enabled = true;
            }
            else
            {
                buildWindow.Enabled = true;
                upNextWindow.Enabled = true;
                speedControlWindow.Enabled = false;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(LeftBorder, Vector2.Zero, Color.White);
            spriteBatch.Draw(RightBorder, Vector2.Zero, Color.White);
            buildWindow.Draw(gameTime);
            upNextWindow.Draw(gameTime);
            speedControlWindow.Draw(gameTime);
        }

        class InputStates
        {
            public MouseState CurrentMouse { get; private set; }
            public MouseState LastMouse { get; private set; }

            public KeyboardState CurrentKey { get; private set; }
            public KeyboardState LastKey { get; private set; }

            public void Update()
            {
                LastMouse = CurrentMouse;
                LastKey = CurrentKey;
                CurrentMouse = Mouse.GetState();
                CurrentKey = Keyboard.GetState();
            }
        }

        class UIComponent : DrawableGameComponent
        {
            public string Name { get; private set; }
            internal SpriteBatch spriteBatch;
            public Texture2D Texture;
            private Rectangle bounds;
            public Vector2 offsetLocation = Vector2.Zero;
            public Rectangle Bounds { get { return new Rectangle(bounds.Location + parentWindow.Bounds.Location + offsetLocation.ToPoint(), bounds.Size); } private set { bounds = value; } }
            public Color Color { get; set; }
            internal Window parentWindow;

            public UIComponent(string name, Game game, Window parent, Texture2D texture, Rectangle bounds, Color? color) : base(game)
            {
                Name = name;
                parentWindow = parent;
                spriteBatch = game.Services.GetService<SpriteBatch>();
                Texture = texture;
                Bounds = bounds;
                if (!color.HasValue)
                    color = Color.White;
                Color = color.Value;
            }

            public void SetLocation(int x, int y)
            {
                bounds.X = x;
                bounds.Y = y;
            }
            public void SetLocation(Point p)
            {
                SetLocation(p.X, p.Y);
            }

            public override void Draw(GameTime gameTime)
            {
                if(Visible)
                    spriteBatch.Draw(Texture, Bounds, Color);
            }
        }

        class Button : UIComponent
        {
            Texture2D HoverTexture;
            Texture2D ClickTexture;
            Texture2D DisabledTexture;
            enum UIButtonState { Default, Hover, Click }
            UIButtonState currentState = UIButtonState.Default;

            public delegate void ButtonClickedEventHandler(Button sender);
            public event ButtonClickedEventHandler OnClick;
            public delegate void ButtonHoveredEventHandler(Button sender);
            public event ButtonHoveredEventHandler OnHover;
            public delegate void ButtonLeftEventHandler(Button sender);
            public event ButtonLeftEventHandler OnLeave;

            public Button(string name, Game game, Window parent, Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Texture2D disabledTexture, Rectangle bounds, Color? color) : base(name, game, parent, texture, bounds, color)
            {
                SetTextures(texture, hoverTexture, clickTexture, disabledTexture);
            }

            public void SetTextures(Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Texture2D disabledTexture)
            {
                Texture = texture;
                HoverTexture = (hoverTexture != null)? hoverTexture : Texture;
                ClickTexture = (clickTexture != null) ? clickTexture : Texture;
                DisabledTexture = (disabledTexture != null) ? disabledTexture : Texture;
                
            }

            public override void Update(GameTime gameTime)
            {
                if (Enabled)
                {
                    //if moues is inside button
                    if (Bounds.Contains(parentWindow.ui.inputStates.CurrentMouse.Position))
                    {
                        //if the mouse wasn't inside the button last frame, it is now hovering.
                        if (!Bounds.Contains(parentWindow.ui.inputStates.LastMouse.Position))
                            OnHover?.Invoke(this);

                        currentState = UIButtonState.Hover;
                        //if mouse pressed on current state, use clicked texture
                        if (parentWindow.ui.inputStates.CurrentMouse.LeftButton == ButtonState.Pressed)
                        {
                            currentState = UIButtonState.Click;
                        }
                        //if mouse is not pressed, but it was last frame, fire click event.
                        else if (parentWindow.ui.inputStates.LastMouse.LeftButton == ButtonState.Pressed)
                        {
                            OnClick?.Invoke(this);
                        }
                    }
                    else if (Bounds.Contains(parentWindow.ui.inputStates.LastMouse.Position))
                    {
                        currentState = UIButtonState.Default;
                        OnLeave?.Invoke(this);
                    }
                }
            }

            public override void Draw(GameTime gameTime)
            {
                if (Visible)
                {
                    if(!Enabled)
                        spriteBatch.Draw(DisabledTexture, Bounds, Color);
                    else
                        switch (currentState)
                        {
                            default:
                                spriteBatch.Draw(Texture, Bounds, Color);
                                break;
                            case UIButtonState.Hover:
                                spriteBatch.Draw(HoverTexture, Bounds, Color);
                                break;
                            case UIButtonState.Click:
                                spriteBatch.Draw(ClickTexture, Bounds, Color);
                                break;
                        }
                }
            }
        }
        
        class Window : DrawableGameComponent
        {
            internal UI ui;
            internal List<UIComponent> components = new List<UIComponent>();
            internal Vector2 offsetLocation = Vector2.Zero;
            internal Rectangle bounds;
            public Rectangle Bounds { get { return new Rectangle(bounds.Location + offsetLocation.ToPoint(), bounds.Size); } set { bounds = value; } }
            public Texture2D Background;

            public Window(Game game, UI parent, Rectangle bounds, Texture2D background = null) : base(game)
            {
                ui = parent;
                Bounds = bounds;
                Background = background;
            }

            public override void Update(GameTime gameTime)
            {
                if(Enabled)
                    foreach(UIComponent c in components)
                    {
                        c.Update(gameTime);
                    }
            }

            public override void Draw(GameTime gameTime)
            {
                if (Visible)
                {
                    if (Background != null)
                        ui.spriteBatch.Draw(Background, Bounds, Color.White);
                    foreach (UIComponent c in components)
                    {
                        c.Draw(gameTime);
                    }
                }
            }
        }

        class BuildWindow : Window
        {
            //pixels per second
            float ScrollOffscreenSpeed = 256f;
            float ScrollOnscreenSpeed = 512f;

            public BuildWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                Background = game.Content.Load<Texture2D>("Textures/UI/Bottom Border");
                Button WallButton           = new Button("wall",            game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(229, 8, 60, 60), null);
                Button BasicTowerButton     = new Button("basicTower",      game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(322, 8, 60, 60), null);
                Button PoisonTowerButton    = new Button("poisonTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(413, 8, 60, 60), null);
                Button PiercingTowerButton  = new Button("piercingTower",   game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(506, 8, 60, 60), null);
                Button SniperTowerButton    = new Button("sniperTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(598, 8, 60, 60), null);
                Button ExplosiveTowerButton = new Button("explosiveTower",  game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(691, 8, 60, 60), null);
                Button FreezeTowerButton    = new Button("freezeTower",     game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(783, 8, 60, 60), null);
                Button LightningTowerButton = new Button("lightningTower",  game, this, game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButton"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonH"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonC"), game.Content.Load<Texture2D>("Textures/UI/Buttons/TestButtonD"), new Rectangle(875, 8, 60, 60), null);

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
                if(Enabled)
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

                    switch(animState)
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


            private void TowerButton_OnLeave(Button sender)
            {
                Console.WriteLine("button leave " + sender.Name);
            }

            private void TowerButton_OnClick(Button sender)
            {
                Console.WriteLine("button click " + sender.Name);
                if (sender.Name == "basicTower")
                    ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.TowerType.Basic);
                else if (sender.Name == "poisonTower")
                    ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.TowerType.Poison);
                else if (sender.Name == "piercingTower")
                    ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.TowerType.Piercing);
                else if (sender.Name == "sniperTower")
                    ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.TowerType.Sniper);
                else if (sender.Name == "explosiveTower")
                    ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.TowerType.Explosive);
                else if (sender.Name == "freezeTower")
                    ui.level.Building = ui.building = new Tower.AoETower(Game, ui.level, Point.Zero, Tower.TowerType.Freeze);
                else if (sender.Name == "lightningTower")
                    ui.level.Building = ui.building = new Tower.AoETower(Game, ui.level, Point.Zero, Tower.TowerType.Lightning);
                else if (sender.Name == "wall")
                    ui.level.Building = ui.building = new Obstacle(Game, ui.level, Point.Zero, Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile1"));

            }

            private void TowerButton_OnHover(Button sender)
            {
                Console.WriteLine("buton hobver " + sender.Name);
            }
        }

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
                components.Add(backgroundImage);
            }

            public override void Update(GameTime gameTime)
            {
                if (Enabled)
                {
                    if (animState == WindowAnimState.Paused && offsetLocation.Y < 0)
                    {
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

                    switch(animState)
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
                        animState = WindowAnimState.Button;
                    if(animState == WindowAnimState.Button && BeginNextWaveButton.offsetLocation.Y < -39)
                    {
                        BeginNextWaveButton.offsetLocation.Y = -39;
                        animState = WindowAnimState.Wait;
                    }
                    if(animState == WindowAnimState.Wait)
                    {
                        TimerBetweenAnimations += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(TimerBetweenAnimations >= TimeBetweenAnimations)
                        {
                            TimerBetweenAnimations = 0f;
                            animState = WindowAnimState.Moving;
                        }
                    }
                    if(animState == WindowAnimState.Moving && offsetLocation.Y < -138)
                    {
                        offsetLocation.Y = -138;
                        animState = WindowAnimState.Paused;
                    }

                    switch(animState)
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
                base.Update(gameTime);
            }

            private void BeginNextWaveButton_OnClick(Button sender)
            {
                Console.WriteLine("BeginNextButton Clicked");
                ui.level.StartNextWave();
            }
        }

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

        class TowerContextMenu : Window
        {
            public TowerContextMenu (Game game, UI parent, Rectangle bounds, HexEntity entity) : base(game, parent, bounds)
            {
                //show context for entity.
                //
            }
        }

    }
}
