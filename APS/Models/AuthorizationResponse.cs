using Newtonsoft.Json.Linq;

namespace APS.Controllers
{
    public class AuthorizationResponse
    {
        public string Action { get; set; }
        public bool HasAccess { get; set; }
        public JObject RedactedResource { get; set; }
    }
}