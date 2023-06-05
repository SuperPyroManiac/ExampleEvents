using System;
using System.Collections.Generic;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperEvents;
using SuperEvents.EventFunctions;

namespace ExampleEvents.Events
{
    
    
    //This is a more advanced example, showing how you might actually use an event. For a simple example, check CleanExample.cs
    
    
    
    public class Fight : AmbientEvent // All events must inherit from the AmbientEvent base class.
        {
        private string _name1;
        private string _name2;
        private Vector3 EventLocation;
        private UIMenuItem _speakSuspect;
        private UIMenuItem _speakSuspect2;
        private Ped _suspect;
        private Ped _suspect2;
        private Tasks _tasks = Tasks.CheckDistance;

        protected override void StartEvent() // This is the first method in AmbientEvent that is executed. Use this to setup your scene.
        {
            //Setup
            API.SideOfRoadLocation(120, 45, out EventLocation, out _); // This is part of the SuperEvents.API class which adds some useful features. This will find a location on the side of the road.
            EventLocation = EventLocation; // This is where you tell SE where the spawnpoint of your event is, this is critical!
            if (EventLocation.DistanceTo(Player) < 35f) // This is very important, make sure the event spawned at a reasonable distance or cancel the event.
            {
                End(true); // End() method is different from LSPDFR, this one requires a bool. false will dismiss everything in the lists below, and true will delete everything in the lists below.
                return;
            }
            Game.LogTrivial("ExampleEvents: Fight event loaded!"); // Please always add some sort of identifiers to your events for the log!

            //Peds
            _suspect = new Ped(EventLocation) {IsPersistent = true, BlockPermanentEvents = true};
            API.SetDrunk(_suspect, true); // Another features in the SuperEvents.API class.
            _name1 = Functions.GetPersonaForPed(_suspect).FullName;
            _suspect.Metadata.stpAlcoholDetected = true;
            EntitiesToClear.Add(_suspect); // This is the list SuperEvents stores entities in! It is critical you use this as it helps with cleanups in case of errors.
            _suspect2 = new Ped(_suspect.FrontPosition) {IsPersistent = true, BlockPermanentEvents = true};
            API.SetDrunk(_suspect2, true);
            _name2 = Functions.GetPersonaForPed(_suspect2).FullName;
            _suspect2.Metadata.stpAlcoholDetected = true;
            NativeFunction.Natives.x5AD23D40115353AC(_suspect2, _suspect, -1);
            NativeFunction.Natives.x5AD23D40115353AC(_suspect, _suspect2, -1);
            EntitiesToClear.Add(_suspect2); // We add all entities to this list, don't forget!
            //UI Items
            _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
            _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name2);
            ConvoMenu.AddItem(_speakSuspect); // ConvoMenu is the "Speak" button in the SuperEvents menu. You can easily add things to it like this for dialogue.
            ConvoMenu.AddItem(_speakSuspect2);

            base.StartEvent();
        }

        protected override void Process() // This is the second method that is run in SE directly after the StartEvent() one finished. This method is in a loop until End(bool)! (Like LSPDFR)
        {
            try
            {
                switch (_tasks)
                {
                    case Tasks.CheckDistance:
                        if (Game.LocalPlayer.Character.DistanceTo(EventLocation) < 20f)
                        {
                            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                            _suspect2.PlayAmbientSpeech("GENERIC_CURSE_MED");
                            if (Settings.ShowHints) // This is the SuperEvents.Settings class, some settings are public so you can read their values! It is important that you include these features.
                            // For the sake of consistency, please use the same logo and colors. If you do not use these, your plugins will not be recommended on the SE page.
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~A Fight", "Stop the fight, and make sure everyone is ok.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                            // Again this is a requirement to be on the SE page. The interact key by default in Y but can be changed in the SE.ini file.
                            Questioning.Enabled = true; // This is part of the built in SuperEvents RNUI menu. If you include dialogue you can easily use the built in button.
                            _tasks = Tasks.OnScene;
                        }

                        break;
                    case Tasks.OnScene:
                        var choice = new Random().Next(1, 4);
                        Game.LogTrivial("ExampleEvents: Fight event picked scenerio #" + choice);
                        switch (choice)
                        {
                            case 1:
                                _suspect.Tasks.FightAgainst(_suspect2);
                                _suspect2.Tasks.FightAgainst(_suspect);
                                break;
                            case 2:
                                _suspect.Tasks.FightAgainst(_suspect2);
                                _suspect2.Tasks.FightAgainst(_suspect);
                                GameFiber.Wait(2000);
                                _suspect.Tasks.ClearImmediately();
                                _suspect2.Tasks.ClearImmediately();
                                _suspect.Tasks.FightAgainst(Player);
                                _suspect2.Tasks.FightAgainst(Player);
                                break;
                            case 3:
                                _suspect.Tasks.Cower(-1);
                                _suspect2.Inventory.Weapons.Add(WeaponHash.Pistol).Ammo = -1;
                                _suspect2.Tasks.ClearImmediately();
                                _suspect2.Tasks.AimWeaponAt(_suspect, -1);
                                GameFiber.Wait(500);
                                _suspect2.Tasks.FireWeaponAt(_suspect, -1, FiringPattern.BurstFirePistol);
                                _suspect2.BlockPermanentEvents = false;
                                break;
                            default:
                                End(true); // It should never get to this point, but if an issue happens, you can use End(true) to forcefully end and cleanup the event.
                                break;
                        }

                        _tasks = Tasks.End;
                        break;
                    case Tasks.End:
                        break;
                    default:
                        End(true); // It should never get to this point, but if an issue happens, you can use End(true) to forcefully end and cleanup the event.
                        break;
                }

                base.Process();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
                Game.LogTrivial("ExampleEvents Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("ExampleEvents Error Report End");
                End(true);
            }
        }

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index) // This is the method for ConvoMenu, so you can easily add your button to it and override it!
        {
            if (selItem == _speakSuspect)
            {
                if (_suspect.IsDead)
                {
                    _speakSuspect.Enabled = false;
                    _speakSuspect.RightLabel = "~r~Dead";
                    return;
                }

                var dialog1 = new List<string>
                {
                    "~b~You~s~: What's going on? Why were you guys fighting?",
                    "~r~" + _name1 + "~s~: What does it matter to you?!",
                    "~b~You~s~: Finding out what's going on is my job.",
                    "~r~" + _name1 + "~s~: Whatever, it's not my job to tell you."
                };
                var dialog2 = new List<string>
                {
                    "~b~You~s~: What's going on? Why were you guys fighting?",
                    "~r~" + _name1 + "~s~: He started it! I was just defending myself!",
                    "~b~You~s~: What did he do to start it?",
                    "~r~" + _name1 + "~s~: He is drunk, started saying rude stuff about my cat Ruffles!",
                    "~b~You~s~: Alright, well I'll take note of that."
                };
                var dialogIndex1 = 0;
                var dialogIndex2 = 0;
                var dialogOutcome = new Random().Next(0, 101);
                var stillTalking = true;

                if (Player.DistanceTo(_suspect) > 5f)
                {
                    Game.DisplaySubtitle("Too far to talk!");
                    return;
                }

                NativeFunction.Natives.x5AD23D40115353AC(_suspect, Game.LocalPlayer.Character, -1);
                GameFiber.StartNew(delegate
                {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5) stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                });
            }

            if (selItem == _speakSuspect2)
            {
                if (_suspect2.IsDead)
                {
                    _speakSuspect2.Enabled = false;
                    _speakSuspect2.RightLabel = "~r~Dead";
                    return;
                }

                var dialog1 = new List<string>
                {
                    "~b~You~s~: Why were you two fighting? What's going on!",
                    "~r~" + _name2 + "~s~: Screw you, I hope you die!",
                    "~b~You~s~: You need to tell me what's going on.",
                    "~r~" + _name2 + "~s~: I don't need to tell you anything."
                };
                var dialog2 = new List<string>
                {
                    "~b~You~s~: Explain to me what's going on.",
                    "~r~" + _name2 + "~s~: I don't even know where I am sir.",
                    "~b~You~s~: Have you used any drugs or had anything to drink?",
                    "~r~" + _name2 + "~s~: All of it.",
                    "~b~You~s~: Alright, well I'll take note of that."
                };
                var dialogIndex1 = 0;
                var dialogIndex2 = 0;
                var dialogOutcome = new Random().Next(0, 101);
                var stillTalking = true;

                if (Player.DistanceTo(_suspect2) > 5f)
                {
                    Game.DisplaySubtitle("Too far to talk!");
                    return;
                }

                NativeFunction.Natives.x5AD23D40115353AC(_suspect2, Game.LocalPlayer.Character, -1);
                GameFiber.StartNew(delegate
                {
                    while (stillTalking)
                    {
                        if (dialogOutcome > 50)
                        {
                            Game.DisplaySubtitle(dialog1[dialogIndex1]);
                            dialogIndex1++;
                        }
                        else
                        {
                            Game.DisplaySubtitle(dialog2[dialogIndex2]);
                            dialogIndex2++;
                        }

                        if (dialogIndex1 == 4 || dialogIndex2 == 5) stillTalking = false;
                        GameFiber.Wait(6000);
                    }
                });
            }

            base.Conversations(sender, selItem, index);
        }

        private enum Tasks
        {
            CheckDistance,
            OnScene,
            End
        }
    }
}