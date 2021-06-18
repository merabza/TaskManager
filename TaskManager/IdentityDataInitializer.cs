using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager
{
  //კლასის დანიშნულებაა შექმნას მომხმარებლები და როლები პროგრამის ინიციალიზაციისას
  public static class IdentityDataInitializer
  {

    private static readonly UserInfo[] UserInfos =
    {
      new() {UserEmail = "user@example.com", Password = "user", RoleName = ERole.User.ToString()},
      new() {UserEmail = "support@example.com", Password = "support", RoleName = ERole.Support.ToString()},
      new() {UserEmail = "admin@example.com", Password = "admin", RoleName = ERole.Admin.ToString()}
    };


    //როლებისა და მომხმარებლების შექმნა//, ITaskManagerRepository taskManagerRepository
    public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      SeedRoles(roleManager);
      SeedUsers(userManager);
    }

    //მომხმარებლების შექმნა
    private static void SeedUsers(UserManager<IdentityUser> userManager)
    {

      foreach (UserInfo userInfo in UserInfos)
      {
        if (userManager.FindByNameAsync(userInfo.UserEmail).Result != null) 
          continue;
        //თუ არ არსებობს, შევქმნათ
        IdentityUser user = new IdentityUser {UserName = userInfo.UserEmail, Email = userInfo.UserEmail};
        IdentityResult result = userManager.CreateAsync(user, userInfo.Password).Result;
        if (result.Succeeded) //თუ ყველაფერი რიგზეა, მივცეთ როლი
        {
          userManager.AddToRoleAsync(user, userInfo.RoleName).Wait();
        }

      }

    }

    //როლების შექმნა
    private static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
      foreach (string roleName in UserInfos.Select(s=>s.RoleName).Distinct())
      {
        if (roleManager.RoleExistsAsync(roleName).Result)
          continue;
        //თუ არ არსებობს, შევქმნათ
        IdentityRole role = new IdentityRole {Name = roleName};
        roleManager.CreateAsync(role).Wait();
        roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Role, roleName));
        ERole rl = Enum.Parse<ERole>(roleName);
        switch (rl)
        {
          case ERole.Admin:
            break;
          case ERole.User:
            break;
          case ERole.Support:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
}