using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Milo.Helper
{
    public static class EncryptedStorageHelper
    {

        private const string TokenKey = "Token";
        private const string Email = "Email";
        private const string Password = "Password";

        public static async Task SaveTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(TokenKey, token);
            }
            catch (Exception)
            {
            }
        }
        public static async Task SaveEmailAsync(string email)
        {
            try
            {
                await SecureStorage.SetAsync(Email, email);
            }
            catch (Exception)
            {
            }
        }
        public static async Task SavePasswordAsync(string password)
        {
            try
            {
                await SecureStorage.SetAsync(Password, password);
            }
            catch (Exception)
            {
            }
        }

        public static async Task<string> GetEmailAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(Email);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<string> GetPasswordAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(Password);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(TokenKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void DeleteToken()
        {
            try
            {
                SecureStorage.Remove(TokenKey); ;
            }
            catch (Exception)
            {
            }
        }

        public static void ClearCache()
        {
            try
            {
                SecureStorage.RemoveAll();
                Application.Current.Properties.Clear();
            }
            catch (Exception)
            {
            }
        }
    }
}