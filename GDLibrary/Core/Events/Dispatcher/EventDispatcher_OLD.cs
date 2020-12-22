using GDLibrary.Enums;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GDLibrary.Events
{
    public class EventDispatcher_OLD : GameComponent
    {
        private static Queue<EventData> queue;

        public EventDispatcher_OLD(Game game) : base(game)
        {
            queue = new Queue<EventData>();
        }

        //pointer to a function, event that stores the list of functions that subscribed
        public delegate void CameraEventHandler(string s);
        public event CameraEventHandler CameraChanged;

        public delegate void PlayerEventHandler(string s);
        public event PlayerEventHandler PlayerChanged;

        public static void Publish(EventData eventData)
        {
            queue.Enqueue(eventData);
        }

        public override void Update(GameTime gameTime)
        {
            Process();
            base.Update(gameTime);
        }
        private void Process()
        {
            for(int i = 0; i < queue.Count; i++)
            {
                EventData evt = queue.Dequeue();

                switch (evt.EventCategoryType)
                {
                    case EventCategoryType.Player:
                        OnPlayerChanged(evt);
                        break;

                    case EventCategoryType.Camera:
                        OnCameraChanged(evt);
                        break;

                    default:
                        break;
                }
            }
        }

        public void OnPlayerChanged(EventData evt)
        {
            PlayerChanged?.Invoke("player changed data..."  + evt);
        }

        public void OnCameraChanged(EventData evt)
        {
            CameraChanged?.Invoke("camera changed data..." + evt);
        }

    }
}
