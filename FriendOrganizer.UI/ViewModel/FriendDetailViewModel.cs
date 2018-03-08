using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {

        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
        private IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        //  private bool _hasChanges;


        public FriendDetailViewModel(IFriendRepository friendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
            : base(eventAggregator, messageDialogService)
        {

            _friendRepository = friendRepository;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            eventAggregator.GetEvent<AfterCollectionSavedEvent>().Subscribe(AfterCollectionSaved);

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberCommand, OnRemovePhoneNumberCanExecute);

            ProgrammingLanguages = new ObservableCollection<Lookupitem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }


        private void OnRemovePhoneNumberCommand()
        {
            SelectedPhoneNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;

            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = "";

        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        public override async Task LoadAsync(int friendId)
        {
            var friend = friendId > 0
                ? await _friendRepository.GetByIdAsync(friendId)
                : CreateNewFriend();
            Id = friend.Id;

            InitializeFriend(friend);

            InitializeFriendPhoneNumbers(friend.PhoneNumbers);

            await LoadProgrammingLenguagesLookupAsync();
        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> phoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();

            foreach (var friendPhoneNumber in phoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }

        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public FriendWrapper Friend
        {
            get { return _friend; }

            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        private FriendPhoneNumberWrapper _selectedPhoneNumber;

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }


        public ICommand AddPhoneNumberCommand { get; }

        public ICommand RemovePhoneNumberCommand { get; }

        public ObservableCollection<Lookupitem> ProgrammingLanguages { get; }

        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        private void InitializeFriend(Friend friend)
        {
            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Friend.FirstName)
                || e.PropertyName == nameof(Friend.LastName))
                {
                    SetTitle();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
            {
                //Litle trick to trriger the validation
                Friend.FirstName = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Friend.FirstName} {Friend.LastName}";
        }

        private async Task LoadProgrammingLenguagesLookupAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem { DisplayMember = " - " });
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var loocupItem in lookup)
            {
                ProgrammingLanguages.Add(loocupItem);
            }
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurencyAsync(_friendRepository.SaveAsync,
            () =>
            {
                HasChanges = _friendRepository.HasChanges();
                Id = Friend.Id;
                ReiseDetailSaveEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");

            });
           


        }

        protected override bool OnSaveCanExecute()
        {
            //To DO  :  Chack in addition if friend has changes 
            return Friend != null && !Friend.HasErrors && HasChanges;
        }

        protected override async void OnDeleteExecute()
        {

            if (await _friendRepository.HasMeetingsAsync(Friend.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"{Friend.FirstName} {Friend.LastName} can't be deleted, He part of least one meeting");
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you realy want to delete the friend {Friend.FirstName} " +
                $"{Friend.LastName}", "Question");

            if (result == MessageDialogResult.Ok)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                ReiseDetailDeleteEvent(Friend.Id);

            }

        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(ProgrammingLanguageDetailViewModel))
            {
                await LoadProgrammingLenguagesLookupAsync();
            }
        }

    }
}
