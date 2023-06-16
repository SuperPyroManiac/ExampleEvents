using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents;
using SuperEvents.Attributes;

namespace ExampleEvents.Events;

[EventInfo("Abandoned Vehicle", "Investigate the vehicle.")] //Required attribute, title and description of the event.
public class ExampleSimple : AmbientEvent
{
    protected override Vector3 EventLocation { get; set; }//Required, this is the location for the event. SuperEvents uses this to add the blip, and cleanup radius.

    private Vehicle _eVehicle;
    
    protected override void OnStartEvent() //First method run by SuperEvents, use it to create the scene.
    {
        PyroFunctions.FindSideOfRoad(120, 45, out var spawnPoint, out _); //PyroCommon API to find a location on the side of the road.
        EventLocation = spawnPoint; //Setting the EventLocation to the side of road we collected. REQUIRED
        OnSceneDistance = 25; //Optional value, this is the distance the player needs to be from an event to trigger the "OnScene" method, and other things like event hint. Defaults to 20
        if (EventLocation.DistanceTo(Player) < 35f) // I personally like to end the event if the event spawns too close
        {
            End(true); 
            //When you end an event, you can add true in the field to make it forcefully cleanup any items in the entity list.
            //If you manage your entities in OnCleanup() yourself, then this has no effect.
            return;
        }
        PyroFunctions.SpawnNormalCar(out _eVehicle, EventLocation); //PyroCommon API to spawn a random normal car.
        EntitiesToClear.Add(_eVehicle); //Optional list you can add entities to, this list is automatically managed and cleaned up at the end of an event.
    }

    protected override void OnProcess() //Second method run by SuperEvents, this runs in a loop until End()!
    {
        Questioning.Enabled = true; //This enabled the convo menu if you added buttons to it earlier.
        if (!_eVehicle) End(true); //Checks if the vehicle is null
        if (Player.IsDead) End(); //Checks if the player is dead.
        //This method runs in a loop, so do as you please here.
    }

    protected override void OnScene()
        //This method is called from OnProcess() in the base class when the player is in the range of OnSceneDistance. This method is called once, and does not loop / repeat!
        //OnProcess() is still running after this is called as well, this is just an optional override to save you the hassle of making on onScene bool yourself.
    {
        _eVehicle.Explode();
    }

    protected override void OnCleanup()
        //This runs when you call End() It stops OnProcess() and is the last thing run before the event ends.
        //If your event is simple and you put all of your entities in the EntitiestoClear list you don't have to do anything here.
    {
        //_eVehicle?.Dismiss();
        //Since this vehicle is in the cleanup list, we don't actually have to dismiss it ourself. Ofc you can handle entities yourself if you prefer.
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        //This is an override for the Interaction event from RageNativeUI
        //This specific one is the main menu (Y menu) so if you wish to add buttons there you can.
        
        base.Interactions(sender, selItem, index);
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        //This is an override for the Conversation event from RageNativeUI
        //This specific one is the convo menu (Y menu > Speak with subjects) so if you wish to add buttons there you can.
        //Keep in mind, if you add button to the convo menu, make sure to use Questioning.Enabled = true; in onScene()!
        
        base.Conversations(sender, selItem, index);
    }
}