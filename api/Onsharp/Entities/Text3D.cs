using System;
using System.Collections.Generic;
using Onsharp.Enums;
using Onsharp.Native;
using Onsharp.World;

namespace Onsharp.Entities
{
    public class Text3D : LifelessEntity
    {
        /// <summary>
        /// The context text of the 3D text.
        /// </summary>
        public string Text
        {
            get => InternalText;
            set
            {
                InternalText = value;
                Onset.SetText3DText(Id, value);
            } 
        }

        internal string InternalText { get; set; }
        
        public Text3D(int id) : base(id, "Text3D")
        {
        }

        public override void SetPosition(double x, double y, double z)
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }

        public override void SetPosition(Vector vector)
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }

        public override Vector GetPosition()
        {
            throw new NotImplementedException("Text3D does not implement Positioning");
        }

        /// <summary>
        /// Sets the visibility of the 3D text for the given players.
        /// </summary>
        /// <param name="visible">True, if the 3D text should be visible</param>
        /// <param name="players">The players for which the visibility should be changed</param>
        public void SetVisibilityFor(bool visible, params Player[] players)
        {
            foreach (Player player in players)
            {
                player.SetText3DVisibility(this, visible);
            }
        }

        /// <summary>
        /// Sets the visibility of the 3D text for the given players.
        /// </summary>
        /// <param name="visible">True, if the 3D text should be visible</param>
        /// <param name="players">The players for which the visibility should be changed</param>
        public void SetVisibilityFor(bool visible, List<Player> players)
        {
            foreach (Player player in players)
            {
                player.SetText3DVisibility(this, visible);
            }
        }

        /// <summary>
        /// Attaches the 3D text to the given entity.
        /// </summary>
        /// <param name="target">The entity on which the object is attached</param>
        /// <param name="pos">The relative position</param>
        /// <param name="rot">The relative rotation</param>
        /// <param name="socketName">The socket where to attach to. See https://dev.playonset.com/wiki/PlayerBones</param>
        public void Attach(Entity target, Vector pos, Vector rot, string socketName = "")
        {
            AttachType type = Bridge.GetTypeByEntity(target);
            if(type == AttachType.None) throw new ArgumentException("The target entity is not a valid attach target entity!");
            Onset.SetText3DAttached(Id, (int) type, target.Id, pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, socketName);
        }
    }
}