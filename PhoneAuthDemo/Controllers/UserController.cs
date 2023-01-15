
using FirebaseAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhoneAuthDemo.Models;
using PhoneAuthDemo.Services;
using RestSharp;

namespace PhoneAuthDemo.Controllers
{
    /// <summary>
    /// User Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// User controller
        /// </summary>
        /// <param name="userService">User service</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Dummy function to test whether application running or not
        /// </summary>
        /// <returns></returns>
        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok(new { Message = "Running..." });
        }

        /// <summary>
        /// To register the user
        /// </summary>
        /// <param name="model">Model for registration</param>
        /// <returns>Response as success (200), Internal server error (500) or Bad request (404)</returns>
        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] RegisterationModel model)
        {

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(errorList);
            }
            try
            {
                var response =await _userService.UserRegisterAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                ClsResponse response = new ClsResponse
                {
                    Status = false,
                    Message = "Error occurred."
                };
                return StatusCode(500, response);
            }

        }

        /// <summary>
        /// To verify the user
        /// </summary>
        /// <param name="model">Model for verification</param>
        /// <returns>Response as success (200), Internal server error (500) or Bad request (404)</returns>
        [HttpPost("verify")]
        public async Task<IActionResult> UserVerification([FromBody] VerificationModel model)
        {

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                   kvp => kvp.Key,
                   kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(errorList);
            }
            try
            {
                var response =await _userService.UserVerificationAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                ClsResponse response = new ClsResponse
                {
                    Status = false,
                    Message = "Error occurred."
                };
                return StatusCode(500, response);
            }

        }

        /// <summary>
        /// To genearte the OTP for user login
        /// </summary>
        /// <param name="model">Model for verification</param>
        /// <returns>Response as success (200), Internal server error (500) or Bad request (404)</returns>
        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromBody] GenerateOTPModel model)
        {

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                   kvp => kvp.Key,
                   kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(errorList);
            }
            try
            {
                var response = await _userService.GenerateOTPAsync(model.ContactNumber);
                return Ok(response);
            }
            catch (Exception)
            {
                ClsResponse response = new ClsResponse
                {
                    Status = false,
                    Message = "Error occurred."
                };
                return StatusCode(500, response);
            }

        }

        /// <summary>
        /// To verify the user
        /// </summary>
        /// <param name="model">Model for login</param>
        /// <returns>Response as success (200), Internal server error (500) or Bad request (404)</returns>
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(errorList);
            }
            try
            {
                var response =await _userService.UserLoginAsync(model);
                return Ok(response);
            }
            catch (Exception)
            {
                ClsResponse response = new ClsResponse
                {
                    Status = false,
                    Message = "Error occurred."
                };
                return StatusCode(500, response);
            }

        }

    }
}
