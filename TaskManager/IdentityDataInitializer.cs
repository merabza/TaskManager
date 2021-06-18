using System.Linq;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager
{
  //კლასის დანიშნულებაა შექმნას მომხმარებლები და როლები პროგრამის ინიციალიზაციისას
  public static class IdentityDataInitializer
  {

    private static readonly UserInfo[] UserInfos =
    {
      new() {UserEmail = "user@example.com", Password = "user", RoleName = "User"},
      new() {UserEmail = "support@example.com", Password = "support", RoleName = "Support"},
      new() {UserEmail = "admin@example.com", Password = "admin", RoleName = "Admin"}
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
          userManager.AddToRoleAsync(user, userInfo.RoleName).Wait();
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
      }
    }
  }
}