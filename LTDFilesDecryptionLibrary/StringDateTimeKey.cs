using System;
using System.Collections.Generic;

namespace LTDFilesDecryptionLibrary
{
    public class StringDateTimeKey : IEqualityComparer<StringDateTimeKey>, IEquatable<StringDateTimeKey>
    {
        private RangePeriod AccuracyPeriod;
        public string Name { get; set; }
        public DateTime DateTimeValue { get; set; }

        public StringDateTimeKey(string name, DateTime dateTimeValue, RangePeriod accuracyPeriod = 0)
        {
            Name = name;
            DateTimeValue = dateTimeValue;
            AccuracyPeriod = accuracyPeriod;
        }

        #region IEquatable implementation
        public bool Equals(StringDateTimeKey other)
        {
            return Equals(this, other);
        }
        #endregion IEquatable implementation

        #region IEqualityComparer implementation
        public bool Equals(StringDateTimeKey x, StringDateTimeKey y)
        {
            bool dateEquals = true;
            switch (AccuracyPeriod)
            {
                case RangePeriod.Hour:
                    dateEquals = dateEquals
                        && x.DateTimeValue.Hour == y.DateTimeValue.Hour;
                    goto case RangePeriod.Day;
                case RangePeriod.Day:
                    dateEquals = dateEquals
                        && x.DateTimeValue.Day == y.DateTimeValue.Day;
                    goto case RangePeriod.Month;
                case RangePeriod.Month:
                    dateEquals = dateEquals
                        && x.DateTimeValue.Month == y.DateTimeValue.Month
                        && x.DateTimeValue.Year == y.DateTimeValue.Year;
                    break;
            }
            return x != null && y != null 
                && dateEquals && x.Name == y.Name;
        }
        public int GetHashCode(StringDateTimeKey obj)
        {
            return EqualityComparer<string>.Default.GetHashCode(obj.Name);
        }
        #endregion IEqualityComparer implementation

        #region overrides
        public override bool Equals(object obj)
        {
            return Equals(obj as StringDateTimeKey);
        }
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
        #endregion overrides
    }
}
