using System;
using System.Collections.Generic;
using System.Linq;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Managers
{
    public class Tweener
    {
        public readonly Actor3D Actor;
        public readonly int TimeInMs;
        public readonly Vector3 Destination;
        public readonly EasingType EasingType;
        public readonly LoopType LoopType;
        public readonly bool Relative;
        public readonly Action<Actor3D> Callback;

        private Vector3 origin;
        private int currentTimeInMs;

        public Tweener(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null, LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
        {
            Actor = actor;
            TimeInMs = timeInMs;
            Destination = destination;
            EasingType = easingType;
            LoopType = loopType;
            Relative = relative;
            Callback = callback;

            origin = actor.Transform3D.Translation;
            currentTimeInMs = timeInMs;

            if (relative)
                this.Destination += actor.Transform3D.Translation;
        }

        public bool Process(GameTime gameTime)
        {
            if (Actor == null || (Actor.StatusType & StatusType.Update) != StatusType.Update)
                return false;

            currentTimeInMs -= gameTime.ElapsedGameTime.Milliseconds;

            if (currentTimeInMs <= 0)
            {
                Actor.Transform3D.Translation = Destination;

                switch (LoopType)
                {
                    case LoopType.PlayOnce:
                        Callback.Invoke(Actor);
                        return true;
                }
            }

            float x = 1f - (float)currentTimeInMs / TimeInMs;
            float easedValue = Easer.ApplyEasing(x, EasingType);
            Actor.Transform3D.Translation = Vector3.Lerp(origin, Destination, easedValue);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Actor, TimeInMs, Destination, (int) EasingType, (int) LoopType, Relative, Callback);
        }
        
        public override bool Equals(object? obj)
        {
            return obj is Tweener @object &&
                   Actor.Equals(@object.Actor) &&
                   TimeInMs == @object.TimeInMs &&
                   Destination.Equals(@object.Destination) &&
                   EasingType == @object.EasingType &&
                   LoopType == @object.LoopType &&
                   Relative == @object.Relative &&
                   Callback == @object.Callback;
        }
    }

    public class TweeningManager : PausableGameComponent
    {
        private HashSet<Tweener> tweens;
        private List<Tweener> tweensToAdd, tweensToRemove;

        public TweeningManager(Game game, StatusType statusType) : base(game, statusType)
        {
            tweens = new HashSet<Tweener>();
            tweensToAdd = new List<Tweener>();
            tweensToRemove = new List<Tweener>();
        }

        private void RemoveActorTweens(Actor3D actor)
        {
            foreach (Tweener tween in tweens)
                if(tween.Actor.Equals(actor))
                    tweensToRemove.Add(tween);
        }

        private void AddTween(Tweener tweener)
        {
            if(tweener != null)
                tweensToAdd.Add(tweener);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (tweensToRemove.Count > 0)
            {
                foreach (Tweener tweener in tweensToRemove)
                    tweens.Remove(tweener);

                tweensToRemove.Clear();
            }

            if (tweensToAdd.Count > 0)
            {
                foreach (Tweener tweener in tweensToAdd)
                    if (!tweens.Contains(tweener))
                        tweens.Add(tweener);

                tweensToAdd.Clear();
            }

            foreach (Tweener tweener in tweens)
                if(tweener.Process(gameTime))
                    tweensToRemove.Add(tweener);
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            EventDispatcher.Subscribe(EventCategoryType.Tween, HandleEvent);
        }

        public override void HandleEvent(EventData eventData)
        {
            base.HandleEvent(eventData);

            if (eventData.EventCategoryType == EventCategoryType.Tween)
            {
                switch (eventData.EventActionType)
                {
                    case EventActionType.OnAdd:
                        AddTween(eventData.Parameters[0] as Tweener);
                        break;
                    case EventActionType.OnRemoveActor:
                        RemoveActorTweens(eventData.Parameters[0] as Actor3D);
                        break;
                }
            }
            else if (eventData.EventCategoryType == EventCategoryType.Object)
                if (eventData.EventActionType == EventActionType.OnRemoveActor)
                    RemoveActorTweens(eventData.Parameters[0] as Actor3D);
        }
    }
}
