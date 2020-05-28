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

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whalerator.Client;
using Whalerator.Config;
using Whalerator.Queue;

namespace Whalerator.WebAPI
{
    public class IndexWorker : QueueWorker<IndexRequest>
    {
        private readonly IIndexStore indexStore;
        private readonly RegistryAuthenticationDecoder authDecoder;
        private readonly IAuthHandler authHandler;

        public IndexWorker(ILogger<IndexWorker> logger, ServiceConfig config, IWorkQueue<IndexRequest> queue, IClientFactory clientFactory, IIndexStore indexStore,
           RegistryAuthenticationDecoder authDecoder, IAuthHandler authHandler) : base(logger, config, queue, clientFactory)
        {
            this.indexStore = indexStore;
            this.authDecoder = authDecoder;
            this.authHandler = authHandler;
        }


        public override void DoRequest(IndexRequest request)
        {
            try
            {
                var authResult = authDecoder.AuthenticateAsync(request.Authorization).Result;
                if (!authResult.Succeeded)
                {
                    logger.LogWarning(authResult.Failure, "Authorization failed for the work item. A token may have expired since it was first submitted.");
                }
                else
                {
                    authHandler.Login(authResult.Principal.ToRegistryCredentials());
                    var client = clientFactory.GetClient(authHandler);

                    var imageSet = client.GetImageSet(request.TargetRepo, request.TargetDigest);
                    if ((imageSet?.Images?.Count() ?? 0) != 1) { throw new Exception($"Couldn't find a valid image for {request.TargetRepo}:{request.TargetDigest}"); }
                    var image = imageSet.Images.First();

                    var indexes = client.GetIndexes(request.TargetRepo, image, request.TargetPath);
                    indexStore.SetIndex(indexes, image.Digest, request.TargetPath);
                    logger.LogInformation($"Completed indexing {request.TargetRepo}:{request.TargetDigest} {(string.IsNullOrEmpty(request.TargetPath) ? "" : $"({request.TargetPath})")}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Processing failed for work item\n {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
            }
        }
    }
}
