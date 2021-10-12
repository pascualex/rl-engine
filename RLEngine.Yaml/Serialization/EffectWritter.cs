﻿using RLEngine.Yaml.Utils;

using RLEngine.Abilities;
using RLEngine.Utils;

using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RLEngine.Yaml.Serialization
{
    public class EffectWritter
    {
        private readonly GenericWritter genericWritter;

        public EffectWritter(GenericWritter CustomTCSerializer)
        {
            genericWritter = CustomTCSerializer;
        }

        public void WriteField(IEmitter emitter, Effect effect)
        {
            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            emitter.Format(nameof(effect.Type));
            genericWritter.WriteField(emitter, effect.Type, typeof(EffectType));

            var effectType = effect.GetEffectType();
            if (effectType is null)
            {
                emitter.Emit(new MappingEnd());
                return;
            }

            foreach (var propertyInfo in effectType.GetPublicProperties())
            {
                if (propertyInfo.Name == nameof(IIdentifiable.ID)) continue;
                var propertyValue = (object?)propertyInfo.GetValue(effect);
                if (propertyValue is null) continue;
                if (propertyValue is string str && str.Length == 0) continue;
                emitter.Format(propertyInfo.Name);
                genericWritter.WriteField(emitter, propertyValue, propertyInfo.PropertyType);
            }

            emitter.Emit(new MappingEnd());
        }
    }
}