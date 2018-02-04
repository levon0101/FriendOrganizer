using System.Threading.Tasks;
using Prism.Events;
using FriendOrganizer.UI.Event;
using System;
using FriendOrganizer.UI.View.Services;
using System.Windows.Input;
using Prism.Commands;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private IDetailViewModel _detailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
               .Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

            NavigationViewModel = navigationViewModel;
        }


        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            private set
            {
                _detailViewModel = value;
                OnPropertyChanged();
            }
        }



        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Nawigate away ?", "Question");

                if (result == MessageDialogResult.Cancel)
                    return;
            }
           
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    {
                        DetailViewModel = _friendDetailViewModelCreator();
                        
                        break;
                    }
            }

            await DetailViewModel.LoadAsync(args.Id);

        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs {
                ViewModelName = viewModelType.Name
                });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }
    }
}
