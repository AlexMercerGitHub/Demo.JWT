﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Demo.JWT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [Route("/api/jwt")]
        [HttpGet]
        public async Task<dynamic> JWT()
        {

            return "无权限获取";
        }

        [Route("/api/Authorize")]
        [HttpGet]
        [Authorize]
        public  dynamic Authorize()
        {

            return "有权限获取";
        }
        // [AllowAnonymous] 是什么意思

        [Route("/api/login")]
        [HttpPost]
        public IActionResult login()
        {

            var claims = new[]
            {
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, "测试jwt")
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Const.Domain,
                audience: Const.Domain,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public static class Const
    {
        /// <summary>
        /// 这里为了演示，写死一个密钥。实际生产环境可以从配置文件读取,这个是用网上工具随便生成的一个密钥
        /// </summary>
        public const string SecurityKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1+SitIeJSWtLJU8/Wz2m7gStexajkeD+Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB";
        /// <summary>
        /// 站点地址
        /// </summary>
        public const string Domain = "http://localhost:5000";

        /// <summary>
        /// 受理人，之所以弄成可变的是为了用接口动态更改这个值以模拟强制Token失效
        /// 真实业务场景可以在数据库或者redis存一个和用户id相关的值，生成token和验证token的时候获取到持久化的值去校验
        /// 如果重新登陆，则刷新这个值
        /// </summary>
        public static string ValidAudience;
    }
}
