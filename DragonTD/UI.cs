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
        TreasureWindow treasureWindow;

        Tower.Tower building;

        Matrix ViewMatrix;

        public UI(DragonTDGame game, Level level) : base(game)
        {
            screenSize = game.GraphicsDevice.Viewport.Bounds.Size;
            centerScreen = new Point(screenSize.X / 2, screenSize.Y / 2);
            spriteBatch = game.Services.GetService<SpriteBatch>();
            this.level = level;
            buildWindow = new BuildWindow(game, this, new Rectangle(0, screenSize.Y-96, screenSize.X, 96));
            upNextWindow = new UpNextWindow(game, this, new Rectangle(centerScreen.X-250, 0, 500, 150));
            treasureWindow = new TreasureWindow(game, this, new Rectangle(screenSize.X-260, screenSize.Y-96, 260, 96));
            ViewMatrix = game.ViewMatrix;
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
                }
                else
                {
                    building.Color = new Color(64, 64, 64, 192);
                }

                //after pressing LMB, place building
                if (inputStates.CurrentMouse.LeftButton == ButtonState.Released && inputStates.LastMouse.LeftButton == ButtonState.Pressed)
                {
                    
                    Console.WriteLine("screen:{0} world:{1} hex:{2}",inputStates.CurrentMouse.Position, p, building.Position);
                    if (level.PlaceHexEntity(building))
                    {
                        building.Color = Color.White;
                        level.Building = building = null;
                    }
                }
            }

            buildWindow.Update(gameTime);
            upNextWindow.Update(gameTime);
            treasureWindow.Update(gameTime);

            if (inputStates.CurrentKey.IsKeyUp(Keys.Q) && inputStates.LastKey.IsKeyDown(Keys.Q))
                buildWindow.Enabled = !buildWindow.Enabled;

            

        }

        public override void Draw(GameTime gameTime)
        {
            buildWindow.Draw(gameTime);
            upNextWindow.Draw(gameTime);
            treasureWindow.Draw(gameTime);
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
            public Texture2D Texture { get; private set; }
            private Rectangle bounds;
            public Rectangle Bounds { get { return new Rectangle(bounds.Location + parentWindow.Bounds.Location, bounds.Size); } private set { bounds = value; } }
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
                HoverTexture = hoverTexture;
                ClickTexture = clickTexture;
                DisabledTexture = disabledTexture;
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

            public Window(Game game, UI parent, Rectangle bounds) : base(game)
            {
                ui = parent;
                Bounds = bounds;
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
                if(Visible)
                    foreach (UIComponent c in components)
                    {
                        c.Draw(gameTime);
                    }
            }
        }

        class BuildWindow : Window
        {
            //pixels per second
            float ScrollOffscreenSpeed = 128f;

            public BuildWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                Button RedTowerButton = new Button("Red Tower", game, this, game.Content.Load<Texture2D>("textures/ui/buttons/test"), game.Content.Load<Texture2D>("textures/ui/buttons/testhover"), game.Content.Load<Texture2D>("textures/ui/buttons/testclick"), game.Content.Load<Texture2D>("textures/ui/buttons/testdisabled"), new Rectangle(64, 0, 80, 80), null);
                Button WhiteTowerButton = new Button("White Tower", game, this, game.Content.Load<Texture2D>("textures/ui/buttons/test"), game.Content.Load<Texture2D>("textures/ui/buttons/testhover"), game.Content.Load<Texture2D>("textures/ui/buttons/testclick"), game.Content.Load<Texture2D>("textures/ui/buttons/testdisabled"), new Rectangle(180, 0, 80, 80), null);
                RedTowerButton.OnHover += TowerButton_OnHover;
                RedTowerButton.OnClick += TowerButton_OnClick;
                RedTowerButton.OnLeave += TowerButton_OnLeave;
                WhiteTowerButton.OnHover += TowerButton_OnHover;
                WhiteTowerButton.OnClick += TowerButton_OnClick;
                WhiteTowerButton.OnLeave += TowerButton_OnLeave;
                WhiteTowerButton.Enabled = false;
                components.Add(RedTowerButton);
                components.Add(WhiteTowerButton);
            }

            public override void Update(GameTime gameTime)
            {
                if(Enabled)
                {
                    if(offsetLocation.Y > 0)
                        offsetLocation.Y -= ScrollOffscreenSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (offsetLocation.Y < 0)
                        offsetLocation.Y = 0;
                    base.Update(gameTime);
                }
                else
                {
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
                ui.level.Building = ui.building = new Tower.ProjectileTower(Game, ui.level, Point.Zero, Tower.ProjectileTower.ProjectileTowerType.Basic);
            }

            private void TowerButton_OnHover(Button sender)
            {
                Console.WriteLine("buton hobver " + sender.Name);
            }
        }

        class UpNextWindow : Window
        {
            public UpNextWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {

            }
        }

        class TreasureWindow : Window
        {
            public TreasureWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {

            }
        }

        class TowerContextMenu : Window
        {
            public TowerContextMenu (Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {

            }
        }

    }
}
