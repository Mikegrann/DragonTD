using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DragonTD
{
    /// <summary>
    /// This is the main type for your game. HELLO WORLD
    /// </summary>
    public class DragonTDGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;
        UI.UI ui;

        Rectangle PlayArea = new Rectangle(112, 102, 1056, 518);

        public Matrix ViewMatrix;

        Texture2D whiteSquare;

        public DragonTDGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            ViewMatrix = Matrix.CreateTranslation(new Vector3(288, 276, 0)) * Matrix.CreateScale(0.5f);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //adds spritebatch to the service list, so that it can be used anywhere.
            this.Services.AddService<SpriteBatch>(spriteBatch);

            level = new Level(this, new Point(8, 4));


            float scaleW = PlayArea.Width / level.ScreenSize.X;
            float scaleH = PlayArea.Height / level.ScreenSize.Y;

            float scale = (scaleW < scaleH) ? scaleW : scaleH;
            
            Vector2 s = new Vector2(PlayArea.X + (PlayArea.Width / 2f)  - ((level.ScreenSize.X * scale)/2f) + (level.ScreenOffset.X * scale), 
                                    PlayArea.Y + (PlayArea.Height / 2f) - ((level.ScreenSize.Y * scale)/2f) + (level.ScreenOffset.Y * scale) );

            ViewMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(new Vector3(s.X, s.Y, 0));

            ui = new UI.UI(this, level);

            whiteSquare = Content.Load<Texture2D>("Textures/UI/whiteSquare");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                level.Update(gameTime);
                ui.Update(gameTime);
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //renders the context menu's render target before rendering everything else, because switching
            //render targets clears the backbuffer.
            ui.DrawRenderTargets(gameTime);

            //Draw Level
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/Background/Background"), new Vector2(0, 0), Color.White); // TODO: Move background drawing back to Level somehow
            spriteBatch.End();
            //for now, draw at 0.5 scale.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, ViewMatrix);
            level.Draw(gameTime);
            spriteBatch.End();
            //Then Draw UI
            spriteBatch.Begin();
            ui.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
