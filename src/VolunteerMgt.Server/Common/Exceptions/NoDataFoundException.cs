using System.Net;

namespace VolunteerMgt.Server.Common.Exceptions;

public class NoDataFoundException(
    string message,
    List<string>? errors = default)
    : CustomException(message, errors, HttpStatusCode.NotFound);