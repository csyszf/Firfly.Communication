using System.Threading.Tasks;

namespace Firfly.Communication.Kafka
{
    public interface IMessageAdapter
    {
        Task ProduceAsync(string topic, byte[] key, byte[] val);
    }
}
