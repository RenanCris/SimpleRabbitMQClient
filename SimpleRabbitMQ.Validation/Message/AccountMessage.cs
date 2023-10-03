﻿namespace SimpleRabbitMQ.Validation.Message
{
    public class AccountMessage
    {
        public int Id { get; set; }

        public int AccountNumber { get; set; }
        public string OwnerName { get; set; }
        public decimal Balance { get; private set; }
    }
}
