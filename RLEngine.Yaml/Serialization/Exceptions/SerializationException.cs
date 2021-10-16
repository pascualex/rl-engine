﻿using System;

namespace RLEngine.Yaml.Serialization
{
    public class SerializationException : Exception
    {
        public SerializationException() : base()
        { }

        public SerializationException(string message) : base(message)
        { }

        public SerializationException(string message, Exception innerException)
        : base(message, innerException)
        { }
    }
}