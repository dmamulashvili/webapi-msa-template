using System;

namespace SharedKernel.Interfaces;

public interface IIdentityService
{
    Guid GetUserIdentity();
}