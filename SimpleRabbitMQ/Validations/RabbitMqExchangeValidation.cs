using SimpleRabbitMQ.Configurations;

namespace SimpleRabbitMQ.Validations
{
    internal class RabbitMqExchangeValidation : ValidationBase<RabbitMqExchangeOptions>
    {
        public RabbitMqExchangeValidation(RabbitMqExchangeOptions objectClass) : base(objectClass)
        {
        }

        protected override bool IsInValidInternal()
        {
            return string.IsNullOrEmpty(_objectClass.Name);
        }
    }
}
