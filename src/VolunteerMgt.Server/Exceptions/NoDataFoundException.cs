using System.Net;

namespace VolunteerMgt.Server.Exceptions;

public class NoDataFoundException(
    string message,
    List<string>? errors = default)
    : CustomException(message, errors, HttpStatusCode.NotFound);