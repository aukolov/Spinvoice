using System;
using System.Collections.Generic;
using Spinvoice.Domain.ExternalBook;

namespace QuickBooksAuth
{
    internal class TestOAuthProfileDataAccess : IOAuthProfileDataAccess
    {
        private OAuthProfile _profile;

        public TestOAuthProfileDataAccess()
        {
            _profile = new OAuthProfile();
        }

        public void Dispose()
        {
        }

        public OAuthProfile[] GetAll()
        {
            return new[] { _profile };
        }

        public OAuthProfile Get(string id)
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdate(OAuthProfile entity)
        {
            _profile = entity;
        }

        public void AddOrUpdate(IEnumerable<OAuthProfile> entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
        }
    }
}