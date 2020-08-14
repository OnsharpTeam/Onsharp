namespace Onsharp.Commands
{
    /// <summary>
    /// The command failure enum defines the flag which will be set for the command failure event.
    /// </summary>
    public enum CommandFailure
    {
        /// <summary>
        /// No error. Just a placeholder.
        /// </summary>
        None,
        /// <summary>
        /// The command could not be found by the given name.
        /// </summary>
        NoCommand,
        /// <summary>
        /// The entered arguments are not enough to fulfill the required argument list.
        /// </summary>
        TooFewArgs,
        /// <summary>
        /// The player has not enough permission to run this command.
        /// </summary>
        NoPermissions
    }
}