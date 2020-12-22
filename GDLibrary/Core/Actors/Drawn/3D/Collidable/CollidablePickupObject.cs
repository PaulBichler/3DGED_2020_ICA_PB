using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Represent a pickup within the game
    /// </summary>
    public class CollidablePickupObject : CollidablePrimitiveObject
    {
        #region Variables
        private PickupParameters pickupParameters;
        #endregion Variables

        #region Properties

        public PickupParameters PickupParameters
        {
            get
            {
                return pickupParameters;
            }
            set
            {
                pickupParameters = value;
            }
        }

        #endregion Properties

        //used to draw collidable primitives that a value associated with them e.g. health
        public CollidablePickupObject(string id, ActorType actorType,
            StatusType statusType, Transform3D transform,
            EffectParameters effectParameters,
            IVertexData vertexData,
             ICollisionPrimitive collisionPrimitive,
             ObjectManager objectManager, PickupParameters pickupParameters)
            : base(id, actorType, statusType, transform, effectParameters, vertexData, collisionPrimitive, objectManager)
        {
            this.pickupParameters = pickupParameters;
        }

        public new object Clone()
        {
            return new CollidablePickupObject("clone - " + ID, //deep
             ActorType, //deep
             StatusType, //deep
             Transform3D.Clone() as Transform3D, //deep
             EffectParameters.Clone() as EffectParameters, //deep
             this.IVertexData, //shallow - its ok if objects refer to the same vertices
             CollisionPrimitive.Clone() as ICollisionPrimitive, //deep
             ObjectManager, //shallow - reference
             pickupParameters.Clone() as PickupParameters); //deep
        }
    }

    /// <summary>
    /// Encapsulates the parameters for a collectable collidable object (e.g. "ammo", 10)
    /// </summary>
    public class PickupParameters
    {
        #region Fields
        private string description;
        private float value;

        //an optional array to store multiple parameters (used for play with sound/video when we pickup this object)
        private object[] additionalParameters;

        #endregion Fields

        #region Properties

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = (value.Length != 0) ? value : "no description specified";
            }
        }

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = (value >= 0) ? value : 0;
            }
        }

        public object[] AdditionalParameters
        {
            get
            {
                return additionalParameters;
            }
            set
            {
                additionalParameters = value;
            }
        }

        #endregion Properties

        public PickupParameters(string description, float value)
            : this(description, value, null)
        {
        }

        public PickupParameters(string description, float value, object[] additionalParameters)
        {
            this.value = value;
            this.description = description;
            this.additionalParameters = additionalParameters;
        }

        public override bool Equals(object obj)
        {
            PickupParameters other = obj as PickupParameters;
            bool bEquals = description.Equals(other.Description) && value == other.Value;
            return bEquals && ((additionalParameters != null && additionalParameters.Length != 0) ? additionalParameters.Equals(other.additionalParameters) : true);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 11 + description.GetHashCode();
            hash = hash * 17 + value.GetHashCode();

            if (additionalParameters != null && additionalParameters.Length != 0)
            {
                hash = hash * 31 + additionalParameters.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            return "Desc.:" + description + ", Value: " + value;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}