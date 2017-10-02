using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace Spinvoice.Utils
{
    public class ListCollectionProxyView<TDomain, TViewModel, TDomainCollection> : ListCollectionView, IDisposable
        where TDomainCollection : class, INotifyCollectionChanged, IEnumerable<TDomain>
    {
        private readonly Func<TDomain, TViewModel> _viewModelFactory;
        private readonly Func<TDomain, TViewModel, bool> _comparer;

        private readonly TDomainCollection _domainCollection;

        public ListCollectionProxyView(
            TDomainCollection domainCollection,
            Func<TDomain, TViewModel> viewModelFactory,
            Func<TDomain, TViewModel, bool> comparer)
            : base(new ObservableCollection<TViewModel>())
        {
            _domainCollection = domainCollection;
            _viewModelFactory = viewModelFactory;
            _comparer = comparer;

            _domainCollection.Select(domain => _viewModelFactory(domain))
                .ForEach(vm => ViewModels.Add(vm));

            MoveCurrentToFirst();

            _domainCollection.CollectionChanged += OnDomainCollectionChanged;
        }

        public ObservableCollection<TViewModel> ViewModels => (ObservableCollection<TViewModel>)SourceCollection;

        private void OnDomainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.OfType<TDomain>()
                        .Select(domain => _viewModelFactory(domain))
                        .ForEach(vm => ViewModels.Add(vm));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.OfType<TDomain>()
                        .ForEach(domain =>
                        {
                            var viewModel = ViewModels.First(vm => _comparer(domain, vm));
                            ViewModels.Remove(viewModel);
                            TryDispose(viewModel);
                        });
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var viewModel in ViewModels)
                    {
                        TryDispose(viewModel);
                    }
                    ViewModels.Clear();
                    _domainCollection.Select(domain => _viewModelFactory(domain))
                        .ForEach(vm => ViewModels.Add(vm));
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (CurrentItem == null)
                MoveCurrentToFirst();
        }

        private static void TryDispose(TViewModel viewModel)
        {
            var disposableViewModel = viewModel as IDisposable;
            disposableViewModel?.Dispose();
        }

        public void Dispose()
        {
            _domainCollection.CollectionChanged -= OnDomainCollectionChanged;
        }
    }
}
