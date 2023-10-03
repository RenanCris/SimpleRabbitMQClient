﻿using System.Net.Security;

namespace SimpleRabbitMQ.Configurations
{
    public class RabbitMqSslOption 
    {
        /// <summary>
        /// Canonical server name.
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// Path to the certificate.
        /// </summary>
        public string CertificatePath { get; set; } = string.Empty;

        /// <summary>
        /// A pass-phrase for the certificate.
        /// </summary>
        public string CertificatePassphrase { get; set; } = string.Empty;

        /// <summary>
        /// Flag that defines if certificate should be used.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Acceptable policy errors.
        /// </summary>
        /// <remarks>
        /// SslPolicyErrors is a flag enum, so you can have multiple set values.
        /// </remarks>
        public SslPolicyErrors? AcceptablePolicyErrors { get; set; }
    }
}
