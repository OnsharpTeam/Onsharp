namespace Onsharp.Service
{
    /// <summary>
    /// The service provider takes care of the management of the <see cref="IService"/> instances
    /// as well as the provision of these.
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Returns the instance of the service belonging to the wanted type.
        /// </summary>
        /// <typeparam name="T">The type of the service which is requested</typeparam>
        /// <returns>The instance of the service or null if no service was found</returns>
        T Get<T>() where T : IService;

        /// <summary>
        /// Registers an instance as a service.
        /// </summary>
        /// <param name="service">The service to be registered</param>
        /// <typeparam name="T">The type of the service</typeparam>
        void Provide<T>(T service) where T : IService;
    }
}