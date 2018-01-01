using FriendOrganizer.Model;
using System.Collections.ObjectModel;
using FriendOrganizer.UI.Data;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel: ViewModelBase
    {

        private IFriendDataService _FriendDataService;
        private Friend _selectedFriend;

        public ObservableCollection<Friend> Friends { get; set; }

        public MainViewModel(IFriendDataService FriendDataService)
        {
            Friends = new ObservableCollection<Friend>();
            _FriendDataService = FriendDataService;
        }

        public async Task LoadAsync()
        {
            var firends = await _FriendDataService.GetAllAsync();
            Friends.Clear();
            foreach (var frined in firends)
            {
                Friends.Add(frined);
            }
        }

        

        

        public Friend SelectedFriend
        {
            get { return _selectedFriend; }
            set { _selectedFriend = value;
                //OnPropertyChanged("SelectedFriend");
                //OnPropertyChanged(nameof(SelectedFriend)); // in C# 6.0 same as OnPropertyChanged( "SelectedFriend")
                OnPropertyChanged(); //[CallerMemberName] will automatic send propName parameter
            }

        }
     

    }
}
