namespace Ordina.UNite.Security.APS.Controllers
{
    public class AuthorizationRequest
    {
        public string ResourceType { get; set; }
        public string Action { get; set; }
        public object Resource { get; set; }
    }
}