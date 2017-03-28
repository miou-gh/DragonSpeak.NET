# DragonSpeak.NET

DragonSpeak –_or DS for short_– is a functional scripting language used in Furcadia.

## Example

```csharp
var engine = new DragonSpeakEngine();
var page = engine.LoadFromString(script);

var censors = new List<string>() { "humbug", "darn", "fluff" };

// (0:1) when someone joins a room,
page.SetTriggerHandler(new Trigger(TriggerCategory.Cause, 1), (ctx, trigger) => {
  return true;
});

// (1:1) and their username contains a censor,
page.SetTriggerHandler(new Trigger(TriggerCategory.Condition, 1), (ctx, trigger) => {
  return censors.Any(ctx.Username.Contains);
});

// (5:1) disconnect the user from the room.
page.SetTriggerHandler(new Trigger(TriggerCategory.Effect, 1), (ctx, trigger) => {
  if (ctx.TryDisconnect(out var success))
      return success;
});

// execute when the user has joined a room
page.Execute(context: user, ids: 1);
```

Disclaimer: _This website is in no way affiliated with, authorized, maintained, sponsored or endorsed by the Dragon's Eye Productions or any of its affiliates or subsidiaries._
