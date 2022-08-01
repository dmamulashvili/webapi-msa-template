using System;

namespace SharedKernel.Interfaces
{
    public interface IIdentityServiceProvider
    {
        void SetUserIdentity(Guid userIdentity);
    }
}