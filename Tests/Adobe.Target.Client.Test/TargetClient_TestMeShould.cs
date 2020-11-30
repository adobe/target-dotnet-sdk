namespace Adobe.Target.Client.Test
{
    using Adobe.Target.Client;
    using Xunit;

    public class TargetClient_TestMeShould
    {
        [Fact]
        public void TestMe_Return1()
        {
            var targetClient = new TargetClient();
            int actual = targetClient.TestMe();

            Assert.Equal(1, actual);
        }
    }
}
