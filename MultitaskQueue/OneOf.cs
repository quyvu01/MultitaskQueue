using System;

namespace MultitaskQueue
{
    public struct OneOf<T0, T1>
    {
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;

        private OneOf(int index, T0 val0 = default, T1 val1 = default) => (_index, _value0, _value1) = (index, val0, val1);

        public bool IsT0 => _index == 0;

        public bool IsT1 => _index == 1;

        public object Value => _index switch
        {
            0 => _value0,
            1 => _value1,
            _ => throw new Exception("Invaild Index"),
        };

        public static implicit operator OneOf<T0, T1>(T0 val0) => new OneOf<T0, T1>(0, val0: val0);

        public static implicit operator OneOf<T0, T1>(T1 val1) => new OneOf<T0, T1>(1, val1: val1);

        public TResult Match<TResult>(Func<T0, TResult> func0, Func<T1, TResult> func1)
        {
            if (func0 == null || func1 == null) throw new ArgumentNullException("Function0 or Function1 can not be null");
            return _index switch
            {
                0 => func0.Invoke(_value0),
                1 => func1.Invoke(_value1),
                _ => throw new Exception("Invaild Index"),
            };
        }
        public void Switch(Action<T0> action0, Action<T1> action1)
        {
            if (action0 == null || action0 == null) throw new ArgumentNullException("Action0 or Action1 can not be null");
            if (_index == 0) action0.Invoke(_value0);
            else if (_index == 1) action1.Invoke(_value1);
            else throw new Exception("Invaild Index");
        }
    }
}