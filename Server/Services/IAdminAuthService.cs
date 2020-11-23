using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions; 
using System.Threading.Tasks;
using AutoMapper;
using Context;
using Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Services
{
    public interface IAdminAuthService {
        Task<UserManagerResponse> RegisterUserAsync(Users model);
        Task<UserManagerResponse> LoginUserAsync(LoginModel model);
        // Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        // Task<UserManagerResponse> ForgotPasswordAsync(string email);
        // Task<UserManagerResponse> ResetPasswordAsync(ResetPassword model);
    }

    public class AdminAuthService : IAdminAuthService
    {
        private UserManager<IdentityUser> _userManager;
        private Server _dbContext;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public AdminAuthService(
            IWebHostEnvironment hostEnvironment, 
            Server dbContext, 
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration, 
            IMailService mailService,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile) {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName+DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images\\AdminProfile", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create)) {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(Users model)
        {
            if (model == null)
                throw new NullReferenceException("Register Model is null");
            
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null ) {
                return new UserManagerResponse {
                    Message = "There is a user with this Email Address",
                    IsSuccess = false
                };
            }

            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
            Regex rg = new Regex(pattern);
            if (!rg.IsMatch(model.Password)) {
                return new UserManagerResponse {
                    Message = "Passwords must have at least one non alphanumeric character",
                    IsSuccess = false
                };
            }
            
            var identityUser = new IdentityUser{
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded) {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirm_email?userId={identityUser.Id}&token={validEmailToken}";

                var response = await _mailService.SendEmailAsync(
                        model.Email, "kodebottle@gmail.com", 
                        "Confirm your Email",
                        $"<h1>Welcome To Stadium</h1><p>Please confirm your email <a href='{url}'>Click here</a></p>"
                    );

                if (model.ImageFile != null) {
                    model.ImageName = await SaveImage(model.ImageFile);
                }

                model.Password = "555555";
                await _dbContext.Users.AddAsync(model);
                await _dbContext.SaveChangesAsync();

                return new UserManagerResponse{
                    Message = "User created successfully",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse{
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null ) {
                return new UserManagerResponse {
                    Message = "There is no user with this Email Address",
                    IsSuccess = false
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if(!result) 
                return new UserManagerResponse {
                    Message = "Invalid Password",
                    IsSuccess = false
                };

            var loginDetails = _dbContext.Users.FirstOrDefault(p => p.Email == model.Email);
            
            if(loginDetails.UserType != "Admin") {
                return new UserManagerResponse {
                    Message = "Not an admin",
                    IsSuccess = false
                };
            }
            
            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuthentication:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtAuthentication:Issuer"],
                audience: _configuration["JwtAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            ); 

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse{
                Message = tokenString,
                IsSuccess = true,
                ExpirationDate = token.ValidTo,
                Data = _mapper.Map<UsersDto>(loginDetails)
            };
        }

        // public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        // {
        //     var user = await _userManager.FindByIdAsync(userId);
        //     if(user == null) {
        //         return new UserManagerResponse{
        //             Message = "User does not exist",
        //             IsSuccess = false
        //         };
        //     }

        //     var decodedToken = WebEncoders.Base64UrlDecode(token);
        //     var normalToken = Encoding.UTF8.GetString(decodedToken);

        //     var result = await _userManager.ConfirmEmailAsync(user, normalToken);

        //     if(result.Succeeded) {
        //         return new UserManagerResponse{
        //             Message = "Email Confirmed Successfully",
        //             IsSuccess = true
        //         };
        //     }

        //     return new UserManagerResponse{
        //         Message = "Email Is not confirmed",
        //         IsSuccess = false,
        //         Errors = result.Errors.Select(e => e.Description)
        //     };
        // }

        // public async Task<UserManagerResponse> ForgotPasswordAsync(string email)
        // {
        //     var user = await _userManager.FindByEmailAsync(email);
        //     if (user == null) {
        //         return new UserManagerResponse{
        //             IsSuccess = false,
        //             Message  = "No User associated with this email"
        //         };
        //     }

        //     var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //     var encodedToken = Encoding.UTF8.GetBytes(token);
        //     var validToken = WebEncoders.Base64UrlEncode(encodedToken);

        //     string url = $"{_configuration["AppUrl"]}/reset_password?email={email}&token={validToken}";

        //     await _mailService.SendEmailAsync(
        //                 email, "kodebottle@gmail.com", 
        //                 "Reset Password",
        //                 $"<h1>Follow the instructions to reset password</h1><p>To reset your password <a href='{url}'>Click here</a></p>"
        //             );
        //     return new UserManagerResponse{
        //         IsSuccess = true,
        //         Message = "Reset password url has been sent to provided email"
        //     };
        // }

        // public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        // {
        //     var user = await _userManager.FindByEmailAsync(model.Email);

        //     if (user == null) {
        //         return new UserManagerResponse {
        //             Message = "There is no user with this Email Address",
        //             IsSuccess = false
        //         };
        //     }

        //     var result = await _userManager.CheckPasswordAsync(user, model.Password);

        //     if(!result) 
        //         return new UserManagerResponse {
        //             Message = "Invalid Password",
        //             IsSuccess = false
        //         };
            
        //     var claims = new[]
        //     {
        //         new Claim("Email", model.Email),
        //         new Claim(ClaimTypes.NameIdentifier, user.Id),

        //     };

        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuthentication:Key"]));

        //     var token = new JwtSecurityToken(
        //         issuer: _configuration["JwtAuthentication:Issuer"],
        //         audience: _configuration["JwtAuthentication:Audience"],
        //         claims: claims,
        //         expires: DateTime.Now.AddDays(30),
        //         signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        //     ); 

        //     string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //     return new UserManagerResponse{
        //         Message = tokenString,
        //         IsSuccess = true,
        //         ExpirationDate = token.ValidTo
        //     };
        // }
        // public async Task<UserManagerResponse> ResetPasswordAsync(ResetPassword model)
        // {
        //      var user = await _userManager.FindByEmailAsync(model.EmailAddress);
        //     if (user == null) {
        //         return new UserManagerResponse{
        //             IsSuccess = false,
        //             Message  = "No User associated with this email"
        //         };
        //     }

        //     if (model.NewPassword != model.ConfirmPassword) return new UserManagerResponse{
        //             IsSuccess = false,
        //             Message  = "Password does not match Confirmation"
        //         };

        //     var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
        //     var normalToken = Encoding.UTF8.GetString(decodedToken);

        //     var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

        //     if(result.Succeeded) {
        //         return new UserManagerResponse{
        //             IsSuccess = true,
        //             Message = "PAssword has been reset"
        //         };
        //     }

        //     return new UserManagerResponse{
        //             IsSuccess = true,
        //             Message = "Soemthing went wrong",
        //             Errors = result.Errors.Select(e => e.Description)
        //         };
        // }
    
    }
}