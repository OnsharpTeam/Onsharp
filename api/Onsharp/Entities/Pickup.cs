using System;
using System.Collections.Generic;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Pickup : LifelessEntity
    {
        /// <summary>
        /// The scale of the pickup.
        /// </summary>
        public Vector Scale
        {
            get
            {
                double x = 0;
                double y = 0;
                double z = 0;
                Onset.GetPickupScale(Id, ref x, ref y, ref z);
                Vector vector = new Vector(x, y, z);
                vector.SyncCallback = () => Scale = vector;
                return vector;
            }
            set => Onset.SetPickupScale(Id, value.X, value.Y, value.Z);
        }
        
        public Pickup(int id) : base(id, "Pickup")
        {
        }

        public override Vector GetPosition()
        {
            throw new NotImplementedException("Pickup does not implement Getter Positioning");
        }

        /// <summary>
        /// Sets the visibility of the pickup for the given players.
        /// </summary>
        /// <param name="visible">True, if the pickup should be visible</param>
        /// <param name="players">The players for which the visibility should be changed</param>
        public void SetVisibilityFor(bool visible, params Player[] players)
        {
            foreach (Player player in players)
            {
                player.SetPickupVisibility(this, visible);
            }
        }

        /// <summary>
        /// Sets the visibility of the pickup for the given players.
        /// </summary>
        /// <param name="visible">True, if the pickup should be visible</param>
        /// <param name="players">The players for which the visibility should be changed</param>
        public void SetVisibilityFor(bool visible, List<Player> players)
        {
            foreach (Player player in players)
            {
                player.SetPickupVisibility(this, visible);
            }
        }
    }
}