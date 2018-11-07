namespace Public.Api.Clients
{
    public class AuthorizationRequest
    {
        public string ResourceType { get; set; }
        public string Action { get; set; }
        public object Resource { get; set; }
    }
}