using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ceaseless
{
    public interface IEngine
    {
        Task AddPersonToPray(string userId, string name);
        Task<IList<string>> GetPeopleToPray(string userId);
    }
}