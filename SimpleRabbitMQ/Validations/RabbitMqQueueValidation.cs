using SimpleRabbitMQ.Configurations;

namespace SimpleRabbitMQ.Validations
{
    internal class RabbitMqQueueValidation : ValidationBase<RabbitMqQueueOptions>
    {
        public RabbitMqQueueValidation(RabbitMqQueueOptions objectClass) : base(objectClass)
        {
        }

        protected override bool IsInValidInternal()
        {
            return string.IsNullOrEmpty(_objectClass.Name);
        }
    }
}
