using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Models.Common;
using TestApp.Services.Mocking;
using TestApp.Services.Utils;

namespace TestApp.Services.TrainingDomain
{
    public class TrainingTagMockService : ITrainingTagService
    {

        private const int GeneratedHashtagsNumber = 50;
        private const int GeneratedHashtagsOffset = 100;

        public int Delay = 0;

        private uint _hashtagLastId;


        public List<ISimpleTagElement> TrainingHashtags { get; private set; }


        #region Ctors

        #endregion
        public TrainingTagMockService() : this((int)Utils.AppEnvironment.BackendCommunicationLatency.High) { }
        public TrainingTagMockService(int latency)
        {
            Delay = latency;

            TrainingHashtags = new List<ISimpleTagElement>
            {
                MockingService.HashtagFactory.Fitness,
                MockingService.HashtagFactory.Healthy,
                MockingService.HashtagFactory.VeryLong,
                MockingService.HashtagFactory.MediumLength,
                MockingService.HashtagFactory.MaxLength,
                MockingService.HashtagFactory.Bulk,
            };

            for (uint i = 0; i < GeneratedHashtagsNumber; i++)
                TrainingHashtags.Add(MockingService.HashtagFactory.BuildHashtag(GeneratedHashtagsOffset + i));

            _hashtagLastId = TrainingHashtags.Max(x => x.Id);
        }


        public async Task<uint> AddTrainingHashtagAsync(string body)
        {
            await Task.Delay(Delay);
            TrainingHashtags.Add(new Hashtag { Id = ++_hashtagLastId, Body = body, });
            return _hashtagLastId;
        }

        public async Task<IEnumerable<uint>> AddTrainingHashtagsAsync(IEnumerable<string> hashtags)
        {
            await Task.Delay(Delay);
            int idBefore = (int)_hashtagLastId;

            TrainingHashtags.AddRange(
                hashtags.Select(x => new Hashtag
                {
                    Id = ++_hashtagLastId,
                    Body = x,
                }));

            int idAfter = (int)_hashtagLastId;

            return Enumerable.Range(idBefore + 1, (idAfter - idBefore)).Select(x => (uint)x);
        }

        public async Task<ISimpleTagElement> FindTrainingHashtagByBody(string body)
        {
            await Task.Delay(Delay);
            return TrainingHashtags.SingleOrDefault(x => x.Body == body);
        }

    }
}
