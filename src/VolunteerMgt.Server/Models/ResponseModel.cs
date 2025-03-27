using System.Net;

namespace VolunteerMgt.Server.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; } 
        public T? Data { get; set; }
    }
}
