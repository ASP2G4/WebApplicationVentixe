using Authentication.Dtos;
using Authentication.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Authentication.Services
{
    public interface IVerificationService
    {
        string GenerateVerificationCode();
        Task<VerificationServiceResult> SendVerificationCodeAsync(string email);

        Task<VerificationServiceResult> SaveVerificationCodeAsync(string email, string code, int validForInMinutes = 5);

        Task<VerificationServiceResult> ValidateVerificationCodeAsync(string email, string code);
    }
    public class VerificationService(IEmailService emailService, IMemoryCache cache) : IVerificationService
    {
        private readonly IEmailService _emailService = emailService;
        private readonly IMemoryCache _cache = cache;

        public string GenerateVerificationCode()
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            return code;
        }

        public async Task<VerificationServiceResult> SendVerificationCodeAsync(string email)
        {
            var verificationCode = GenerateVerificationCode();
            //PROTOBUF eller något annat som modell för att skicka verifieringskoden till frontend

            var emailMessage = new EmailMessageModel
            {
                Recipients = [email],
                Subject = $"Verification Code: {verificationCode}",
                PlainText = $@"
                Verify Your Email Address

                Hello,

                To Complete your verification, please enter the following code:

                {verificationCode}

                Alternativly, you can open the verification page using the following link:
                https://localhost:7168/account-verification?email={email}&token=

                Privacy Policy:
                https://localhost:7168/privacy
                ",

                HTML = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Your verification code</title>
                    </head>
                    <body style='margin:0; padding: 32px; font-family: Inter, sans-serif; background-color: #F7F7F7; color: #1E1E20;'>
                        <div style='max-width: 600px; margin: 32px auto; background: #FFF; border-radius: 16px; padding:32px;'>
                            <h1 style='font-size: 32px; font-weight: 600; margin-bottom: 16px; color:#37437D; text-align: center;'>Verify your Email Address</h1>

                            <p style='font-size: 16px; line-height: 1.5; margin-bottom: 16px; color:#1E1E20'>Hello,</p>

                            <p style='font-size: 16px; line-height: 1.5; margin-bottom: 16px;'>To complete your verification, please enter the code below or click the button to open a new page</p>

                            <div style='display:flex; justify-content: center; align-items: center; padding: 16px; background-color: #FcD3FE; font-size: 24px; font-weight: 600;'>{verificationCode}</div>

                            <div style='text-align: center; margin-top: 32px;'>
                                <a href='https://localhost:7168/account-verification?email={email}&token=' style='background-color: #F26cF9; color: #FFF; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-size: 16px;'>Open Verification Page</a>
                        </div>

                            <p style='font-size: 16px; line-height: 1.5; margin-top: 32px; color:#1E1E20'>If you did not request this email, please ignore it.</p>
                            <p style='font-size: 16px; line-height: 1.5; margin-top: 32px; color:#1E1E20'>Privacy Policy:</p>
                            <a href='https://localhost:7168/privacy' style='color:#F26cF9;'>https://localhost:7168/privacy</a>
    
                    </body>
                    </html>"
            };

            //kontaktar en seperat ms tjänst eller kör via azure service bus FIXA FIXA FIXA
            var response = await _emailService.SendEmailAsync(emailMessage);
            if (response.Succeeded)
            {
                await SaveVerificationCodeAsync(email, verificationCode);
            }

            return response.Succeeded
                ? new VerificationServiceResult { Succeeded = true }
                : new VerificationServiceResult { Succeeded = false, Error = "Unable too send verification code." };
        }

        public Task<VerificationServiceResult> SaveVerificationCodeAsync(string email, string code, int validForInMinutes = 5)
        {
            try
            {
                _cache.Set(email.ToLowerInvariant(), code, TimeSpan.FromMinutes(validForInMinutes));
                return Task.FromResult(new VerificationServiceResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new VerificationServiceResult { Succeeded = false, Error = ex.Message });
            }
        }

        public Task<VerificationServiceResult> ValidateVerificationCodeAsync(string email, string code)
        {
            var key = email.ToLowerInvariant();

            if (_cache.TryGetValue(key, out string? cachedCode))
            {
                if (cachedCode == code)
                {
                    _cache.Remove(key);
                    return Task.FromResult(new VerificationServiceResult { Succeeded = true });
                }
            }
                return Task.FromResult(new VerificationServiceResult { Succeeded = false, Error = "Invalid or expired verification code." });

        }
    }
}
