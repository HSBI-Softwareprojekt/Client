using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Qos.Apis.QosDiscovery;
using Unity.Services.Qos.Http;
using Unity.Services.Qos.Internal;
using Unity.Services.Qos.Runner;
using UnityEngine;

namespace Unity.Services.Qos
{
    class QosPackageInitializer : IInitializablePackageV2
    {
        const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";
        const string k_PackageName = "com.unity.services.qos";
        const string k_StagingEnvironment = "staging";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void InitializeOnLoad()
        {
            var initializer = new QosPackageInitializer();
            initializer.Register(CorePackageRegistry.Instance);
        }

        public void Register(CorePackageRegistry registry)
        {
            registry.Register(this)
                .DependsOn<IAccessToken>()
                .DependsOn<IMetricsFactory>()
                .DependsOn<IProjectConfiguration>()
                .ProvidesComponent<IQosResults>()
                .ProvidesComponent<IQosServiceComponent>();
        }

        internal void Register(CoreRegistry registry)
        {
            registry.RegisterPackage(this)
                .DependsOn<IAccessToken>()
                .DependsOn<IMetricsFactory>()
                .DependsOn<IProjectConfiguration>()
                .ProvidesComponent<IQosResults>()
                .ProvidesComponent<IQosServiceComponent>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            QosService.Instance = InitializeService(registry);
            return Task.CompletedTask;
        }

        public Task InitializeInstanceAsync(CoreRegistry registry)
        {
            InitializeService(registry);
            return Task.CompletedTask;
        }

        IQosService InitializeService(CoreRegistry registry)
        {
            var projectConfiguration = registry.GetServiceComponent<IProjectConfiguration>();

            var accessTokenQosDiscovery = registry.GetServiceComponent<IAccessToken>();
            var metricsFactory = registry.GetServiceComponent<IMetricsFactory>();
            var metrics = metricsFactory.Create(k_PackageName);

            var httpClient = new HttpClient();

            // Set up internal QoS Discovery API client & config
            var internalQosService = new InternalQosDiscoveryService(GetHost(projectConfiguration), httpClient, accessTokenQosDiscovery);

            var httpClientV2 = new V2.Http.HttpClient();
            var v2Config = new V2.Configuration(basePath: GetHost(projectConfiguration), requestTimeout: 10, numRetries: 4, headers: null);
            var qosDiscoveryApiClientV2 = new V2.Apis.QosDiscovery.QosDiscoveryApiClient(httpClientV2, accessTokenQosDiscovery, v2Config);

            // Set up public QoS interface
            var wrappedQosService = new WrappedQosService(internalQosService.QosDiscoveryApi, qosDiscoveryApiClientV2,
                new BaselibQosRunner(), accessTokenQosDiscovery, metrics);

            registry.RegisterService<IQosService>(wrappedQosService);
            registry.RegisterServiceComponent<IQosResults>(new QosResults(wrappedQosService));
            registry.RegisterServiceComponent<IQosServiceComponent>(new QosServiceComponent(wrappedQosService));

            return wrappedQosService;
        }

        string GetHost(IProjectConfiguration projectConfiguration)
        {
            var cloudEnvironment = projectConfiguration?.GetString(k_CloudEnvironmentKey);

            switch (cloudEnvironment)
            {
                case k_StagingEnvironment:
                    return "https://qos-discovery-stg.services.api.unity.com";
                default:
                    return "https://qos-discovery.services.api.unity.com";
            }
        }
    }

    /// <summary>
    /// InternalQosDiscoveryService
    /// </summary>
    class InternalQosDiscoveryService
    {
        const int RequestTimeout = 10;
        const int NumRetries = 4;

        /// <summary>
        /// Constructor for InternalQosDiscoveryService
        /// </summary>
        /// <param name="httpClient">The HttpClient for InternalQosDiscoveryService.</param>
        /// <param name="accessToken">The Authentication token for the service.</param>
        internal InternalQosDiscoveryService(string host, HttpClient httpClient, IAccessToken accessToken = null)
        {
            Configuration = new Configuration(host, RequestTimeout, NumRetries, null);

            QosDiscoveryApi = new QosDiscoveryApiClient(httpClient, accessToken, Configuration);
        }

        public IQosDiscoveryApiClient QosDiscoveryApi { get; set; }

        /// <summary> Configuration properties for the service.</summary>
        public Configuration Configuration { get; set; }
    }
}
