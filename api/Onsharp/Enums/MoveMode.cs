namespace Onsharp.Enums
{
    /// <summary>
    /// Defines the player's move mode and state.
    /// </summary>
    public enum MoveMode
    {
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is standing still.
        /// </summary>
        StandingStill,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> aims with e.g. a gun while walking.
        /// </summary>
        AimWalking,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is just walking.
        /// </summary>
        Walking,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is running.
        /// </summary>
        Running,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is crouching
        /// </summary>
        Crouched,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is falling.
        /// </summary>
        Falling,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is sky diving.
        /// </summary>
        Skydiving,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> has its parachute open.
        /// </summary>
        Parachuting,
        /// <summary>
        /// A <see cref="Onsharp.Entities.Player"/> is swimming.
        /// </summary>
        Swimming
    }
}