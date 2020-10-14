using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionEventApi.Common.Models.V1;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Store
{
    /// <summary>
    /// This class determines whether the recursion should continue based on the path / folder structure
    /// </summary>
    public class DatePathFilter : IPathFilter
    {
        // This assumes structure is for example from root '/[Year]/[Month]/[Day]/[Hour]/[FileId]/metadata.json'
        private const int NumberOfPartsBeforeFileDirectory = 4;

        private readonly DateTimeOffset _start;
        private readonly DateTimeOffset _end;

        public DatePathFilter(FileStoreFilterV1 filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            _start = filter.TimestampRangeStart ?? throw new ArgumentException("Start was null", nameof(filter));
            _end = filter.TimestampRangeEnd ?? throw new ArgumentException("End was null", nameof(filter));
        }

        public PathAction DecideAction(string path)
        {
            var parts = path?.Split('/') ?? Enumerable.Empty<string>().ToArray();

            return ShouldRecurse(parts) ? PathAction.Recurse
                     : parts.Length > NumberOfPartsBeforeFileDirectory ? PathAction.Collect
                     : PathAction.Stop;
        }

        private bool ShouldRecurse(IReadOnlyList<string> parts)
        {
            return parts.Count switch
            {
                1 => YearInRange(_start, _end, parts),
                2 => MonthFolderInRange(_start, _end, parts),
                3 => DayOfMonthInRange(_start, _end, parts),
                4 => HourOfDayInRange(_start, _end, parts),
                _ => false
            };
        }

        private static bool YearInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var val)) return false;
            return val >= start.Year && val <= end.Year;
        }

        private static bool MonthFolderInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;

            if (start.Year == end.Year) return parsedMonth >= start.Month && parsedMonth <= end.Month;
            if (parsedYear == start.Year) return parsedMonth >= start.Month;
            if (parsedYear == end.Year) return parsedMonth <= end.Month;
            return true;
        }

        private static bool DayOfMonthInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;

            var date = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay));
            return date >= start && date <= end;
        }

        private static bool HourOfDayInRange(DateTimeOffset start, DateTimeOffset end, IReadOnlyList<string> folderParts)
        {
            if (!int.TryParse(folderParts[0], out var parsedYear)) return false;
            if (!int.TryParse(folderParts[1], out var parsedMonth)) return false;
            if (!int.TryParse(folderParts[2], out var parsedDay)) return false;
            if (!int.TryParse(folderParts[3], out var parsedHour)) return false;

            var date = new DateTimeOffset(new DateTime(parsedYear, parsedMonth, parsedDay, parsedHour, 0, 0));
            return date >= start && date <= end;
        }
    }
}