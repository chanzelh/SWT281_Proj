using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone_2
{
    // This is a concrete subclass of the abstract "Responder" class.
    // It represents an EMS (Emergency Medical Services) vehicle that
    // responds to medical emergencies. 
    //
    // An EMS vehicle could include rapid response units, paramedic 
    // vehicles, or small specialized emergency vehicles that respond 
    // faster than full ambulances.
    //
    // Inherits from:
    //   - Responder (which provides properties such as ResponderName, 
    //     ResponderSurname, ResponderID, CertificationNumber, Location, 
    //     and IsAvailable).
    //
    // Implements (required from Responder):
    //   - RespondToCall() → What happens when EMS is dispatched.
    //   - UpdateStatus()  → What happens when EMS finishes a call.
    public class EMS : Responder
    {
        // Called when this EMS vehicle is dispatched to an emergency.
        // 1. Prints a log to the console (for tracking and debugging).
        // 2. Marks the responder as unavailable (since it is now busy).
        public override void RespondToCall()
        {
            Console.WriteLine($"EMS Vehicle with {ResponderName} {ResponderSurname} is responding from {Location}.");
            // Mark EMS unit as unavailable while on call
            IsAvailable = false;
        }
        // Called when the EMS vehicle has completed its emergency call.
        // 1. Resets IsAvailable back to true (ready for next call).
        // 2. Prints a message to confirm availability.
        public override void UpdateStatus()
        {
            // Back to available after finishing call
            IsAvailable = true;
            Console.WriteLine($"EMS Vehicle is now available again.");
        }
    }
}


