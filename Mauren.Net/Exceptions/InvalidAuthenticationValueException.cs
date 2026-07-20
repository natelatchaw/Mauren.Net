using System;

namespace Mauren.Net.Exceptions
{
    /// <summary>
    /// Represents errors that occur when invalid authentication values are encountered.
    /// </summary>
    public class InvalidAuthenticationValueException : Exception
    {
        /// <summary>
        /// Constructs an exception.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type of authentication value that is invalid.
        /// </param>
        /// 
        /// <inheritdoc/>
        public InvalidAuthenticationValueException(String? type = default, Exception? innerException = default)
            : base(GetMessage(type), innerException) { }

        static String? GetMessage(String? type)
        {
            type ??= "Authentication";
            return $"The {type} value is invalid.";
        }
    }

    /// <summary>
    /// Represents errors that occur when expired authentication values are encountered.
    /// </summary>
    public class ExpiredAuthenticationValueException : InvalidAuthenticationValueException
    {
        /// <summary>
        /// Constructs an exception.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type of authentication value that is expired.
        /// </param>
        /// 
        /// <inheritdoc/>
        public ExpiredAuthenticationValueException(String? type = default, DateTimeOffset? expiration = default, Exception? innerException = default)
            : base(GetMessage(type, expiration), innerException) { }

        static String? GetMessage(String? type, DateTimeOffset? expiration)
        {
            type ??= "Authentication";
            return expiration switch
            {
                DateTimeOffset value => $"The {type} value has expired at {value}",
                _ => $"The {type} value has expired.",
            };
        }
    }

    /// <summary>
    /// Represents errors that occur when missing authentication values are encountered.
    /// </summary>
    public class MissingAuthenticationValueException : InvalidAuthenticationValueException
    {
        /// <summary>
        /// Constructs an exception.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type of authentication value that is missing.
        /// </param>
        /// 
        /// <inheritdoc/>
        public MissingAuthenticationValueException(String? type = default, Exception? innerException = default)
            : base(GetMessage(type), innerException) { }

        static String? GetMessage(String? type)
        {
            type ??= "Authentication";
            return $"The {type} value is missing.";
        }
    }
}
