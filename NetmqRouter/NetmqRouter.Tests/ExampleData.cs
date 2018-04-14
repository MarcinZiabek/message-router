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
        [Route("Raw")]
        public void RawSubscriber(byte[] data)
        {

        }

        [Route("Text")]
        public void TextSubscriber(string text)
        {

        }

        [Route("Object")]
        public void ObjectSubscriber(SimpleObject _object)
        {

        }
    }
}
