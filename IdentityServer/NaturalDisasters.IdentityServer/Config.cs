// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace NaturalDisasters.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
      {
            new ApiResource("resource_aid") { Scopes = { "aid_fullpermission" }},
            new ApiResource("resource_photo_stock") { Scopes = { "photo_stock_fullpermission" }},
            //new ApiResource("https://10.0.2.2:5001/resources") { Scopes = { "scope_name" }}, //
            //new ApiResource("https://10.0.2.2:5011/resources") { Scopes = { "scope_name2" }}, //

            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
      };
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                       new IdentityResources.Email(),
                       new IdentityResources.OpenId(), //mutlaka olmalı
                       new IdentityResources.Profile(),
                       new IdentityResource()
                       {
                           Name = "roles",
                           DisplayName = "Roles" ,
                           Description = "Kullanıcı Rolleri",
                           UserClaims = new[]{"role"}
                       }
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("aid_fullpermission","Aid API için sınırlı erişim"),
                new ApiScope("photo_stock_fullpermission","Photo Stock API için full erişim"),
                //new ApiScope("scope_name", "Description of the scope"), //
                //new ApiScope("scope_name2", "Description of the scope"), //

                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                 new Client
                {
                    ClientName="Asp.Net Core Mobile",
                    ClientId = "MobileClientForUser",
                    AllowOfflineAccess = true,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, //refresh token var
                    AllowedScopes = {
                        "aid_fullpermission",
                        "photo_stock_fullpermission",
                        //"scope_name",
                        //"scope_name2",

                         IdentityServerConstants.StandardScopes.Email,
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile,
                         IdentityServerConstants.StandardScopes.OfflineAccess, //refresh token dönebilmek için
                         IdentityServerConstants.LocalApi.ScopeName,
                         "roles"
                    },
                    AccessTokenLifetime=1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime=(int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds,
                    RefreshTokenUsage = TokenUsage.ReUse
                },
            };
    }
}