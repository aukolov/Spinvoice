using System;
using System.Linq;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using Spinvoice.Utils;

namespace Spinvoice.QuickBooks.Connection
{
    public class ExternalConnection
    {
        private readonly OAuthProfile _oauthProfile;
        private readonly OAuthParams _oauthParams;
        private DataService _dataService;

        public ExternalConnection(
            OAuthProfile oauthProfile, 
            OAuthParams oauthParams)
        {
            _oauthProfile = oauthProfile;
            _oauthParams = oauthParams;

            _oauthProfile.Updated += TryConnect;
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
            if (!_oauthProfile.IsReady)
            {
                return;
            }
            var oauthRequestValidator = new OAuthRequestValidator(
                _oauthProfile.AccessToken,
                _oauthProfile.AccessSecret,
                _oauthParams.ConsumerKey,
                _oauthParams.ConsumerSecret);
            var serviceContext = new ServiceContext(_oauthProfile.RealmId, IntuitServicesType.QBO, oauthRequestValidator);
            _dataService = new DataService(serviceContext);
            Connected.Raise();
        }
    }
}