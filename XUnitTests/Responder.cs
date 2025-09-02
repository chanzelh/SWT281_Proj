using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone_2
{
    // --- ABSTRACT BASE CLASS: Responder ---
    // This abstract class represents a generic "Responder" in the 
    // emergency dispatch system. Examples of responders:
    //   - A paramedic
    //   - An ambulance crew
    //   - A helicopter medical team
    // 
    // By making this class abstract:
    //   - You cannot directly create a "Responder" object.
    //   - Instead, specific types of responders (e.g., AmbulanceResponder,
    //     HelicopterResponder) will inherit from this class and 
    //     implement their own specialized behaviors.
    // 
    // Implements:
    //   - IResponder (custom interface that forces responders to have 
    //     RespondToCall() and UpdateStatus() methods).
    //   - IComparable<Responder> (built-in interface that allows responder 
    //     objects to be compared to each other, useful for sorting lists).
    // ============================================================
    public abstract class Responder : IResponder, IComparable<Responder>
    {
        // First name of the responder
        public string ResponderName { get; set; }
        // Surname of the responder
        public string ResponderSurname { get; set; }
        // Unique identifier for the responder (e.g., employee/crew ID)
        // Used for identification and for comparison
        public string ResponderID { get; set; }
        // Certification or license number of the paramedic
        // Helps verify qualification or track professional registration
        public int CertificationNumber { get; set; }
        // Current location of the responder
        // This may be used to determine "nearest responder"
        public string Location { get; set; }
        // Availability status
        // True  = the responder is free to take a new call
        // False = the responder is currently busy with an incident
        public bool IsAvailable { get; set; }

        // Abstract method (must be implemented by subclasses).
        // This is called when the responder is dispatched to a call.
        // Typical implementation:
        //   - Change IsAvailable to false
        //   - Log that they are responding
        public abstract void RespondToCall();
        // Abstract method (must be implemented by subclasses).
        // This is called when the responder finishes handling a call.
        // Typical implementation:
        //   - Change IsAvailable back to true
        //   - Update status in the system
        public abstract void UpdateStatus();
        // Allows responders to be compared to each other for sorting
        // Implements IComparable<Responder> → CompareTo() method
        //
        // This implementation compares responders by their ParamedicID
        // (string-based comparison, alphabetical order).
        //
        // Returns:
        //   > 0 → this responder comes after "other"
        //   = 0 → they are equal
        //   < 0 → this responder comes before "other"
        public int CompareTo(Responder other)
        {
            // If "other" is null, treat this object as "greater"
            // (a null reference is considered smaller than any valid object)
            if (other == null) return 1;
            // Compare ParamedicIDs alphabetically (Ordinal = binary comparison, case-sensitive)
            return string.Compare(this.ResponderID, other.ResponderID, StringComparison.Ordinal);
        }
    }
}