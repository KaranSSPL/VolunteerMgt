using System.Net;

namespace VolunteerMgt.Server.Common.Exceptions;

public class ResourceAlreadyExistsException(
    string message,
    List<string>? errors = default)
    : CustomException(message, errors, HttpStatusCode.Conflict);