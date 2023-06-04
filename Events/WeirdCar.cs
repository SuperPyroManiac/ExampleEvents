using System;
using Rage;
using SuperEvents;
using SuperEvents.EventFunctions;

namespace ExampleEvents.Events
{
    
    
    
    
    //This is a more advanced example, showing how you might actually use an event. For a simple example, check CleanExample.cs
    
    
    
    
    public class WeirdCar : AmbientEvent // All events must inherit from the AmbientEvent base class.
     {
        private Vehicle _eVehicle;
        private Vector3 _spawnPoint;

        private Tasks _tasks = Tasks.CheckDistance;

        protected override void StartEvent() // This is the first method in AmbientEvent that is executed. Use this to setup your scene.
        {
            //Setup
            API.SideOfRoadLocation(120, 45, out _spawnPoint, out _); // This is part of the SuperEvents.API class which adds some useful features. This will find a location on the side of the road.
            EventLocation = _spawnPoint; // This is where you tell SE where the spawnpoint of your event is, this is critical!
            if (_spawnPoint.DistanceTo(Player) < 35f) // This is very important, make sure the event spawned at a reasonable distance or cancel the event.
            {
                End(true);
                return;
            }
            Game.LogTrivial("ExampleEvents: WeirdCar event loaded!"); // Please always add some sort of identifiers to your events for the log!
            //eVehicle
            API.SpawnNormalCar(out _eVehicle, _spawnPoint);
            EntitiesToClear.Add(_eVehicle);

            base.StartEvent();
        }

        protected override void Process() // This is the second method that is run in SE directly after the StartEvent() one finished. This method is in a loop until End(bool)! (Like LSPDFR)
        {
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 25f)
                        {
                            if (Settings.ShowHints)// This is the SuperEvents.Settings class, some settings are public so you can read their values! It is important that you include these features.
                                // For the sake of consistency, please use the same logo and colors. If you do not use these, your plugins will not be recommended on the SE page.
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Abandoned Vehicle", "Investigate the vehicle.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            // Again this is a requirement to be on the SE page. The interact key by default in Y but can be changed in the SE.ini file.
                            _tasks = Tasks.OnScene;
                        }

                        break;
                    case Tasks.OnScene:
                        var choice = new Random().Next(1, 7);
                        Game.LogTrivial("SuperEvents: Abandoned Vehicle event picked scenerio #" + choice);
                        switch (choice)
                        {
                            case 1:
                                API.DamageVehicle(_eVehicle, 200, 200);
                                break;
                            case 2:
                                API.DamageVehicle(_eVehicle, 200, 200);
                                _eVehicle.IsStolen = true;
                                break;
                            case 3:
                                _eVehicle.IsEngineOn = true;
                                _eVehicle.IsInteriorLightOn = true;
                                break;
                            case 4:
                                _eVehicle.Rotation = new Rotator(0f, 180f, 0f);
                                break;
                            case 5:
                                _eVehicle.IsStolen = true;
                                break;
                            case 6:
                                _eVehicle.AlarmTimeLeft = TimeSpan.MaxValue;
                                break;
                            default:
                                End(true);
                                break;
                        }

                        _tasks = Tasks.End;
                        break;
                    case Tasks.End:
                        break;
                    default:
                        End(true);
                        break;
                }

                base.Process();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("SuperEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("SuperEvents Error Report End");
                End(true);
            }
        }

        private enum Tasks
        {
            CheckDistance,
            OnScene,
            End
        }
    }
}