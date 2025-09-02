using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using System.Threading;
//using static PRG281_Milestone_2.Program;

namespace PRG281_Milestone_2
{
    public enum EmergencyPriority
    {
        Critical = 1,   // Highest priority, requires immediate response
        Urgent = 2,     // High priority, but not immediately life-threatening
        Routine = 3     // Standard priority, non-urgent cases
    }

    // --- Delegates define the "signature" of methods that can handle events ---
    // They specify what kind of method can subscribe to an event.

    // Delegate for when a critical emergency is received
    public delegate void CriticalEmergencyHandler(EmergencyCall call);
    // Delegate for when the response time limit has been exceeded
    public delegate void ResponseTimeExceededHandler(EmergencyCall call);
    public class DispatchSystem
    {
        // --- Events ---
        // Events are notifications that something has happened.
        // Other classes can "subscribe" to these events and react when they are triggered.

        // Event raised when a critical emergency call is logged
        public event CriticalEmergencyHandler OnCriticalEmergencyReceived;
        // Event raised when the allowed response time for a call is exceeded
        public event ResponseTimeExceededHandler OnResponseTimeExceeded;

        // --- Data Storage ---

        // These lists represent the state of the dispatch system.
        public List<EmergencyCall> ActiveCall { get; set; } = new List<EmergencyCall>(); // Calls that are still being handled
        public List<EmergencyCall> PastIncident { get; set; } = new List<EmergencyCall>(); // Completed calls that are logged
        public List<Responder> Responders { get; set; } = new List<Responder>(); // List of responders
        public List<DispatchOperator> Operators { get; set; } = new List<DispatchOperator>(); // Dispatch operators who take calls

        // --- Event Raising Methods ---

        // Raise the Critical Emergency Event
        public void RaiseCriticalEmergency(EmergencyCall call)
        {
            Console.WriteLine($"ALERT: Critical emergency received: {call.EmergencyType}.");
        }
        // Raise the Response Time Exceeded Event
        public void RaiseResponseTimeExceeded(EmergencyCall call)
        {
            Console.WriteLine($"Response time exceeded for call: {call.PatientName}");
        }

        // --- Core Dispatch Logic ---

        // Assign the nearest available responder to a call
        public void AssignNearestResponder(EmergencyCall emergencyCall)
        {
            // Get all responders that are available
            List<Responder> availableResponders = Responders
                .Where(r => r.IsAvailable)
                .ToList();

            // Case 1: More than one responder available
            if (availableResponders.Count > 1)
            {
                Responder closest = availableResponders[0]; // Pick the first as default
                Random random = new Random();

                // Simulate checking which responder is closest
                foreach (Responder responder in availableResponders)
                {
                    int distanceToCall = random.Next(10); // Simulate distance
                    if (distanceToCall > 6) // If "closer" (arbitrary condition)
                    {
                        closest = responder;
                    }
                }
                // Assign chosen responder
                emergencyCall.AssignedResponder = closest;
                closest.RespondToCall(); // Mark as unavailable
            }
            // Case 2: Only one responder available
            else if (availableResponders.Count == 1)
            {
                emergencyCall.AssignedResponder = availableResponders[0];
                availableResponders[0].RespondToCall(); // Mark as unavailable
            }
            // Case 3: No responders available
            else
            {
                Console.WriteLine("No responders are currently available! Use another medical service provider.");
            }
        }

        // --- Monitoring Logic (runs in separate background thread) ---
        // Monitors how long a responder takes to arrive at the scene
        public void MonitorCall(EmergencyCall call)
        {
            // Define allowed time limit based on call priority
            TimeSpan limit;

            switch (call.Priority)
            {
                case EmergencyPriority.Critical:
                    limit = TimeSpan.FromMinutes(10); // 10 minutes for critical
                    break;

                case EmergencyPriority.Urgent:
                    limit = TimeSpan.FromMinutes(15); // 15 minutes for urgent
                    break;

                case EmergencyPriority.Routine:
                    limit = TimeSpan.FromMinutes(20); // 20 minutes for routine
                    break;

                default:
                    limit = TimeSpan.FromMinutes(20);
                    break;
            }

            // Start a new background thread to monitor this call
            Thread monitorThread = new Thread(() =>
            {
                // If no dispatch time was set → use current time
                DateTime dispatchTime = call.DispatchTime == default
                    ? DateTime.Now
                    : call.DispatchTime;

                DateTime deadline = dispatchTime + limit;

                // Loop in small sleeps until the deadline passes
                while (DateTime.Now < deadline)
                {
                    if (call.CancelMonitoring || call.ArrivalTime != default)
                        return; // Stop monitoring if call is canceled OR responder arrived

                    Thread.Sleep(1000); // Wait 1 second before checking again
                }

                // If time ran out and no arrival yet
                if (call.ArrivalTime == default && !call.CancelMonitoring)
                {
                    Console.WriteLine();
                    OnResponseTimeExceeded?.Invoke(call); // Trigger event
                }
            });

            monitorThread.IsBackground = true; // Run in background (doesn't block program exit)
            monitorThread.Start(); // Start monitoring
        }

        // --- Logging New Calls ---

        // Log a new emergency call into the system
        public void StoreIncident()
        {
            EmergencyCall emergencyCall = new EmergencyCall();

            emergencyCall.DispatchTime = DateTime.Now;

            // Collect caller and patient details (with validation)
            emergencyCall.CallerName = ReadValidName("Input Caller Name:");
            emergencyCall.CallerSurname = ReadValidName("Input Caller Surname:");
            emergencyCall.CallerPhoneNumber = ReadValidPhoneNumber("Input Caller Phone Number:");
            emergencyCall.PatientName = ReadValidName("Input Patient Name:");
            emergencyCall.PatientSurname = ReadValidName("Input Patient Surname:");
            Console.WriteLine("Input Emergency Type:");
            emergencyCall.EmergencyType = Console.ReadLine();


            // Input priority and validate against enum
            bool validInputEmergencyPriority = false;
            string[] priorities = Enum.GetNames(typeof(EmergencyPriority));

            while (!validInputEmergencyPriority)
            {
                Console.WriteLine("Input Emergency Priority (Critical, Urgent, Routine):");
                string priorityInput = Console.ReadLine();
                if (priorities.Contains(priorityInput))
                {
                    validInputEmergencyPriority = true;
                    emergencyCall.Priority = (EmergencyPriority)Enum.Parse(typeof(EmergencyPriority), priorityInput);
                }
            }

            // Add to active call list
            ActiveCall.Add(emergencyCall);

            // Raise event if priority is critical
            if (emergencyCall.Priority == EmergencyPriority.Critical)
            {
                OnCriticalEmergencyReceived?.Invoke(emergencyCall);
            }

            // Immediately assign a responder
            AssignNearestResponder(emergencyCall);
        }

        // --- Arrival Handling ---

        // Allow user to choose which active call the responder has arrived at
        public void MarkAmbulanceArrived()
        {
            if (ActiveCall.Count == 0)
            {
                Console.WriteLine("No active calls to mark as arrived.");
                return;
            }

            Console.WriteLine("Select the call that the ambulance has arrived for:");

            // Show active calls for selection
            for (int i = 0; i < ActiveCall.Count; i++)
            {
                var call = ActiveCall[i];
                Console.WriteLine($"{i + 1}. {call.PatientName} {call.PatientSurname} - {call.EmergencyType}");
            }

            int choice;
            while (true)
            {
                Console.Write("Enter the number of the call: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out choice) && choice >= 1 && choice <= ActiveCall.Count)
                    break;
                else
                    Console.WriteLine("Invalid selection, try again.");
            }


            try
            {
                EmergencyCall selectedCall = ActiveCall[choice - 1];
                MarkAmbulanceArrived(selectedCall); // Process arrival
            }
            catch (ArgumentOutOfRangeException ex)
            {
                //if the user enters a number that is out of range this message will be displayed
                Console.WriteLine("Error: Selected call is out of range.");
            }
            catch (Exception ex)
            {
                //this catches any other unexpected errors
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }


        }

        // Process arrival of responder at the scene
        public void MarkAmbulanceArrived(EmergencyCall emergencyCall)
        {
            if (emergencyCall == null)
            {
                Console.WriteLine("Error: No call selected.");
                return;
            }

            if (emergencyCall.AssignedResponder == null)
            {
                Console.WriteLine("No responder assigned to this call yet.");
                return;
            }

            // Tell monitoring thread to stop
            emergencyCall.CancelMonitoring = true;

            // Move call from active to past
            ActiveCall.Remove(emergencyCall);

            emergencyCall.ArrivalTime = DateTime.Now; // Set arrival time
            emergencyCall.CalculateResponseTime(); // Calculate response duration

            emergencyCall.AssignedResponder.UpdateStatus(); // Free the responder again

            PastIncident.Add(emergencyCall); // Store in past incidents

            Console.WriteLine($"Call for {emergencyCall.PatientName} marked as arrived. Duration: {emergencyCall.CalculateResponseTime().ToString(@"%m\:%s")} minutes");
        }

        // Read and validate name inputRaiseCriticalEmergency
        string ReadValidName(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                //ensuring that the input is not empty and only has letters
                if (!string.IsNullOrWhiteSpace(input) && input.All(char.IsLetter))
                    return input;
                Console.WriteLine("Invalid input. Please enter characters from the alphabet only.");
            }
        }

        // --- Input Validation Helpers ---

        // Read a valid name (letters only)
        string ReadValidPhoneNumber(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();

                // Read a valid phone number (digits only, 7-15 characters)
                if (input.All(char.IsDigit) && input.Length == 10)
                    return input;
                Console.WriteLine("Invalid phone number. Please enter digits only (7-15 characters in length).");
            }
        }

        // Display list of active incidents
        public void ViewActiveIncidents()
        {
            if (ActiveCall.Count == 0)//checking to see if there is any active calls
            {
                // If there are no active calls this message will be displayed
                Console.WriteLine("No active incidents at the momment.");
                return;
            }

            //if there is an active call this heading will be displayed
            Console.WriteLine("\n--- Active Incidents ---");

            // Iterating through the active calls and displaying the details
            foreach (var call in ActiveCall)
            {
                Console.WriteLine($"{call.PatientName} {call.PatientSurname} - {call.EmergencyType} - Priority: {call.Priority}");
            }
        }
        // Display list of past incidents
        public void ViewPastIncidents()
        {
            if (PastIncident.Count == 0)
            {
                // If there are no past incidents this message will be displayed
                Console.WriteLine("No past incidents at the momment.");
                return;
            }

            Console.WriteLine("\n--- Past Incidents ---");

            // Iterating through the past incidents and displaying the details
            foreach (var call in PastIncident)
            {
                Console.WriteLine($"{call.PatientName} {call.PatientSurname} - {call.EmergencyType} - Priority: {call.Priority}");
            }
        }

        // --- User Menu ---

        // Displays a text-based menu for the dispatch system
        public void DisplayMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Emergency Dispatch Menu ---");
                Console.WriteLine("1. Log New Call");
                Console.WriteLine("2. Mark Responder Arrived");
                Console.WriteLine("3. Report");
                Console.WriteLine("4. Exit");
                Console.Write("Choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StoreIncident(); // Log new call
                        Console.WriteLine("Call logged.");
                        MonitorCall(ActiveCall.Last()); // Start monitoring the new call
                        break;

                    case "2":
                        MarkAmbulanceArrived(); // Mark arrival
                        break;

                    case "3":
                        // Show active incidents
                        ViewActiveIncidents();
                        // Show past incidents
                        ViewPastIncidents();
                        break;

                    case "4":
                        return; // Exit the menu

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
                // Pause and clear before showing menu again
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}

