using Microsoft.Maui.ApplicationModel.Communication;
using SaRLAB.MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.MobileApp.Dto
{
    public class UserDto : IUserDto
    {
        async Task<User> IUserDto.Login(string email, string password)
        {
            var client = new HttpClient();
            string url = "http://localhost:5200/api/User/login/" + email + "/" + password;
            client.BaseAddress = new Uri(url);
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                User user = await response.Content.ReadFromJsonAsync<User>();
                return await Task.FromResult(user!);
            }
            return null!;
        }
    }
}
