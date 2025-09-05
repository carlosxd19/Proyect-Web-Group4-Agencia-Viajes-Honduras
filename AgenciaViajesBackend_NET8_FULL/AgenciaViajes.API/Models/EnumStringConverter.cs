using Google.Cloud.Firestore;
using System;

namespace AgenciaViajes.API.Models
{
    public class EnumStringConverter<T> : IFirestoreConverter<T> where T : struct, Enum
    {
        public object ToFirestore(T value) => value.ToString();
        public T FromFirestore(object value)
        {
            if (value is string s && Enum.TryParse<T>(s, true, out var result)) return result;
            throw new ArgumentException($"Valor Firestore inválido para enum {typeof(T).Name}: {value}");
        }
    }
}
