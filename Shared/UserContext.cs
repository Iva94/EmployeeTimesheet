using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace EmployeeTimesheet.Shared
{
	public class UserContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserContext(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public int? GetUserId()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null)
				return null;
			else
				return Int32.Parse(userId);
		}

		public string GetUserName()
		{
			return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
		}

		public string GetUserRole()
		{
			return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
		}
	}
}