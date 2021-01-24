namespace TmwServices.Domain.Shifts.Configuration
{
    using System;

    /// <summary>
    /// Defines rules for Shifts management
    /// </summary>
    [Serializable]
    public class ShiftRulesConfiguration
    {
        /// <summary>
        /// The section name in the settings file
        /// </summary>
        public const string SectionName = @"ShiftRules";

        /// <summary>
        /// Gets or sets minimum length of the shift in hours
        /// </summary>
        public int MinShiftLength { get; set; }

        /// <summary>
        /// Gets or sets maximum length of the shift in hours
        /// </summary>
        public int MaxShiftLength { get; set; }

        /// <summary>
        /// Gets or sets minimum number of hours that must pass between the shifts of the user
        /// </summary>
        public int MinShiftsGape { get; set; }

        /// <summary>
        /// Gets or sets flag that specifies whether Shift can overlap two (or more! - if shifts are very long) or more days.
        /// </summary>
        public bool AllowDayOverlap { get; set; }
    }
}