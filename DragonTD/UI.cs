﻿using Microsoft.Xna.Framework;
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

        public UI(Game game, Level level) : base(game)
        {
            screenSize = game.GraphicsDevice.Viewport.Bounds.Size;
            centerScreen = new Point(screenSize.X / 2, screenSize.Y / 2);
            spriteBatch = game.Services.GetService<SpriteBatch>();
            this.level = level;
            buildWindow = new BuildWindow(game, this, new Rectangle(0, screenSize.Y-96, screenSize.X, 96));
            upNextWindow = new UpNextWindow(game, this, new Rectangle(centerScreen.X-250, 0, 500, 150));
            treasureWindow = new TreasureWindow(game, this, new Rectangle(screenSize.X-260, screenSize.Y-96, 260, 96));
        }

        public override void Update(GameTime gameTime)
        {
            inputStates.Update();
            buildWindow.Update(gameTime);
            upNextWindow.Update(gameTime);
            treasureWindow.Update(gameTime);
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
            internal SpriteBatch spriteBatch;
            public Texture2D Texture { get; private set; }
            private Rectangle bounds;
            public Rectangle Bounds { get { return new Rectangle(bounds.Location + parentWindow.Bounds.Location, bounds.Size); } private set { bounds = value; } }
            public Color Color { get; set; }
            internal Window parentWindow;

            public UIComponent(Game game, Window parent, Texture2D texture, Rectangle bounds, Color? color) : base(game)
            {
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
                spriteBatch.Draw(Texture, Bounds, Color);
            }
        }

        class Button : UIComponent
        {
            Texture2D HoverTexture;
            Texture2D ClickTexture;
            enum UIButtonState { Default, Hover, Click }
            UIButtonState currentState = UIButtonState.Default;

            public delegate void ButtonClickedEventHandler(object sender);
            public event ButtonClickedEventHandler OnClick;
            public delegate void ButtonHoveredEventHandler(object sender);
            public event ButtonHoveredEventHandler OnHover;
            public delegate void ButtonLeftEventHandler(object sender);
            public event ButtonLeftEventHandler OnLeave;

            public Button(Game game, Window parent, Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Rectangle bounds, Color? color) : base(game, parent, texture, bounds, color)
            {
                HoverTexture = hoverTexture;
                ClickTexture = clickTexture;
            }

            public override void Update(GameTime gameTime)
            {
                //if moues is inside button
                if(Bounds.Contains(parentWindow.ui.inputStates.CurrentMouse.Position))
                {
                    //if the mouse wasn't inside the button last frame, it is now hovering.
                    if(!Bounds.Contains(parentWindow.ui.inputStates.LastMouse.Position))
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
                else if(Bounds.Contains(parentWindow.ui.inputStates.LastMouse.Position))
                {
                    currentState = UIButtonState.Default;
                    OnLeave?.Invoke(this);
                }
            }

            public override void Draw(GameTime gameTime)
            {
                switch(currentState)
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
        
        class Window : DrawableGameComponent
        {
            internal UI ui;
            internal List<UIComponent> components = new List<UIComponent>();
            public Rectangle Bounds;

            public Window(Game game, UI parent, Rectangle bounds) : base(game)
            {
                ui = parent;
                Bounds = bounds;
            }

            public override void Update(GameTime gameTime)
            {
                foreach(UIComponent c in components)
                {
                    c.Update(gameTime);
                }
            }

            public override void Draw(GameTime gameTime)
            {
                foreach (UIComponent c in components)
                {
                    c.Draw(gameTime);
                }
            }
        }

        class BuildWindow : Window
        {
            public BuildWindow(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                Button RedTowerButton = new Button(game, this, game.Content.Load<Texture2D>("textures/ui/buttons/test"), game.Content.Load<Texture2D>("textures/ui/buttons/testhover"), game.Content.Load<Texture2D>("textures/ui/buttons/testclick"), new Rectangle(64, 0, 80, 80), null);
                RedTowerButton.OnHover += RedTowerButton_OnHover;
                RedTowerButton.OnClick += RedTowerButton_OnClick;
                RedTowerButton.OnLeave += RedTowerButton_OnLeave;
                components.Add(RedTowerButton);
            }

            private void RedTowerButton_OnLeave(object sender)
            {
                Console.WriteLine("button leave");
            }

            private void RedTowerButton_OnClick(object sender)
            {
                Console.WriteLine("button click");
            }

            private void RedTowerButton_OnHover(object sender)
            {
                Console.WriteLine("buton hobver");
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
