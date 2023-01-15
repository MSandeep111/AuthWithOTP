using Azure;
using Dapper;
using Microsoft.Extensions.Configuration;
using PhoneAuthDemo.Helpers;
using PhoneAuthDemo.Models;
using PhoneAuthDemo.Services;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PhoneAuthDemo.Repositories
{
    /// <summary>
    /// Repository to handle the user related operations
    /// </summary>
    public class UserRepository : IUserService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// User repository
        /// </summary>
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("PhoneAuthConn"));
            }
        }

        /// <summary>
        /// Register the user
        /// </summary>
        /// <param name="model">Model for user registration</param>
        /// <returns></returns>
        public async Task<ClsResponse> UserRegisterAsync(RegisterationModel model)
        {

            ClsResponse response = new ClsResponse();
            string mobileOTP = PasswordGenerator.GeneratePassword(false, false, true, false, 6);

            using (IDbConnection con = Connection)
            {
                response = (await con.QueryFirstOrDefaultAsync<ClsResponse>("[dbo].[insert_employee]"
                   , new
                   {
                       EmployeeName = model.Name,
                       ContactNumber = model.ContactNumber,
                       OTP = mobileOTP
                   }
                   , commandType: CommandType.StoredProcedure)
                   .ConfigureAwait(false));

                if (response.Status)
                {
                    // If user record inserted successfully then sent an OTP
                    var otpResponse = SendMobileMessage(mobileOTP, model.ContactNumber);

                    // If otp sending failed
                    if (otpResponse == null || !otpResponse.IsSuccessful)
                    {
                        response.Status = false;
                        response.Message = "Error while sending the OTP";
                        return response;
                    }
                }

                return response;
            }
        }

        /// <summary>
        /// Register the user
        /// </summary>
        /// <param name="model">Model for user registration</param>
        /// <returns></returns>
        public async Task<ClsResponse> UserVerificationAsync(VerificationModel model)
        {
            using (IDbConnection con = Connection)
            {
                var response = (await con.QueryFirstOrDefaultAsync<ClsResponse>("[dbo].[verify_employee]"
                    , new
                    {
                        ContactNumber = model.ContactNumber,
                        OTP = model.OTP
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false));

                return response;
            }
        }

        /// <summary>
        /// Register the user
        /// </summary>
        /// <param name="mobileNumber">Model for user login otp</param>
        /// <returns></returns>
        public async Task<ClsResponse> GenerateOTPAsync(string mobileNumber)
        {
            ClsResponse response = new ClsResponse();
            string mobileOTP = PasswordGenerator.GeneratePassword(false, false, true, false, 6);

            // If user record inserted successfully then sent an OTP
            var otpResponse = SendMobileMessage(mobileOTP, mobileNumber);

            // If otp sending failed
            if (otpResponse == null || !otpResponse.IsSuccessful)
            {
                response.Status = false;
                response.Message = "Error while sending the OTP";
                return response;
            }

            using (IDbConnection con = Connection)
            {
                response = (await con.QueryFirstOrDefaultAsync<ClsResponse>("[dbo].[save_otp]"
                   , new
                   {
                       ContactNumber = mobileNumber,
                       OTP = mobileOTP
                   }
                   , commandType: CommandType.StoredProcedure)
                   .ConfigureAwait(false));

                return response;
            }
        }


        /// <summary>
        /// Register the user
        /// </summary>
        /// <param name="model">Model for user registration</param>
        /// <returns></returns>
        public async Task<ClsResponse> UserLoginAsync(LoginModel model)
        {
            using (IDbConnection con = Connection)
            {
                var response = (await con.QueryFirstOrDefaultAsync<ClsResponse>("[dbo].[login_employee]"
                    , new
                    {
                        ContactNumber = model.ContactNumber,
                        OTP = model.OTP
                    }
                    , commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false));

                return response;
            }
        }



        /// <summary>
        /// Function to send the OTP to provided mobile number
        /// </summary>
        /// <param name="phoneNumber">Contact number of user with contry code eg. 98********</param>
        /// <returns>Response as null</returns>
        private RestResponse? SendMobileMessage(string mobileOTP, string phoneNumber)
        {

            string? smsApiKey = _configuration.GetSection("SMSAPIKey").Value;
            if (smsApiKey == null)
            {
                return null;
            }

            // Send OTP to mobile number
            var client = new RestClient();
            var mobileOtpRequest = new RestRequest("https://www.fast2sms.com/dev/bulkV2", Method.Post);
            mobileOtpRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            mobileOtpRequest.AddHeader("authorization", smsApiKey);

            mobileOtpRequest.AddParameter("route", "v3");
            mobileOtpRequest.AddParameter("sender_id", "FTWSMS");
            mobileOtpRequest.AddParameter("message", mobileOTP);
            mobileOtpRequest.AddParameter("language", "english");
            mobileOtpRequest.AddParameter("flash", "0");
            mobileOtpRequest.AddParameter("numbers", phoneNumber);

            RestResponse mobileOtpResponse = client.Execute(mobileOtpRequest);
            return mobileOtpResponse;
        }


    }
}
