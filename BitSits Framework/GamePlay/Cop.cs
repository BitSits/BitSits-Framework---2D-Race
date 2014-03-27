using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace BitSits_Framework
{
    class Cop : Car
    {
        public Cop(Vector2 position, GameContent gameContent, World world)
            : base(position, gameContent, world) { }
    }
}
