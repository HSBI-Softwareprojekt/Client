using Unity.Services.Core.Internal;

namespace Unity.Services.Qos
{
    /// <summary>
    /// Component providing the qos service.
    /// </summary>
    public interface IQosServiceComponent : IServiceComponent
    {
        /// <summary>
        /// Returns the qos service
        /// </summary>
        IQosService Service { get; }
    }

    class QosServiceComponent : IQosServiceComponent
    {
        public IQosService Service { get; }

        internal QosServiceComponent(IQosService qos)
        {
            Service = qos;
        }
    }
}
