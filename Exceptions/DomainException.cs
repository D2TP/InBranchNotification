using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convey.Test.Accounts.Exceptions
{
    public abstract class DomainException : Exception
    {
        public virtual string Code { get; }

        protected DomainException(string message) : base(message)
        {
        }
    }
}
