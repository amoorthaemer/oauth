using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherForecast.Api.Services;

internal sealed class ClaimsTransformer: IClaimsTransformation {

	public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) {
//		var token = GenerateToken(principal);
		return Task.FromResult(principal);
	}

	//public string GenerateToken(ClaimsPrincipal principal) {
	//	var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
	//	var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

	//	var myIssuer = "http://mysite.com";
	//	var myAudience = "http://myaudience.com";

	//	var tokenHandler = new JwtSecurityTokenHandler();
	//	var tokenDescriptor = new SecurityTokenDescriptor {
	//		Subject = new ClaimsIdentity(principal.Claims),
	//		Expires = DateTime.UtcNow.AddDays(7),
	//		Issuer = myIssuer,
	//		Audience = myAudience,
	//		SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
	//	};

	//	var token = tokenHandler.CreateToken(tokenDescriptor);
	//	return tokenHandler.WriteToken(token);
	//}
}
