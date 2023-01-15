namespace PhoneAuthDemo.Helpers
{
    /// <summary>
    /// To generate the password
    /// </summary>
    public static class PasswordGenerator
    {
        /// <summary>
        /// To generate the password
        /// </summary>
        /// <param name="useLowercase">Boolean value to describe whether lowecase needed in password</param>
        /// <param name="useUppercase">Boolean value to describe whether uppercase needed in password</param>
        /// <param name="useNumbers">Boolean value to describe whether Numbers needed in password</param>
        /// <param name="useSpecial">Boolean value to describe whether Special needed in password</param>
        /// <param name="passwordSize">provide the size needed for password</param>
        /// <returns></returns>
        public static string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial,
        int passwordSize)
        {
            const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
            const string UPPER_CAES = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMBERS = "123456789";
            const string SPECIALS = @"!@£$%^&*()#€";

            char[] _password = new char[passwordSize];
            string charSet = ""; // Initialize to blank
            System.Random _random = new Random();
            int counter;

            // Build up the character set to choose from
            if (useLowercase)
            {
                charSet += LOWER_CASE;
            }

            if (useUppercase)
            {
                charSet += UPPER_CAES;
            }

            if (useNumbers)
            {
                charSet += NUMBERS;
            }

            if (useSpecial)
            {
                charSet += SPECIALS;
            }

            for (counter = 0; counter < passwordSize; counter++)
            {
                _password[counter] = charSet[_random.Next(charSet.Length - 1)];
            }

            return String.Join(null, _password);
        }
    }
}
