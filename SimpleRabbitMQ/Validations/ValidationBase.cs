using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Validations
{
    internal abstract class ValidationBase<TClass>
    {
        protected readonly TClass _objectClass;
        private bool _isInvalid;

        protected ValidationBase(TClass objectClass) {
            _objectClass = objectClass;
        }
        protected abstract bool IsInValidInternal();

        public ValidationBase<TClass> IsInValid() 
        {
            _isInvalid = IsInValidInternal();

            return this;
        }

        public ValidationBase<TClass> ThrowException(string message)
        {
            if(_isInvalid)
                throw new InvalidOperationException(message);

            return this;
        }
    }
}
