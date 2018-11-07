namespace Public.Api.Clients
{
    public class AuthorizationResponse
    {
        public string Action { get; set; }
        public bool HasAccess { get; set; }
        public object RedactedResource { get; set; }
    }
}