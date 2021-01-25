namespace TmwServices.Domain.Shifts.Implementation
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using TmwServices.Domain.Shifts.Configuration;
    using TmwServices.Domain.Shifts.Model;

    /// <inheritdoc cref="IShiftEventsEmitterService" />
    public class ShiftEventsEmitterService : IShiftEventsEmitterService, IDisposable
    {
        private readonly ILogger<ShiftEventsEmitterService> _logger;
        private readonly bool _enabled;
        private readonly ServiceBusConnection _successConn;
        private readonly ServiceBusConnection _failureConn;
        private readonly MessageSender _failureEmitter;
        private readonly MessageSender _successEmitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftEventsEmitterService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public ShiftEventsEmitterService(IOptions<QueuesSubsystemConfiguration> configuration,
            ILogger<ShiftEventsEmitterService> logger)
        {
            var subsystemConfiguration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _enabled = subsystemConfiguration.RaiseEventsEnabled && 
                       subsystemConfiguration.ShiftsRegistrationFailedEvents != null &&
                       subsystemConfiguration.ShiftsRegistrationSuccessEvents != null;

            if (_enabled)
            {
                _successConn = new ServiceBusConnection(subsystemConfiguration.ShiftsRegistrationSuccessEvents?.ServiceBusAccessKey);
                _failureConn = new ServiceBusConnection(subsystemConfiguration.ShiftsRegistrationFailedEvents?.ServiceBusAccessKey);

                _failureEmitter = new MessageSender(_failureConn, subsystemConfiguration.ShiftsRegistrationFailedEvents?.QueueName);
                _successEmitter = new MessageSender(_successConn, subsystemConfiguration.ShiftsRegistrationSuccessEvents?.QueueName);
            }
        }

        /// <inheritdoc />
        public async Task ShiftConflictedAsync(Shift shift)
        {
            if (!_enabled)
            {
                return;
            }

            // TODO: build exit message and throw into queue

            // await _failureEmitter.SendAsync(msg);
        }

        /// <inheritdoc />
        public async Task ShiftRegisteredAsync(Shift insertedShift)
        {
            if (!_enabled)
            {
                return;
            }

            // TODO: build exit message and throw into queue

            // await _successEmitter.SendAsync(msg);
        }

        /// <inheritdoc />
        public async Task ShiftRejectedAsync(Shift shift)
        {
            if (!_enabled)
            {
                return;
            }

            // TODO: build exit message and throw into queue

            // await _failureEmitter.SendAsync(msg);
        }

        public void Dispose()
        {
            // a bit unnecessary, but good to remember if using different patters of Start / Stop
            _failureEmitter?.CloseAsync();
            _successEmitter?.CloseAsync();
            _failureConn?.CloseAsync();
            _successConn?.CloseAsync();
        }
    }
}