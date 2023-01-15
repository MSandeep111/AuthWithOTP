using PhoneAuthDemo.Models;

namespace PhoneAuthDemo.Services
{
    /// <summary>
    /// Service for user related operations
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// To register the user
        /// </summary>
        /// <param name="model">Model for registration</param>
        /// <returns>Response with status, message and data if any</returns>
        Task<ClsResponse> UserRegisterAsync(RegisterationModel model);

        /// <summary>
        /// To verify the user
        /// </summary>
        /// <param name="model">Model for verification</param>
        /// <returns>Response with status, message and data if any</returns>
        Task<ClsResponse> UserVerificationAsync(VerificationModel model);

        /// <summary>
        /// To generate the OTP
        /// </summary>
        /// <param name="mobileNumber">Mobile number for otp generation</param>
        /// <returns>Response with status, message and data if any</returns>
        Task<ClsResponse> GenerateOTPAsync(string mobileNumber);

        /// <summary>
        /// To login the user
        /// </summary>
        /// <param name="model">Model for login</param>
        /// <returns>Response with status, message and data if any</returns>
        Task<ClsResponse> UserLoginAsync(LoginModel model);
    }
}
