namespace VolunteerMgt.Server.Models.Wrapper;
public interface IResult
{
    List<string>? Errors { get; }

    List<string> Messages { get; set; }

    bool Succeeded { get; }
}

public interface IResult<out T> : IResult
{
    T? Payload { get; }
}