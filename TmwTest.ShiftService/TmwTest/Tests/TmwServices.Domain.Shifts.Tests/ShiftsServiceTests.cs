using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TmwServices.Domain.Shifts.Model;

namespace TmwServices.Domain.Shifts.Tests
{
    using NUnit.Framework;

    using TmwServices.Domain.Shifts.Configuration;

    public class ShiftsServiceTests
    {
        private IOptions<ShiftRulesConfiguration> _defaultConfig;
        private Mock<ILogger<InMemoryShiftsRepository>> _repositoryLoggerMock;
        private Mock<ILogger<ShiftsService>> _serviceLoggerMock;
        private IShiftsRepository _repository;
        private IShiftsService _shiftService;
        private TimeZoneInfo _workerTimeZone;
        private Guid _workerOneIdentifier;

        [SetUp]
        public void Setup()
        {
            _workerTimeZone = TimeZoneInfo.Local;
            _workerOneIdentifier = Guid.Parse(@"af1ae2f1-91ac-483f-aefd-7945bf5655ba");

            var rules = new ShiftRulesConfiguration
            {
                MinShiftLength = 8, // hours
                MaxShiftLength = 8, // hours
                MinShiftsGape = 16, // hours
                AllowDayOverlap = true
            };
            _defaultConfig = Options.Create(rules);

            _repositoryLoggerMock = new Mock<ILogger<InMemoryShiftsRepository>>();
            _serviceLoggerMock = new Mock<ILogger<ShiftsService>>();

            _repository = new InMemoryShiftsRepository(_repositoryLoggerMock.Object);
            _shiftService = new ShiftsService(_defaultConfig, _repository, _serviceLoggerMock.Object);
        }

        [Test]
        public void correct_shift_should_be_added_to_empty_base_and_available()
        {
            
            var startHour = new DateTime(2021, 1, 23, 7, 0, 0);
            var endHour = startHour.AddHours(8);

            var newShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };

            var insertionStatus = _shiftService.TryRegisterShiftAsync(newShift).Result;

            Assert.IsTrue(insertionStatus.IsSuccess);
            Assert.AreNotEqual(insertionStatus.Value.ShiftId, Guid.Empty);

            // ...

            var registeredShifts = _shiftService
                .GetWorkerShiftsAsync(_workerOneIdentifier, DateTime.MinValue, DateTime.MaxValue).Result;

            Assert.IsNotNull(registeredShifts);
            Assert.IsNotEmpty(registeredShifts);
            Assert.AreEqual(1, registeredShifts.Length);
            Assert.AreEqual(registeredShifts[0].ShiftId, insertionStatus.Value.ShiftId);
        }
    }
}