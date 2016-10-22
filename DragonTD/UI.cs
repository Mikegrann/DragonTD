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
        Vector2 screenSize;
        Vector2 centerScreen;
        InputStates inputStates;

        public UI(Game game, Level level) : base(game)
        {
            screenSize = game.GraphicsDevice.Viewport.Bounds.Size.ToVector2();
            centerScreen = screenSize / 2;
            spriteBatch = game.Services.GetService<SpriteBatch>();
            this.level = level;
        }

        public override void Update(GameTime gameTime)
        {
            inputStates.Update();
        }

        public override void Draw(GameTime gameTime)
        {

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
            SpriteBatch spriteBatch;
            public Texture2D Texture { get; private set; }
            public Rectangle Bounds { get; private set; }
            public Color Color { get; set; }
            internal UI ui;

            public UIComponent(Game game, UI parent, Texture2D texture, Rectangle bounds, Color? color) : base(game)
            {
                ui = parent;
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
            SpriteBatch spriteBatch;
            public Texture2D HoverTexture { get; private set; }
            public Texture2D ClickTexture { get; private set; }
            enum UIButtonState { Default, Hover, Click }
            UIButtonState currentState = UIButtonState.Default;

            public delegate void ButtonClickedEventHandler(object sender);
            public event ButtonClickedEventHandler OnClick;

            public Button(Game game, UI parent, Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Rectangle bounds, Color? color) : base(game, parent, texture, bounds, color)
            {
            }

            public override void Update(GameTime gameTime)
            {
                //if moues is inside button
                if(Bounds.Contains(ui.inputStates.CurrentMouse.Position))
                {
                    currentState = UIButtonState.Hover;
                    //if mouse pressed on current state, use clicked texture
                    if (ui.inputStates.CurrentMouse.LeftButton == ButtonState.Pressed)
                    {
                        currentState = UIButtonState.Click;
                    }
                    //if mouse is not pressed, but it was last frame, fire click event.
                    else if (ui.inputStates.LastMouse.LeftButton == ButtonState.Pressed)
                    {
                        OnClick?.Invoke(this);
                    }
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
            List<UIComponent> components = new List<UIComponent>();

            public Window(Game game, UI parent) : base(game)
            {
                ui = parent;
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
            public BuildWindow(Game game, UI parent) : base(game, parent)
            {

            }
        }

        class UpNextWindow : Window
        {
            public UpNextWindow(Game game, UI parent) : base(game, parent)
            {

            }
        }

        class TreasureWindow : Window
        {
            public TreasureWindow(Game game, UI parent) : base(game, parent)
            {

            }
        }

        class TowerContextMenu : Window
        {
            public TowerContextMenu (Game game, UI parent) : base(game, parent)
            {

            }
        }

    }
}
