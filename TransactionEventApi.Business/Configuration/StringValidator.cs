using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration.Validation.Errors;

namespace Glasswall.Administration.K8.TransactionEventApi.Business.Configuration
{
    public class StringValidator
        : IConfigurationItemValidator
    {
        private readonly int _minLengthInclusive;
        private readonly int _maxLengthInclusive;

        public StringValidator(int? minLengthInclusive = null, int? maxLengthInclusive = null)
        {
            _minLengthInclusive = minLengthInclusive.GetValueOrDefault(int.MinValue);
            _maxLengthInclusive = maxLengthInclusive.GetValueOrDefault(int.MaxValue);
        }

        public bool TryParse(string key, string rawValue, List<ConfigurationParserError> validationErrors, out object parsed)
        {
            var thisItemsErrors = new List<ConfigurationParserError>();

            var length = rawValue?.Length ?? 0;

            if (length < _minLengthInclusive)
                thisItemsErrors.Add(new ConfigurationParserError(key, $"Value must be at least {_minLengthInclusive} characters. Got {rawValue?.Length}"));

            //if (length > _maxLengthInclusive)
            //    thisItemsErrors.Add(new ConfigurationParserError(key, $"Value must not be longer than {_minLengthInclusive} characters. Got {rawValue?.Length}"));

            validationErrors.AddRange(thisItemsErrors);

            parsed = rawValue;
            return !thisItemsErrors.Any();
        }
    }
}