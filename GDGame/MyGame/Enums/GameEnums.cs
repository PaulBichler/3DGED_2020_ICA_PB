using System;

namespace GDGame.MyGame.Enums
{
    [Flags]
    public enum CollisionLayer : sbyte
    {
        Default,
        Projectile,
        Player
    }

    public enum GameObjectType
    {
        Player,
        GrassTile,
        WaterTile,
        RoadTile,
        Projectile
    }

    public enum LoopType
    {
        PlayOnce,
        PlayOnceAndReverse,
        Repeat,
        ReverseAndRepeat
    }

    public enum EasingType
    {
        linear,
        easeIn,
        easeOut
    }
}
