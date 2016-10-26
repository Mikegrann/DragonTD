using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DragonTD.UI
{
    partial class UI : DrawableGameComponent
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

        TowerContextMenu contextMenu;

        HexEntity building;

        Matrix ViewMatrix;

        Texture2D LeftBorder;
        Texture2D RightBorder;

        SpriteFont font;

        public UI(DragonTDGame game, Level level) : base(game)
        {
            screenSize = game.GraphicsDevice.Viewport.Bounds.Size;
            centerScreen = new Point(screenSize.X / 2, screenSize.Y / 2);
            spriteBatch = game.Services.GetService<SpriteBatch>();
            this.level = level;
            buildWindow = new BuildWindow(game, this, new Rectangle(0, 613, screenSize.X, 107));
            upNextWindow = new UpNextWindow(game, this, new Rectangle(0, 0, screenSize.X, 138));
            speedControlWindow = new SpeedControlsWindow(game, this, new Rectangle(screenSize.X-65, screenSize.Y-69, 64, 66));
            contextMenu = new TowerContextMenu(game, this, Rectangle.Empty);
            speedControlWindow.Enabled = false;
            ViewMatrix = game.ViewMatrix;
            LeftBorder = game.Content.Load<Texture2D>("Textures/UI/Left Border");
            RightBorder = game.Content.Load<Texture2D>("Textures/UI/Right Border");
            font = game.Content.Load<SpriteFont>("Fonts/console");
        }

        public override void Update(GameTime gameTime)
        {
            inputStates.Update();
            Vector2 pointer = (Vector2.Transform(inputStates.CurrentMouse.Position.ToVector2(), Matrix.Invert(ViewMatrix)));

            if (building != null)
            {
                //hide context menu while building.
                contextMenu.Hide();
                BuildingLogic(pointer);
            }
            //if we are not building, clicking on a tower opens context menu.
            else
            {
                if (inputStates.CurrentMouse.LeftButton == ButtonState.Released && inputStates.LastMouse.LeftButton == ButtonState.Pressed)
                {
                    Point p = HexEntity.PixelToHex(pointer - new Vector2(64, 0));
                    if (level.InBounds(p))
                    {
                        HexEntity Target = level.Map[p.Y, p.X];
                        if (Target.GetType().IsSubclassOf(typeof(Tower.Tower)))
                            contextMenu.Show((Tower.Tower)Target);
                        else
                            contextMenu.Hide();
                    }
                    else
                        contextMenu.Hide();
                }
                //right clicking hides open context menu
                if (inputStates.LastMouse.RightButton == ButtonState.Pressed && inputStates.CurrentMouse.RightButton == ButtonState.Released)
                {
                    contextMenu.Hide();
                }
            }

            buildWindow.Update(gameTime);
            upNextWindow.Update(gameTime);
            speedControlWindow.Update(gameTime);
            contextMenu.Update(gameTime);

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

        private void BuildingLogic(Vector2 pointer)
        {
            //right click to cancel building.
            if (inputStates.LastMouse.RightButton == ButtonState.Pressed && inputStates.CurrentMouse.RightButton == ButtonState.Released)
            {
                level.Building = building = null;
                return;
            }
            //cancel building if insuficient funds
            if (level.Money < building.Cost)
            {
                level.Building = building = null;
                return;
            }
            // Draw temporary building on hover
            Vector2 p = pointer - new Vector2(64, 0);
            building.Position = HexEntity.PixelToHex(p);
            building.ScreenPosition = HexEntity.CalculateScreenPosition(building.Position);

            if (level.IsPlaceable(building))
            {
                building.Color = new Color(128, 128, 128, 128);

                //after pressing LMB, place building
                if (inputStates.CurrentMouse.LeftButton == ButtonState.Released && inputStates.LastMouse.LeftButton == ButtonState.Pressed &&
                    !buildWindow.Bounds.Contains(inputStates.CurrentMouse.Position) &&
                    !upNextWindow.Bounds.Contains(inputStates.CurrentMouse.Position) &&
                    !speedControlWindow.Bounds.Contains(inputStates.CurrentMouse.Position) &&
                    !contextMenu.Bounds.Contains(inputStates.CurrentMouse.Position))
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

        public void DrawRenderTargets(GameTime gameTime)
        {
            contextMenu.DrawRenderTargets(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(LeftBorder, Vector2.Zero, Color.White);
            spriteBatch.Draw(RightBorder, Vector2.Zero, Color.White);
            buildWindow.Draw(gameTime);
            upNextWindow.Draw(gameTime);
            speedControlWindow.Draw(gameTime);
            contextMenu.Draw(gameTime);

            spriteBatch.DrawString(font, "$" + level.Money.ToString(), new Vector2(1076, 688), Color.Black);
        }
        

        

    }
}
