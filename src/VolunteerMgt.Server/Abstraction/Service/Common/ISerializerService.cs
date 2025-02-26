namespace VolunteerMgt.Server.Abstraction.Service.Common;

public interface ISerializerService : ITransientService
{
    string Serialize<T>(T obj);

    string Serialize<T>(T obj, Type type);

    string SerializeDefault<T>(T obj);

    T? Deserialize<T>(string text);
}
