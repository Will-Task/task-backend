using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AuthServer;

[Dependency(ReplaceServices = true)]
public class AuthServerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "AuthServer";
}
