﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Whalerator.Client;
using Whalerator.Support;
using Whalerator.WebAPI.Contracts;

namespace Whalerator.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private ICryptoAlgorithm _Crypto;

        public TokenController(ICryptoAlgorithm crypto)
        {
            _Crypto = crypto;
        }

        [HttpPost]
        public IActionResult Post([FromBody]RegistryCredentials credentials)
        {
            using (var client = new HttpClient())
            {
                var result = client.GetAsync($"https://{credentials.Registry}/v2/").Result;
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var handler = new BasicAuthHandler() { UserName = credentials.Username, Password = credentials.Password };
                    var token = handler.GetToken(result.Headers.WwwAuthenticate.First());
                    var json = JsonConvert.SerializeObject(credentials);
                    var cipherText = _Crypto.Encrypt(json);

                    var jwt = Jose.JWT.Encode(new Token { Crd = cipherText }, _Crypto.ToRSACryptoServiceProvider(), Jose.JwsAlgorithm.RS256);
                    return Ok(new { token = jwt });
                }
                else
                {
                    return BadRequest($"The remote server returned an unexpected status: {result.StatusCode}");
                }
            }
        }
    }
}