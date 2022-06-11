using System;

namespace MSA.Template.SharedKernel.Interfaces
{
    public interface IIdentityServiceProvider
    {
        void SetIdentity(Guid userIdentity);
    }
}