using System.Collections.Generic;
using TestApp.Services.DomainPresenters;
using Xunit;
using TestApp.Models.TrainingDomain;
using System.Collections.ObjectModel;
using System.Linq;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;

namespace TestApp.UnitTest.Utils
{

    public class WorkingSetPresenterTests
    {
        
        public static readonly IntensityTechnique Dropset = AppEnvironment.IntensityTechniques.Single(x => x.Id == AppEnvironment.DropsetId);
        public static readonly int AMRAP = AppEnvironment.WsAmrapValue;
        public static readonly int FullRest = AppEnvironment.WsFullRestValue;
        public static readonly TrainingEffortType RM = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "RM");
        public static readonly TrainingEffortType IntPerc = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "%");
        public static readonly TrainingEffortType RPE = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "RPE");



        [Fact]
        public void WorkingSetPresenter_ToAll_NoWorkingSets()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>()
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            throw new System.Exception("I really don't know why the following is throwing...Cannot debug though...");
            Assert.Equal(expected, presenter.ToFormattedRepetitions());
            Assert.Equal(expected, presenter.ToFormattedRest());
            Assert.Equal(expected, presenter.ToFormattedEffort());
            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRepetitions_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Rest = 60, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRepetitions_NotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = null },
                }
            };
            ITrainingPresenter presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRepetitions_Amrap()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = AMRAP },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "max";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRepetitions_ClusteredAllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkingSetPresenter(test);
            var expected = "10+1+max";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRepetitions_ClusteredWithNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = null, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkingSetPresenter(test);
            var expected = "10+1+...";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10,  },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_Ok()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "60" + BasicTrainingPresenterService.DefaultRestMeasUnitString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_NotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = null, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_FullRest()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultFullRestString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_Clustered()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = 120, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "120" + BasicTrainingPresenterService.DefaultRestMeasUnitString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_ClusteredWithNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = null, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedRest_ClusteredWithFullRest()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 0, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultFullRestString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_Ok()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X30", },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "1X30";

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_Clustered_AllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "1X30";

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_Clustered_Different()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X20", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_NotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedTempo_Clustered_OneNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, LiftingTempo = null, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate() 
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_AllEqual_RM()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(9f, RM.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9f, RM.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9f, RM.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "9 " + RM.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_AllEqual_RPE()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, RPE.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, RPE.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, RPE.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "9,5 " + RPE.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_AllEqual_IntPerc()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "91,5 " + IntPerc.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_OneDifferentEffortTypes()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, IntPerc.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, RPE.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, IntPerc.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_DifferentEffortTypes()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(12f, RM.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_OneIsNull()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                    new WorkingSetTemplate {  },
                    new WorkingSetTemplate { Effort = new TrainingEffort(91.5f, IntPerc.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_Range()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(8.5f, RPE.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(9.5f, RPE.Id), },
                    new WorkingSetTemplate { Effort = new TrainingEffort(10f, RPE.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = "8,5"
                + BasicTrainingPresenterService.DefaultRangeSeparatorString
                + "10 " + RPE.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkingSetPresenter_ToFormattedEffort_RangeOneNull()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Effort = new TrainingEffort(8.5f, RPE.Id), },
                    new WorkingSetTemplate { },
                    new WorkingSetTemplate { Effort = new TrainingEffort(10f, RPE.Id), },
                }
            };
            var presenter = new WorkingSetPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }
    }
}
