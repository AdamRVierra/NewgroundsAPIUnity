Newgrounds API Support for Unity
Written by Adam R. Vierra
adam@adamvierra.com

Instructions:
- Drag the API GameObject onto your start scene. Make sure it has the tag "NewgroundsAPI". It'll be transferred over to other scenes and won't duplicate itself.

- It'll have three parameters that you need to fill in. Your API ID and your encryption key can both be found on your game project page. The third one is a bit trickier. If you want to test the medal functionality locally, you need to fill this in. You have to get a proper session ID. Load your game on Newgrounds and get the sessionID which is placed in the game's url (NewgroundsAPI_SessionID). The session ID will expire every six hours, so you'll have to update your backup sessionID everytime the console complains about an expired sessionID. Otherwise, the sessionID will always work if the game's being hosted in Newgrounds.

How to unlock a medal:
- An example is provided in the Main script (Using Newgrounds Namespace) in case you get stuck.

- Include the Newgrounds namespace

- In one of your scripts, set up a reference to the API's script (GameObject.Find then GetComponent). I only have a C# version done, so if you're using Javascript, you'll have to use the SendMessage command to the API GameObject instead in order to communicate with it.

- Once you hooked up the reference, connect the game to the NG servers by starting a Coroutine to API's Connect() function.

- Once you're connected, start a Coroutine to API's GetMedals function. This will load the medals locally for quicker use in case you need more information about them in the future.

- When you're ready to unlock a medal, start a Coroutine to API's UnlockMedal function. It takes one argument, your Medal's name. The console will let you know whether the unlock was successful or not. (Bad medal name, expired session ID, misc errors). The pop up signifying that you unlocked a medal isn't done yet.

Setting up events:

- This is used to call events once the API receives a specific command from the server. It's similar to eventListeners in flash.
-To subscribe a function to an event, use the static AddEvent function in the APIEvent class. eg: APIEvent.AddEvent(APIEvent.EventNames.MEDALS_LOADED, LoadMedals); The first argument is used to determine which event will trigger the function call. All functions using MEDALS_LOADED will be ran every time you load the medals. The second argument is the function you're going to subscribe.

- The actual function must have three parameters. LoadMedals(string type, bool success, object data). The first parameter is the event type string. Second determines whether the load was successful or not. Third contains the data. This object can be all different types, it's based on which event you subscribed to. Main.cs has examples of what's returned for each event type.
