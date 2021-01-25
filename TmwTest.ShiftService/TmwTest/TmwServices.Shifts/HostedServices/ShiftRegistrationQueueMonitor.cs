namespace TmwServices.ShiftsService.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using TmwServices.Core.Configuration;
    using TmwServices.Domain.Shifts;
    using TmwServices.Domain.Shifts.Configuration;
    using TmwServices.Domain.Shifts.Model;

    /// <summary>
    /// Monitors incoming queue for Shift registration request
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    public class ShiftRegistrationQueueMonitor : IHostedService
    {
        private readonly ILogger<ShiftRegistrationQueueMonitor> _logger;
        private readonly IShiftsService _shiftsService;
        private readonly QueuesSubsystemConfiguration _configuration;
        private readonly AzureQueueConfiguration _monitoringConfiguration;

        private ServiceBusConnection _conn;
        private MessageReceiver _queueClient;

        // TODO: separate hosting service and queue monitoring (to avoid unwanted depndencies...) and then move queue stuff into domain....
        // TODO: OR improve even more if need. Or merge into one DLL, depending of the decided architecture and creation of shared libraries

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftRegistrationQueueMonitor"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger"></param>
        /// <param name="shiftsService"></param>
        /// <exception cref="ArgumentNullException">configuration</exception>
        public ShiftRegistrationQueueMonitor(IOptions<QueuesSubsystemConfiguration> configuration,
            ILogger<ShiftRegistrationQueueMonitor> logger,
            IShiftsService shiftsService)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shiftsService = shiftsService ?? throw new ArgumentNullException(nameof(shiftsService));

            _configuration = configuration.Value;
            _monitoringConfiguration = _configuration?.ShiftsRegistrationsSource;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Starting [{nameof(ShiftRegistrationQueueMonitor)}]...");

            if (_configuration == null || !_configuration.MonitoringEnabled || _monitoringConfiguration == null)
            {
                _logger.LogTrace($"Monitoring not configured or disabled.");
                return Task.CompletedTask;
            }

            _conn = new ServiceBusConnection(_monitoringConfiguration.ServiceBusAccessKey);
            _queueClient = new MessageReceiver(_conn, 
                _monitoringConfiguration.QueueName,
                ReceiveMode.PeekLock,
                prefetchCount: 1)
            {
                OperationTimeout = TimeSpan.FromMinutes(2)
            };

            MessageHandlerOptions options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = true, // TODO: FALSE? Depending of usage / transaction model
                MaxAutoRenewDuration = TimeSpan.FromMinutes(1),
                MaxConcurrentCalls = 3
            };

            _queueClient.RegisterMessageHandler(RegistrationMessageHandler, options);

            _logger.LogTrace($"[{nameof(ShiftRegistrationQueueMonitor)}] started.");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace($"Stopping [{nameof(ShiftRegistrationQueueMonitor)}]...");

            if (_configuration == null || !_configuration.MonitoringEnabled || _monitoringConfiguration == null)
            {
                _logger.LogTrace($"Monitoring not configured or disabled.");
                return;
            }

            await _queueClient.CloseAsync();
            await _conn.CloseAsync();

            _logger.LogTrace($"[{nameof(ShiftRegistrationQueueMonitor)}] stopped.");
        }

        private async Task RegistrationMessageHandler(Message msg, CancellationToken cancellationToken)
        {
            Shift shift = null;

            // TODO: here read and convert ShiftModel from msg.Body

            await _shiftsService.TryRegisterShiftAsync(shift);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, $"MessageReceiver failed because of: {arg.ExceptionReceivedContext.ToString()}");

            return Task.CompletedTask;
        }
    }
}
