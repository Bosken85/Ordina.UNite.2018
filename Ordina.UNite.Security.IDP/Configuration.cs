﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Ordina.UNite.Security.IDP
{
    public class Configuration
    {
        public static IEnumerable<IdentityResource> IdentityResources = new List<IdentityResource>
        {
            new IdentityServer4.Models.IdentityResources.OpenId(),
            new IdentityServer4.Models.IdentityResources.Profile(),
            new IdentityServer4.Models.IdentityResources.Address(),
            new IdentityResource("roles", "Your roles", new List<string> { "role" }),
            new IdentityResource("ordina", "Your Ordina information", new List<string>
            {
                "unit",
                "function",
                "level",
                "years_service",
            })
        };

        public static IEnumerable<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource("public_api", "Public API")
            {
                ApiSecrets = {
                    new Secret("api_secret".Sha256())
                },
                UserClaims =
                {
                    // Not all claims needed in private services need to be enabled in the public Api Gateway
                    "given_name",
                    "family_name",
                    "role"
                }
            },
            new ApiResource("private_api", "Private API")
            {
                UserClaims =
                {
                    "given_name",
                    "family_name",
                    "role",
                    "unit",
                    "function",
                    //"level"
                }
            },
            new ApiResource("aps", "Authorization policy service")
            {
                UserClaims =
                {
                    "given_name",
                    "family_name",
                    "role",
                    "unit",
                    //"function",
                    "level"
                }
            }
            // Add additional private ApiResource
        };

        public static IEnumerable<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",

                AllowedGrantTypes = GrantTypes.Hybrid,
                // This is the default and is done to minimize the payload of the id_token
                AlwaysIncludeUserClaimsInIdToken = false, 
                // this is set to true because we pass identity claims in the access token
                UpdateAccessTokenClaimsOnRefresh = true,
                // Request reference tokens for use in the public domain
                AccessTokenType = AccessTokenType.Reference,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RedirectUris           = { "http://localhost:44302/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:44302/signout-callback-oidc" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "ordina",
                    "public_api"
                },
                AllowOfflineAccess = true
            },
            // Add the api that will delegate calls to the private api
            new Client
            {
                ClientId = "public_api.client",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                AllowedGrantTypes = { "delegation" },
                AllowedScopes = new List<string>
                {
                    "private_api",
                    "aps"
                    // Add additional private api services
                }
            }
        };

        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "85441825-C3EC-4314-8102-08EE8699D96A",
                Username = "kevin",
                Password = "pass",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Kevin"),
                    new Claim("family_name", "Bosteels"),
                    new Claim("email", "kevin.bosteels@ordina.com"),
                    new Claim("address", "Red Street 16, 1000 Brussels, Belgium"),
                    new Claim("role", "Employee"),
                    new Claim("unit", "NCore"),
                    new Claim("function", "Developer"),
                    new Claim("level", "Senior"),
                    new Claim("years_service", "3")
                }
            },
            new TestUser
            {
                SubjectId = "85441825-C3EC-4314-8102-08EE8699D96B",
                Username = "jorgen",
                Password = "pass",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Jorgen"),
                    new Claim("family_name", "Jacob"),
                    new Claim("email", "jorgen.jacob@ordina.com"),
                    new Claim("address", "green Street 16, 1000 Brussels, Belgium"),
                    new Claim("role", "Admin"),
                    new Claim("unit", "NCore"),
                    new Claim("function", "Bum"),
                    new Claim("level", "Senior"),
                    new Claim("years_service", "2")
                }
            },
            new TestUser
            {
                SubjectId = "85441825-C3EC-4314-8102-08EE8699D96B",
                Username = "guest",
                Password = "pass",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Guest"),
                    new Claim("family_name", "Anonymous"),
                    new Claim("email", "guest.anonymous@gmail.com"),
                    new Claim("address", "Green Street 18, 1000 Brussels, Belgium"),
                    new Claim("role", "Guest")
                }
            }
        };
    }
}
