using EYB;
using EYB.FileManager;
using EYB.MVVM;
using SteamKeychain.Models;
using System;
using System.Collections.Generic;

namespace SteamKeychain.Data
{
    public class SteamKeyRepository : IRepository<SteamKey>
    {
        private JsonFile _file = null;
        private List<SteamKey> _keys = null;
        private ReadOnlyList<SteamKey> _items = null;

        public ReadOnlyList<SteamKey> Items { get { return _items; } }

        public SteamKeyRepository(JsonFile file)
        {
            _file = file;
            _keys = file.Load(new List<SteamKey>(0));
            _items = new ReadOnlyList<SteamKey>(ref _keys);
        }

        public void Delete(SteamKey item)
        {
            if (_keys.Remove(item))
            {
                _file.Save(_keys);
            }
        }

        public void Update(SteamKey item)
        {
            if (_keys.Contains(item))
            {
                _file.Save(_keys);
            }
        }

        public void Add(SteamKey item)
        {
            if (_keys.Contains(item))
            {
                throw new ArgumentException("Cannot add the same reference twice.", nameof(item));
            }

            _keys.Add(item);
            _file.Save(_keys);
        }
    }
}
