using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Controllers
{
    public class RotationController : Controller
    {
        private float rotationSpeed;
        private Vector3 rotationAxis;

        public RotationController(string id, ControllerType controllerType,
         float rotationSpeed, Vector3 rotationAxis) : base(id, controllerType)
        {
            this.rotationSpeed = rotationSpeed;
            this.rotationAxis = rotationAxis;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                parent.Transform3D.RotationInDegrees += rotationSpeed * rotationAxis;
            }

            base.Update(gameTime, actor);
        }

        public new object Clone()
        {
            return new RotationController("clone - " + this.ID,
                this.ControllerType, this.rotationSpeed, this.rotationAxis);
        }

        public override bool Equals(object obj)
        {
            return obj is RotationController controller &&
                   ID == controller.ID &&
                   ControllerType == controller.ControllerType &&
                   StatusType == controller.StatusType &&
                   rotationSpeed == controller.rotationSpeed &&
                   rotationAxis.Equals(controller.rotationAxis);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, ControllerType, StatusType, rotationSpeed, rotationAxis);
        }
    }
}