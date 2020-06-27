namespace Onsharp.IO
{
    /// <summary>
    /// The data storage interface for interacting with the IO of the onsharp system.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Retrieves the wanted data from the storage.
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="default">The default value if no data is found</param>
        /// <returns>The data belonging to the type</returns>
        T Retrieve<T>(T @default = default);

        /// <summary>
        /// Initializes the config and passes its to the data storage.
        /// </summary>
        /// <typeparam name="T">The type of the config</typeparam>
        /// <returns>The config object</returns>
        T Config<T>();
    }
}