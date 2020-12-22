using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDGame
{
    public class EventHandler : IEventHandler
    {
        private IActor parent;
        private EventCategoryType eventCategoryType;

        public IActor Parent { get => parent; set => parent = value; }
        public EventCategoryType EventCategoryType { get => eventCategoryType; set => eventCategoryType = value; }

        public EventHandler(EventCategoryType eventCategoryType, IActor parent)
        {
            //store the parent actor that this event is attached tp
            this.Parent = parent;
            this.EventCategoryType = eventCategoryType;

            SubscribeToEvents();
        }

        public virtual void SubscribeToEvents()
        {
            //subscribe to the event so that HandleEvent() will be called
            EventDispatcher.Subscribe(eventCategoryType, HandleEvent);
        }

        public virtual void HandleEvent(EventData eventData)
        {
        }
    }
}