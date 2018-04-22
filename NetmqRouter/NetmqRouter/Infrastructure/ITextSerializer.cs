namespace NetmqRouter.Infrastructure
{
    public interface ITextSerializer
    {
        byte[] Serialize(string text);
        string Desialize(byte[] data);
    }
} 