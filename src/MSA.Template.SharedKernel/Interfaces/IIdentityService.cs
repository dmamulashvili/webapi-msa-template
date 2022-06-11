using System;

namespace MSA.Template.SharedKernel.Interfaces;

public interface IIdentityService
{
    Guid GetUserIdentity();
}