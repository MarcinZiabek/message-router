using MessageRouter.Infrastructure;

namespace MessageRouter.Fluent
{
    public class SendMessageFluent
    {
        private readonly IMessageSender _sender;
        private readonly object _payload;
        
        public SendMessageFluent(IMessageSender sender, object payload)
        {
            _sender = sender;
            _payload = payload;
        }

        public void To(string address)
        {
            _sender.SendMessage(address, _payload);
        }
    }
}