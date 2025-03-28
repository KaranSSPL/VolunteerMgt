﻿using System.Net;

namespace VolunteerMgt.Server.Common.Exceptions;

public class CustomException : Exception
{
    public List<string>? ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public CustomException() : base() { }

    public CustomException(string? message) : base(message) { }

    public CustomException(string? message, Exception? innerException) : base(message, innerException) { }

    protected CustomException(string message, List<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }
}