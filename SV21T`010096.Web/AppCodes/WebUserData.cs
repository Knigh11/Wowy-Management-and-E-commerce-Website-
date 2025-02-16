using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020096.Web
{
    /// <summary>
    /// Lưu giữ thông tin của người dùng được lưu trong Cookie
    /// </summary>
    public class WebUserData
    {
        internal bool UserId = false;
        public string UserID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }
        /// <summary>
        /// Danh sách các clainm
        /// </summary>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserID), UserID),
                    new Claim(nameof(UserName), UserName),
                    new Claim(nameof(DisplayName), DisplayName),
                };
                if (Roles != null)
                    foreach (var role in Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                return claims;
            }
        }
        /// <summary>
        /// tạo ClaimsPrincipal dự trên thông tin của người dùng (cần lưu trong Cookie)
        /// </summary>
        /// <returns></returns> 
        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
