using SharedKernel.Interfaces;

namespace MSA.Template.BackgroundTasks.Services;

public class IdentityService : IIdentityService, IIdentityServiceProvider
{
    private Guid? _userIdentity;

    public Guid GetUserIdentity()
    {
        return _userIdentity ?? throw new ArgumentNullException(nameof(_userIdentity));
    }

    public void SetUserIdentity(Guid userIdentity)
    {
        _userIdentity = userIdentity;
    }
}