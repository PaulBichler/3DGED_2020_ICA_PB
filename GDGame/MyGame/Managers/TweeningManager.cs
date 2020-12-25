using System;
using System.Collections.Generic;
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
        public Actor3D actor;
        public int timeInMs;
        public Vector3 destination;
        public EasingType easingType;
        public LoopType loopType;
        public bool relative;
        public Action<Actor3D> callback;

        private Vector3 origin;
        private int currentTimeInMs;

        public Tweener(Actor3D actor, int timeInMs, Vector3 destination, bool relative, Action<Actor3D> callback = null, LoopType loopType = LoopType.PlayOnce, EasingType easingType = EasingType.linear)
        {
            this.actor = actor;
            this.timeInMs = timeInMs;
            this.destination = destination;
            this.easingType = easingType;
            this.loopType = loopType;
            this.relative = relative;
            this.callback = callback;

            origin = actor.Transform3D.Translation;
            currentTimeInMs = timeInMs;

            if (relative)
                this.destination += actor.Transform3D.Translation;
        }

        public bool Process(GameTime gameTime)
        {
            if (actor == null || (actor.StatusType & StatusType.Update) != StatusType.Update)
                return false;

            currentTimeInMs -= gameTime.ElapsedGameTime.Milliseconds;

            if (currentTimeInMs <= 0)
            {
                actor.Transform3D.Translation = destination;

                switch (loopType)
                {
                    case LoopType.PlayOnce:
                        callback.Invoke(actor);
                        return true;
                }
            }

            float x = 1f - (float)currentTimeInMs / timeInMs;
            float easedValue = Easer.ApplyEasing(x, easingType);
            actor.Transform3D.Translation = Vector3.Lerp(origin, destination, easedValue);

            return false;
        }
    }

    public class TweeningManager : PausableGameComponent
    {
        private Dictionary<string, Tweener> tweens;
        private List<Tweener> tweensToAdd;
        private List<string> tweensToRemove;

        public TweeningManager(Game game, StatusType statusType) : base(game, statusType)
        {
            tweens = new Dictionary<string, Tweener>();
            tweensToAdd = new List<Tweener>();
            tweensToRemove = new List<string>();
        }

        private void RemoveActorTween(Actor3D actor)
        {
            if(tweens.ContainsKey(actor.ID))
                tweensToRemove.Add(actor.ID);
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
                foreach (string key in tweensToRemove)
                    tweens.Remove(key);

                tweensToRemove.Clear();
            }

            if (tweensToAdd.Count > 0)
            {
                foreach (Tweener tweener in tweensToAdd)
                    if (!tweens.ContainsKey(tweener.actor.ID))
                        tweens.Add(tweener.actor.ID, tweener);

                tweensToAdd.Clear();
            }

            foreach (KeyValuePair<string, Tweener> tweener in tweens)
                if(tweener.Value.Process(gameTime))
                    tweensToRemove.Add(tweener.Key);
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
                        RemoveActorTween(eventData.Parameters[0] as Actor3D);
                        break;
                }
            }
            else if (eventData.EventCategoryType == EventCategoryType.Object)
                if (eventData.EventActionType == EventActionType.OnRemoveActor)
                    RemoveActorTween(eventData.Parameters[0] as Actor3D);
        }
    }
}
