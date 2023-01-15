namespace PhoneAuthDemo.Models
{
    /// <summary>
    /// Firebase error
    /// </summary>
    public class FirebaseError
    {
        public Error error { get; set; }
    }


    /// <summary>
    /// Error details related with firebase
    /// </summary>
    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Error> errors { get; set; }
    }
}
