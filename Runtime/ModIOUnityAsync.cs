﻿using ModIO.Implementation;
using System;
using JetBrains.Annotations;
using UnityEngine;
using System.Threading.Tasks;
using ModIO.Implementation.API.Objects;

#pragma warning disable 4014 // Ignore warnings about calling async functions from non-async code

namespace ModIO
{
    /// <summary>
    /// Main async interface for the mod.io Unity plugin. Every method within
    /// ModIOUnity.cs that has a callback can also be found in ModIOUnityAsync with an asynchronous
    /// alternative method (if you'd rather not use callbacks).
    /// </summary>
    /// <seealso cref="ModIOUnity"/>
    public static class ModIOUnityAsync
    {
#region Initialization and Maintenance

        /// <summary>
        /// Cancels any running public operations, frees plugin resources, and invokes
        /// any pending callbacks with a cancelled result code.
        /// </summary>
        /// <remarks>
        /// pending operations during a shutdown can be checked with
        /// Result.IsCancelled()
        /// </remarks>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     await ModIOUnityAsync.Shutdown();
        ///     Debug.Log("Finished shutting down the ModIO Plugin");
        /// }
        /// </code>
        public static async Task Shutdown()
        {
            await ModIOUnityImplementation.Shutdown(() => { });
        }

#endregion // Initialization and Maintenance

#region Authentication

        /// <summary>
        /// Sends an email with a security code to the specified Email Address. The security code
        /// is then used to Authenticate the user session using ModIOUnity.SubmitEmailSecurityCode()
        /// </summary>
        /// <remarks>
        /// The operation will return a Result object.
        /// If the email is successfully sent Result.Succeeded() will equal true.
        /// If you haven't Initialized the plugin then Result.IsInitializationError() will equal
        /// true. If the string provided for the emailaddress is not .NET compliant
        /// Result.IsAuthenticationError() will equal true.
        /// </remarks>
        /// <param name="emailaddress">the Email Address to send the security code to, eg "JohnDoe@gmail.com"</param>
        /// <seealso cref="SubmitEmailSecurityCode"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.RequestAuthenticationEmail("johndoe@gmail.com");
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Succeeded to send security code");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to send security code to that email address");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> RequestAuthenticationEmail(string emailaddress)
        {
            return await ModIOUnityImplementation.RequestEmailAuthToken(emailaddress);
        }

        /// <summary>
        /// Attempts to Authenticate the current session by submitting a security code received by
        /// email from ModIOUnity.RequestAuthenticationEmail()
        /// </summary>
        /// <remarks>
        /// It is intended that this function is used after ModIOUnity.RequestAuthenticationEmail()
        /// is performed successfully.
        /// </remarks>
        /// <param name="securityCode">The security code received from an authentication email</param>
        /// <seealso cref="RequestAuthenticationEmail"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example(string userSecurityCode)
        /// {
        ///     Result result = await ModIOUnityAsync.SubmitEmailSecurityCode(userSecurityCode);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("You have successfully authenticated the user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate the user");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> SubmitEmailSecurityCode(string securityCode)
        {
            return await ModIOUnityImplementation.SubmitEmailSecurityCode(securityCode);
        }
/// <summary>
        /// This retrieves the terms of use text to be shown to the user to accept/deny before
        /// authenticating their account via a third party provider, eg steam or google.
        /// </summary>
        /// <remarks>
        /// If the operation succeeds it will also provide a TermsOfUse struct that contains a
        /// TermsHash struct which you will need to provide when calling a third party
        /// authentication method such as ModIOUnity.AuthenticateUserViaSteam()
        /// </remarks>
        /// <param name="serviceProvider">The provider you intend to use for authentication,
        /// eg steam, google etc. (You dont need to display terms of use to the user if they are
        /// authenticating via email security code)</param>
        /// <seealso cref="TermsOfUse"/>
        /// <seealso cref="AuthenticateUserViaDiscord"/>
        /// <seealso cref="AuthenticateUserViaGoogle"/>
        /// <seealso cref="AuthenticateUserViaGOG"/>
        /// <seealso cref="AuthenticateUserViaItch"/>
        /// <seealso cref="AuthenticateUserViaOculus"/>
        /// <seealso cref="AuthenticateUserViaSteam"/>
        /// <seealso cref="AuthenticateUserViaSwitch"/>
        /// <seealso cref="AuthenticateUserViaXbox"/>
        /// <seealso cref="AuthenticateUserViaPlayStation"/>
        /// <code>
        /// async void Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<TermsOfUse>> GetTermsOfUse()
        {
            return await ModIOUnityImplementation.GetTermsOfUse();
        }

        /// <summary>
        /// Attempts to authenticate a user via the steam API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="steamToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaSteam(steamToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaSteam(string steamToken,
                                                                  [CanBeNull] string emailAddress,
                                                                  [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                steamToken, AuthenticationServiceProvider.Steam, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the epic API.
        /// </summary>
        /// <param name="epicToken">the user's epic token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <seealso cref="ModIOUnity.AuthenticateUserViaEpic"/>
        public static async Task<Result> AuthenticateUserViaEpic(string epicToken,
                                                                  [CanBeNull] string emailAddress,
                                                                  [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                epicToken, AuthenticationServiceProvider.Steam, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the steam API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="authCode">the user's authcode token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaPlayStation(authCode, "johndoe@gmail.com", modIOTermsOfUse.hash, PlayStationEnvironment.np);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaPlayStation(string authCode,
                                                                  [CanBeNull] string emailAddress,
                                                                  [CanBeNull] TermsHash? hash,
                                                                  PlayStationEnvironment environment)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                authCode, AuthenticationServiceProvider.PlayStation, emailAddress, hash, null, null,
                null, environment);
        }

        /// <summary>
        /// Attempts to authenticate a user via the GOG API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="gogToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaGOG(gogToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaGOG(string gogToken, [CanBeNull] string emailAddress,
                                                                [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(gogToken, AuthenticationServiceProvider.GOG,
                emailAddress, hash, null, null, null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the Itch.io API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="itchioToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaItch(itchioToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaItch(string itchioToken,
                                                                 [CanBeNull] string emailAddress,
                                                                 [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                itchioToken, AuthenticationServiceProvider.Itchio, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the Xbox API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="xboxToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaItch(xboxToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaXbox(string xboxToken,
                                                                 [CanBeNull] string emailAddress,
                                                                 [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(xboxToken, AuthenticationServiceProvider.Xbox,
                emailAddress, hash, null, null, null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the switch API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="switchToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaItch(switchToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaSwitch(string switchToken,
                                                                   [CanBeNull] string emailAddress,
                                                                   [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                switchToken, AuthenticationServiceProvider.Switch, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the discord API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="discordToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaDiscord(discordToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaDiscord(string discordToken,
                                                                    [CanBeNull] string emailAddress,
                                                                    [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                discordToken, AuthenticationServiceProvider.Discord, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the google API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="googleToken">the user's steam token</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaGoogle(googleToken, "johndoe@gmail.com", modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaGoogle(string googleToken,
                                                                   [CanBeNull] string emailAddress,
                                                                   [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                googleToken, AuthenticationServiceProvider.Google, emailAddress, hash, null, null,
                null, 0);
        }

        /// <summary>
        /// Attempts to authenticate a user via the oculus API.
        /// </summary>
        /// <remarks>
        /// You will first need to get the terms of use and hash from the ModIOUnity.GetTermsOfUse()
        /// method.
        /// </remarks>
        /// <param name="oculusToken">the user's oculus token</param>
        /// <param name="oculusDevice">the device you're authenticating on</param>
        /// <param name="nonce">the nonce</param>
        /// <param name="userId">the user id</param>
        /// <param name="emailAddress">the user's email address</param>
        /// <param name="hash">the TermsHash retrieved from ModIOUnity.GetTermsOfUse()</param>
        /// <seealso cref="GetTermsOfUse"/>
        /// <code>
        /// // First we get the Terms of Use to display to the user and cache the hash
        /// async void GetTermsOfUse_Example()
        /// {
        ///     ResultAnd&#60;TermsOfUser&#62; response = await ModIOUnityAsync.GetTermsOfUse();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully retrieved the terms of use: " + response.value.termsOfUse);
        ///
        ///         //  Cache the terms of use (which has the hash for when we attempt to authenticate)
        ///         modIOTermsOfUse = response.value;
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to retrieve the terms of use");
        ///     }
        /// }
        ///
        /// // Once we have the Terms of Use and hash we can attempt to authenticate
        /// async void Authenticate_Example()
        /// {
        ///     Result result = await ModIOUnityAsync.AuthenticateUserViaOculus(OculusDevice.Quest,
        ///                                                                     nonce,
        ///                                                                     userId,
        ///                                                                     oculusToken,
        ///                                                                     "johndoe@gmail.com",
        ///                                                                     modIOTermsOfUse.hash);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully authenticated user");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to authenticate");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AuthenticateUserViaOculus(OculusDevice oculusDevice, string nonce,
                                                                   long userId, string oculusToken,
                                                                   [CanBeNull] string emailAddress,
                                                                   [CanBeNull] TermsHash? hash)
        {
            return await ModIOUnityImplementation.AuthenticateUser(
                oculusToken, AuthenticationServiceProvider.Oculus, emailAddress, hash, nonce,
                oculusDevice, userId.ToString(), 0);
        }

        /// <summary>
        /// Informs you if the current user session is authenticated or not.
        /// </summary>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.IsAuthenticated();
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("current session is authenticated");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("current session is not authenticated");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> IsAuthenticated()
        {
            return await ModIOUnityImplementation.IsAuthenticated();
        }

#endregion // Authentication

#region Mod Browsing

        /// <summary>
        /// Gets the existing tags for the current game Id that can be used when searching/filtering
        /// mods.
        /// </summary>
        /// <remarks>
        /// Tags come in category groups, eg "Color" could be the name of the category and the tags
        /// themselves could be { "Red", "Blue", "Green" }
        /// </remarks>
        /// <seealso cref="SearchFilter"/>
        /// <seealso cref="TagCategory"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     ResultAnd&#60;TagCategory[]&#62; response = await ModIOUnityAsync.GetTagCategories();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         foreach(TagCategory category in response.value)
        ///         {
        ///             foreach(Tag tag in category.tags)
        ///             {
        ///                 Debug.Log(tag.name + " tag is in the " + category.name + "category");
        ///             }
        ///         }
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to get game tags");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<TagCategory[]>> GetTagCategories()
        {
            return await ModIOUnityImplementation.GetGameTags();
        }

        /// <summary>
        /// Uses a SearchFilter to retrieve a specific Mod Page and returns the ModProfiles and
        /// total number of mods based on the Search Filter.
        /// </summary>
        /// <remarks>
        /// A ModPage contains a group of mods based on the pagination filters in SearchFilter.
        /// eg, if you use SearchFilter.SetPageIndex(0) and SearchFilter.SetPageSize(100) then
        /// ModPage.mods will contain mods from 1 to 100. But if you set SearchFilter.SetPageIndex(1)
        /// then it will have mods from 101 to 200, if that many exist.
        /// (note that 100 is the maximum page size).
        /// </remarks>
        /// <param name="filter">The filter to apply when searching through mods (also contains
        /// pagination parameters)</param>
        /// <seealso cref="SearchFilter"/>
        /// <seealso cref="ModPage"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     SearchFilter filter = new SearchFilter();
        ///     filter.SetPageIndex(0);
        ///     filter.SetPageSize(10);
        ///     ResultAnd&#60;ModPage&#62; response = await ModIOUnityAsync.GetMods(filter);
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("ModPage has " + response.value.modProfiles.Length + " mods");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to get mods");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<ModPage>> GetMods(SearchFilter filter)
        {
            return await ModIOUnityImplementation.GetMods(filter);
        }

        /// <summary>
        /// Requests a single ModProfile from the mod.io server by its ModId.
        /// </summary>
        /// <remarks>
        /// If there is a specific mod that you want to retrieve from the mod.io database you can
        /// use this method to get it.
        /// </remarks>
        /// <param name="modId">the ModId of the ModProfile to get</param>
        /// <seealso cref="ModId"/>
        /// <seealso cref="ModProfile"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     ModId modId = new ModId(1234);
        ///     ResultAnd&#60;ModProfile&#62; response = await ModIOUnityAsync.GetMod(modId);
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("retrieved mod " + response.value.name);
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to get mod");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<ModProfile>> GetMod(ModId modId)
        {
            return await ModIOUnityImplementation.GetMod(modId.id);
        }

        /// <summary>
        ///
        /// </summary>
        public static async Task<ResultAnd<ModDependencies[]>> GetModDependencies(ModId modId)
        {
            return await ModIOUnityImplementation.GetModDependencies(modId);
        }

        /// <summary>
        /// Get all mod rating's submitted by the authenticated user. Successful request will return an array of Rating Objects.
        /// </summary>
        /// <param name="modId">the ModId of the ModProfile to get</param>
        /// <seealso cref="ModId"/>
        /// <seealso cref="Rating"/>
        /// <seealso cref="ResultAnd"/>
        /// <code>
        /// async void Example()
        /// {
        ///    ResultAnd&lt;Rating[]&gt; response = await ModIOUnityAsync.GetCurrentUserRatings();
        ///
        ///    if (response.result.Succeeded())
        ///    {
        ///        foreach(var ratingObject in response.value)
        ///        {
        ///            Debug.Log($"retrieved rating {ratingObject.rating} for {ratingObject.modId}");
        ///        }
        ///    }
        ///    else
        ///    {
        ///        Debug.Log("failed to get ratings");
        ///    }
        /// }
        /// </code>
        public static async Task<ResultAnd<Rating[]>> GetCurrentUserRatings()
        {
            return await ModIOUnityImplementation.GetCurrentUserRatings();
        }


        /// <summary>
        /// Gets the rating that the current user has given for a specified mod. You must have an
        /// authenticated session for this to be successful.
        /// </summary>
        /// <remarks>Note that the rating can be 'None'</remarks>
        /// <param name="modId">the id of the mod to check for a rating</param>
        /// <seealso cref="ModRating"/>
        /// <seealso cref="ModId"/>
        /// <seealso cref="ResultAnd"/>
        /// <code>
        /// async void Example()
        /// {
        ///    ModId modId = new ModId(1234);
        ///    ResultAnd&lt;ModRating&gt; response = await ModIOUnityAsync.GetCurrentUserRatingFor(modId);
        ///
        ///    if (response.result.Succeeded())
        ///    {
        ///        Debug.Log($"retrieved rating: {response.value}");
        ///    }
        ///    else
        ///    {
        ///        Debug.Log("failed to get rating");
        ///    }
        /// }
        /// </code>
        public static async Task<ResultAnd<ModRating>> GetCurrentUserRatingFor(ModId modId)
        {
            return await ModIOUnityImplementation.GetCurrentUserRatingFor(modId);
        }

#endregion // Mod Browsing

#region User Management
        /// <summary>
        /// Used to submit a rating for a specified mod.
        /// </summary>
        /// <remarks>
        /// This can be used to change/overwrite previous ratings of the current user.
        /// </remarks>
        /// <param name="modId">the m=ModId of the mod being rated</param>
        /// <param name="rating">the rating to give the mod. Allowed values include ModRating.Positive, ModRating.Negative, ModRating.None</param>
        /// <seealso cref="ModRating"/>
        /// <seealso cref="Result"/>
        /// <seealso cref="ModId"/>
        /// <code>
        ///
        /// ModProfile mod;
        ///
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.RateMod(mod.id, ModRating.Positive);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully rated mod");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to rate mod");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> RateMod(ModId modId, ModRating rating)
        {
            return await ModIOUnityImplementation.AddModRating(modId, rating);
        }

        /// <summary>
        /// Adds the specified mod to the current user's subscriptions.
        /// </summary>
        /// <remarks>
        /// If mod management has been enabled via ModIOUnity.EnableModManagement() then the mod
        /// will be downloaded and installed.
        /// </remarks>
        /// <param name="modId">ModId of the mod you want to subscribe to</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="ModId"/>
        /// <seealso cref="EnableModManagement(ModIO.ModManagementEventDelegate)"/>
        /// <seealso cref="GetCurrentModManagementOperation"/>
        /// <code>
        ///
        /// ModProfile mod;
        ///
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.SubscribeToMod(mod.id);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully subscribed to mod");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to subscribe to mod");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> SubscribeToMod(ModId modId)
        {
            return await ModIOUnityImplementation.SubscribeTo(modId);
        }

        /// <summary>
        /// Removes the specified mod from the current user's subscriptions.
        /// </summary>
        /// <remarks>
        /// If mod management has been enabled via ModIOUnity.EnableModManagement() then the mod
        /// will be uninstalled at the next opportunity.
        /// </remarks>
        /// <param name="modId">ModId of the mod you want to unsubscribe from</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="ModId"/>
        /// <seealso cref="EnableModManagement(ModIO.ModManagementEventDelegate)"/>
        /// <seealso cref="GetCurrentModManagementOperation"/>
        /// <code>
        ///
        /// ModProfile mod;
        ///
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.UnsubscribeFromMod(mod.id);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("Successfully unsubscribed from mod");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("Failed to unsubscribe from mod");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> UnsubscribeFromMod(ModId modId)
        {
            return await ModIOUnityImplementation.UnsubscribeFrom(modId);
        }

        /// <summary>
        /// Gets the current user's UserProfile struct. Containing their mod.io username, user id,
        /// language, timezone and download references for their avatar.
        /// </summary>
        /// <remarks>
        /// This requires the current session to have an authenticated user, otherwise
        /// Result.IsAuthenticationError() from the Result will equal true.
        /// </remarks>
        /// <seealso cref="Result"/>
        /// <seealso cref="UserProfile"/>
        /// <seealso cref="IsAuthenticated"/>
        /// <code>
        /// async void Example()
        /// {
        ///     ResultAnd&#60;UserProfile&#62; response = await ModIOUnityAsync.GetCurrentUser();
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("Got user: " + response.value.username);
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to get user");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<UserProfile>> GetCurrentUser()
        {
            return await ModIOUnityImplementation.GetCurrentUser();
        }

        /// <summary>
        /// Mutes a user which effectively hides any content from that specified user
        /// </summary>
        /// <remarks>The userId can be found from the UserProfile. Such as ModProfile.creator.userId</remarks>
        /// <param name="userId">The id of the user to be muted</param>
        /// <seealso cref="UserProfile"/>
        public static void MuteUser(long userId)
        {
            ModIOUnityImplementation.MuteUser(userId);
        }

        /// <summary>
        /// Un-mutes a user which effectively reveals previously hidden content from that user
        /// </summary>
        /// <remarks>The userId can be found from the UserProfile. Such as ModProfile.creator.userId</remarks>
        /// <param name="userId">The id of the user to be muted</param>
        /// <seealso cref="UserProfile"/>
        public static void UnmuteUser(long userId)
        {
            ModIOUnityImplementation.UnmuteUser(userId);
        }
#endregion

#region Mod Management

        /// <summary>
        /// This retrieves the user's subscriptions from the mod.io server and synchronises it with
        /// our local instance of the user's subscription data. If mod management has been enabled
        /// via ModIOUnity.EnableModManagement() then it may begin to install/uninstall mods.
        /// </summary>
        /// <remarks>
        /// This requires the current session to have an authenticated user, otherwise
        /// Result.IsAuthenticationError() from the Result will equal true.
        /// </remarks>
        /// <seealso cref="Result"/>
        /// <seealso cref="EnableModManagement(ModIO.ModManagementEventDelegate)"/>
        /// <seealso cref="IsAuthenticated"/>
        /// <seealso cref="RequestAuthenticationEmail"/>
        /// <seealso cref="SubmitEmailSecurityCode"/>
        /// <seealso cref="AuthenticateUserViaDiscord"/>
        /// <seealso cref="AuthenticateUserViaGoogle"/>
        /// <seealso cref="AuthenticateUserViaGOG"/>
        /// <seealso cref="AuthenticateUserViaItch"/>
        /// <seealso cref="AuthenticateUserViaOculus"/>
        /// <seealso cref="AuthenticateUserViaSteam"/>
        /// <seealso cref="AuthenticateUserViaSwitch"/>
        /// <seealso cref="AuthenticateUserViaXbox"/>
        /// <code>
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.FetchUpdates();
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("updated user subscriptions");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to get user subscriptions");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> FetchUpdates()
        {
            return await ModIOUnityImplementation.FetchUpdates();
        }

#endregion // Mod Management

#region Mod Uploading
        /// <summary>
        /// Creates a new mod profile on the mod.io server based on the details provided from the
        /// ModProfileDetails object provided. Note that you must have a logo, name and summary
        /// assigned in ModProfileDetails in order for this to work.
        /// </summary>
        /// <remarks>
        /// Note that this will create a new profile on the server and can be viewed online through
        /// a browser.
        /// </remarks>
        /// <param name="token">the token allowing a new unique profile to be created from ModIOUnity.GenerateCreationToken()</param>
        /// <param name="modProfileDetails">the mod profile details to apply to the mod profile being created</param>
        /// <seealso cref="GenerateCreationToken"/>
        /// <seealso cref="CreationToken"/>
        /// <seealso cref="ModProfileDetails"/>
        /// <seealso cref="Result"/>
        /// <seealso cref="ModId"/>
        /// <code>
        /// ModId newMod;
        /// Texture2D logo;
        /// CreationToken token;
        ///
        /// async void Example()
        /// {
        ///     token = ModIOUnity.GenerateCreationToken();
        ///
        ///     ModProfileDetails profile = new ModProfileDetails();
        ///     profile.name = "mod name";
        ///     profile.summary = "a brief summary about this mod being submitted"
        ///     profile.logo = logo;
        ///
        ///     ResultAnd&#60;ModId&#62; response = await ModIOUnityAsync.CreateModProfile(token, profile);
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         newMod = response.value;
        ///         Debug.Log("created new mod profile with id " + response.value.ToString());
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to create new mod profile");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<ModId>> CreateModProfile(CreationToken token,
                                                                    ModProfileDetails modProfileDetails)
        {
            return await ModIOUnityImplementation.CreateModProfile(token, modProfileDetails);
        }

        /// <summary>
        /// This is used to edit or change data in an existing mod profile on the mod.io server.
        /// </summary>
        /// <remarks>
        /// You need to assign the ModId of the mod you want to edit inside of the ModProfileDetails
        /// object included in the parameters
        /// </remarks>
        /// <param name="modProfile">the mod profile details to apply to the mod profile being created</param>
        /// <seealso cref="ModProfileDetails"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// ModId modId;
        ///
        /// async void Example()
        /// {
        ///     ModProfileDetails profile = new ModProfileDetails();
        ///     profile.modId = modId;
        ///     profile.summary = "a new brief summary about this mod being edited"
        ///
        ///     Result result = await ModIOUnityAsync.EditModProfile(profile);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("edited mod profile");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to edit mod profile");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> EditModProfile(ModProfileDetails modprofile)
        {
            return await ModIOUnityImplementation.EditModProfile(modprofile);
        }

        /// <summary>
        /// Used to upload a mod file to a mod profile on the mod.io server. A mod file is the
        /// actual archive of a mod. This method can be used to update a mod to a newer version
        /// (you can include changelog information in ModfileDetails).
        /// </summary>
        /// <param name="modfile">the mod file and details to upload</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="ModfileDetails"/>
        /// <seealso cref="ArchiveModProfile"/>
        /// <seealso cref="GetCurrentUploadHandle"/>
        /// <code>
        ///
        /// ModId modId;
        ///
        /// async void Example()
        /// {
        ///     ModfileDetails modfile = new ModfileDetails();
        ///     modfile.modId = modId;
        ///     modfile.directory = "files/mods/mod_123";
        ///
        ///     Result result = await ModIOUnityAsync.UploadModfile(modfile);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("uploaded mod file");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to upload mod file");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> UploadModfile(ModfileDetails modfile)
        {
            return await ModIOUnityImplementation.UploadModfile(modfile);
        }



        /// <summary>
        /// This is used to update the logo of a mod or the gallery images. This works very similar
        /// to EditModProfile except it only affects the images.
        /// </summary>
        /// <param name="modProfileDetails">this holds the reference to the images you wish to upload</param>
        /// <seealso cref="ModProfileDetails"/>
        /// <seealso cref="Result"/>
        /// <seealso cref="EditModProfile"/>
        /// <code>
        /// ModId modId;
        /// Texture2D newTexture;
        ///
        /// async void Example()
        /// {
        ///     ModProfileDetails profile = new ModProfileDetails();
        ///     profile.modId = modId;
        ///     profile.logo = newTexture;
        ///
        ///     Result result = await ModIOUnityAsync.UploadModMedia(profile);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("uploaded new mod logo");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to uploaded mod logo");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> UploadModMedia(ModProfileDetails modProfileDetails)
        {
            return await ModIOUnityImplementation.UploadModMedia(modProfileDetails);
        }

        /// <summary>
        /// Removes a mod from being visible on the mod.io server.
        /// </summary>
        /// <remarks>
        /// If you want to delete a mod permanently you can do so from a web browser.
        /// </remarks>
        /// <param name="modId">the id of the mod to delete</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="CreateModProfile"/>
        /// <seealso cref="EditModProfile"/>
        /// <code>
        ///
        /// ModId modId;
        ///
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnityAsync.ArchiveModProfile(modId);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("archived mod profile");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to archive mod profile");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> ArchiveModProfile(ModId modId)
        {
            return await ModIOUnityImplementation.ArchiveModProfile(modId);
        }

        /// <summary>
        /// Get all mods the authenticated user added or is a team member of.
        /// Successful request will return an array of Mod Objects. We
        /// recommended reading the filtering documentation to return only
        /// the records you want.
        /// </summary>
        public static async Task<ResultAnd<ModPage>> GetCurrentUserCreations(SearchFilter filter)
        {
            return await ModIOUnityImplementation.GetCurrentUserCreations(filter);
        }

        /// <summary>
        /// Adds the provided tags to the specified mod id. In order for this to work the
        /// authenticated user must have permission to edit the specified mod. Only existing tags
        /// as part of the game Id will be added.
        /// </summary>
        /// <param name="modId">Id of the mod to add tags to</param>
        /// <param name="tags">array of tags to be added</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="DeleteTags"/>
        /// <seealso cref="ModIOUnityAsync.AddTags"/>
        /// <code>
        ///
        /// ModId modId;
        /// string[] tags;
        ///
        /// void Example()
        /// {
        ///     Result result = await ModIOUnity.AddTags(modId, tags);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("added tags");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to add tags");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> AddTags(ModId modId, string[] tags)
        {            
            return await ModIOUnityImplementation.AddTags(modId, tags);
        }

        /// <summary>
        /// Deletes the specified tags from the mod. In order for this to work the
        /// authenticated user must have permission to edit the specified mod.
        /// </summary>
        /// <param name="modId">the id of the mod for deleting tags</param>
        /// <param name="tags">array of tags to be deleted</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="AddTags"/>
        /// <seealso cref="ModIOUnityAsync.DeleteTags"/>
        /// <code>
        ///
        /// ModId modId;
        /// string[] tags;
        ///
        /// async void Example()
        /// {
        ///     Result result = await ModIOUnity.DeleteTags(modId, tags);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("deleted tags");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to delete tags");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> DeleteTags(ModId modId, string[] tags)
        {
            return await ModIOUnityImplementation.DeleteTags(modId, tags);
        }
#endregion // Mod Uploading

#region Media Download

        /// <summary>
        /// Downloads a texture based on the specified download reference.
        /// </summary>
        /// <remarks>
        /// You can get download references from UserProfiles and ModProfiles
        /// </remarks>
        /// <param name="downloadReference">download reference for the texture (eg UserObject.avatar_100x100)</param>
        /// <seealso cref="Result"/>
        /// <seealso cref="DownloadReference"/>
        /// <seealso cref="Texture2D"/>
        /// <code>
        ///
        /// ModProfile mod;
        ///
        /// async void Example()
        /// {
        ///     ResultAnd&#60;Texture2D&#62; response = await ModIOUnityAsync.DownloadTexture(mod.logoImage_320x180);
        ///
        ///     if (response.result.Succeeded())
        ///     {
        ///         Debug.Log("downloaded the mod logo texture");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to download the mod logo texture");
        ///     }
        /// }
        /// </code>
        public static async Task<ResultAnd<Texture2D>> DownloadTexture(DownloadReference downloadReference)
        {
            return await ModIOUnityImplementation.DownloadTexture(downloadReference);
        }
        
        public static async Task<ResultAnd<byte[]>> DownloadImage(DownloadReference downloadReference)
        {
            return await ModIOUnityImplementation.GetImage(downloadReference);
        }

#endregion // Media Download

#region Reporting

        /// <summary>
        /// Reports a specified mod to mod.io.
        /// </summary>
        /// <param name="report">the object containing all of the details of the report you are sending</param>
        /// <seealso cref="Report"/>
        /// <seealso cref="Result"/>
        /// <code>
        /// async void Example()
        /// {
        ///     Report report = new Report(new ModId(123),
        ///                                 ReportType.Generic,
        ///                                 "reporting this mod for a generic reason",
        ///                                 "JohnDoe",
        ///                                 "johndoe@mod.io");
        ///
        ///     Result result = await ModIOUnityAsync.Report(report);
        ///
        ///     if (result.Succeeded())
        ///     {
        ///         Debug.Log("successfully sent a report");
        ///     }
        ///     else
        ///     {
        ///         Debug.Log("failed to send a report");
        ///     }
        /// }
        /// </code>
        public static async Task<Result> Report(Report report)
        {
            return await ModIOUnityImplementation.Report(report);
        }

#endregion // Reporting
    }
}

#pragma warning restore 4014 // Restore warnings about calling async functions from non-async code