namespace CommerceBuilder.DigitalDelivery
{
    /// <summary>
    /// Class that represents a Readme object in the database
    /// </summary>
    public partial class Readme
    {
        /// <summary>
        /// Deletes a readme, reassociating any digital goods with the specified readme.
        /// </summary>
        /// <param name="newReadmeId">The readme that associated digital goods should be switched to</param>
        /// <returns>True if the readme is deleted, false otherwise.</returns>
        public virtual bool Delete(int newReadmeId)
        {
            ReadmeDataSource.MoveDigitalGoods(this.ReadmeId, newReadmeId);
            return this.Delete();
        }

    }
}
