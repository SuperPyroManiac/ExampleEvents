using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents;
using SuperEvents.EventFunctions;

namespace ZExampleEvents.Events
{
    public class CleanExample : AmbientEvent // Inherits the AmbientEvent base class.
    {
        private Ped ped;
        private Vehicle vehicle;
        private Blip blip;
        
        protected override void StartEvent() // Firt method loaded by SuperEvents - Setup here
        {
            EventTitle = "Example Event";
            EventDescription = "Can I get uhhhhhhhhhhhh... Boneless pizza?";
            EventLocation = new Vector3(); // You must set the EventLocation.
            if (EventLocation.DistanceTo(Player) < 35f)
            {
                End(true); // Its a good idea to make sure events don't just pop in too close to the player.
                return;
            }

            ped = new Ped(EventLocation);
            ped.IsPersistent = true;
            API.SpawnNormalCar(out vehicle, EventLocation);
            blip = new Blip(new Vector3(0, 0, 0));
            
            EntitiesToClear.Add(ped); //Add entities to this list! SE will handle all the cleaning of them!
            EntitiesToClear.Add(vehicle); //Entities include things like Humanoids, Animals, Cars, Aircraft, Etc. Add it all!
            
            BlipsToClear.Add(blip); // If you need to use more blips than the standard SE location one, add them to this list, SE will handle the cleanup of them!
            
            base.StartEvent();
        }

        protected override void Process() // This is the second method SE runs, this runs in a loop until End(bool) is called.
        {
            Game.LogTrivial("I will spam you log, I run in a loop!");

            if (ped.IsDead)
            {
                Game.LogTrivial("Wow the ped is dead, great work!");
                End(false); //This is a standard ending, this will just dismiss all entities, and delete blips.
            }

            if (!ped.Exists())
            {
                Game.LogTrivial("This is an error, the ped is null!");
                End(true); //This is a forced ending, this will delete all entities and blips related to the event, and move on.
            }
            
            base.Process();
        }

        protected override void End(bool forceCleanup) // This method is what stops the event. In most cases you should not need to override this.
        {
            Game.LogTrivial("You most likely will never need to override this, but the option is available.,");
            base.End(forceCleanup);
        }

        protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index) // Optional override if you add RNUI buttons in the SE menu.
        {
            Game.LogTrivial("You can add buttons and things for RageNativeUI here. This is the Interaction menu.");
            base.Interactions(sender, selItem, index);
        }

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index) //Optional override if you add RNUI buttons in the SE convo menu.
        {
            Game.LogTrivial("You can add buttons and things for RageNativeUI here. This is the Convo menu.");

            base.Conversations(sender, selItem, index);
        }
    }
}