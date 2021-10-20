using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Authentication
{
    public partial class Login
    {
        public string Password { get; set; }
        public bool PasswordVisibility { get; set; }
        public InputType PasswordInput { get; set; } = InputType.Password;
        public string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;

        public void TogglePasswordVisibility()
        {
            if (PasswordVisibility)
            {
                PasswordVisibility = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                PasswordVisibility = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        public void SignIn()
        {

        }
    }
}
