﻿using System.Security.Claims;

namespace SV21T1020096.Shop
{
    /// <summary>
    /// Tạo thêm phương thức
    /// </summary>
    public static class WebUserExtensions
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                {
                    return null;
                }
                var userData = new WebUserData();
                userData.UserID = principal.FindFirstValue(nameof(userData.UserID)) ?? "";
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName)) ?? "";
                userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName)) ?? "";
                userData.Photo = principal.FindFirstValue(nameof(userData.Photo)) ?? "";
                userData.Roles = new List<string>();
                foreach (var item in principal.FindAll(ClaimTypes.Role))
                {
                    userData.Roles.Add(item.Value);
                }
                return userData;
            }
            catch { return null; }
        }
    }
}
