using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Controls.Events;
using TestApp.Models.Common;
using TestApp.Models.TrainingDomain;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class TrainingPlanDetailViewModel : BaseViewModel
    {

        private const int DebounceTimer = 1000;

        private ITrainingPlanService _trainingPlanService;
        private ITrainingTagService _taggingService;
        private INavigationService _navigationService;
        private IDebouncerService _debouncer = new DebouncerService();


        #region Backing Fields
        private TrainingPlan _trainingPlan;
        private bool _hasProgression;
        private string _oldPlanName;
        public int _trainingWeeksCounter;
        public ICommand _trainingWeekChangedCommand;
        public ICommand _trainingPlanNameChangedCommand;
        public ICommand _navigateToHashtagsManagerCommand;
        public ICommand _changeBookmarkFlagCommand;
        public ICommand _proficienciesChangedCommand;
        public ICommand _phasesChangedCommand;
        #endregion


        /// <summary>
        /// The model
        /// </summary>        
        public TrainingPlan TrainingPlan
        {
            get => _trainingPlan;
            set
            {
                _trainingPlan = value;
                RaisePropertyChanged();
            }
        }

        public bool HasProgression
        {
            get => _hasProgression;
            set
            {
                _hasProgression = value;
                RaisePropertyChanged();
            }
        }

        public int TrainingWeeksCounter
        {
            get => _trainingWeeksCounter;
            set
            {
                _trainingWeeksCounter = value;
                RaisePropertyChanged();
            }
        }


        #region Commands

        public ICommand TrainingWeekChangedCommand => _trainingWeekChangedCommand ?? (_trainingWeekChangedCommand = 
            new Command(async x =>
            {
                int value = int.Parse(x.ToString());
                await _debouncer.DebounceAsync(DebounceTimer, e => AddOrRemoveTrainingWeekAsync(value));
            }));

        public ICommand TrainingPlanNameChangedCommand => _trainingPlanNameChangedCommand ?? (_trainingPlanNameChangedCommand =
            new Command(async () =>
            {
                await ChangeTrainingPlanNameAsync();    // Not debouncing for now
            }));

        public ICommand NavigateToHashtagsManagerCommand => _navigateToHashtagsManagerCommand ?? (_navigateToHashtagsManagerCommand =
            new Command(async () =>
            {
                await _navigationService.NavigateToAsync<TrainingPlanHashtagManagerViewModel>(TrainingPlan.Details.Hashtags);
            }));

        public ICommand ChangeBookmarkFlagCommand => _changeBookmarkFlagCommand ?? (_changeBookmarkFlagCommand = 
            new Command(async () =>
            {
                //await _trainingPlanService.SetTrainingPlanBookmarkedAsync(TrainingPlan.Details.IsBookmarked);
            }));

        public ICommand ProficienciesChangedCommand => _proficienciesChangedCommand ?? (_proficienciesChangedCommand =
            new Command(async x =>
            {
                // We suppose the user will not continuously select/deselect hence we keep the function simple (no debounce, no changes detection)
                if(x is OptionSelectedEventsArgs args)
                {
                    if (args.IsSelected)
                        await TagWithProficiencyAsync(args.ItemSelected as ISimpleTagElement);
                    else
                        await UntagProficiencyAsync(args.ItemSelected as ISimpleTagElement);
                }
            }));

        public ICommand PhasesChangedCommand => _phasesChangedCommand ?? (_phasesChangedCommand = 
            new Command(async x =>
            {
                // We suppose the user will not continuously select/deselect hence we keep the function simple (no debounce, no changes detection)
                if(x is OptionSelectedEventsArgs args)
                {
                    if (args.IsSelected)
                        await TagWithPhaseAsync(args.ItemSelected as ISimpleTagElement);
                    else
                        await UntagPhaseAsync(args.ItemSelected as ISimpleTagElement);
                }
            }));


        #endregion



        #region Ctors

        public TrainingPlanDetailViewModel(ITrainingPlanService trainingPlanService, ITrainingTagService taggingService, INavigationService navigationService)
        {
            _trainingPlanService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _taggingService = taggingService ?? throw new ArgumentNullException(nameof(taggingService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }


        #endregion


        public override void Dispose()
        {
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<ISimpleTagElement>>(this, MessageKeys.HashtagsChanged);
            base.Dispose();
        }


        /// <summary>
        /// Initializes the ViewModel.
        /// The caller must ensure that at least the Training Plan ID is passed as navigation parameter.
        /// </summary>
        /// <param name="navigationData">A TrainingPlan instance, loaded with all the available pre-fetched data (ID is mandatory)</param>
        /// <returns>The initialization Task</returns>
        public override async Task InitializeAsync(object navigationData)
        {
            // Check for pre-fetched data, but only if the page is not already in memory
            if (TrainingPlan == null && navigationData is TrainingPlan cachedPlan)
                TrainingPlan = cachedPlan;

            // Full load only if necessary - Should be performed according to the specific Parent View Model
            if (TrainingPlan == null)
                TrainingPlan.Details = await _trainingPlanService.GetTrainingPlanFullDetailsAsync(TrainingPlan.Details.UserPlanId).ConfigureAwait(false);
            else
                await LoadMissingDetails().ConfigureAwait(false);

            //await LoadTrainingSchedulesAsync();   

            _oldPlanName = TrainingPlan.Details.Name;
            TrainingWeeksCounter = TrainingPlan.Details.TrainingWeeksCounter ?? 0;
            HasProgression = TrainingPlan.Details.TrainingWeeksCounter > 0;

            // Subscriptions
            MessagingCenter.Unsubscribe<BaseViewModel, IEnumerable<ISimpleTagElement>>(this, MessageKeys.HashtagsChanged);
            MessagingCenter.Subscribe<BaseViewModel, IEnumerable<ISimpleTagElement>>(this, MessageKeys.HashtagsChanged, async (sender, args) => await ManageHashtags(args));

            await base.InitializeAsync(navigationData);
        }

        /// <summary>
        /// Load the fields which has no been fetched in the main form
        /// </summary>
        /// <returns>The loading task</returns>
        private async Task LoadMissingDetails()
        {
            TrainingPlanDetails missingDetails = await _trainingPlanService.GetTrainingPlanDetailsAsync(TrainingPlan.Details.UserPlanId).ConfigureAwait(false);

            // Map them
            TrainingPlan.Details.NoteId = missingDetails.NoteId;
            TrainingPlan.Details.Note = missingDetails.Note;
            TrainingPlan.Details.OwnerId = missingDetails.OwnerId;
            TrainingPlan.Details.ParentPlan = missingDetails.ParentPlan;
            TrainingPlan.Details.MusclesFocuses = missingDetails.MusclesFocuses;
        }


        /// <summary>
        /// Fetch the Training Plan schedules with related data and load them into the Model.
        /// </summary>
        /// <returns></returns>
        public async Task LoadTrainingSchedulesAsync()
        {
            //TrainingPlan.Schedules = await _trainingPlanService.GetTrainingSchedulesAsync(TrainingPlan.Id);
        }


        private async Task AddOrRemoveTrainingWeekAsync(int weeksCounter)
        {
            Debug.WriteLine("--- Changing the weeks counter to " + weeksCounter);

            IEnumerable<uint> resultWeekIds;
            string messageKey;
            int delta = weeksCounter - TrainingPlan.Details.TrainingWeeksCounter.Value;

            if (delta == 0)
                return;

            if (delta > 0)
            {
                // Adding
                resultWeekIds = await _trainingPlanService.AddTrainingWeeksAsync(TrainingPlan.Id.Value, delta);
                messageKey = MessageKeys.TrainingWeeksAdded;
            }
            else
            {
                // Removing
                resultWeekIds = await _trainingPlanService.RemoveTrainingWeeksAsync(TrainingPlan.Id.Value, -delta);
                messageKey = MessageKeys.TrainingWeeksRemoved;
            }

            // Now we can update the model
            TrainingPlan.Details.TrainingWeeksCounter = weeksCounter;

            // Check if command was ok
            if (resultWeekIds.Count() > 0)
            {
                TrainingWeeksCounter = TrainingPlan.Details.TrainingWeeksCounter.Value;
                // Tell to listeners that their model might be out-of-date
                MessagingCenter.Send(this as BaseViewModel, messageKey, resultWeekIds);
            }
            else
            {
                //RIGM: notify the user (no rollback needed)
            }
        }

        private async Task ChangeTrainingPlanNameAsync()
        {
            string newName = TrainingPlan.Details.Name;

            if (newName != _oldPlanName && !string.IsNullOrEmpty(newName))
            {
                if (!await _trainingPlanService.RenameTrainingPlanAsync(TrainingPlan.Details.UserPlanId, newName))
                {
                    TrainingPlan.Details.Name = _oldPlanName;

                    //RIGM: notify user
                }
            }
        }

        private async Task TagWithProficiencyAsync(ISimpleTagElement proficiency)
        {
            Debug.WriteLine("--- Adding proficiency: " + proficiency.Body);

            if (!await _trainingPlanService.AddTrainingPlanProficiencyAsync(TrainingPlan.Details.UserPlanId, proficiency))
                ; //RIGM: notify the user
        }

        private async Task UntagProficiencyAsync(ISimpleTagElement proficiency)
        {
            Debug.WriteLine("--- Removing proficiency: " + proficiency.Body);

            if (!await _trainingPlanService.RemoveTrainingPlanProficiencyAsync(TrainingPlan.Details.UserPlanId, proficiency))
                ; //RIGM: notify the user
        }

        private async Task TagWithPhaseAsync(ISimpleTagElement phase)
        {
            Debug.WriteLine("--- Adding proficiency: " + phase.Body);

            if (!await _trainingPlanService.AddTrainingPlanProficiencyAsync(TrainingPlan.Details.UserPlanId, phase))
                ; //RIGM: notify the user
        }

        private async Task UntagPhaseAsync(ISimpleTagElement phase)
        {
            Debug.WriteLine("--- Removing proficiency: " + phase.Body);

            if (!await _trainingPlanService.RemoveTrainingPlanProficiencyAsync(TrainingPlan.Details.UserPlanId, phase))
                ; //RIGM: notify the user
        }

        private async Task ChangePhasesAsync()
        {
            Debug.WriteLine("--- Changing proficiencies ");

            throw new NotImplementedException();
        }

        private async Task ManageHashtags(IEnumerable<ISimpleTagElement> hashtags)
        {
            bool ok = true;
            IEnumerable<ISimpleTagElement> removedHashtags = TrainingPlan.Details.Hashtags.Except(hashtags);
            IEnumerable<ISimpleTagElement> addedHashtags = hashtags.Except(TrainingPlan.Details.Hashtags).Where(x => x.Id != 0);

            // Let's pretend the operation will always be ok
            TrainingPlan.Details.Hashtags = new ObservableCollection<ISimpleTagElement>(hashtags);

            ok = await CreateNewHashtags(TrainingPlan.Details.Hashtags);

            if (!await _trainingPlanService.AddTrainingPlanHashtagsAsync(TrainingPlan.Details.UserPlanId, addedHashtags))
            {
                // Rollback!
                foreach (ISimpleTagElement invalidHashtag in removedHashtags)
                    TrainingPlan.Details.Hashtags.Remove(invalidHashtag);

                ok = false;
            }

            if (!await _trainingPlanService.RemoveTrainingPlanHashtagsAsync(TrainingPlan.Details.UserPlanId, removedHashtags))
            {
                // Rollback!
                foreach (ISimpleTagElement invalidHashtag in removedHashtags)
                    TrainingPlan.Details.Hashtags.Add(invalidHashtag);

                ok = false;
            }
            if (!ok)
                ;       //RIGM: Retry adn/or notify the user!
        }

        /// <summary>
        /// Add the hashtags which does not exist in the hashtag library (Id = 0).
        /// This function also modifies the input list (by adding the IDs) or rollbacks if operation fails.
        /// </summary>
        /// <param name="hashtags">The full hashtag list. IMPORTANT: this is modified as part of the function</param>
        /// <returns>The operation status</returns>
        private async Task<bool> CreateNewHashtags(IEnumerable<ISimpleTagElement> hashtags)
        {
            IEnumerable<ISimpleTagElement> requestedNewHashtags = hashtags.Where(x => x.Id == 0);

            IEnumerable<uint> createdHashtagsIds = await _taggingService.AddTrainingHashtagsAsync(requestedNewHashtags.Select(x => x.Body));

            if (createdHashtagsIds.Count() != requestedNewHashtags.Count())
                return false;       // This is a rollback

            var srcEnumerator = requestedNewHashtags.GetEnumerator();
            var destEnumerator = createdHashtagsIds.GetEnumerator();

            while(srcEnumerator.MoveNext() && destEnumerator.MoveNext())
                srcEnumerator.Current.Id = destEnumerator.Current;

            // Now hashtags stores the updated IDs, while requestedNewHashtag is empty
            return true;
        }
    }
}
