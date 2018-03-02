﻿using System.Threading.Tasks;
using Prism.Events;
using FriendOrganizer.UI.Event;
using System;
using FriendOrganizer.UI.View.Services;
using System.Windows.Input;
using Prism.Commands;
using Autofac.Features.Indexed;
using System.Collections.ObjectModel;
using System.Linq;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;
        private IDetailViewModel _selectedDetailViewModel;



        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _detailViewModelCreator = detailViewModelCreator;
            _messageDialogService = messageDialogService;

            DetailViewModels = new ObservableCollection<IDetailViewModel>();

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
               .Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>() //AfterDetailDeletedEvent
                .Subscribe(AfterDetailDeleted);
            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Subscribe(AfterDetailClosed);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

            NavigationViewModel = navigationViewModel;
        }



        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }

        public IDetailViewModel SelectedDetailViewModel
        {
            get { return _selectedDetailViewModel; }
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }



        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            /*  //-Old Version of single tab controls
            if (SelectedDetailViewModel != null && SelectedDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Nawigate away ?", "Question");

                if (result == MessageDialogResult.Cancel)
                    return;
            }*/
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id
                 && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                await detailViewModel.LoadAsync(args.Id);
                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel; // using AutoFac.Indexer constructor


        }

        private int nextNewItemId = 0;
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs
            {
                Id = nextNewItemId--,
                ViewModelName = viewModelType.Name
            });
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == id
                 && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
        }

    }
}
