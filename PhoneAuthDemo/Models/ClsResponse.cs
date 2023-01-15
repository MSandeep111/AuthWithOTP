namespace PhoneAuthDemo.Models
{


    /// <summary>
    /// Response class
    /// </summary>
    public class ClsResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string? Data { get; set; }

        /// <summary>
        /// Default contructor
        /// </summary>
        public ClsResponse()
        {

        }

        /// <summary>
        /// Parameterized method for response
        /// </summary>
        /// <param name="status">Status of the response</param>
        /// <param name="message">Message to end-user</param>
        /// <param name="data">Data if any</param>
        public ClsResponse(bool status, string message, string? data = null)
        {
            Status = true;
            Message = message;
            Data = data;
        }
    }


}
