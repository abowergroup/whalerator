﻿/*
   Copyright 2018 Digimarc, Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   SPDX-License-Identifier: Apache-2.0
*/

using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whalerator.Support
{
    public class RedCache<T> : ICache<T> where T : class
    {
        private IConnectionMultiplexer _Mux;
        private int _Db;
        private TimeSpan _Ttl;

        public RedCache(IConnectionMultiplexer redisMux, int db, TimeSpan ttl)
        {
            _Mux = redisMux;
            _Db = db;
            _Ttl = ttl;
        }

        public bool Exists(string key) => _Mux.GetDatabase(_Db).KeyExists(key);

        public void Set(string key, T value, TimeSpan? ttl)
        {
            var json = JsonConvert.SerializeObject(value);
            _Mux.GetDatabase(_Db).StringSet(key, json, ttl);
        }

        public void Set(string key, T value) => Set(key, value, _Ttl);

        public Lock TakeLock(string key, TimeSpan lockTime, TimeSpan lockTimeout)
        {
            var value = Guid.NewGuid().ToString();
            var db = _Mux.GetDatabase(_Db);
            var start = DateTime.UtcNow;
            while (!db.LockTake(key, value, lockTime))
            {
                if (DateTime.Now - start > lockTimeout) { throw new TimeoutException($"Could not get a lock for {key} in the allotted time."); }
                System.Threading.Thread.Sleep(500);
            }
            return new Lock(
                releaseAction: () => { db.LockRelease(key, value); },
                extendAction: (time) => { return db.LockExtend(key, value, time); }
            );
        }

        public bool TryGet(string key, out T value)
        {
            try
            {
                var json = _Mux.GetDatabase(_Db).StringGet(key);
                value = json.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                value = null;
            }
            return value == null ? false : true;
        }
    }
}
