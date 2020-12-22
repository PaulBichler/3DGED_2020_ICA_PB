using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using GDLibrary.Utilities;
using Microsoft.Xna.Framework;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Moves the target actor along a predefined rail defined by RailParameters
    /// </summary>
    public class RailController : Controller
    {
        private Actor3D target;
        private RailParameters railParameters;
        private bool bFirstUpdate = true;

        public RailController(string id,
            ControllerType controllerType,
            Actor3D target, RailParameters railParameters) : base(id, controllerType)
        {
            this.target = target;
            this.railParameters = railParameters;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                if (bFirstUpdate)
                {
                    parent.Transform3D.Translation = railParameters.MidPoint;
                    bFirstUpdate = false;
                }

                //step 1 - camera to target, normalise
                Vector3 cameraToTarget = MathUtility.GetNormalizedObjectToTargetVector(parent.Transform3D, target.Transform3D);
                //round to prevent floating-point precision errors across updates
                cameraToTarget = MathUtility.Round(cameraToTarget, 3);

                //step 2 - get dot product of look(rail) . look(cameraToTarget)
                float dotProduct = Vector3.Dot(cameraToTarget, railParameters.Look);

                //step 3 - add dot * look(rail) + position(camera)
                Vector3 projectedCameraPosition = parent.Transform3D.Translation + dotProduct * railParameters.Look;// gameTime.ElapsedGameTime.Milliseconds; //removed gameTime multiplier - was causing camera judder when object close to camera
                projectedCameraPosition = MathUtility.Round(projectedCameraPosition, 3); //round to prevent floating-point precision errors across updates

                //step 4 - do not allow the camera to move outside the rail
                if (railParameters.InsideRail(projectedCameraPosition))
                    parent.Transform3D.Translation = projectedCameraPosition;

                //step 5 - update the look of the camera
                parent.Transform3D.Look = cameraToTarget;
            }

            // base.Update(gameTime, actor);
        }

        //to do...add Clone, Equals, GetHashCode
    }
}