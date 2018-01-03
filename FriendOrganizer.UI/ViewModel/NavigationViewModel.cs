﻿using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupDataService;

        public ObservableCollection<Lookupitem> Friends { get; }

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService )
        {
            _friendLookupDataService = friendLookupDataService;
            Friends = new ObservableCollection<Lookupitem>();
        }
        public async Task LoadAsync()
        {
            var lookup = await _friendLookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
                Friends.Add(item);
        }
    }
}