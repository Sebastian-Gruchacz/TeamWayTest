using TmwServices.Domain.Shifts.Implementation;

namespace TmwServices.ShiftsService
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using TmwServices.Domain.Shifts;
    using TmwServices.Domain.Shifts.Configuration;
    using TmwServices.ShiftsService.HostedServices;

    /// <summary>
    /// Provides extension method used to register Shift Domain subsystems
    /// </summary>
    public static class ShiftsDomainRegistrations
    {
        /// <summary>
        /// Registers the shifts domain.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static IServiceCollection RegisterShiftsDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ShiftRulesConfiguration>(configuration.GetSection(ShiftRulesConfiguration.SectionName));
            services.Configure<QueuesSubsystemConfiguration>(configuration.GetSection(QueuesSubsystemConfiguration.SectionName));

            services.AddTransient<IShiftEventsEmitterService, ShiftEventsEmitterService>();

            services.AddTransient<IShiftsService, Domain.Shifts.Implementation.ShiftsService>();
            services.AddSingleton<IShiftsRepository, InMemoryShiftsRepository>();

            // TODO: when DB is implemented - replace with services.AddTransient<IShiftsRepository, Domain.Shifts.SqlShiftsRepository>();

            services.AddHostedService<ShiftRegistrationQueueMonitor>();

            return services;
        }
    }
}
