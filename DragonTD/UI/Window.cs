using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.UI
{
    partial class UI
    {
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
                    foreach (UIComponent c in components)
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
    }
}
