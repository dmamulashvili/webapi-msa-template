using SharedKernel.Interfaces;

namespace MSA.Template.API.Services;

public class IdentityService : IIdentityService, IIdentityServiceProvider
{
    private Guid? _userIdentity;

    public Guid GetUserIdentity()
    {
        return _userIdentity ?? throw new ArgumentNullException(nameof(_userIdentity));
    }

    public void SetIdentity(Guid userIdentity)
    {
        _userIdentity = userIdentity;
    }
}