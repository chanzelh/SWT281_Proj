namespace XUnitTests
{
    public class UnitTest1
    {
        //LOGIN TEST
        //Test 1.1: This test checks the Login() method with multiple username/password scenarios
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

        //MARK AMBULANCE ARRIVED TESTS
        //Test 2.1: Test when no Emergency Call is provided/null
        [Fact]
        public void MarkAmbulanceArrived_CallIsNull()
        {
            //Assign
            DispatchSystem objCallNothing = new DispatchSystem();
            //Act
            objCallNothing.MarkAmbulanceArrived(null);
            //Assert
            Assert.Empty(objCallNothing.PastIncident);
            Assert.Empty(objCallNothing.ActiveCall);
        }

        //Test 2.2: Test when an EmergencyCall is exists but no responder is assigned
        [Fact]
        public void MarkAmbulanceArrived_NoResponderAssigned()
        {
            //Assign
            DispatchSystem objNoResponder  =  new DispatchSystem();
            EmergencyCall callWithoutResponder = new EmergencyCall {PatientName = 'Kasper van Niekerk'};
            objNoResponder.ActiveCall.Add(callWithoutResponder);
            
            //Act: Attempt to mark the ambulance as arrived
            objNoResponder.MarkAmbulanceArrived(callWithoutResponder);
            
            //Assert
            Assert.Contains(callWithoutResponder, objNoResponder.ActiveCall);
            Asssert.Empty(objNoResponder.PastIncident);
        }

        //Test 2.3: Test when an EmergencyCall exists and a responder is assigned
        [Fact]
        public void MarkAmbulanceArrived_ResponderAssigned()
        {
            //Assign
            DispatchSystem objResponderAssigned = new DispatchSystem();
            Responder assignedResponder = new Responder { isAvailable = false];
            EmergencyCall callWithResponder = new EmergencyCall
            {
                PatientName = "Adolf Jonker";
                AssignedResponder = assignedResponder;
            }
            objResponderAssigned.ActiveCall.Add(callWithResponder);
                                                         
            //Act
            objResponderAssigned.MarkAmbulanceArrived(callWithResponder);
                                                         
            //Assert
            Assert.DoesNotContain(callWithResponder, objResponderAssigned.ActiveCall);//call must be removed as an active call
            Assert.Contains(callWithResponder, objResponderAssigned.PastIncident);//call must be added as a Past Incident
            Assert.True(callWithResponder.AssignedResponder.isAvailable); //responder should be available again
        }

        //RAISE RESPONSE TIME EXCEEDED TESTS
        //Test 3.1: Testing that the method does not throw any exceptions when a valid call is entered 
        [Fact]
        public void RaiseResponseTimeExceeded_NoExceptionForValidCall()
        {
            //Assign
            DispatchSystem objNoExceptionValidCall =  new DispatchSystem();
            EmergencyCall validCall = new EmergencyCall { PatientName = "Kaylee" };
            //Act: ensuring no exceptions are thrown
            Exception exceptionTest = Record.Exception(() => objNoExceptionValidCall.RaiseResponseTimeExceeded(validCall));
            //Assert
            Assert.null(exceptionTest);
        }

        //Test 3.2: test that the method does not throw an exception when call is null
        [Fact]
        public void RaiseResponceTimeExceeded_NoExceptionNullCall()
        {
            //Arrange
            DispatchSystem objNullCall = new DispatchSystem();
            
            //Act
            var exception = Record.Excyption(() => dispatchSystem.RaiseResponseTimeExceeded(null));
            //Assert
            Assert.Null(exception);
        }

        //Test 3.3: test that calling RaiseResponseTimeExceeded with null does not change system state
        [Fact]
        public void RaisResponseTimeExceeded_NoChangingOfState()
        {
           //Arrange
            DispatchSystem objNoStateChange = new DispatchSystem();
            
            //Act
            objNoStateChange.RaiseResponseTimeExceeded(null);
            //Assert
            Assert.Empty(objNoStateChange.ActiveCall);
            Assert.Empty(objNoStateChange.PastIncident);
        }

    }
}
