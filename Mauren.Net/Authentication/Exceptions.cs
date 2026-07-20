using System;

namespace Mauren.Net.Authentication
{
    /// <summary>
    /// An exception that is thrown when the internal authorization state is invalid.
    /// </summary>
    public class AuthenticationStateException : Exception
    {
        public AuthenticationStateException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }


    /// <summary>
    /// An exception that is thrown when invalid credentials are detected.
    /// </summary>
    public class InvalidCredentialException : AuthenticationStateException
    {
        public InvalidCredentialException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }


    /// <summary>
    /// An exception that is thrown when expired credentials are detected.
    /// </summary>
    public class ExpiredCredentialException : InvalidCredentialException
    {
        public ExpiredCredentialException(String? message, DateTimeOffset? expiration = default, Exception? innerException = default)
            : base(message, innerException)
        {
            Expiration = expiration;
        }

        public DateTimeOffset? Expiration { get; }
    }
}

namespace Mauren.Net.Authentication
{
    /// <summary>
    /// An exception that is thrown when an error occurs in the authorization service.
    /// </summary>
    public class AuthenticationServiceException : Exception
    {
        public AuthenticationServiceException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }

    public class InvalidAuthenticationStateException : AuthenticationServiceException
    {
        public InvalidAuthenticationStateException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }

    public class RequestNotProvidedException : AuthenticationServiceException
    {
        public RequestNotProvidedException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }

    public class MissingAuthenticationHeaderException : AuthenticationServiceException
    {
        public MissingAuthenticationHeaderException(String? message, Exception? innerException = default)
            : base(message, innerException) { }
    }
}