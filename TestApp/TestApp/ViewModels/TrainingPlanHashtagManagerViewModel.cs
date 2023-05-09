using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApp.Models.Common;
using TestApp.Services.AppSession;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TestApp.Services.Utils;
using TestApp.ViewModels.Base;
using Xamarin.Forms;

namespace TestApp.ViewModels
{
    public class TrainingPlanHashtagManagerViewModel : BaseViewModel
    {


        private INavigationService _navigationService;
        private ITrainingPlanService _trainingService;
        private ITrainingTagService _tagService;
        private IDebouncerService _debouncer = new DebouncerService();



        #region Backing Fields
        private ObservableCollection<ISimpleTagElement> _userFavouriteHashtags;
        private ObservableCollection<ISimpleTagElement> _planHashtags;
        private string _hashtagText; 
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        private ICommand _removeHashtagCommand;
        private ICommand _addHashtagCommand;
        private ICommand _suggestedHashtagSelectedCommand;
        #endregion

        public string HashtagText
        {
            get => _hashtagText;
            set
            {
                _hashtagText = value;
                RaisePropertyChanged();
                RefreshSuggestedHashtags(_hashtagText);     //RIGM: todo
            }
        }

        public ObservableCollection<ISimpleTagElement> UserFavouriteHashtags
        {
            get => _userFavouriteHashtags;
            set
            {
                _userFavouriteHashtags = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ISimpleTagElement> PlanHashtags
        {
            get => _planHashtags;
            set
            {
                _planHashtags = value;
                RaisePropertyChanged();
            }
        }





        #region Commands

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new Command(async () =>
         {
             MessagingCenter.Send(this as BaseViewModel, MessageKeys.HashtagsChanged, PlanHashtags as IEnumerable<ISimpleTagElement>);
             await _navigationService.GoBackAsync(true);
         }));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new Command(async () =>
         {
             await _navigationService.GoBackAsync(true);
         }));

        public ICommand RemoveHashtagCommand => _removeHashtagCommand ?? (_removeHashtagCommand = new Command(async x =>
         {
             PlanHashtags.Remove(x as ISimpleTagElement);
         }));

        public ICommand AddHashtagCommand => _addHashtagCommand ?? (_addHashtagCommand = new Command(async inputText =>
         {
             string body = inputText as string;
             ISimpleTagElement hashtag = await _tagService.FindTrainingHashtagByBody(body);

             AddHashtag(hashtag ?? new SimpleTagElement
             {
                 Id = 0,
                 Body = body,
             });
         }));

        public ICommand SuggestedHashtagSelectedCommand => _suggestedHashtagSelectedCommand ?? (_suggestedHashtagSelectedCommand = new Command(async x =>
         {
             ISimpleTagElement hashtag = x as ISimpleTagElement;

             if (!PlanHashtags.Contains(hashtag))
                 AddHashtag(hashtag);
         }));

        #endregion




        #region Ctors

        public TrainingPlanHashtagManagerViewModel(ITrainingPlanService trainingPlanService,ITrainingTagService tagService, INavigationService navigationService)
        {
            _trainingService = trainingPlanService ?? throw new ArgumentNullException(nameof(trainingPlanService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion





        private void RefreshSuggestedHashtags(string typedText)
        {
            //_debouncer.DebounceAsync(500, _taggingService.GetHashtagsStartingWith(typedText));
        }

        private void AddHashtag(ISimpleTagElement hashtag)
        {
            if(PlanHashtags.Count < Services.Utils.AppEnvironment.TrainingHashtagsMaxNumber)
            {
                if(hashtag.Id == 0)
                {
                    // If the hashtag is already present in the list then do nothing
                    if (!PlanHashtags.Any(x => x.Body == hashtag.Body))
                    {
                        PlanHashtags.Add(hashtag);
                        HashtagText = string.Empty;
                    }
                }
                else
                {
                    // If the hashtag is already present in the list then do nothing
                    if (!PlanHashtags.Contains(hashtag))
                    {
                        PlanHashtags.Add(hashtag);
                        HashtagText = string.Empty;
                    }
                }
            }
        }




        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is ObservableCollection<ISimpleTagElement> hashtags)
                PlanHashtags = new ObservableCollection<ISimpleTagElement>(hashtags);       // Value copy, the caller will commit/rollback
            else
                PlanHashtags = new ObservableCollection<ISimpleTagElement>();
            
            UserFavouriteHashtags = new ObservableCollection<ISimpleTagElement>(
                await _trainingService.GetFavouriteHashtagsAsync(AppSession.CurrentUserId.Value));

            await base.InitializeAsync(navigationData);
        }

    }
}
