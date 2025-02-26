using System.Net;

namespace VolunteerMgt.Server.Exceptions;

public class BadRequestException(
    string message,
    List<string>? errors = default)
    : CustomException(message, errors, HttpStatusCode.BadRequest);
