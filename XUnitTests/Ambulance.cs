using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone_2
{
    // This is a *concrete subclass* of the abstract Responder class.
    // It represents an actual ambulance unit in the emergency dispatch system.
    // 
    // Inherits from:
    //   - Responder (abstract base class with common properties like 
    //     ResponderName, Location, and IsAvailable).
    //
    // Implements:
    //   - RespondToCall() → what happens when an ambulance is dispatched.
    //   - UpdateStatus()  → what happens when the ambulance finishes its call.
    // 
    // Each ambulance has:
    //   - An assigned paramedic/crew (inherited from Responder).
    //   - A unique ambulance number for tracking vehicles.
    public class Ambulance : Responder
    {
        // Unique identifier for the ambulance vehicle itself
        // (e.g., "AMB-001" or "Gauteng-EMS-12")
        public string AmbulanceNumber { get; set; }
        // This method is called when the ambulance is dispatched to an emergency call.
        // It prints a message to the console for logging/tracking.
        // It also sets the responder’s availability to false (busy).
        public override void RespondToCall()
        {
            Console.WriteLine($"Ambulance {AmbulanceNumber} with {ResponderName} {ResponderSurname} is en route from {Location}.");
            // Mark the ambulance (responder) as unavailable until call is completed
            IsAvailable = false;
        }
        // This method is called once the ambulance has completed its call.
        // It resets availability back to true so the ambulance can take new calls.
        // It also logs to the console that the ambulance is available again.
        public override void UpdateStatus()
        {
            // Mark the ambulance as available again
            IsAvailable = true;
            Console.WriteLine($"Ambulance {AmbulanceNumber} is now available again.");
        }
    }
}

