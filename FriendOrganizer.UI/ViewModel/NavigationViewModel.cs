using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
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

        public ObservableCollection<Lookupitem> Friends { get; }

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService, 
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
        private Lookupitem _selectedFriend;

        public Lookupitem SelectedFriend
        {
            get { return _selectedFriend; }
            set {
                _selectedFriend = value;
                OnPropertyChanged();
                if(_selectedFriend != null)
                {
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>().Publish(_selectedFriend.Id);
                } 
            }
        }

    }
}
