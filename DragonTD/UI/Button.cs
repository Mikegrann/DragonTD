using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DragonTD.UI
{
    partial class UI
    {
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

            public Button(string name, Game game, Window parent, Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Texture2D disabledTexture, Rectangle bounds, Color? color, bool drawnOnRenderTarget = false) : base(name, game, parent, texture, bounds, color, drawnOnRenderTarget)
            {
                SetTextures(texture, hoverTexture, clickTexture, disabledTexture);
            }

            public void SetTextures(Texture2D texture, Texture2D hoverTexture, Texture2D clickTexture, Texture2D disabledTexture)
            {
                Texture = texture;
                HoverTexture = (hoverTexture != null) ? hoverTexture : Texture;
                ClickTexture = (clickTexture != null) ? clickTexture : Texture;
                DisabledTexture = (disabledTexture != null) ? disabledTexture : Texture;

            }

            public override void Update(GameTime gameTime)
            {
                if (Enabled && parentWindow.Enabled)
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
                    if (DrawnOnRenderTarget)
                    {
                        if (!Enabled)
                            spriteBatch.Draw(DisabledTexture, BoundsWithoutParent, Color);
                        else
                            switch (currentState)
                            {
                                default:
                                    spriteBatch.Draw(Texture, BoundsWithoutParent, Color);
                                    break;
                                case UIButtonState.Hover:
                                    spriteBatch.Draw(HoverTexture, BoundsWithoutParent, Color);
                                    break;
                                case UIButtonState.Click:
                                    spriteBatch.Draw(ClickTexture, BoundsWithoutParent, Color);
                                    break;
                            }
                    }
                    else
                    {
                        if (!Enabled)
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
        }
    }
}