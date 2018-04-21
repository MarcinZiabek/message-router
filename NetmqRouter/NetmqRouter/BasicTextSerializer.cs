using System.Text;
using NetmqRouter.Attributes;

namespace NetmqRouter
{
    public class BasicTextSerializer : ITextSerializer
    {
        public byte[] Serialize(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        public string Desialize(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}