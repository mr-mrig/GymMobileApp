using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Models.Common;

namespace TestApp.Services.TrainingDomain
{
    public interface ITrainingTagService
    {

        #region Queries 

        Task<ISimpleTagElement> FindTrainingHashtagByBody(string body);
        #endregion


        #region Commands

        Task<uint> AddTrainingHashtagAsync(string body);
        Task<IEnumerable<uint>> AddTrainingHashtagsAsync(IEnumerable<string> hashtags);
        #endregion
    }
}
