namespace NetmqRouter.Attributes
{
    public interface ITextSerializer
    {
        byte[] Serialize(string text);
        string Desialize(byte[] data);
    }
}