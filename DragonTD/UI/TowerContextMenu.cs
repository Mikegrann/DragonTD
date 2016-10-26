using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.UI
{
    partial class UI
    {
        class TowerContextMenu : Window
        {
            RenderTarget2D rt;
            Tower.Tower Target;
            bool statsOnly;
            float transparency = 0;
            const float fadeTime = 0.05f;
            float fadeTimer = 0f;
            SpriteFont font;
            string[] towerInfo;

            public TowerContextMenu(Game game, UI parent, Rectangle bounds) : base(game, parent, bounds)
            {
                rt = new RenderTarget2D(game.GraphicsDevice, 300, 300);
                font = game.Content.Load<SpriteFont>("Fonts/Console");
                Visible = false;
                offsetLocation.Y = -rt.Height / 2f;
                Background = game.Content.Load<Texture2D>("Textures/UI/ContextMenu/test");
            }

            public void Show(Tower.Tower target, bool statsOnly = false)
            {
                //if we clicked on the same target again, close
                if (Target != null && Target.Position.Equals(target.Position))
                {
                    Hide();
                    return;
                }
                Target = target;
                this.statsOnly = statsOnly;
                Enabled = true;
                bounds.Location = Vector2.Transform(Target.ScreenPosition, ui.ViewMatrix).ToPoint() + offsetLocation.ToPoint();
                switch (Target.TType)
                {
                    case Tower.TowerType.Basic:
                        towerInfo = new string[] { "Basic Tower" };
                        break;
                    case Tower.TowerType.Poison:
                        towerInfo = new string[] { "Poison Tower" };
                        break;
                    case Tower.TowerType.Piercing:
                        towerInfo = new string[] { "Piercing Tower" };
                        break;
                    case Tower.TowerType.Sniper:
                        towerInfo = new string[] { "Sniper Tower" };
                        break;
                    case Tower.TowerType.Explosive:
                        towerInfo = new string[] { "Explosive Tower" };
                        break;
                    case Tower.TowerType.Freeze:
                        towerInfo = new string[] { "Frost Tower" };
                        break;
                    case Tower.TowerType.Lightning:
                        towerInfo = new string[] { "Lightning Tower" };
                        break;

                }
            }

            public void Hide()
            {
                Enabled = false;
            }


            public void DrawRenderTargets(GameTime gameTime)
            {
                if (Visible && Target != null)
                {
                    ui.Game.GraphicsDevice.SetRenderTarget(rt);
                    ui.spriteBatch.Begin();

                    ui.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                    ui.spriteBatch.Draw(Background, rt.Bounds, Color.White);

                    ui.spriteBatch.DrawString(font, towerInfo[0], new Vector2(72, 10), Color.Black);

                    for (int i = 1; i < towerInfo.Length; i++)
                    {
                        ui.spriteBatch.DrawString(font, towerInfo[i], new Vector2(72, 10 + (i * 16)), Color.Black);
                    }

                    ui.spriteBatch.End();
                    ui.Game.GraphicsDevice.SetRenderTarget(null);
                }
            }

            public override void Draw(GameTime gameTime)
            {
                if (Visible && Target != null)
                {
                    ui.spriteBatch.Draw(rt, bounds.Location.ToVector2(), Color.FromNonPremultiplied(255, 255, 255, (int)(transparency * 255f)));
                }
                base.Draw(gameTime);
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
                    }
                }
                transparency = fadeTimer / fadeTime;

                Visible = transparency > 0;


                base.Update(gameTime);
            }

        }
    }
}
