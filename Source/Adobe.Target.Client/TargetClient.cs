namespace Adobe.Target.Client
{
    using System;

    /// <summary>
    /// The main TargetClient class.
    /// Contains methods for creating and using TargetClient SDK.
    /// </summary>
    public class TargetClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClient"/> class.
        /// </summary>
        public TargetClient()
        {
            Console.WriteLine("Target Client");
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <returns>
        /// A test value.
        /// </returns>
        public int TestMe()
        {
            return 1;
        }
    }
}
