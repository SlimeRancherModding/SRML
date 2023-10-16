using SRML.Config.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SRML.Config
{
    public class Range<T> : IStringParserProvider
    {
        T _value;
        public T Max { get; set; }
        public T Min { get; set; }

        public bool IsInRange(T val)
        {
            return Expression.Lambda<Func<bool>>(Expression.GreaterThanOrEqual(Expression.Constant(val), Expression.Constant(Min))).Compile()()&& Expression.Lambda<Func<bool>>(Expression.LessThan(Expression.Constant(val), Expression.Constant(Max))).Compile()();
        }

        public static implicit operator T(Range<T> range) => range.Value;

        public T RoundToRange(T val)
        {
            if (IsInRange(val)) return val;
            if (Expression.Lambda<Func<bool>>(Expression.LessThan(Expression.Constant(val), Expression.Constant(Min))).Compile()()) return Min;
            return Max;
        }

        public IStringParser GetParser()
        {
            return new RangeParser<T>(this);
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = RoundToRange(value);
            }
        }

        public Range(T min, T max, T initialValue)
        {
            Min = min;
            Max = max;
            Value = initialValue;
        }

        public class RangeParser<T> : IStringParser
        {
            public Type ParsedType => typeof(Range<T>);
            Range<T> range;

            public RangeParser(Range<T> range)
            {
                this.range = range;
            }

            public string EncodeObject(object obj)
            {
                Range<T> range = (Range<T>)obj;
                return ParserRegistry.GetParser(typeof(T)).EncodeObject(range.Value);
            }

            public string GetUsageString()
            {
                return typeof(T) + " in range of " + range.Min.ToString() + " to " + range.Max.ToString();
            }

            public object ParseObject(string str)
            {
                range.Value = (T)ParserRegistry.GetParser(typeof(T)).ParseObject(str);
                return range;
            }
        }
    }
}
