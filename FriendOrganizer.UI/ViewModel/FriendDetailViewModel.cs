using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;


namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IEventAggregator _eventAggregator;
        private IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        //private Friend Friend;

        public FriendDetailViewModel(IFriendRepository friendRepository, IEventAggregator eventAggregator) 
        {
            
            _friendRepository = friendRepository;
            _eventAggregator = eventAggregator; 
           

            SaveCommand = new DelegateCommand(OnSaveExecute,OnSaveCanExecute);  
        }

        public async Task LoadAsync(int friendId)
        {
            var friend = await _friendRepository.GetByIdAsync(friendId);

            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();  
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
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

        public ICommand SaveCommand { get; }

        private async void OnSaveExecute()
        {
           await _friendRepository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendSaveEvent>().Publish(new AfterFriendSaveEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        }
         
        private bool OnSaveCanExecute()
        {
            //To DO  :  Chack in addition if friend has changes 
            return Friend != null && !Friend.HasErrors;
        }

    }
}
