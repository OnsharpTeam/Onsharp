namespace Onsharp.Enums
{
    /// <summary>
    /// The type of damage that can being dealt.
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// Defines damage dealt by <see cref="Onsharp.Enums.Weapon"/>
        /// </summary>
        Weapon = 1, 
        /// <summary>
        /// Defines damage dealt by explosion.
        /// </summary>
        Explosion = 2, 
        /// <summary>
        /// Defines damage dealt by fire.
        /// </summary>
        Fire = 3, 
        /// <summary>
        /// Defines damage dealt by falling from high places.
        /// </summary>
        Fall = 4,
        /// <summary>
        /// Defines damage dealt by vehicle collisions.
        /// </summary>
        Vehicle = 5
    }
}