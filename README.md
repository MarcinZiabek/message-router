NetmqRouter
===========

What is it and what is it good for?
-----------------------------------

The NetmqRouter library can be considered as an add-on for the NetMQ library, that is a port of the ZeroMQ library for the .NET environment. 

Those two libraries solve a lot of problems such as sending information over newtork in a lightweigh manner, making sure that a whole message is received or distrubuting messages across many subscribers. In real word scenarios, it is not enough. 

The HTTP protocol provides different endpoints and message from them are appropriately routed to methods in your code. The messaging system in ZeroMQ does not contain any header information and, therefore, does not provide any similar functinality.

The NetmqRouter acts as an additional layer of abstraction between your code and communication layer.

Wait, there is more!
--------------------

**Open to communication layer** - this library can utilize any socket type from NetMQ. You are not limited to producer-subscriber pattern.

**Flexible routing** - declare many kinds of routes with different levels of addresses. 

**Serialization layer** - use any serialization library you want - you can use JSON protocol, ProtoBuf or implement your own solution.

**Serializer per type** - if your architecture requires various serialization formats for different object types, you can always register serializers for specific type.

**Sending messages** - if you want do everything for yourself, send a simple message via the MessageRouter method:

**Register subscribers** - in order to create a subscriber, annotate methods in your class and register it in the MessageRouter class. This library will check if your methods use correct types and are safe to run.

**Base routes** - you can annotate your class with the BaseRoute attribute to use subscribe to a specified route or family of routes.

**Responding** - your method subscribers can respond to incoming message - just use ResponseRoute attribute. This library will check if your methods use correct types and are safe to run.

**Events** - there is a possibility to subscribe to an event, send event as an empty message or response to incoming message via returning nothing. This library can determine such cases for yourself.

**Fast** - this library can be used in heavy environments because does not introduce any significant performance impact.

**Well tested** - don't worry about your message, everything is well tested :)

How to use it
-------------

Do you want to improve architecture of your system and use this library. Congratulations, you made a good choice! At this moment, this framework is still under development and shouldn't be used in production but in nearest weeks it would be totally stable. Please wait for the official nuget package :)
