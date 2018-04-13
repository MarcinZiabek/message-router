using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetmqRouter.Tests
{
    class SimpleObject
    {
        public int Data { get; set; }
    }

    [Route("Example")]
    class ExampleData
    {
        public string CalledMethod { get; set; }

        public void NormalMethod()
        {
            CalledMethod = nameof(NormalMethod);
        }

        [Route("Event")]
        public void EventSubscriber()
        {
            CalledMethod = nameof(EventSubscriber);
        }

        [Route("Raw")]
        public void RawSubscriber(byte[] data)
        {
            CalledMethod = nameof(RawSubscriber);
        }

        [Route("Text")]
        public void TextSubscriber(string text)
        {
            CalledMethod = nameof(TextSubscriber);
        }

        [Route("Object")]
        public void ObjectSubscriber(SimpleObject _object)
        {
            CalledMethod = nameof(ObjectSubscriber);
        }
    }
}
