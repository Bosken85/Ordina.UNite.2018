namespace APS.Controllers
{
    public class AuthorizationRequest
    {
        public string ResourceType { get; set; }
        public string Action { get; set; }
        public dynamic Resource { get; set; }
    }
}