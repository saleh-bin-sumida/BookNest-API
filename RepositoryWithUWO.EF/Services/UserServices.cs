using Microsoft.AspNetCore.Identity;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.EF.Services;

public class UserServices(UserManager<IdentityUser> userManager) : IUserServices
{
    public UserManager<IdentityUser> _userManager  = userManager;




    public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
    {
        if (model is null)
            throw new NullReferenceException("the register model is null");


        if (model.Password != model.ConfirmPassword)
            return  new UserManagerResponse("confirm password doesnt match password", false);

        var identityUser = new IdentityUser { Email = model.Email, UserName = model.Usrname };

        var result = await( _userManager.CreateAsync(identityUser, model.Password));

        if (result.Succeeded)
            return new UserManagerResponse("User is created succefully", true);

        return new UserManagerResponse("User did not created", false,result.Errors.Select( e => e.Description));

    }

    public async Task<UserManagerResponse> Login(LoginModel model)
    {
        if (model is null)
            throw new NullReferenceException("the login model is null");

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user is null)
            return new UserManagerResponse("no user with that username",false);

        var IsUser = await _userManager.CheckPasswordAsync(user, model.Password);

        if (IsUser)
            return new UserManagerResponse("password is correct",true);

        return new UserManagerResponse("invalid password", false);

    }



}
