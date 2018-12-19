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

using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Whalerator.Support
{
    public class MemCacheFactory : ICacheFactory
    {
        private IMemoryCache _Cache;
        private TimeSpan _Ttl;

        public MemCacheFactory(IMemoryCache cache, TimeSpan ttl)
        {
            _Cache = cache;
            _Ttl = ttl;
        }

        public ICache<T> Get<T>() where T : class
        {
            return new MemCache<T>(_Cache, _Ttl);
        }
    }
}
