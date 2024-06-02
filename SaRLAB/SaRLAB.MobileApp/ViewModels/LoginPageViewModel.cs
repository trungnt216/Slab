using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SaRLAB.MobileApp.Dto;
using SaRLAB.MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.MobileApp.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;

    readonly IUserDto _userDto = new UserDto();


    [RelayCommand]
    public async Task SignIn()
    {
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
                {

                    Console.WriteLine(Email);
                    Console.WriteLine(Password);
                    User user = await _userDto.Login(Email, Password);
                    if (user == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "Username/Password is incorrect", "Ok");
                        return;
                    }
                    if (Preferences.ContainsKey(nameof(App.user)))
                    {
                        Preferences.Remove(nameof(App.user));
                    }
                    string userDetails = JsonConvert.SerializeObject(user);
                    Preferences.Set(nameof(App.user), userDetails);
                    App.user = user;
                    await Shell.Current.DisplayAlert("OKE", user.Role.ToString(), "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
                    return;
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No Internet Access", "Ok");
                return;
            }

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Warning", ex.Message, "Ok");
            return;
        }
    }
}