using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    /// <summary>
    /// All the Contents of the Game is loaded and stored here
    /// so that all other screen can copy from here
    /// </summary>
    public class GameContent
    {
        public readonly ContentManager content;
        public readonly Vector2 viewportSize;
        
        public readonly Random random = new Random();

        public readonly float Scale = 30;

        // Textures
        public readonly Texture2D blank, gradient;
        public readonly Texture2D menuBackground, mainMenuTitle;

        public readonly Texture2D playerCar;
        public readonly Vector2 playerCarOrigin;

        public readonly Texture2D tile, road, block;

        // Fonts
        public readonly SpriteFont debugFont;

        // Audio objects
        public readonly AudioEngine audioEngine;
        public readonly SoundBank soundBank;
        public readonly WaveBank waveBank;
        

        /// <summary>
        /// Load GameContents
        /// </summary>
        public GameContent(GameComponent screenManager)
        {
            content = screenManager.Game.Content; 
            Viewport viewport = screenManager.Game.GraphicsDevice.Viewport;
            viewportSize = new Vector2(viewport.TitleSafeArea.Width, viewport.TitleSafeArea.Height);

            blank = content.Load<Texture2D>("Graphics/blank");
            gradient = content.Load<Texture2D>("Graphics/gradient");
            menuBackground = content.Load<Texture2D>("Graphics/menuBackground");

            mainMenuTitle = content.Load<Texture2D>("Graphics/mainMenuTitle");

            tile = content.Load<Texture2D>("Graphics/tile");
            road = content.Load<Texture2D>("Graphics/road");
            block = content.Load<Texture2D>("Graphics/block");

            playerCar = content.Load<Texture2D>("Graphics/Road_Fighter_Player");
            playerCarOrigin = new Vector2(playerCar.Width / 2, playerCar.Height);

            debugFont = content.Load<SpriteFont>("Fonts/debugFont");


            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload GameContents
        /// </summary>
        public void UnloadContent() { content.Unload(); }
    }
}
