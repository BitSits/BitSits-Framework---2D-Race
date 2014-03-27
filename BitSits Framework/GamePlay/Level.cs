using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;

namespace BitSits_Framework
{
    class Level : IDisposable
    {
        #region Fields

        public int Score { get; private set; }

        public bool IsLevelUp { get; private set; }
        public bool ReloadLevel { get; private set; }
        int levelIndex;

        GameContent gameContent;
        World world = new World(new Vector2(0, 0), true);

        Texture2D[,] tiles; 

        Camera2D camera;
        Car car;

        #endregion

        #region Initialization


        public Level(GameContent gameContent, int levelIndex)
        {
            this.gameContent = gameContent;
            this.levelIndex = levelIndex;

            LoadTiles(levelIndex);
            //LoadGround();

            car = new Car(new Vector2(-200), gameContent, world);

            camera = new Camera2D(gameContent.viewportSize, false);
            camera.Position = gameContent.viewportSize / 2;
            camera.Origin = new Vector2(400, 550);
            camera.Scale = .6f;
        }

        void LoadGround()
        {
            Body body = world.CreateBody(new BodyDef());

            PolygonShape shape = new PolygonShape();
            shape.SetAsEdge(Vector2.Zero, new Vector2(0, 600) / gameContent.Scale);
            body.CreateFixture(shape, 0);

            shape.SetAsEdge(new Vector2(0, 600) / gameContent.Scale, new Vector2(800, 600) / gameContent.Scale);
            body.CreateFixture(shape, 0);

            shape.SetAsEdge(new Vector2(800, 600) / gameContent.Scale, new Vector2(800, 0) / gameContent.Scale);
            body.CreateFixture(shape, 0);

            shape.SetAsEdge(new Vector2(800, 0) / gameContent.Scale, Vector2.Zero / gameContent.Scale);
            //body.CreateFixture(shape, 0);
        }

        void LoadTiles(int levelIndex)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            lines = gameContent.content.Load<List<string>>("Levels/worldMap");

            width = lines[0].Length;

            tiles = new Texture2D[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < lines.Count; ++y)
            {
                if (lines[y].Length != width)
                    throw new Exception(String.Format(
                        "The length of line {0} is different from all preceeding lines.", lines.Count));

                for (int x = 0; x < lines[0].Length; ++x)
                {
                    // to load each tile.
                    LoadTile(lines[y][x], x, y);
                }
            }
        }

        void LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '.': tiles[x, y] = gameContent.road; break;
                case '#': tiles[x, y] = gameContent.block;
                    Body b = world.CreateBody(new BodyDef());
                    b.Position = (new Vector2(x, y) * 100 + new Vector2(100) / 2) / gameContent.Scale;
                    PolygonShape ps = new PolygonShape();
                    ps.SetAsBox(50f / gameContent.Scale, 50f / gameContent.Scale);
                    b.CreateFixture(ps, 0);
                    break;
                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format(
                        "Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public void Dispose() { }


        #endregion

        #region Update and HandleInput

        public void Update(GameTime gameTime)
        {
            car.Update(gameTime);

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 3, 8);

            UpdateCameraChaseTarget(gameTime);
        }

        void UpdateCameraChaseTarget(GameTime gameTime)
        {
            float stiffness = 1800.0f, damping = 600.0f, mass = 50.0f;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate spring force
            Vector2 stretch = camera.Position - car.body.Position * gameContent.Scale;
            Vector2 force = -stiffness * stretch - damping * camera.Velocity;

            // Apply acceleration
            Vector2 acceleration = force / mass;
            camera.Velocity += acceleration * elapsed;

            // Apply velocity
            camera.Position += camera.Velocity * elapsed;

            // Rotation
            float angularStretch = camera.Rotation - car.body.Rotation;
            float angularForce = -stiffness * angularStretch - damping * camera.angularVelocity;
            float angularAcceleration = angularForce / mass;
            camera.angularVelocity += angularAcceleration * elapsed;
            camera.Rotation += camera.angularVelocity * elapsed;
        }

        public void HandleInput(InputState input, int playerIndex)
        {
            camera.HandleInput(input, playerIndex);

            if (input.CurrentMouseState[0].LeftButton == ButtonState.Pressed ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Up) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.W)) car.engineSpeed = car.MaxEngineSpeed;

            if (input.CurrentMouseState[0].RightButton == ButtonState.Pressed ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Down) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.S)) 
                car.engineSpeed = -car.MaxEngineSpeed * 0.3f;

            car.steerAngle = 0;
            if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Left) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.A)) car.steerAngle -= car.MaxSteerAngle;
            else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Right) ||
                input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.D)) car.steerAngle += car.MaxSteerAngle;
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.End();            

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                SaveStateMode.SaveState, camera.Transform);

            spriteBatch.Draw(gameContent.blank, new Rectangle(0, 0, 800, 600), Color.White);

            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    spriteBatch.Draw(tiles[i, j], new Vector2(i * 100, j * 100), Color.White);

            Vector2 v = camera.Position - gameContent.viewportSize;
            v = new Vector2((int)(v.X / 100) * 100, (int)(v.Y / 100) * 100);

            for (int i = 0; i < 16; i++) for (int j = 0; j < 12; j++)
                    spriteBatch.Draw(gameContent.tile, v + new Vector2(i, j) * 100, Color.White);

            car.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
        }

        #endregion
    }
}
