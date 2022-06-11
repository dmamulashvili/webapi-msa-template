using System;
using System.Threading.Tasks;

namespace MSA.Template.SharedKernel.Interfaces;

public interface IRequestManager
{
    Task<bool> ExistAsync(Guid id);

    Task CreateRequestForCommandAsync<T>(Guid id);
}