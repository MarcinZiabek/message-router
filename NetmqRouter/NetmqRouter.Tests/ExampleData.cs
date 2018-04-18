using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetmqRouter.Attributes;

namespace NetmqRouter.Tests
{
    public class SimpleObject
    {
        public int Data { get; set; }
    }

    public class ExampleSubscriber
    {
        public string CalledMethod { get; set; }

        public void NormalMethod()
        {
            CalledMethod = nameof(NormalMethod);
        }

        [Route("Void")]
        [ResponseRoute("Response")]
        public byte[] EventSubscriber()
        {
            CalledMethod = nameof(EventSubscriber);
            return null;
        }

        [Route("Raw")]
        [ResponseRoute("Response")]
        public string RawSubscriber(byte[] data)
        {
            CalledMethod = nameof(RawSubscriber);
            return null;
        }

        [Route("Text")]
        public object TextSubscriber(string text)
        {
            CalledMethod = nameof(TextSubscriber);
            return null;
        }

        [Route("Object")]
        public void ObjectSubscriber(SimpleObject _object)
        {
            CalledMethod = nameof(ObjectSubscriber);
        }
    }

    [BaseRoute("Example")]
    class ExampleSubscriberWithBaseRoute : ExampleSubscriber
    {

    }
}

