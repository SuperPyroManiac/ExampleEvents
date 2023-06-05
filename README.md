# SuperEvents API Guide
***By SuperPyroManiac***
<br>
> This will help you understand and use the API to easily create your own events!

<br>

## Requirements
- Basic C# experience.
- A .NET Framework project that is using the LSPDFR API.
- References to RageNativeUI, SuperEvents, LSPDFR, Windows.forms, and System.
- Experience making callouts for LSPDFR will make this very easy to use as well.

<br>

## Creating the Event class.
>> Your class will inherit from the AmbientEvent class. Similar to how callouts inherit from the Callout class in LSPDFR.<br>
>>`public class CleanExample : AmbientEvent`
> <br>
## Main Overriding methods
>You will override a few methods to create the events. At minimum two. There are 5 methods in total you can override.<br>
>StartEvent(), Process(), End(bool), Interactions(UIMenu, UIMenuItem, int), Conversations(UIMenu, UIMenuItem, int)<br>
> ![Example of overrides](https://i.imgur.com/btKMryp.png)<br>
><br>
## StartEvent()
> This is the first method that is executed. This is where you should setup the event. There are a couple of requirements here to ensure the event works correctly.<br>
> ### These are required!
>  - **Vector3 EventLocation**: Must be set, this is the spawn location of the event! This value is where SuperEvents will check distances, and add the blip if it's enabled in the SuperEvents.ini file.
>  - **EntityList EntitiesToClear**: All vehicles, peds, and other entities related to the event must be added to this list via EntitiesToClear.Add(entity). It's critical that this is done as this is how SuperEvents will clean up entities when the event ends. It also helps SuperEvents clean up if the event crashes.
>  - **BlipList BlipsToClear**: This is where all blips you create should be added, similar to the other list. It's unlikely you will need this as SuperEvents will create a blip automatically if it's enabled in the config. Regardless this list exists just incase you need other blips.
>  - **String EventTitle**: This is the title of the event shown if the SE option for show hints is true.
>  - **String EventDescription**: This is the description of the event shown if the SE option for show hints is true.
>  ![Example Usage](https://i.imgur.com/1gPfaxr.png)<br>
><br>

## Process()
> This is executed after the StartEvent() method. **This runs in a loop!** This will run until the event is ended.<br>
> ![Example](https://i.imgur.com/EJyVMEE.png)<br>

## End(bool ForceCleanup)
> This is executed from End(bool). It is automatically done if SuperEvents detects an error, or if the player is too far from the event. Other then that, the event will not end unless they player ends it via keybind or SE menu, or if you add an End(bool) in your code.
> The bool is for deciding on how to cleanup the scene.
> - **End(false)** Standard way of ending an event. This will dismiss all entities in the EntitiesToClear list, and delete all blips in the list.
> - **End(true)** Non-standard way of ending an event. This will forcefully delete all items related to the event and try to move on. This should only be used in the case of errors.
> <br><br>
> The End(bool) method doesn't need to be overridden in almost all cases, as adding your items to the correct lists will handle everything behind the scenes. Even the RageNativeUI related items are handled in the base class. And ideal event does not override this method. **If you use End() without an argumnt, it will default to false.**
> <br><br>
>
> ##### <br><br>These three methods alone can handle all simple events. There are two other methods which can be used for RageNativeUI objects for dialogue or actions. There is a good example of them being used in the Fight.cs class in the Events directory.
<br>
<br>

## Registering Events
> You will need to register all events in your main.cs class at runtime. It's very simple to do, just use the `API.RegisterEvent(typeof(EventClass))` by default the priority on events in normal. There are 3 priorities you can choose from, Low, Normal, High.
> To assign a priority, when you register the event you can add a second arg. Example: `API.RegisterEvent(typeof(Fight), API.Priority.High)`
> ![Example](https://i.imgur.com/HvNiubz.png)
