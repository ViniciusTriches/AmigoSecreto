using System;
using System.Collections.Generic;
using System.Text;

namespace AmigoSecreto.Classes.Exceptions
{
    public class RegistrationException : ApplicationException
    {
        public RegistrationException(string message) : base(message) { }
    }
}
