using System;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Utilities
{
    public class CustomHelper
    {
        public static float GetAngleBetweenVectors(Vector3 vector1, Vector3 vector2)
        {
            float dot = Vector3.Dot(vector1, vector2);
            return MathF.Acos(dot / vector1.Length() * vector2.Length());
        }
    }
}
