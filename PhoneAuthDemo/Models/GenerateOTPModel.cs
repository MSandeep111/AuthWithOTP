using System.ComponentModel.DataAnnotations;

namespace PhoneAuthDemo.Models
{
    /// <summary>
    /// Class for otp-generation
    /// </summary>
    public class GenerateOTPModel
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression("(0|91)?[6-9][0-9]{9}", ErrorMessage = "Please validate the mobile number. eg: 9765******")]
        public string ContactNumber { get; set; }
    }
}
