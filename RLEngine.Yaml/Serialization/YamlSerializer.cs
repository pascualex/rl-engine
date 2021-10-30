﻿using RLEngine.Yaml.Utils;

using RLEngine.Core.Games;
using RLEngine.Core.Utils;

using System;
using System.IO;

namespace RLEngine.Yaml.Serialization
{
    public static class YamlSerializer
    {
        public static void Serialize(GameContent gameContent)
        {
            var serializationQueue = new SerializationQueue();
            var writter = new GenericWritter(serializationQueue);

            Directory.CreateDirectory(gameContent.ID);
            Serialize(writter, gameContent.ID, gameContent, typeof(GameContent));
            while (serializationQueue.Count > 0)
            {
                var (element, type) = serializationQueue.Dequeue();
                var typePath = Path.Combine(gameContent.ID, SerializationPaths.Get(type));
                Serialize(writter, typePath, element, type);
            }
        }

        private static void Serialize(GenericWritter writter,
        string path, IIdentifiable element, Type type)
        {
            Directory.CreateDirectory(path);
            if (element.ID.Length == 0) throw new ArgumentNullException();
            var filename = element.ID + ".yml";
            using var streamWriter = new StreamWriter(Path.Combine(path, filename));
            writter.WriteObject(streamWriter, element, type);
        }
    }
}