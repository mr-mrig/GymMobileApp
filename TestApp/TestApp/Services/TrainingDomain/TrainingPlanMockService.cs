using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Mocking;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;

namespace TestApp.Services.TrainingDomain
{
    public class TrainingPlanMockService : ITrainingPlanService
    {
        
        private static uint _workUnitCounter = 0;

        private ITrainingTagService _taggingService;

        public int CommandsLatency { get; set; }
        public int QueriesLatency { get; set; }


        #region Cotrs

        public TrainingPlanMockService(ITrainingTagService taggingService) 
            : this(taggingService, (int)Utils.AppEnvironment.BackendCommunicationLatency.Normal, (int)Utils.AppEnvironment.BackendCommunicationLatency.Normal) { }

        public TrainingPlanMockService(ITrainingTagService taggingService, int queriesLatency, int commandsLatency) 
        {
            QueriesLatency = queriesLatency;
            CommandsLatency = commandsLatency;

            _taggingService = taggingService;
            _workoutTemplates = CreateFullWorkoutsTemplates();

            foreach (var plan in _trainingPlans)
            {
                plan.Details.Hashtags = new ObservableCollection<ISimpleTagElement>(_trainingPlanSummaries.SingleOrDefault(x => x.PlanId == plan.Details.UserPlanId).Hashtags);
                plan.Details.TargetPhases = new ObservableCollection<ISimpleTagElement>(_trainingPlanSummaries.SingleOrDefault(x => x.PlanId == plan.Details.UserPlanId).TargetPhases);
                plan.Details.TargetProficiencies = new ObservableCollection<ISimpleTagElement>(_trainingPlanSummaries.SingleOrDefault(x => x.PlanId == plan.Details.UserPlanId).TargetProficiencies);
            }
        }
        #endregion


        private List<WorkingSetTemplate> _workingSets = new List<WorkingSetTemplate>();


        private List<TrainingPlanSummary> _trainingPlanSummaries = new List<TrainingPlanSummary>
        {
            new TrainingPlanSummary
            {
                PlanId = 1,
                PlanName = "Plan1",
                OwnerId = 2,
                IsBookmarked = false,
                ParentPlanId = null,
                PlanUserLibraryId = 1,
                AvgIntensityPercentage = 71.2f,
                AvgWorkingSets = 33,
                WeeksNumber = 3,
                MinWorkoutsPerWeek = 3,
                MaxWorkoutsPerWeek = 4,
                LastWorkoutTimestamp = DateTime.UtcNow.AddDays(-300),
                Hashtags = new ObservableCollection<Hashtag>
                {
                    MockingService.HashtagFactory.BuildHashtag(1),
                    MockingService.HashtagFactory.BuildHashtag(2),
                    MockingService.HashtagFactory.VeryLong,
                    MockingService.HashtagFactory.Fitness,
                    MockingService.HashtagFactory.BuildHashtag(11),
                    MockingService.HashtagFactory.BuildHashtag(101),
                    MockingService.HashtagFactory.BuildHashtag(3),
                },
                TargetProficiencies = new List<TrainingProficiency>
                {
                    MockingService.Proficiency.Beginner,
                    MockingService.Proficiency.Intermediate,
                },
                TargetPhases = new List<TrainingPhase>
                {
                    MockingService.Phase.Cut,
                    MockingService.Phase.Recomp,
                }
            },
            new TrainingPlanSummary
            {
                PlanId = 2,
                PlanName = "Plan2 - variant of 1 with very_long_description_whcih_whould_not_fit_the_screen_width",
                OwnerId = 1,
                IsBookmarked = false,
                ParentPlanId = 1,
                PlanUserLibraryId = 2,
                AvgIntensityPercentage = 72.44f,
                AvgWorkingSets = 33,
                WeeksNumber = 2,
                MinWorkoutsPerWeek = 6,
                MaxWorkoutsPerWeek = 6,
                LastWorkoutTimestamp = DateTime.UtcNow.AddDays(-200),
                Hashtags = new ObservableCollection<Hashtag>
                {
                    MockingService.HashtagFactory.BuildHashtag(1),
                    MockingService.HashtagFactory.BuildHashtag(55),
                },
                TargetProficiencies = new List<TrainingProficiency>
                {
                    MockingService.Proficiency.Advanced,
                },
                TargetPhases = new List<TrainingPhase>
                {
                    MockingService.Phase.Bulk,
                    MockingService.Phase.Strength,
                }
            },
            new TrainingPlanSummary
            {
                PlanId = 3,
                PlanName = "Plan3",
                OwnerId = 1,
                IsBookmarked = true,
                ParentPlanId = null,
                PlanUserLibraryId = 3,
                AvgIntensityPercentage = 81f,
                AvgWorkingSets = 27,
                MinWorkoutsPerWeek = 3,
                MaxWorkoutsPerWeek = 4,
                LastWorkoutTimestamp = null,
                Hashtags = new ObservableCollection<Hashtag>
                {
                    
                },
                TargetProficiencies = new List<TrainingProficiency>
                {
                    MockingService.Proficiency.Pro,
                    MockingService.Proficiency.Advanced,
                },
                TargetPhases = new List<TrainingPhase>
                {
                    MockingService.Phase.Cut,
                }
            },
            new TrainingPlanSummary
            {
                PlanId = 4,
                PlanName = "Plan4 - inherited",
                OwnerId = 2,
                IsBookmarked = true,
                ParentPlanId = null,
                PlanUserLibraryId = 4,
                AvgIntensityPercentage = 66.41f,
                AvgWorkingSets = 35,
                MinWorkoutsPerWeek = 3,
                MaxWorkoutsPerWeek = 3,
                LastWorkoutTimestamp = null,
                Hashtags = new ObservableCollection<Hashtag>
                {
                    MockingService.HashtagFactory.VeryLong,
                },
                TargetProficiencies = new List<TrainingProficiency>
                {
                    MockingService.Proficiency.Beginner,
                },
                TargetPhases = new List<TrainingPhase>
                {
                    MockingService.Phase.Conditioning,
                }
            },
        };

        private List<TrainingWeek> _workoutsPlan = new List<TrainingWeek>
        {
            new TrainingWeek { Id = 1, ProgressiveNumber = 0, WeekTypeId = 1, Workouts = new ObservableCollection<WorkoutTemplate>
                {
                    new WorkoutTemplate { Id = 1, Name =  "Day A", WeekdayId = 1, WeekId = 1, WorkUnits = null },
                    new WorkoutTemplate { Id = 2, Name =  "Day B", WeekdayId = 1, WeekId = 1, WorkUnits = null },
                    new WorkoutTemplate { Id = 3, Name =  "Day C", WeekdayId = 1, WeekId = 1, WorkUnits = null },
                    new WorkoutTemplate { Id = 4, Name =  "Day D", WeekdayId = 1, WeekId = 1, WorkUnits = null },
                },
            },
            new TrainingWeek { Id = 2, ProgressiveNumber = 1, WeekTypeId = 1, Workouts = new ObservableCollection<WorkoutTemplate>
                {
                    new WorkoutTemplate { Id = 5, Name =  "Day A", WeekdayId = 1, WeekId = 2, WorkUnits = null },
                    new WorkoutTemplate { Id = 6, Name =  "Day B", WeekdayId = 1, WeekId = 2, WorkUnits = null },
                    new WorkoutTemplate { Id = 7, Name =  "Day C", WeekdayId = 1, WeekId = 2, WorkUnits = null },
                    new WorkoutTemplate { Id = 8, Name =  "Day D", WeekdayId = 1, WeekId = 2, WorkUnits = null },
                    new WorkoutTemplate { Id = 9, Name =  "Day E", WeekdayId = 1, WeekId = 2, WorkUnits = null },
                },
            },
            new TrainingWeek { Id = 3, ProgressiveNumber = 2, WeekTypeId = 1, Workouts = new ObservableCollection<WorkoutTemplate>
                {
                    new WorkoutTemplate { Id = 10, Name =  "Day A", WeekdayId = 1, WeekId = 3, WorkUnits = null },
                    new WorkoutTemplate { Id = 11, Name =  "Day B", WeekdayId = 1, WeekId = 3, WorkUnits = null },
                    new WorkoutTemplate { Id = 12, Name =  "Day A", WeekdayId = 1, WeekId = 3, WorkUnits = null },
                },
            },
        };

        private List<WorkoutTemplate> _workoutTemplates;

        private List<TrainingPlan> _trainingPlans = new List<TrainingPlan>
        {
            new TrainingPlan
            {
                Id = 1,
                Details = new TrainingPlanDetails
                {
                    UserPlanId = 1,
                    ParentPlan = new ParentPlanRelation
                    {
                        ParentId = 3,
                        ParentName = "My Mocked Parent Plan",
                        RelationTypeId = 1,
                    },
                    NoteId = 1,
                    Note = MockingService.LoremIpsum,
                    MusclesFocuses = new ObservableCollection<ISimpleTagElement>
                    {
                        new MuscleFocus { Id = 1, Body = "Pecs", },
                        new MuscleFocus { Id = 2, Body = "Delts", },
                    },
                }
            },
            new TrainingPlan
            {
                Id = 2,
                Details = new TrainingPlanDetails
                {
                    UserPlanId = 2,
                    ParentPlan = new ParentPlanRelation
                    {
                        ParentId = 3,
                        ParentName = "My Mocked Parent Plan",
                        RelationTypeId = 2,
                    },
                    NoteId = 1,
                    Note = null,
                    MusclesFocuses = new ObservableCollection<ISimpleTagElement>
                    {
                        new MuscleFocus { Id = 2, Body = "Delts", },
                        new MuscleFocus { Id = 4, Body = "Hams", },
                    },
                }
            },
            new TrainingPlan
            {
                Id = 3,
                Details = new TrainingPlanDetails
                {
                    UserPlanId = 3,
                    ParentPlan = null,
                    NoteId = 1,
                    Note = MockingService.LoremIpsum,
                    MusclesFocuses = new ObservableCollection<ISimpleTagElement>
                    {
                        new MuscleFocus { Id = 1, Body = "Pecs", },
                    },
                }
            },
            new TrainingPlan
            {
                Id = 4,
                Details = new TrainingPlanDetails
                {
                    UserPlanId = 4,
                    ParentPlan = null,
                    NoteId = 1,
                    Note = MockingService.LoremIpsum,
                    MusclesFocuses = null,
                }
            }
        };

        #region Getters
        private TrainingPlan FindPlanByUserPlanId(uint userTrainingPlanId) => _trainingPlans.SingleOrDefault(x => x.Details.UserPlanId == userTrainingPlanId);
        private TrainingPlan FindPlanById(uint trainingPlanId) => _trainingPlans.SingleOrDefault(x => x.Id == trainingPlanId);
        private TrainingPlanSummary FindSummary(uint trainingPlanId) => _trainingPlanSummaries.SingleOrDefault(x => x.PlanId == trainingPlanId);
        private WorkoutTemplate FindWorkout(uint workoutId) => _workoutTemplates.SingleOrDefault(x => x.Id == workoutId);
        private uint GetNextTrainingWeekId() => _workoutsPlan.SelectMany(x => x.Workouts.Select(wo => wo.WeekId)).Max() + 1;
        private uint GetNextWorkoutId() => _workoutsPlan.SelectMany(x => x.Workouts).Max(x => x.Id.Value) + 1;
        private uint GetNextWorkingSetId() => (uint)_workingSets.Count() + 1;
        //private uint GetNextWorkingSetId() => _workoutsPlan.SelectMany(x => x.Workouts.SelectMany(wo => wo.WorkUnits.SelectMany(wu => wu.WorkingSets))).Max(x => x.Id.Value) + 1;

        private uint GetNextWorkUnitId() => _workoutsPlan.SelectMany(x => x.Workouts.SelectMany(wo => wo.WorkUnits)).Max(x => x.Id.Value) + 1;
        private TrainingWeek FindLastTrainingWeekOf(uint planId) => FindPlanById(planId).TrainingWeeks.Last();
        private WorkUnitTemplate FindWorkUnit(uint workoutId, uint progressiveNumber)
        {
            try
            {
                return FindWorkout(workoutId).WorkUnits.SingleOrDefault(x => x.ProgressiveNumber == progressiveNumber);
            }
            catch (InvalidOperationException exc)
            {
                throw new InvalidOperationException("Have you deleted a Work Unit during the test? If so this can be ignored!", exc);
            }
        }
        #endregion


        #region Private functions
        private List<WorkoutTemplate> AddWorkoutsFrom(IEnumerable<WorkoutTemplate> workouts)
        {
            List<WorkoutTemplate> res = new List<WorkoutTemplate>();

            foreach (var wo in workouts)
            {
                WorkoutTemplate newWo = new WorkoutTemplate
                {
                    Id = GetNextWorkoutId(),
                    Name = wo.Name,
                    WeekdayId = wo.WeekdayId,
                    WeekId = wo.WeekId,
                    WorkUnits = new ObservableCollection<WorkUnitTemplate>(),   // no need to mock everything, we will verify later..
                };
                res.Add(newWo);
            }

            return res;

            foreach (var wo in workouts)
            {
                WorkoutTemplate newWo = new WorkoutTemplate
                {
                    Id = GetNextWorkoutId(),
                    Name = wo.Name,
                    WeekdayId = wo.WeekdayId,
                    WeekId = wo.WeekId,
                    WorkUnits = new ObservableCollection<WorkUnitTemplate>(),
                };

                foreach(var wu in wo.WorkUnits)
                {
                    var newWu = new WorkUnitTemplate
                    {
                        Id = GetNextWorkUnitId(),
                        Excercise = new Excercise
                        {
                            Id = wu.Excercise.Id,
                            Name = wu.Excercise.Name,
                            ImageUrl = wu.Excercise.ImageUrl,
                            MuscleGroupId = wu.Excercise.MuscleGroupId,
                        },
                        IntensityTechnique = wu.IntensityTechnique,
                        Note = wu.Note,
                        NoteId = wu.NoteId,
                        ProgressiveNumber = wu.ProgressiveNumber,
                        WorkingSets = new List<WorkingSetTemplate>(),
                        //WorkingSets = new ExtendedObservableCollection<WorkingSetTemplate>(),
                    };

                    foreach(var ws in wu.WorkingSets)
                    {
                        var newWs = new WorkingSetTemplate
                        {
                            Id = GetNextWorkingSetId(),
                            Effort = ws.Effort,
                            IntensityTechniques = ws.IntensityTechniques,
                            LiftingTempo = ws.LiftingTempo,
                            ProgressiveNumber = ws.ProgressiveNumber,
                            Repetitions = ws.Repetitions,
                            Rest = ws.Rest,
                        };
                        newWu.WorkingSets.Add(newWs);
                    }
                    newWo.WorkUnits.Add(newWu);
                }
                res.Add(newWo);
            }

            return res;
        }

        private void ForceWorkUnitsProgressiveNumbers(uint workoutId)
        {
            var iterator = FindWorkout(workoutId).WorkUnits.OrderBy(x => x.ProgressiveNumber).GetEnumerator();
            uint counter = 0;
            while (iterator.MoveNext())
                iterator.Current.ProgressiveNumber = counter++;

            //for (uint i = 0; i < workUnits.Count; i++)
            //{
            //    WorkUnitTemplate wu = workUnits[(int)i];
            //    wu.ProgressiveNumber = i;
            //}
        }

        private void RemoveWorkUnitLink(uint workoutId, uint workUnitProgressiveNumber)
        {
            // Previous WU is already unlinked, but we must unlink the next one also
            // NOTE: after the delete, the following WU has now the Progressive Number the removed one had before!
            WorkUnitTemplate nextOne = FindWorkUnit(workoutId, workUnitProgressiveNumber);

            if (nextOne?.IntensityTechnique != null && nextOne.IntensityTechnique.IsLinking)
                nextOne.IntensityTechnique = null;
        }

        private void LogServiceCall([CallerMemberName] string callerName = "")
        {
            Console.WriteLine($"----- Backend call by {callerName}");
        }

        private void LogFullServiceCall(params object[] paramList)
        {
            Console.Write("----- Backend call \t");

            for (int i = 0; i < paramList.Length; i++)
            {
                if (paramList[i] is int)
                    Console.Write(paramList[i].ToString() + " ");
                else
                    Console.Write(paramList[i] + " ");
            }
            Console.WriteLine();
        }
        #endregion



        #region ITrainingPlanService Implementation

        public async Task<TrainingPlanDetails> GetTrainingPlanDetailsAsync(uint userTrainingPlanId)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return FindPlanByUserPlanId(userTrainingPlanId).Details;
        }

        public async Task<IEnumerable<TrainingPlanSummary>> GetTrainingPlansSummariesAsync(uint userId)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return _trainingPlanSummaries;
        }

        public async Task<IEnumerable<TrainingWeek>> GetWorkoutsPlanAsync(uint trainingPlanId)
        {
            await Task.Delay(QueriesLatency * 6).ConfigureAwait(false);
            return _workoutsPlan;
        }

        public async Task<TrainingPlanDetails> GetTrainingPlanFullDetailsAsync(uint userTrainingPlanId)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<ISimpleTagElement>> GetTopRatedHashtagsAsync()
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return new List<ISimpleTagElement>
            {
                MockingService.HashtagFactory.Fitness,
                MockingService.HashtagFactory.Healthy,
            };
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetFavouriteHashtagsAsync(uint userId)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return new List<ISimpleTagElement>
            {
                MockingService.HashtagFactory.MaxLength,
                MockingService.HashtagFactory.MediumLength,
                MockingService.HashtagFactory.BuildHashtag(101),
                MockingService.HashtagFactory.BuildHashtag(102),
                MockingService.HashtagFactory.BuildHashtag(103),
                MockingService.HashtagFactory.BuildHashtag(3333),
                MockingService.HashtagFactory.BuildHashtag(87),
                MockingService.HashtagFactory.VeryLong,
            };
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetTopRatedPhasesAsync()
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return MockingService.Phase.GetAllPublic();
        }

        public async Task<IEnumerable<ISimpleTagElement>> GetFavouritePhasesAsync(uint userId)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return MockingService.Phase.GetAllPrivate();
        }
        
        public async Task<IList<WorkoutTemplate>> GetWorkoutTemplatesAsync(IEnumerable<uint> workoutsIds)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return _workoutTemplates.Where(x => workoutsIds.Contains(x.Id.Value)).ToList();
        }

        public async Task<IList<WorkoutTemplate>> GetWorkoutTemplatesByWeekAsync(IEnumerable<uint> weekIds, string workoutName)
        {
            await Task.Delay(QueriesLatency).ConfigureAwait(false);
            return _workoutTemplates.Where(x => weekIds.Contains(x.WeekId)).Where(x => x.Name == workoutName).ToList();
        }

        public async Task<IEnumerable<uint>> AddTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToAdd)
        {
            LogServiceCall();

            TrainingPlan plan = FindPlanById(trainingPlanId);
            TrainingWeek lastWeek = FindLastTrainingWeekOf(trainingPlanId);
            List<uint> newIds = new List<uint>();

            for (int i = 0; i < numberOfWeeksToAdd; i++)
            {
                newIds.Add(GetNextTrainingWeekId());

                plan.TrainingWeeks.Add(new TrainingWeek
                {
                    Id = newIds.Last(),
                    WeekTypeId = Utils.AppEnvironment.WeekTypesIds.Generic,
                    ProgressiveNumber = lastWeek.ProgressiveNumber + 1,
                    Workouts = new ObservableCollection<WorkoutTemplate>(AddWorkoutsFrom(lastWeek.Workouts.ToList())),
                });
            }

            // Align the summary
            FindSummary(trainingPlanId).WeeksNumber = (int)lastWeek.ProgressiveNumber + numberOfWeeksToAdd + 1;

            //RIGM: returning only the week ID, the View will then need to fetch the new Workouts from this!
            return newIds;
        }

        public async Task<IEnumerable<uint>> RemoveTrainingWeeksAsync(uint trainingPlanId, int numberOfWeeksToRemove)
        {
            LogServiceCall();
            TrainingPlan plan = FindPlanById(trainingPlanId);
            List<TrainingWeek> removed = plan.TrainingWeeks.Reverse().Take(numberOfWeeksToRemove).ToList();


            (plan.TrainingWeeks as List<TrainingWeek>).RemoveAll(x => removed.Select(y => y.Id).Contains(x.Id));

            return removed.Select(x => x.Id.Value);
        }
        public async Task<bool> RenameTrainingPlanAsync(uint userTrainingPlanId, string name)
        {
            LogServiceCall();
            return await Task.FromResult(true);
        }

        public async Task<bool> AddTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement> hashtags)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            var plan = FindPlanByUserPlanId(userTrainingPlanId);

            foreach (var tag in hashtags)
            {
                //if (tag.Id == 0)
                //    tag.Id = await _taggingService.AddTrainingHashtagAsync(tag.Body);

                plan.Details.Hashtags.Add(tag);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveTrainingPlanHashtagsAsync(uint userTrainingPlanId, IEnumerable<ISimpleTagElement> hashtags)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            var plan = FindPlanByUserPlanId(userTrainingPlanId);

            foreach (var tag in hashtags)
                plan.Details.Hashtags.Remove(tag);

            return true;
        }

        public async Task<bool> AddTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            FindPlanByUserPlanId(userTrainingPlanId).Details.TargetProficiencies.Add(proficiency);
            return true;
        }

        public async Task<bool> RemoveTrainingPlanProficiencyAsync(uint userTrainingPlanId, ISimpleTagElement proficiency)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            return FindPlanByUserPlanId(userTrainingPlanId).Details.TargetProficiencies.Remove(proficiency);
        }

        public async Task<bool> AddTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            FindPlanByUserPlanId(userTrainingPlanId).Details.TargetPhases.Add(phase);
            return true;
        }

        public async Task<bool> RemoveTrainingPlanPhaseAsync(uint userTrainingPlanId, ISimpleTagElement phase)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);

            return FindPlanByUserPlanId(userTrainingPlanId).Details.TargetPhases.Remove(phase);
        }

        public async Task<bool> SetTrainingPlanBookmarkedAsync(uint userTrainingPlanId, bool isBookmarked)
        {
            LogServiceCall();
            await Task.Delay(CommandsLatency).ConfigureAwait(false);
            return FindPlanByUserPlanId(userTrainingPlanId).Details.IsBookmarked = isBookmarked;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Should be called to align the mocked data with the latest data.
        /// </summary>
        /// <param name="refreshed">The data holding the most fresh data</param>
        public void RefreshPlan(TrainingPlan refreshed) => _trainingPlans[_trainingPlans.IndexOf(_trainingPlans.Single(x => x.Id == refreshed.Id))] = refreshed;

        #endregion

        #region Helper methods

        private List<WorkoutTemplate> CreateFullWorkoutsTemplates()
        {
            List<WorkoutTemplate> res = new List<WorkoutTemplate>();

            foreach (var workout in _workoutsPlan.SelectMany(x => x.Workouts))
                res.Add(CreateWorkout(workout.Id.Value, workout.Name, workout.WeekId));

            return res;
        }
        private WorkoutTemplate CreateWorkout(uint id, string name, uint weekId)
        {
            Debug.WriteLine("Creating workout: " + id.ToString());
            
            // Different WOs according to the name (only 3 different WOs so far)
            switch (name.ToUpper())
            {
                case "DAY A":
                    return new WorkoutTemplate
                    {
                        Id = id,
                        WorkUnits = new ObservableCollection<WorkUnitTemplate>
                        {
                            // This is the only case of different WUs for different Weeks
                            weekId == 1 ?
                                CreateWorkUnit(0, 1, MockingService.ExcerciseLibrary.BenchPress.Id,
                                    Enumerable.Range(0, 9).Select(x => CreateWorkingSet(x, 6, 180, 9, (int?)MockingService.EffortLibrary.RM.Id, null, null))) :
                                CreateWorkUnit(0, 1, MockingService.ExcerciseLibrary.BenchPress.Id,
                                    Enumerable.Range(0, 8).Select(x => CreateWorkingSet(x, 10, 180, 9, (int?)MockingService.EffortLibrary.RM.Id, null, null))),

                            CreateWorkUnit(1, null, MockingService.ExcerciseLibrary.CableFlyes.Id, new List<WorkingSetTemplate>
                            {
                                CreateWorkingSet(0, 5, 120, 8.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(1, 3, 180, 9.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(2, 1, int.MaxValue, 9.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(3, 5, 180, 8.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(4, 3, 120, 9.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(5, 1, 180, 9.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(6, 5, 120, 8.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(7, 3, 240, 9.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(8, 1, 120, 7.5f, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                            }),

                            CreateWorkUnit(2, 2, MockingService.ExcerciseLibrary.DbBenchPress.Id,
                                Enumerable.Range(0, 3).Select(x => CreateWorkingSet(x, 18, 120, 10, (int?)MockingService.EffortLibrary.RM.Id, "1130", new List<uint> { 4, 3, 1, }))),

                            // This is linked to the previous
                            CreateWorkUnit(3, null, MockingService.ExcerciseLibrary.DbBenchPress.Id,
                                Enumerable.Range(0, 3).Select(x => CreateWorkingSet(x, Utils.AppEnvironment.WsAmrapValue, 120, 10, (int?)MockingService.EffortLibrary.RM.Id, null, null)), 3),

                            CreateWorkUnit(4, null, MockingService.ExcerciseLibrary.SeatedLegCurl.Id, new List<WorkingSetTemplate>
                            {
                                CreateWorkingSet(0, 10, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(1, 8, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                                CreateWorkingSet(2, 6, 90, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                                CreateWorkingSet(3, 10, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(4, 8, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                                CreateWorkingSet(5, 6, 90, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                                CreateWorkingSet(6, 10, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, "3030", null),
                                CreateWorkingSet(7, 8, 0, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                                CreateWorkingSet(8, 6, 90, null, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 3, 2, }),
                            }),
                        },
                    };

                case "DAY B":
                    return new WorkoutTemplate
                    {
                        Id = id,
                        WorkUnits = new ObservableCollection<WorkUnitTemplate>
                        {
                            CreateWorkUnit(0, 2, MockingService.ExcerciseLibrary.Ohp.Id,
                                Enumerable.Range(0, 5).Select(x => CreateWorkingSet(x, 4, null, 80f, (int?)MockingService.EffortLibrary.IntensityPercentage.Id, "2120", null))),

                            CreateWorkUnit(1, null, MockingService.ExcerciseLibrary.LateralRaises.Id,
                                Enumerable.Range(0, 3).Select(x => CreateWorkingSet(x, 20, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, "3030", new List<uint> { 3,}))),

                            CreateWorkUnit(2, null, MockingService.ExcerciseLibrary.Squat.Id,
                                Enumerable.Range(0, 3).Select(x => CreateWorkingSet(x, 7, 120, 10, (int?)MockingService.EffortLibrary.RM.Id, "", new List<uint> { 3,}))),

                            CreateWorkUnit(3, 3, MockingService.ExcerciseLibrary.SingleLegExtensionsButtRaised.Id, new List<WorkingSetTemplate>
                            {
                                CreateWorkingSet(0, 20, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, null, null),
                                CreateWorkingSet(1, 10, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 2, }),
                                CreateWorkingSet(2, 20, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, null, null),
                                CreateWorkingSet(3, 10, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 2, }),
                                CreateWorkingSet(3, 10, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, null, new List<uint> { 2, }),
                            }),
                        },
                    };

                case "DAY C":
                    return new WorkoutTemplate
                    {
                        Id = id,
                        WorkUnits = new ObservableCollection<WorkUnitTemplate>
                        {
                            CreateWorkUnit(0, null, MockingService.ExcerciseLibrary.WideGobletSquatSteps.Id,
                                Enumerable.Range(0, 4).Select(x => CreateWorkingSet(x, 4, null, 5, (int?)MockingService.EffortLibrary.RM.Id, null, new List<uint> { 1, }))),

                            CreateWorkUnit(1, null, MockingService.ExcerciseLibrary.LateralRaises.Id,
                                Enumerable.Range(0, 4).Select(x => CreateWorkingSet(x, 20, 0, 10, (int?)MockingService.EffortLibrary.RPE.Id, "3030", new List<uint> { 3,}))),

                            CreateWorkUnit(2, null, MockingService.ExcerciseLibrary.Squat.Id,
                                Enumerable.Range(0, 4).Select(x => CreateWorkingSet(x, 7, 120, 10, (int?)MockingService.EffortLibrary.RM.Id, "", new List<uint> { 3,}))),
                        },
                    };

                default:
                    return new WorkoutTemplate { Id = id, Name = name, };   // Draft Workout
            }
        }

        private WorkUnitTemplate CreateWorkUnit(uint progressiveNumber, uint? noteId, uint? excerciseId, IEnumerable<WorkingSetTemplate> wsList, uint? intTechniqueId = null)
        {
            var excercise = MockingService.ExcerciseLibrary.GetAll().Single(x => x.Id == excerciseId);

            IntensityTechnique it = intTechniqueId > 0 ? MockingService.IntensityTechniqueLibrary.GetAll().Single(x => x.Id == intTechniqueId) : null;

            return new WorkUnitTemplate
            {
                Id = ++_workUnitCounter,
                Excercise = excercise,
                ProgressiveNumber = progressiveNumber,
                IntensityTechnique = it,
                NoteId = noteId,
                Note = noteId.HasValue ? MockingService.LoremIpsum : null,
                //WorkingSets = wsList == null ? null :  new ExtendedObservableCollection<WorkingSetTemplate>(wsList),
                WorkingSets = wsList == null ? null :  new List<WorkingSetTemplate>(wsList),
            };
        }
        private WorkingSetTemplate CreateWorkingSet(int progressiveNumber, int? reps, int? rest, float? effortVal, int? effortTypeId, string tut, IList<uint> intensityTechniques)
        {
            List<IntensityTechnique> its = intensityTechniques?.Select(x =>
                MockingService.IntensityTechniqueLibrary.GetAll().Single(y => y.Id == x)).ToList();

            //TrainingEffortType effort = MockingService.EffortLibrary.GetAll().SingleOrDefault(x => x.Id == effortTypeId);
            uint id = GetNextWorkingSetId();

            WorkingSetTemplate added = new WorkingSetTemplate
            {
                Id = id,
                ProgressiveNumber = (uint)progressiveNumber,
                Repetitions = reps,
                Rest = rest,
                Effort = !effortVal.HasValue ? null : new TrainingEffort(effortVal.Value, (uint)effortTypeId.Value),
                LiftingTempo = tut,
                IntensityTechniques = its == null ? null : new ObservableCollection<IntensityTechnique>(its),
            };

            _workingSets.Add(added);
            return added;
        }

        #endregion

        public async Task<bool> DeleteWorkUnitTemplates(IEnumerable<uint> workoutIds, uint workUnitProgressiveNumber)
        {
            await Task.Delay(CommandsLatency);

            foreach(WorkoutTemplate wo in _workoutTemplates.Where(x => workoutIds.Contains(x.Id.Value)))
            {
                // The following will throw as the model has been partially modifed by the VM
                // we just workaround this, as this will not happen outside the mocking environment
                //WorkUnitTemplate wu = FindWorkUnit(wo.Id, workUnitProgressiveNumber);
                WorkUnitTemplate wu = FindWorkout(wo.Id.Value).WorkUnits.Where(x => x.ProgressiveNumber == workUnitProgressiveNumber).First();

                if (wo.WorkUnits.Remove(wu))
                {
                    // Sort Progressive numbers
                    ForceWorkUnitsProgressiveNumbers(wo.Id.Value);
                    // Remove links, if any
                    RemoveWorkUnitLink(wo.Id.Value, workUnitProgressiveNumber);
                    return await Task.FromResult(true);
                }
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> LinkToPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber, uint intensityTechniqueId)
        {
            await Task.Delay(CommandsLatency);
            WorkUnitTemplate wu = FindWorkUnit(workoutId, workUnitProgressiveNumber);

            if (wu == null)
                return await Task.FromResult(false);

            wu.IntensityTechnique = AppEnvironment.IntensityTechniques.SingleOrDefault(x => x.Id == intensityTechniqueId);
            return await Task.FromResult(true);
        }

        public async Task<bool> UnlinkFromPreviousWorkUnit(uint workoutId, uint workUnitProgressiveNumber)
        {
            await Task.Delay(CommandsLatency);
            WorkUnitTemplate wu = FindWorkUnit(workoutId, workUnitProgressiveNumber);

            if (wu == null)
                return await Task.FromResult(false);

            wu.IntensityTechnique = null;
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<uint>> SetWorkUnitTemplate(uint workoutId, WorkUnitTemplate workUnit)
        {
            await Task.Delay(CommandsLatency);

            WorkUnitTemplate toReplace = FindWorkUnit(workoutId, workUnit.ProgressiveNumber.Value);
            toReplace.CopyFrom(workUnit);
            //toReplace = workUnit;

            // Assign valid Ids for each new WS
            foreach (var ws in toReplace.WorkingSets.Where(x => x.Id == null))
            {
                CreateWorkingSet(
                    (int)ws.ProgressiveNumber,
                    ws.Repetitions,
                    ws.Rest,
                    ws.Effort?.Effort,
                    (int?)ws.Effort?.EffortType?.Id,
                    ws.LiftingTempo,
                    ws.IntensityTechniques?.Select(x => x.Id.Value)?.ToList());

                ws.Id = _workingSets.Last().Id;
            }  
            return toReplace.WorkingSets.Select(x => x.Id.Value);
        }

        public async Task<IEnumerable<uint>> InsertIntensityTechniques(IEnumerable<IntensityTechnique> intensityTechniques)
        {
            // Nothing to be done yet...
            await Task.Yield();
            return intensityTechniques.Select((x, i) => (uint)(AppEnvironment.NativeIntensityTechniques.GetAll().Count() + i + 1));
        }
    }
}
