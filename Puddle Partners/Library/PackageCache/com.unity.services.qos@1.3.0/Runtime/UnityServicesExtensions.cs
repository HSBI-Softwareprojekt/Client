using Unity.Services.Qos;

namespace Unity.Services.Core
{
    /// <summary>
    /// Unity services extensions
    /// </summary>
    public static class UnityServicesExtensions
    {
        /// <summary>
        /// Retrieve the Qos service from the core service registry
        /// </summary>
        /// <param name="unityServices">The core services instance</param>
        /// <returns>The Qos service instance</returns>
        public static IQosService GetQosService(this IUnityServices unityServices)
        {
            return unityServices.GetService<IQosService>();
        }
    }
}
