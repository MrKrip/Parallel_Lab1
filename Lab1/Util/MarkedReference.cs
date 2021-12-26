using Lab1.Util.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Util
{
    class MarkedReference<T>
    {
        private readonly AtomicReference<ReferenceBase<T>> _reference;

        public MarkedReference(T value, bool marked)
        {
            _reference = new AtomicReference<ReferenceBase<T>>(new ReferenceBase<T>(value, marked));
        }

        public T Get(ref bool marked)
        {
            marked = Marked.Value;
            return Value;
        }

        public T Value => _reference.Value.Value;

        private AtomicBool Marked => _reference.Value.Marked;

        public bool CompareAndExchange(T newValue, bool newMarked, T oldValue, bool oldMarked)
        {
            var oldReference = _reference.Value;

            if (!ReferenceEquals(oldReference.Value, oldValue))
            {
                return false;
            }

            if (oldReference.Marked.Value != oldMarked)
            {
                return false;
            }

            return _reference.CompareAndExchange(new ReferenceBase<T>(newValue, newMarked), oldReference);
        }

        public bool AttemptMark(T oldValue, bool newMark)
        {
            var oldReference = _reference.Value;

            if (!ReferenceEquals(oldReference.Value, oldValue))
            {
                return false;
            }

            return _reference.Value.Marked.SetValue(newMark);
        }
    }
}
