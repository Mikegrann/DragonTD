using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.UI
{
    partial class UI
    {
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
                if (Visible)
                    spriteBatch.Draw(Texture, Bounds, Color);
            }
        }
    }
}
