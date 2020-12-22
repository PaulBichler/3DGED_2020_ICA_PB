using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;

namespace GDLibrary.Core.Managers.State
{
    /// <summary>
    /// Use this manager to listen for related events and perform actions in your game based on events received
    /// </summary>
    public class MyGameStateManager : PausableGameComponent, IEventHandler
    {
        public MyGameStateManager(Game game, StatusType statusType) : base(game, statusType)
        {
        }

        public override void SubscribeToEvents()
        {
            //add new events here...

            base.SubscribeToEvents();
        }

        public override void HandleEvent(EventData eventData)
        {
            //add new if...else if statements to handle events here...

            //remember to pass the eventData down so the parent class can process pause/unpause
            base.HandleEvent(eventData);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            //add code here to check for the status of a particular set of related events e.g. collect all inventory items then...

            base.ApplyUpdate(gameTime);
        }
    }
}