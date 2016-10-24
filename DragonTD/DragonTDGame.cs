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
        UI ui;

        public Matrix ViewMatrix;

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

            level = new Level(this);
            ui = new UI(this, level);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

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
