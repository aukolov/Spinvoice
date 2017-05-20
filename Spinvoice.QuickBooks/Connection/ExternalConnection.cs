using System;
using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using Spinvoice.Domain.ExternalBook;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class ExternalConnection
    {
        private readonly IOAuthRepository _oauthRepository;
        private DataService _dataService;

        public ExternalConnection(
            IOAuthRepository oauthRepository)
        {
            _oauthRepository = oauthRepository;

            _oauthRepository.Profile.Updated += TryConnect;
            TryConnect();
        }

        public event Action Connected;

        public bool IsReady => _dataService != null;

        public T Add<T>(T entity) where T : IEntity
        {
            if (!IsReady) throw new InvalidOperationException();
            return _dataService.Add(entity);
        }

        public T[] GetAll<T>() where T : IEntity, new()
        {
            if (!IsReady) throw new InvalidOperationException();
            return _dataService.FindAll(new T()).ToArray();
        }

        private void TryConnect()
        {
            if (!_oauthRepository.Profile.IsReady)
            {
                return;
            }
            var oauthRequestValidator = new OAuthRequestValidator(
                _oauthRepository.Profile.AccessToken,
                _oauthRepository.Profile.AccessSecret,
                _oauthRepository.Params.ConsumerKey,
                _oauthRepository.Params.ConsumerSecret);
            var serviceContext = new ServiceContext(
                _oauthRepository.Profile.RealmId, 
                IntuitServicesType.QBO, 
                oauthRequestValidator);
            _dataService = new DataService(serviceContext);
            Connected.Raise();
        }
    }
}