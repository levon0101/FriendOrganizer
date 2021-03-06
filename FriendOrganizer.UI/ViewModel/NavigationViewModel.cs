﻿using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IEventAggregator _eventAggregator;
        private IFriendLookupDataService _friendLookupDataService;
        private IMeetingLookupDataService _meetingLookupDataService;

        public ObservableCollection<NavigationItemViewModel> Friends { get; }
        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService,
            IMeetingLookupDataService meetingLookupDataService,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _friendLookupDataService = friendLookupDataService;
            _meetingLookupDataService = meetingLookupDataService;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    nameof(FriendDetailViewModel),
                    _eventAggregator));
            }

            lookup = await _meetingLookupDataService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var item in lookup)
            {
                Meetings.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    nameof(MeetingDetailViewModel),
                    _eventAggregator));
            }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleted(Friends, args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }

        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(f => f.Id == args.Id);
            if (item != null)
            {
                items.Remove(item); 
            }
        }

        private void AfterDetailSaved(AfterDetailSaveEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSaved(Friends, args);
                    break; 
                case nameof(MeetingDetailViewModel):
                    AfterDetailSaved(Meetings, args);
                    break;
            }


        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSaveEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id); 
            if (lookupItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember,
                    nameof(args.ViewModelName),
                    _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMember = args.DisplayMember;
            }
        }
    }
}
