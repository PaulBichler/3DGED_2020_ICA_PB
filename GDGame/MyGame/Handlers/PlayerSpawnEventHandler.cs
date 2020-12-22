using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Interfaces;

namespace GDGame
{
    public class PlayerSpawnEventHandler : EventHandler
    {
        public PlayerSpawnEventHandler(EventCategoryType eventCategoryType, IActor parent)
            : base(eventCategoryType, parent)
        {
        }

        public override void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnSpawn)
            {
                //object[] parameters = eventData.Parameters;
                //Vector3 pos = (Vector3)parameters[2];

                //Actor3D p = Parent as Actor3D;

                //p.StatusType = StatusType.Off;
            }
        }
    }
}