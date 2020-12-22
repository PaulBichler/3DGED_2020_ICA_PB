using System;
using System.Collections.Generic;
using System.Text;

namespace GDLibrary.Interfaces
{
    public interface IDebugInfo
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont);
    }
}
