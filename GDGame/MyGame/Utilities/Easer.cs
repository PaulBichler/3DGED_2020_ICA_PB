using System;
using GDGame.MyGame.Enums;

namespace GDGame.MyGame.Utilities
{
    public class Easer
    {
        public static float ApplyEasing(float x, EasingType easing)
        {
            switch (easing)
            {
                case EasingType.linear:
                    return x;
                case EasingType.easeIn:
                    return 1f - (float)Math.Cos((x * Math.PI) / 2);
                case EasingType.easeOut:
                    return (float)Math.Sin((x * Math.PI) / 2);
            }

            return x;
        }
    }
}
