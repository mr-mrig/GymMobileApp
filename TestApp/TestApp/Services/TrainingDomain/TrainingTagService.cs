using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Models.Common;

namespace TestApp.Services.TrainingDomain
{
    public class TrainingTagService : ITrainingTagService
    {
        public Task<uint> AddTrainingHashtagAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<uint>> AddTrainingHashtagsAsync(IEnumerable<string> hashtags)
        {
            throw new NotImplementedException();
        }

        public Task<ISimpleTagElement> FindTrainingHashtagByBody(string hashtag)
        {
            throw new NotImplementedException();
        }
    }
}
