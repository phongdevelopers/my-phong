namespace CommerceBuilder.Marketing
{
    public partial class EmailListSignup
    {
        /// <summary>
        /// converts a signup into a valid subscription
        /// </summary>
        public void Activate()
        {
            //ADD USER TO LIST
            EmailList list = this.EmailList;
            EmailListUser elu = new EmailListUser(this.EmailListId, this.Email);
            list.Users.Add(elu);
            list.Users.Save();
            //DELETE THIS SUBSCRIPTION REQUEST
            this.Delete();
        }

        /// <summary>
        /// The signup key user for signup
        /// </summary>
        public string SignupKey
        {
            get
            {
                return this.SignupDate.ToString("MMddyyhhmmss");
            }
        }
    }
}
