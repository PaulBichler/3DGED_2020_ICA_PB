using System;
using System.Collections.Generic;
using GDGame.MyGame.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Utilities
{
    /// <summary>
    /// Base Tween class
    /// </summary>
    public abstract class Tween
    {
        #region Fields
        public readonly Actor3D Actor;
        public readonly int TimeInMs;
        public readonly EasingType EasingType;
        public readonly LoopType LoopType;
        public readonly bool Relative;
        public readonly Action<Actor3D> Callback;

        protected Vector3 destination;
        protected List<Actor3D> childActors;
        protected Vector3 origin, previous;
        protected int currentTimeInMs;

        private Vector3 relativeDestination;
        #endregion

        #region Constructor & Core
        protected Tween(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null,
            LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
        {
            Actor = actor;
            TimeInMs = timeInMs;
            EasingType = easingType;
            LoopType = loopType;
            Relative = relative;
            Callback = callback;

            this.destination = relativeDestination = destination;
            childActors = new List<Actor3D>();
            currentTimeInMs = timeInMs;

            Reset();
        }

        public virtual bool Process(GameTime gameTime)
        {
            if (Actor == null || (Actor.StatusType & StatusType.Update) != StatusType.Update)
                return false;

            currentTimeInMs -= gameTime.ElapsedGameTime.Milliseconds;
            return true;
        }

        protected virtual bool FinalProcess()
        {
            switch (LoopType)
            {
                case LoopType.PlayOnce:
                    Callback?.Invoke(Actor);
                    return true;
                case LoopType.ReverseAndRepeat:
                    Callback?.Invoke(Actor);
                    currentTimeInMs = TimeInMs;
                    Vector3 temp = origin;
                    origin = destination;
                    destination = temp;
                    break;
                case LoopType.Repeat:
                    Callback?.Invoke(Actor);
                    currentTimeInMs = TimeInMs;
                    if (Relative)
                    {
                        origin = destination;
                        destination += relativeDestination;
                    }
                    else
                        Reset();
                    break;
            }

            return false;
        }

        protected abstract void Reset();

        protected Vector3 ComputeNextValue()
        {
            float x = 1f - (float)currentTimeInMs / TimeInMs;
            float easedValue = Easer.ApplyEasing(x, EasingType);
            Vector3 next = Vector3.Lerp(origin, destination, easedValue);
            Vector3 vectorToApply = next - previous;
            previous = next;

            return vectorToApply;
        }

        public void AddChild(Actor3D child)
        {
            childActors.Add(child);
        }

        public void RemoveChild(Actor3D child)
        {
            childActors.Remove(child);
        }
        #endregion

        #region Equals & GetHashCode Overrides
        public override int GetHashCode()
        {
            return HashCode.Combine(Actor, TimeInMs, destination, (int)EasingType, (int)LoopType, Relative, Callback);
        }

        public override bool Equals(object obj)
        {
            return obj is Tween @object &&
                   Actor.Equals(@object.Actor) &&
                   TimeInMs == @object.TimeInMs &&
                   destination.Equals(@object.destination) &&
                   EasingType == @object.EasingType &&
                   LoopType == @object.LoopType &&
                   Relative == @object.Relative &&
                   Callback == @object.Callback;
        } 
        #endregion
    }

    /// <summary>
    /// Tween class to perform Translation Animations
    /// </summary>
    public class TranslationTween : Tween
    {
        #region Contructor & Core
        public TranslationTween(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null, LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
            : base(actor, timeInMs, destination, relative, callback, loopType, easingType)
        {
            if (relative)
                base.destination += actor.Transform3D.Translation;
        }

        public override bool Process(GameTime gameTime)
        {
            if (!base.Process(gameTime))
                return false;

            if (currentTimeInMs <= 0)
            {
                Actor.Transform3D.TranslateBy(destination - previous);
                return FinalProcess();
            }

            //Translate the actor(s)
            Vector3 translationToApply = ComputeNextValue();
            Actor.Transform3D.TranslateBy(translationToApply);
            foreach (var childActor in childActors)
                childActor.Transform3D.TranslateBy(translationToApply);

            return false;
        }

        protected override void Reset()
        {
            origin = previous = Actor.Transform3D.Translation;
        }

        #endregion

        #region Equals & GetHashCode Overrides
        public override bool Equals(object obj)
        {
            return obj is TranslationTween &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(GetType());
        } 
        #endregion
    }

    /// <summary>
    /// Tween class to perform Scale Animations
    /// </summary>
    public class ScaleTween : Tween
    {
        #region Constructor & Core
        public ScaleTween(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null, LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
            : base(actor, timeInMs, destination, relative, callback, loopType, easingType)
        {
            if (relative)
                base.destination += actor.Transform3D.Scale;
        }

        public override bool Process(GameTime gameTime)
        {
            if (!base.Process(gameTime))
                return false;

            if (currentTimeInMs <= 0)
            {
                Actor.Transform3D.Scale += (destination - previous);
                return FinalProcess();
            }

            //Translate the actor(s)
            Vector3 scaleToApply = ComputeNextValue();
            Actor.Transform3D.Scale += scaleToApply;
            foreach (var childActor in childActors)
                childActor.Transform3D.Scale += scaleToApply;

            return false;
        }

        protected override void Reset()
        {
            origin = previous = Actor.Transform3D.Scale;
        }

        #endregion

        #region Equals & GetHashCode Overrides
        public override bool Equals(object obj)
        {
            return obj is ScaleTween &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(GetType());
        } 
        #endregion
    }

    /// <summary>
    /// Tween class to perform Rotation Animations
    /// </summary>
    public class RotationTween : Tween
    {
        #region Constructor & Core
        public RotationTween(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null, LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
            : base(actor, timeInMs, destination, relative, callback, loopType, easingType)
        {
            if (relative)
                base.destination += actor.Transform3D.RotationInDegrees;
        }

        public override bool Process(GameTime gameTime)
        {
            if (!base.Process(gameTime))
                return false;

            if (currentTimeInMs <= 0)
            {
                Actor.Transform3D.RotationInDegrees += (destination - previous);
                return FinalProcess();
            }

            //Translate the actor(s)
            Vector3 scaleToApply = ComputeNextValue();
            Actor.Transform3D.RotationInDegrees += scaleToApply;
            foreach (var childActor in childActors)
                childActor.Transform3D.RotationInDegrees += scaleToApply;

            return false;
        }

        protected override void Reset()
        {
            origin = previous = Actor.Transform3D.RotationInDegrees;
        }

        #endregion

        #region Equals & GetHashCode Overrides
        public override bool Equals(object obj)
        {
            return obj is RotationTween &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + HashCode.Combine(GetType());
        } 
        #endregion
    }
}
