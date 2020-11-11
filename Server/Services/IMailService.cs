using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Dtos;
using RestSharp;
using RestSharp.Authenticators;

namespace Services {
    public interface IMailService {
        Task <UserManagerResponse> SendEmailAsync(string to, string from, string subject, string body);
    }

    public class SendEmail : IMailService
    {
        public async Task<UserManagerResponse> SendEmailAsync(string to, string from, string subject, string body)
        {
            RestClient client = new RestClient ();
            DotNetEnv.Env.Load("./.env");
            client.BaseUrl = new Uri (DotNetEnv.Env.GetString("MAIL_GUN_URL"));
            client.Authenticator =
                new HttpBasicAuthenticator ("api",
                                            DotNetEnv.Env.GetString("MAIL_GUN_API_KEY"));
            RestRequest request = new RestRequest ();
            request.AddParameter ("domain", DotNetEnv.Env.GetString("MAIL_GUN_SANDBOX"), ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", from);
            request.AddParameter ("to", to);
            request.AddParameter ("to", "YOU@YOUR_DOMAIN_NAME");
            request.AddParameter ("subject", subject);
            request.AddParameter ("html", body);
            request.Method = Method.POST;
            var response = await client.ExecuteAsync (request);

            Console.Write(response.Content.ToString());

            if (response == null) {
                return new UserManagerResponse(){
                    Message = "Message did not send",
                    IsSuccess = false
                };
            }

            return new UserManagerResponse(){
                Message = "Message went through",
                IsSuccess = true
            };
        }
    }
}