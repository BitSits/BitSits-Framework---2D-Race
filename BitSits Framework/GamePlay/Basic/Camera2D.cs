using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    /// <summary>
    /// Very basic sample program for demonstrating a 2D Camera
    /// Controls are WASD for movement, QE for rotation, and ZC for zooming.
    /// </summary>
    class Camera2D
    {
        Vector2 viewportSize;
        public Vector2 Position;
        public float Rotation, Scale, Speed;
        public bool ManualCamera;
        public int ScrollHeight, ScrollWidth;
        public Vector2 ScrollBar;

        public Vector2 Origin;

        public Vector2 Velocity;
        public float angularVelocity;

        public Camera2D(Vector2 viewportSize, bool manualcamera)
        {
            this.ManualCamera = manualcamera;
            this.viewportSize = viewportSize;

            ScrollWidth = ScrollHeight = int.MaxValue;

            Origin = viewportSize / 2;

            //Position = viewportSize / 2;
            Scale = 1;
            Speed = 10;
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position, 0))
                    * Matrix.CreateRotationZ(-Rotation) * Matrix.CreateScale(new Vector3(Scale, Scale, 0))
                    * Matrix.CreateTranslation(new Vector3(Origin, 0));
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void HandleInput(InputState input, int playerIndex)
        {
            if (ManualCamera)
            {
                //translation controls WASD
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.A)) Position.X += Speed;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.D)) Position.X -= Speed;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.S)) Position.Y -= Speed;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.W)) Position.Y += Speed;

                //rotation controls QE
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Q)) Rotation += 0.01f;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.E)) Rotation -= 0.01f;

                //zoom/scale controls CZ
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.C)) Scale += 0.001f;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.Z)) Scale -= 0.001f;
            }

            /*
            Vector2 mousePos = new Vector2(input.CurrentMouseState[0].X, input.CurrentMouseState[0].Y);
            if (mousePos.X < ScrollBar.X) Position.X -= Speed;
            else if (mousePos.X > viewportSize.X - ScrollBar.X) Position.X += Speed;

            if (mousePos.Y < ScrollBar.Y) Position.Y -= Speed;
            else if (mousePos.Y > viewportSize.Y - ScrollBar.Y) Position.Y += Speed;
            */

            // Clamp
            //Position.X = MathHelper.Clamp(Position.X, viewportSize.X / 2 / Scale,
            //    (ScrollWidth - viewportSize.X / 2 / Scale));
            //Position.Y = MathHelper.Clamp(Position.Y, viewportSize.Y / 2 / Scale,
            //    (ScrollHeight - viewportSize.Y / 2 / Scale));
        }
    }
}
