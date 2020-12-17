namespace Onsharp.Service
{
    /// <summary>
    /// The service interface is used for identifying services. A service can be some kind of API
    /// which needs to be easily transfered to another plugin. If you don't want to expose any classes
    /// other than the API classes, the service provider of Onsharp will take care of the transfer from the
    /// internal plugin with the internals of the API to the other plugins which will use the API as a service.
    /// </summary>
    public interface IService
    {
    }
}