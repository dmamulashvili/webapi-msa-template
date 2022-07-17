using System;

namespace SharedKernel.Interfaces
{
    public interface IIdentityServiceProvider
    {
        void SetIdentity(Guid userIdentity);
    }
}