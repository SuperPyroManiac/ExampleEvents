﻿using System.Reflection;
using ExampleEvents.Events;
using LSPD_First_Response.Mod.API;
using Rage;
using SuperEvents.EventFunctions;

namespace ExampleEvents
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        }
        
        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (!onDuty) return;
            RegisterEvents();
            GameFiber.Wait(5000);
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~ExampleEvents", "~g~Plugin Loaded.",
                "ExampleEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
        }
        
        private static void RegisterEvents()
        {
            API.RegisterEvent(typeof(Fight), API.Priority.High); // This is how you register your events. Simply use the API.RegisterEvent method to add your class.
            API.RegisterEvent(typeof(WeirdCar), API.Priority.Low); // There are 3 priorities you can have for events. Low, Normal, High. Default value is Normal.
        }

        public override void Finally()
        {
            Game.LogTrivial("ExampleEvents by SuperPyroManiac has been cleaned up.");
        }
    }
}