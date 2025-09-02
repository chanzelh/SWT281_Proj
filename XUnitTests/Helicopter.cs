using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone_2
{
    // This is a concrete subclass of the abstract "Responder" class.
    // It represents a medical/emergency helicopter in the dispatch system.
    //
    // Helicopters are typically used for:
    //   - Airlifting patients in critical condition.
    //   - Reaching remote or hard-to-access locations quickly.
    //   - Providing faster transport to hospitals than ground vehicles.
    //
    // Inherits from:
    //   - Responder (provides shared properties like ParamedicName,
    //     ParamedicSurname, ParamedicID, CertificationNumber, Location,
    //     and IsAvailable).
    //
    // Implements (required abstract methods from Responder):
    //   - RespondToCall() → What happens when a helicopter is dispatched.
    //   - UpdateStatus()  → What happens when a helicopter finishes a call.
    public class Helicopter : Responder
    {
        // Called when the helicopter is dispatched to an emergency.
        // 1. Logs a message to the console (for tracking purposes).
        // 2. Sets IsAvailable = false → helicopter is now busy.
        public override void RespondToCall()
        {
            Console.WriteLine($"Helicopter with {ResponderName} {ResponderSurname} is responding from {Location}.");
            // Mark helicopter as unavailable
            IsAvailable = false;
        }
        // Called when the helicopter has finished responding to a call.
        // 1. Sets IsAvailable = true → ready for the next mission.
        // 2. Logs a message confirming availability.
        public override void UpdateStatus()
        {
            // Back to available after finishing call
            IsAvailable = true;
            Console.WriteLine($"The helicopter is now available again.");
        }
    }
}
