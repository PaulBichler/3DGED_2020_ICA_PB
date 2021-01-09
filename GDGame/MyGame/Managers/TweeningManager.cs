using System.Collections.Generic;
using GDGame.MyGame.Utilities;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Managers
{
    public class TweeningManager : PausableGameComponent
    {
        #region Fields
        private HashSet<Tween> tweens;
        private List<Tween> tweensToAdd, tweensToRemove;
        #endregion

        #region Constructor & Core
        public TweeningManager(Game game, StatusType statusType) : base(game, statusType)
        {
            tweens = new HashSet<Tween>();
            tweensToAdd = new List<Tween>();
            tweensToRemove = new List<Tween>();
        }

        private void RemoveActorTweens(Actor3D actor)
        {
            foreach (Tween tween in tweens)
                if (tween.Actor.Equals(actor))
                    tweensToRemove.Add(tween);
        }

        private List<Tween> GetActorTweens(Actor3D actor)
        {
            List<Tween> tweens = new List<Tween>();

            foreach (Tween tween in this.tweens)
                if (tween.Actor.Equals(actor))
                    tweens.Add(tween);

            return tweens;
        }

        private void AddTween(Tween tween)
        {
            if (tween != null)
                tweensToAdd.Add(tween);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //Batch remove
            if (tweensToRemove.Count > 0)
            {
                foreach (Tween tween in tweensToRemove)
                    tweens.Remove(tween);

                tweensToRemove.Clear();
            }

            //Batch add
            if (tweensToAdd.Count > 0)
            {
                foreach (Tween tween in tweensToAdd)
                    if (!tweens.Contains(tween))
                        tweens.Add(tween);

                tweensToAdd.Clear();
            }

            //Update all of the animations and remove the finished ones
            foreach (Tween tween in tweens)
                if (tween.Process(gameTime))
                    tweensToRemove.Add(tween);
        }

        public new void Dispose()
        {
            tweens.Clear();
            tweensToRemove.Clear();
            tweensToAdd.Clear();
        }
        #endregion

        #region Events
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
                List<Tween> tweensFound;
                switch (eventData.EventActionType)
                {
                    case EventActionType.OnAdd:
                        foreach (object parameter in eventData.Parameters)
                            AddTween(parameter as Tween);
                        break;
                    case EventActionType.OnAddChild:
                        tweensFound = GetActorTweens(eventData.Parameters[0] as Actor3D);
                        foreach (var tween in tweensFound)
                            tween.AddChild(eventData.Parameters[1] as Actor3D);
                        break;
                    case EventActionType.OnRemoveChild:
                        tweensFound = GetActorTweens(eventData.Parameters[0] as Actor3D);
                        foreach (var tween in tweensFound)
                            tween.RemoveChild(eventData.Parameters[1] as Actor3D);
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
        #endregion
    }
}
