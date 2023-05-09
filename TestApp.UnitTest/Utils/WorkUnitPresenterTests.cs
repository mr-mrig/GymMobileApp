using System.Collections.Generic;
using TestApp.Services.DomainPresenters;
using Xunit;
using TestApp.Models.TrainingDomain;
using System.Collections.ObjectModel;
using System.Linq;
using TestApp.Services.Utils;
using TestApp.Services.Utils.Extensions;
using System;

namespace TestApp.UnitTest.Utils
{

    public class WorkUnitPresenterTests
    {

        public static readonly IntensityTechnique Dropset = AppEnvironment.IntensityTechniques.Single(x => x.Id == AppEnvironment.DropsetId);
        public static readonly int AMRAP = AppEnvironment.WsAmrapValue;
        public static readonly int FullRest = AppEnvironment.WsFullRestValue;
        public static readonly TrainingEffortType RM = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "RM");
        public static readonly TrainingEffortType IntPerc = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "%");
        public static readonly TrainingEffortType RPE = AppEnvironment.TrainingEfforts.Single(x => x.Abbreviation == "RPE");



        [Fact]
        public void WorkUnitPresenter_ToAll_NoWorkingSets()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>()
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
            Assert.Equal(expected, presenter.ToFormattedRest());
            Assert.Equal(expected, presenter.ToFormattedEffort());
            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Rest = 60, },
                    new WorkingSetTemplate { Rest = 60, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_LastOneIsDifferent()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 6 },
                    new WorkingSetTemplate { Repetitions = 6 },
                    new WorkingSetTemplate { Repetitions = 6 },
                    new WorkingSetTemplate { Repetitions = 10 },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "6, 6, 6, 10";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_FirstOneIsDifferent()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 16 },
                    new WorkingSetTemplate { Repetitions = 6 },
                    new WorkingSetTemplate { Repetitions = 6 },
                    new WorkingSetTemplate { Repetitions = 6 },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "16, 6, 6, 6";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }


        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_NotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 12 },
                    new WorkingSetTemplate { Repetitions = null },
                    new WorkingSetTemplate { Repetitions = 12 },
                    new WorkingSetTemplate { Repetitions = 12 },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }


        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_Amrap()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = AMRAP },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = AMRAP },
                    new WorkingSetTemplate { Repetitions = 10 },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "max, 10, max, 10";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_AllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 10 },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "4 x 10";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_ClusteredAllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = "3 x 10+1+max";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_ClusteredWithNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = null, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 1, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = AMRAP, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultRepetitionsUnsetString;

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_ClusteredLastIsDifferent()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = "10+8+6, 10+8+8, 10+8+6";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_ClusteredLastIsDifferent2()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 7, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 6, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = "10+8+6+6+7, 10+8+6+6+6";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRepetitions_NotEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 7, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                    new WorkingSetTemplate { Repetitions = 10 },
                    new WorkingSetTemplate { Repetitions = 8, IntensityTechniques = new ObservableCollection<IntensityTechnique> { Dropset } },
                }
            };
            ITrainingPresenter presenter = new WorkUnitPresenter(test);
            var expected = "10+8, 10+8, 10+8, 10+8, 10+7, 10+8";

            Assert.Equal(expected, presenter.ToFormattedRepetitions());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRest_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10,  },
                    new WorkingSetTemplate { Repetitions = 10, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRest_AllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "60" + BasicTrainingPresenterService.DefaultRestMeasUnitString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRestAllNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = null, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = null, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRestAllFullRest()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = FullRest, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultFullRestString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRest_Range()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "60" + BasicTrainingPresenterService.DefaultRestMeasUnitString
                + BasicTrainingPresenterService.DefaultRangeSeparatorString
                + "240" + BasicTrainingPresenterService.DefaultRestMeasUnitString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRest_RangeWithNotSet()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, },
                    new WorkingSetTemplate { Repetitions = 20, Rest = null, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "60" + BasicTrainingPresenterService.DefaultRestMeasUnitString
                + BasicTrainingPresenterService.DefaultRangeSeparatorString
                + "240" + BasicTrainingPresenterService.DefaultRestMeasUnitString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedRest_RangeWithFullRest()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = "60" + BasicTrainingPresenterService.DefaultRestMeasUnitString
                + BasicTrainingPresenterService.DefaultRangeSeparatorString
                + BasicTrainingPresenterService.DefaultFullRestString;

            Assert.Equal(expected, presenter.ToFormattedRest());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_Empty()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_AllEqual()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, LiftingTempo = "1X30", },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = "1X30";

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_DifferentAtStart()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X20", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, LiftingTempo = "1X30", },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_DifferentAtEnd()
        {
            WorkUnitTemplate test = new WorkUnitTemplate()
            {
                WorkingSets = new List<WorkingSetTemplate>
                {
                    new WorkingSetTemplate { Repetitions = 20, Rest = 60, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 240, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 20, Rest = FullRest, LiftingTempo = "1X30", },
                    new WorkingSetTemplate { Repetitions = 10, Rest = 60, LiftingTempo = "2X30", },
                }
            };
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_AllNotSet()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedTempo_OneNotSet()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_Empty()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedTempo());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_AllEqual_RM()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = "9 " + RM.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_AllEqual_RPE()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = "9,5 " + RPE.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_AllEqual_IntPerc()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = "91,5 " + IntPerc.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_OneDifferentEffortTypes()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_DifferentEffortTypes()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_OneIsNull()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_Range()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = "8,5"
                + BasicTrainingPresenterService.DefaultRangeSeparatorString
                + "10 " + RPE.Abbreviation;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToFormattedEffort_RangeOneNull()
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
            var presenter = new WorkUnitPresenter(test);
            var expected = BasicTrainingPresenterService.DefaultEmptyString;

            Assert.Equal(expected, presenter.ToFormattedEffort());
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_SingleWorkingSet()
        {
            string test = "12";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(new List<int?> { 12, });

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_SingleWorkingSet2()
        {
            string test = "1 x 12";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(new List<int?> { 12, });

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_SingleClusteredWorkingSet()
        {
            string test = "12+10+8+100";
            var presenter = new WorkUnitPresenter();
            var expected = BuildClusteredWorkingSets(new List<int?> { 12, 10, 8, 100 }, AppEnvironment.DropsetId);

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_BasicWorkingSets()
        {
            string test = "12 x 4";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(Enumerable.Repeat(4, 12).Select(x => (int?)x));

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_BasicWorkingSetsWithAmrap()
        {
            string test = "4 x max";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(Enumerable.Repeat(AppEnvironment.WsAmrapValue, 4).Select(x => (int?)x));

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_BasicWorkingSetsWithUnset()
        {
            string test = "3 x ...";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(new List<int?> { null, null, null, });

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_DifferentWorkingSets()
        {
            string test = "10,8,6,10";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(new List<int?> { 10, 8, 6, 10, });

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_DifferentWorkingSetsMixed()
        {
            string test = "10,8,max,...,4";
            var presenter = new WorkUnitPresenter();
            var expected = BuildWorkingSets(new List<int?> { 10, 8, AppEnvironment.WsAmrapValue, null, 4, });

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_ClusteredWorkingSets()
        {
            string test = "3 x 20+11";
            var presenter = new WorkUnitPresenter();
            var expected = BuildClusteredWorkUnit(3, new List<int?> { 20, 11, }, AppEnvironment.DropsetId);

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_ClusteredDifferentWorkingSets()
        {
            string test = "20+10, 20+10+11";
            var presenter = new WorkUnitPresenter();
            var expected = BuildClusteredWorkingSets(new List<int?> { 20, 10, }, AppEnvironment.DropsetId);
            expected = expected.Concat(BuildClusteredWorkingSets(new List<int?> { 20, 10, 11, }, AppEnvironment.DropsetId, 2));

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_ClusteredDifferentWorkingSets2()
        {
            string test = "20+10, 20+10+3, 10+10";
            var presenter = new WorkUnitPresenter();
            var expected = BuildClusteredWorkingSets(new List<int?> { 20, 10, }, AppEnvironment.DropsetId);
            expected = expected.Concat(BuildClusteredWorkingSets(new List<int?> { 20, 10, 3, }, AppEnvironment.DropsetId, 2));
            expected = expected.Concat(BuildClusteredWorkingSets(new List<int?> { 10, 10, }, AppEnvironment.DropsetId, 5));

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_ClusteredDifferentWorkingSets3()
        {
            string test = "20+10+6, 20+10+8, 20+10+6";
            var presenter = new WorkUnitPresenter();
            var expected = BuildClusteredWorkingSets(new List<int?> { 20, 10, 6, }, AppEnvironment.DropsetId);
            expected = expected.Concat(BuildClusteredWorkingSets(new List<int?> { 20, 10, 8, }, AppEnvironment.DropsetId, 2));
            expected = expected.Concat(BuildClusteredWorkingSets(new List<int?> { 20, 10, 6, }, AppEnvironment.DropsetId, 5));

            presenter.ToModel(test, "", "", "","");
            Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_LiftingTempoTooShort()
        {
            string testRepetitions = "3 x 10";
            string testTempo = "303";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, "", testTempo, "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_LiftingTempoTooLong()
        {
            string testRepetitions = "3 x 10";
            string testTempo = "30311";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, "", testTempo, "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_LiftingTempoWithInvalidChars()
        {
            string testRepetitions = "3 x 10";
            string testTempo = "303t1";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, "", testTempo, "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_LiftingTempoTooLongAndInvalid()
        {
            string testRepetitions = "3 x 10";
            string testTempo = "3t0311";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, "", testTempo, "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_RestWithLetters()
        {
            string testRepetitions = "3 x 10";
            string testRest = "12x";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, testRest, "", "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_RestIsNegative()
        {
            string testRepetitions = "3 x 10";
            string testRest = "-90";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<FormatException>(() => presenter.ToModel(testRepetitions, testRest, "", "", ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_EffortWithNoType()
        {
            string testRepetitions = "3 x 10";
            string testEffort = "10";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<InvalidOperationException>(() => presenter.ToModel(testRepetitions, "", "", testEffort, ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_EffortWithInvalidType()
        {
            string testRepetitions = "3 x 10";
            string testEffort = "10 RMM";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<InvalidOperationException>(() => presenter.ToModel(testRepetitions, "", "", testEffort, ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_EffortIsMalformed()
        {
            string testRepetitions = "3 x 10";
            string testEffort = "10RPE";
            var presenter = new WorkUnitPresenter();

            Assert.Throws<InvalidOperationException>(() => presenter.ToModel(testRepetitions, "", "", testEffort, ""));
        }

        [Fact]
        public void WorkUnitPresenter_ToModel_AllTrainingParameters()
        {
            string testRepetitions = "3 x 10";
            string testTempo = "21X1";
            string testRest = "90";
            string testEffort = "10 RPE";
            var presenter = new WorkUnitPresenter();

            throw new NotImplementedException();

            //presenter.ToModel(testRepetitions, "", testTempo, ""));
            //Assert.Equal(expected, presenter.WorkUnit.WorkingSets);
        }






        public IEnumerable<WorkingSetTemplate> BuildWorkingSets(IEnumerable<int?> repetitions, IList<uint> intensityTechniqueIds = null)

            => repetitions.Select((x, i) => new WorkingSetTemplate
            {
                ProgressiveNumber = (uint)i,
                Repetitions = x,
                IntensityTechniques = intensityTechniqueIds == null ? null :
                    new ObservableCollection<IntensityTechnique>
                    {
                        AppEnvironment.NativeIntensityTechniques.Find(intensityTechniqueIds[i]),
                    },
            });

        /// <summary>
        /// Build the Working Sets. Please notice that the intensity technique will be assigned to the 1,3,..n-th WSs while the first one will be null
        /// </summary>
        /// <param name="repetitions"></param>
        /// <param name="linkingIntensityTechniqueId"></param>
        /// <returns></returns>
        public IEnumerable<WorkingSetTemplate> BuildClusteredWorkingSets(IEnumerable<int?> repetitions, uint linkingIntensityTechniqueId, int startingProgressiveNumber = 0)

            => repetitions.Select((x, i) => new WorkingSetTemplate
            {
                ProgressiveNumber = (uint)(i + startingProgressiveNumber),
                Repetitions = x,
                IntensityTechniques = linkingIntensityTechniqueId > 0 && i > 0 ?
                    new ObservableCollection<IntensityTechnique>
                    {
                        AppEnvironment.NativeIntensityTechniques.Find(linkingIntensityTechniqueId),
                    } :
                    null,
            });


        public IEnumerable<WorkingSetTemplate> BuildClusteredWorkUnit(int clusterCounter, IEnumerable<int?> repetitions, uint linkingIntensityTechniqueId)
        {
            var cluster = BuildClusteredWorkingSets(repetitions, linkingIntensityTechniqueId);

            for (int i = 1; i < clusterCounter; i++)
                cluster = cluster.Concat(BuildClusteredWorkingSets(new List<int?> { 20, 10, }, AppEnvironment.DropsetId, i * repetitions.Count()));

            return cluster;
        }
    }
}
