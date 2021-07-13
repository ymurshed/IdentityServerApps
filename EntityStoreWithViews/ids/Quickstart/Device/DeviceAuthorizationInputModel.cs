using IdentityServerHost.Quickstart.Consent;

namespace IdentityServerHost.Quickstart.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}