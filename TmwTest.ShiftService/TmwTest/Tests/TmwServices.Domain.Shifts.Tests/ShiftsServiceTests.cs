namespace TmwServices.Domain.Shifts.Tests
{
    using System;
    using System.Net;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using NUnit.Framework;

    using TmwServices.Domain.Shifts.Configuration;
    using TmwServices.Domain.Shifts.Model;

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

        [TestCase(7)]
        [TestCase(7.99)]
        [TestCase(12)]
        [TestCase(0)]
        [TestCase(8.01)]
        public void trying_to_insert_too_short_or_too_long_shift_should_cause_error(double shiftLength)
        {
            var startHour = new DateTime(2021, 1, 23, 7, 0, 0);
            var endHour = startHour.AddHours(shiftLength);

            var newShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };

            var insertionStatus = _shiftService.TryRegisterShiftAsync(newShift).Result;

            Assert.IsFalse(insertionStatus.IsSuccess);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, insertionStatus.Code);
            Console.WriteLine(insertionStatus.ErrorMessage);
        }

        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(8)]
        [TestCase(-8)]
        [TestCase(16)]
        [TestCase(-16)]
        [TestCase(23.99)]
        [TestCase(-23.99)]
        public void trying_to_insert_conflicted_shift_should_result_with_error(double shiftMoveTime)
        {
            var startHour = new DateTime(2021, 1, 23, 7, 0, 0);
            var endHour = startHour.AddHours(8);

            // first insert older shift
            var firstShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };
            var insertionStatus = _shiftService.TryRegisterShiftAsync(firstShift).Result;
            Assert.IsTrue(insertionStatus.IsSuccess);

            // now try creating shift within not allowed time

            startHour = startHour.AddHours(shiftMoveTime);
            endHour = startHour.AddHours(8);

            // first insert older shift
            var newShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };
            insertionStatus = _shiftService.TryRegisterShiftAsync(newShift).Result;
            Assert.IsFalse(insertionStatus.IsSuccess);
            Assert.AreEqual((int)HttpStatusCode.Conflict, insertionStatus.Code);
            Console.WriteLine(insertionStatus.ErrorMessage);
        }

        [TestCase(-24)]
        [TestCase(24)]
        [TestCase(25)]
        public void trying_to_insert_non_conflicted_shift_should_result_with_success(int shiftMoveTime)
        {
            var startHour = new DateTime(2021, 1, 23, 7, 0, 0);
            var endHour = startHour.AddHours(8);

            // first insert older shift
            var firstShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };
            var insertionStatus = _shiftService.TryRegisterShiftAsync(firstShift).Result;
            Assert.IsTrue(insertionStatus.IsSuccess);

            // now try creating shift within not allowed time

            startHour = startHour.AddHours(shiftMoveTime);
            endHour = startHour.AddHours(8);

            // first insert older shift
            var newShift = new Shift
            {
                WorkerId = _workerOneIdentifier,
                TimeZone = _workerTimeZone,
                StartUtc = startHour.Add(_workerTimeZone.GetUtcOffset(startHour)),
                EndUtc = endHour.Add(_workerTimeZone.GetUtcOffset(endHour))
            };
            insertionStatus = _shiftService.TryRegisterShiftAsync(newShift).Result;
            Assert.IsTrue(insertionStatus.IsSuccess);
        }
    }
}