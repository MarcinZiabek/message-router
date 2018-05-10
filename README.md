What is it and what is it good for?
-----------------------------------

Nowadays, no one considers implementing the communication layer using pure sockets. Some messaging libraries are developed such as ZeroMQ and its .NET Framework port called NetMQ. A number of problems solved by them is enormous: sending information over the network in a lightweight manner, making sure that a whole message is received or distributing messages to many subscribers. In the real world scenarios, it is not enough.

Let's consider how alternatives look like. The HTTP protocol provides different endpoints - every message is appropriately deserialized and routed to methods in your code. The messaging system in ZeroMQ does not contain any header information and, therefore, does not provide any similar functionality.

The NetmqRouter acts as an additional layer of abstraction between your code and communication layer. In the internal implementation, every message consists of two message parts: a header (containing name of the communication route aka address) and a message body.

Available packges
-----------------

| Package name         | Description                                           | Link       |
| -------------------- | ----------------------------------------------------- | ---------- |
| Message.Router       | A main routing package. Required.                     | [nuget](1) |
| Message.Router.NetMQ | Support for NetMQ framework. Recommented.             | [nuget](2) |
| Message.Router.Json  | General serializer for the JSON format. Recommented.  | [nuget](3) |
| Message.Router.Xml   | General serializer for the Xml format.                | [nuget](4) |

Here you can find all packages available on the Nuget website: [link](https://www.nuget.org/profiles/MarcinZiabek)

[1]: https://www.nuget.org/packages/Message.Router/
[2]: https://www.nuget.org/packages/Message.Router.NetMQ/
[3]: https://www.nuget.org/packages/Message.Router.Json/
[4]: https://www.nuget.org/packages/Message.Router.Xml/

Simple yet powerful routing
---------------------------

This library uses reflection to analyze your code and configure the system behind the scenes. Annotate you endpoint with the Route attribute and as an argument pass name of the communication route. The library will automatically analyze if your endpoint method contains an argument that can be used as a message payload.

```csharp
class ExampleSubscriber
{
    [Route("TestRoute")]
    public string Test(string value)
    {
        // your business logic here
    }
}
```

Responding endpoints
--------------------

Your endpoints can simply respond to any message using the return value. All scenarios are allowed :)

```csharp
class ExampleSubscriber
{
    [Route("TestRoute")]
    [ResponseRoute("AnotherRoute")]
    public string Test(string value)
    {
        // this endpoint will respond with text message
    }

    [Route("TestRoute")]
    [ResponseRoute("AnotherRoute")]
    public void Test(string value)
    {
        // this endpoint will respond with an event
    }
}
```

Empty messages aka events
-------------------------

Empty payloads are treated as a special type of messages, here called "events". Two scenarios are mainly interesting:
- create an endpoint method that does not contain any argument - it will be an event subscriber,
- create an endpoint method with the ResponseRoute attribute but returning void - it will be an event emitter.

Please note, that you can use event approach along the message pattern, e.g. an endpoint can subscribe an event but return text message. All combinations are allowed.

```csharp
class ExampleSubscriber
{
    [Route("TestRoute")]
    public void Test()
    {
        // event subscriber
    }

    [Route("TestRoute")]
    [Route("AnotherRoute")]
    public void Test(string text)
    {
        // event emitter
    }

    [Route("TestRoute")]
    [Route("AnotherRoute")]
    public void Test()
    {
        // event subscriber and emitter at the same time
    }
}
```

Sending messages
----------------

If you want to do everything for yourself, send a simple message via the MessageRouter method:

```csharp
// sending event
router.SendMessage("SomeRoute");

// sending text message
router.SendMessage("AnotherRoute", "Hello world!");

// available methods
public void SendMessage(string routeName);
public void SendMessage(string routeName, byte[] data);
public void SendMessage(string routeName, string text);
public void SendMessage(string routeName, object _object);
```

Register subscribers
--------------------

It is possible to register your own methods as subscribers to specified routes. 

```csharp
router.RegisterSubscriber("IncomingRoute", () => { });
router.RegisterSubscriber<string>("IncomingRoute", payload => { });
router.RegisterSubscriber("IncomingRoute", "OutcomingRoute", () => "Hello world");
router.RegisterSubscriber<string, string>("IncomingRoute", "OutcomingRoute", payload => "Hello " + payload);
```

Base routes
-----------

You can annotate your class with the BaseRoute attribute to use subscribe to a specified route or family of routes.

```csharp
[BaseRoute("BaseRoute")]
class ClassWithBaseRoute
{
    [Route("IncomingRoute")]
    [ResponseRoute("OutcomingRoute")]
    public void Handler()
    {
        // this endpoint will:
        // - subscribe messages from "BaseRoute/IncomingRoute" route,
        // - emit messages to "OutcomingRoute" route.
    }
}
```

Open to communication layers
----------------------------

This library contains preconfigured configuration layer to most popular communication patterns in NetMQ:

```csharp
// basic example for the publisher-subscriber pattern
var publisherSocket = new PublisherSocket();
publisherSocket.Bind(Address);

var subscriberSocket = new SubscriberSocket();
subscriberSocket.Connect(Address);

var router = MessageRouter.WithPubSubConnecton(publisherSocket, subscriberSocket)


// available methods
public static MessageRouter WithPubSubConnecton(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket);
public static MessageRouter WithPubSubConnecton(string publishAddress, string subscribeAddress);

public static MessageRouter WithPushPullConnection(PushSocket pushSocket, PullSocket pullSocket);
public static MessageRouter WithPushPullConnection(string pushAddress, string pullAddress);

public static MessageRouter WithPairConnection(PairSocket socket);
public static MessageRouter WithPairConnection(string address);
```

Implement your own communication layer
--------------------------------------

Do you have a special communication layer with custom business rules? Implement the IConnection interface and be free!

```csharp
// declare your custom connection handler
public class PairConnection : IConnection
{
    PairSocket Socket { get; }
    private readonly object _socketLock = new object();

    public PairConnection(PairSocket socket)
    {
        Socket = socket;
    }

    public void SendMessage(SerializedMessage message)
    {
        lock(_socketLock)
            Socket.SendMessage(message);
    }

    // returns true if message was received successfully
    public bool TryReceiveMessage(out SerializedMessage message)
    {
        lock(_socketLock)
            return Socket.TryReceiveMessage(out message);
    }

    public void Connect(IEnumerable<string> routeNames) { }

    public void Disconnect()
    {
        Socket?.Close();
        Socket?.Dispose();
    }
}


// create message router
var socket = new PairSocket(Address);
var connection = new PairConnection(socket);

var router = new MessageRouter(connection);
```

Serialization layer: type and general serializers
-------------------------------------------------

Your endpoints can return any type of message as long as the library can serialize it to the binary format. Use any serialization library you want - you can use JSON protocol, ProtoBuf or implement your own solution. Batteries are included for types:
- byte arrays,
- text,
- objects serialized in JSON or XML formats (please use appropriate helpers packages).

```csharp
router.RegisterTypeSerializerFor(new RawDataTypeSerializer());
router.RegisterTypeSerializerFor(new BasicTextTypeSerializer());
router.RegisterGeneralSerializerFor(new JsonObjectSerializer());
```

Serializer per type
-------------------

It is possible to register a serializer designed for the specialized type. Below you can find an example implementation of text serializer:

```csharp
// create the serializer
public class BasicTextTypeSerializer : ITypeSerializer<string>
{
    private readonly Encoding _encoding;

    /// <param name="encoding">Encoding that will be used for text serialization.</param>
    public BasicTextTypeSerializer(Encoding encoding)
    {
        _encoding = encoding;
    }

    public BasicTextTypeSerializer() : this(Encoding.UTF8)
    {

    }

    public byte[] Serialize(string text) => _encoding.GetBytes(text);
    public string Deserialize(byte[] data) => _encoding.GetString(data);
}

// register the serializer
router.RegisterTypeSerializerFor(new BasicTextTypeSerializer());
```

General serializers
-------------------

It is possible to register a serializer designed for the group of types. Below you can find an example implementation of JSON serializer:

```csharp
// create the serializer
public class JsonObjectSerializer : IGeneralSerializer<object>
{
    private readonly Encoding _encoding;

    /// <param name="encoding">Encoding that will be used for text serialization.</param>
    public JsonObjectSerializer(Encoding encoding)
    {
        _encoding = encoding;
    }

    public JsonObjectSerializer() : this(Encoding.UTF8)
    {

    }

    public byte[] Serialize(object _object)
    {
        var json = JsonConvert.SerializeObject(_object);
        return _encoding.GetBytes(json);
    }

    public object Deserialize(byte[] data, Type targetType)
    {
        var json = _encoding.GetString(data);
        return JsonConvert.DeserializeObject(json, targetType);
    }
}

// register the serializer
router.RegisterGeneralSerializerFor(new JsonObjectSerializer());
```

Handle every exception
----------------------

There are four main sources (and therefore four types) of exceptions in the NetmqRouter library:
1.  **ConfigurationException** - this kind of exception is connected to wrong router configuration, e.g. lack of serializers for a used data type,
2.  **ConnectionException** - this kind of exception may occur when any problem with connection occurs, e.g. the socket is already taken by another process,
3.  **SerializationException** - this kind of exception is connected to the serialization process, e.g. a passed object cannot be serialized,
4.  **SubscriberException** - this kind of exception decorate every exception that was thrown from endpoint's code.

Please note that by declaring your own business logic (e.g. connection protocols, serializers or endpoints) you are creating code that potentially may throw an exception. All of them will be available in the InnerException field.

The ConfigurationException can occur in the code where your router is configured. Other exceptions are published in the special event:

```csharp
router.OnException += exception =>
{
    // handle any exception here
    // good proposition: handle it using any logging library
    Console.WriteLine(exception.Message);
};
```

Outlook for the whole example
-----------------------------

```csharp
[TestFixture]
public class MessagesRouterTests
{
    private const string Address = "tcp://localhost:6000";

    // will be serialized as JSON
    class CustomPayload
    {
        public string Text { get; set; }
        public int Number { get; set; }

        public CustomPayload(string text, int number)
        {
            Text = text;
            Number = number;
        }

        public override bool Equals(object obj)
        {
            return obj is CustomPayload o &&
                    this.Number == o.Number;
        }
    }

    class ExampleSubscriber
    {
        public CustomPayload PassedValue;

        [Route("TestRoute")]
        public void Test(CustomPayload value)
        {
            PassedValue = value;
        }
    }

    [Test]
    public async Task RoutingTest()
    {
        var publisherSocket = new PublisherSocket();
        publisherSocket.Bind(Address);

        var subscriberSocket = new SubscriberSocket();
        subscriberSocket.Connect(Address);

        var subscriber = new ExampleSubscriber();

        var router = MessageRouter
            .WithPubSubConnecton(publisherSocket, subscriberSocket)
            .RegisterTypeSerializer(new RawDataTypeSerializer())
            .RegisterTypeSerializer(new BasicTextTypeSerializer())
            .RegisterGeneralSerializer(new JsonObjectSerializer())
            .RegisterRoute("TestRoute", typeof(CustomPayload))
            .RegisterSubscriber(subscriber)
            .StartRouting();

        router.SendMessage("TestRoute", new CustomPayload("Hellow world", 123));

        router.OnException += exception =>
        {    
            // handle any exception
        };

        await Task.Delay(TimeSpan.FromSeconds(3));

        router
            .StopRouting()
            .Disconnect();

        var expectedValue = new CustomPayload("Hellow world", 123);
        Assert.AreEqual(expectedValue, subscriber.PassedValue);
    }
}
```

Fast and reliable
-----------------

This library can be used in heavy environments because does not introduce any significant performance impact. Don't worry about your message, everything is well tested :)

Good practices
--------------

There are several ways to improve your code quality. Please consider applying them to your system:
- distribute the MessageRouter object across your system using the dependency injection mechanism. It does implement different interfaces so distribute it using interfaces that are best appropriate for the task. For example, in order to give your endpoints the possibility to send messages, use the IMessageSender interface:

```csharp
public interface IMessageRouter : IMessageRouterManager, IMessageRouterConfiguration, IMessageSender, IExceptionSource, IDisposable
{

}

public interface IMessageSender
{
    void SendMessage(string routeName);
    void SendMessage(string routeName, byte[] data);
    void SendMessage(string routeName, string text);
    void SendMessage(string routeName, object _object);
}
```

- do not use magic strings as in examples, use a static class with appropriate fields:

```csharp
// your class containg route names
internal static class MessageRoutes
{
    public static readonly string IncomingRoute = "IncomingRoute";
    public static readonly string OutcomingRoute = "OutcomingRoute";
    public static readonly string BananaRoute = "BananaRoute";
}

// your subscriber
class ExampleSubscriber
{
    [Route(MessageRoutes.IncomingRoute)]
    public string Test(string value)
    {
        // your business logic here
    }
}

// somewhere in the code where you are sending a message
router.SendMessage(MessageRoutes.BananaRoute, "Hello world!");
```

Let's use it!
-------------

Do you want to improve the architecture of your system and use this library? Congratulations, you made a good choice! At this moment, this framework is still under development and shouldn't be used in production but in nearest weeks it would be totally stable. 