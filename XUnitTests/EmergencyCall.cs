using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
//using static PRG281_Milestone_2.Program;

namespace PRG281_Milestone_2
{    // --- INTERFACE DEFINITIONS ---

    // The IResponder interface ensures that any class representing a responder will implement methods 
    // to respond to an emergency and to update its status afterwards.
    public interface IResponder 
    {
        // This method is called when the responder is assigned to a new emergency call
        void RespondToCall();
        // This method is called once the responder has completed the call
        // It typically marks them as available again for future emergencies
        void UpdateStatus();
    }

    // The ICustomComparable interface is a user-defined version of IComparable
    // It allows for custom comparison between objects.
    // The main goal is to enable sorting of emergency calls by urgency (priority)
    // and possibly by time (dispatch time).
    public interface ICustomComparable// A bulit-in interface for comparing objects
    {
        // Compare this object to another.
        // Returns:
        //   < 0 if this object is "less urgent" than the other
        //   = 0 if they are equal
        //   > 0 if this object is "more urgent"
        int CompareTo(object obj);// This interface will be used to compare emergency calls and sort them by urgency and time
    }

    // Represents an emergency call in the dispatch system
    // Implements IComparable so calls can be sorted (e.g., by priority or dispatch time)
    public class EmergencyCall : IComparable
    {
        // --- Caller Information ---
        // Name of the person who made the call
        public string CallerName { get; set; }
        // Surname of the caller
        public string CallerSurname { get; set; }
        // Phone number of the caller
        public string CallerPhoneNumber { get; set; }
        // Name of the patient involved in the emergency
        // --- Patient Information ---
        public string PatientName { get; set; }
        // Surname of the patient
        public string PatientSurname { get; set; }
        // Type of emergency (e.g., fire, medical, accident)
        public string EmergencyType { get; set; }
        // Time when the emergency was dispatched
        public DateTime DispatchTime { get; set; }
        // Time when the responder arrived at the scene
        public DateTime ArrivalTime { get; set; }
        // The responder assigned to this emergency call
        public Responder AssignedResponder { get; set; }
        // Priority level of the emergency (Critical, Urgent, Routine)
        internal EmergencyPriority Priority { get; set; }
        // Current status of the emergency call (e.g., "Pending", "In Progress", "Completed")
        public string Status { get; set; }
        // Monitoring thread for response time
        public volatile bool CancelMonitoring = false;

        // Logs the emergency call details into a CSV file for reporting
        // This serves as a simple database (flat file storage).
        public void LogCall()
        {
            //CREATING THE CSV FILE

            // Defining the file path for the log
            string filePath = "EmergencyCallLog.csv";
            // Check if the file already exists
            bool fileExists = System.IO.File.Exists(filePath);

            // Use a StringBuilder to efficiently build a CSV row
            var csvBuilder = new StringBuilder();

            // If file does not exist → add header row first
            if (!fileExists)
            {
                csvBuilder.AppendLine("CallerName,CallerSurname,CallerPhoneNumber,PatientName,PatientSurname,EmergencyType,DispatchTime,ArrivalTime,ResponderID,Priority,Status");

            }

            // Extract responder details if assigned; otherwise leave blank
            string responderName = AssignedResponder != null ? AssignedResponder.ResponderName : ""; // Checks if the assigned responder is not null
            string responderSurname = AssignedResponder != null ? AssignedResponder.ResponderSurname : "";
            string responderID = AssignedResponder != null ? AssignedResponder.ResponderID : "";

            // Build CSV row
            // Each property is written as a comma-separated value
            csvBuilder.AppendLine($"{CallerName},{CallerSurname},{CallerPhoneNumber},{PatientName},{PatientSurname},{EmergencyType},{DispatchTime},{ArrivalTime},{responderName},{responderSurname},{responderID},{Priority},{Status}");

            // Append the row (and header if applicable) to the file
            System.IO.File.AppendAllText(filePath, csvBuilder.ToString());

            // Notify user
            Console.WriteLine("Report Generated.");
        }

        // Calculates how long it took the responder to arrive at the emergency
        public TimeSpan CalculateResponseTime()
        {
            // Simple subtraction of DispatchTime from ArrivalTime
            // If ArrivalTime is not set, this may return an incorrect value (negative or zero)
            return ArrivalTime - DispatchTime;
        }

        // Compares this emergency call to another, allowing for sorting.
        // For now, this implementation always returns 0 (meaning "equal").
        // Ideally, this would be implemented to sort calls by:
        //  1. Priority (Critical > Urgent > Routine)
        //  2. DispatchTime (older calls first if same priority)
        public int CompareTo(object obj)
        {
            // Try to cast the object to EmergencyCall
            EmergencyCall otherCall = obj as EmergencyCall;

            // If comparison object is null or wrong type → treat as equal
            if (otherCall == null) return 0;

            // Example: Compare by priority first
            int priorityComparison = this.Priority.CompareTo(otherCall.Priority);

            if (priorityComparison != 0)
                return priorityComparison * -1; // Reverse order so "Critical" comes first

            // If same priority → compare by dispatch time (earlier calls first)
            return this.DispatchTime.CompareTo(otherCall.DispatchTime);
        }
    }
}
