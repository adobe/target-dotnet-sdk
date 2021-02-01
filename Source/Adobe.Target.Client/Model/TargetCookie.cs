namespace Adobe.Target.Client.Model
{
    /// <summary>
    /// TargetCookie
    /// </summary>
    public class TargetCookie
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetCookie"/> class.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="maxAge">Max-Age</param>
        public TargetCookie(string name, string value, int maxAge)
        {
            this.Name = name;
            this.Value = value;
            this.MaxAge = maxAge;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Max-Age
        /// </summary>
        public int MaxAge { get; private set; }
    }
}
