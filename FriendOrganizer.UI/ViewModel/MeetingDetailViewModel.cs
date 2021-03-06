﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private MeetingWrapper _meeting;
        private IMeetingRepository _meetingRepository;


        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;
        private List<Friend> _allFriends;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository
            ) : base(eventAggregator, messageDialogService)
        {

            _meetingRepository = meetingRepository;
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddFriendCommand = new DelegateCommand(OnAddFrendExecute, OnAddFrendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFrendExecute, OnRemoveFrendCanExecute);
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            if(args.ViewModelName == nameof(FriendDetailViewModel))
            {          
                _allFriends = await _meetingRepository.GetAllFriendsAsync();

                SetupPickList();
            }
        }

        private async void AfterDetailSaved(AfterDetailSaveEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await _meetingRepository.ReloadFriendAsync(args.Id);

                _allFriends = await _meetingRepository.GetAllFriendsAsync();

                SetupPickList();
            }
        }

        public ObservableCollection<Friend> AddedFriends { get; }
        public ObservableCollection<Friend> AvailableFriends { get; }
        public DelegateCommand AddFriendCommand { get; }
        public DelegateCommand RemoveFriendCommand { get; }

        public Friend SelectedAvailableFriend
        {
            get
            {
                return _selectedAvailableFriend;
            }
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public Friend SelectedAddedFriend
        {
            get
            {
                return _selectedAddedFriend;
            }
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }
        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            private set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }

        protected override async void OnDeleteExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you realy delete the meeting {Meeting.Title}", "Question");
            if (result == MessageDialogResult.Ok)
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                ReiseDetailDeleteEvent(Meeting.Id);
            }

        }

        public override async Task LoadAsync(int meetingId)
        {
            var meeting = meetingId > 0
                ? await _meetingRepository.GetByIdAsync(meetingId)
                : CreateNewMeeting();

            Id = meeting.Id;

            InitializeMeeting(meeting);

            _allFriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPickList();
        }

        private void SetupPickList()
        {

            var meetingFrendId = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allFriends.Where(f => meetingFrendId.Contains(f.Id)).OrderBy(f => f.FirstName);
            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName);


            AddedFriends.Clear();
            AvailableFriends.Clear();

            foreach (var addedFriend in addedFriends)
            {
                AddedFriends.Add(addedFriend);
            }
            foreach (var availableFriend in availableFriends)
            {
                AvailableFriends.Add(availableFriend);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }



        private bool OnRemoveFrendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        private bool OnAddFrendCanExecute()
        {
            //return true;
            return SelectedAvailableFriend != null;
        }

        private void OnRemoveFrendExecute()
        {
            var friendtoRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendtoRemove);
            AddedFriends.Remove(friendtoRemove);
            AvailableFriends.Add(friendtoRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddFrendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }



        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();

            HasChanges = _meetingRepository.HasChanges();

            Id = Meeting.Id;

            ReiseDetailSaveEvent(Meeting.Id, Meeting.Title);
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date
            };
            _meetingRepository.Add(meeting);

            return meeting;
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Meeting.Title))
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Meeting.Id == 0)
            {
                //Little trick to trigger validation
                Meeting.Title = "";
            }

            SetTitle();
        }

        private void SetTitle()
        {
            Title = Meeting.Title;
        }
    }
}
