﻿using System.Reflection;
using LSPD_First_Response.Mod.API;
using Rage;
using ExampleEvents.Events;
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
            if (onDuty)
            {
                RegisterEvents();
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~ExampleEvents", "~g~Plugin Loaded.",
                    "ExampleEvents version: " + Assembly.GetExecutingAssembly().GetName().Version + " loaded.");
            }
        }

        private static void RegisterEvents()
        {
            EventManager.RegisterEvent(typeof(ExampleFight), EventManager.Priority.Low); //You can set the priority of events here, Low, Normal, High
            EventManager.RegisterEvent(typeof(ExampleSimple)); //If you dont specify priority, it defaults to "normal"
        }

        public override void Finally()
        {
            Game.LogTrivial("ExampleEvents by SuperPyroManiac has been cleaned up.");
        }
    }
}