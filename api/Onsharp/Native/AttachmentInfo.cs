using Onsharp.Entities;
using Onsharp.Enums;

namespace Onsharp.Native
{
    /// <summary>
    /// The struct containing information about an objects attachment, if it is attached.
    /// </summary>
    public readonly struct AttachmentInfo
    {
        /// <summary>
        /// The type of the attachment.
        /// </summary>
        public AttachType Type { get; }
        
        /// <summary>
        /// The entity to which the object is attached to.
        /// </summary>
        public Entity Target { get; }

        internal AttachmentInfo(AttachType type, Entity target)
        {
            Type = type;
            Target = target;
        }
    }
}