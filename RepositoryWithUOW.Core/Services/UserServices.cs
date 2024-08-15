using Microsoft.AspNetCore.Identity;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;


namespace RepositoryWithUOW.Core.Services;

public class UserServices(UserManager<ApplicationUser> userManager) : IUserServices
{
    public UserManager<ApplicationUser> _userManager = userManager;




    public async Task<UserManagerResponse> RegisterUserAsync(RegisterModel model)
    {
        if (model is null)
            throw new NullReferenceException("the register model is null");


        if (model.Password != model.ConfirmPassword)
            return new UserManagerResponse("confirm password doesnt match password", false);

        var identityUser = new ApplicationUser { Email = model.Email, UserName = model.Usrname,FullName = model.FullName };

        var result = await (_userManager.CreateAsync(identityUser, model.Password));

        if (result.Succeeded)
            return new UserManagerResponse("User is created succefully", true);

        return new UserManagerResponse("User did not created", false, result.Errors.Select(e => e.Description));

    }

    public async Task<UserManagerResponse> Login(LoginModel model)
    {
        if (model is null)
            throw new NullReferenceException("the login model is null");

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user is null)
            return new UserManagerResponse("no user with that username", false);

        var IsUser = await _userManager.CheckPasswordAsync(user, model.Password);

        if (IsUser)
            return new UserManagerResponse("password is correct", true);

        return new UserManagerResponse("invalid password", false);

    }



}
