namespace XUnitTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("","",false)]
        [InlineData("admin","admin@123",true)]
        [InlineData("admin","",false)] //correct username only
        [InlineData("","admin@123",false)] //correct password only
        public void TestValid_AdminCredentials(string username, string password, bool expectedResult)
        {
            //Assign

            PRG281_Milestone_2.DispatchOperator dOperator = new PRG281_Milestone_2.DispatchOperator();

            //Act
            bool result = dOperator.Login(username,password);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TestValid_AmbulanceArrived()
        {
            //Assign

            //Act

            //Assert
        }

        [Fact]
        public void TestValid_RaiseResponseTimeExceeded()
        {
            //Assign

            //Act

            //Assert
        }

    }
}