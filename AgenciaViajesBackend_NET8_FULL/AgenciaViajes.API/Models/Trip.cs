using Google.Cloud.Firestore;
using System;

// Alias para evitar conflicto entre el nombre del tipo y la propiedad
using PType = AgenciaViajes.API.Models.PassengerType;
using TStatus = AgenciaViajes.API.Models.TripStatus;

namespace AgenciaViajes.API.Models
{
    [FirestoreData]
    public class Trip
    {
        [FirestoreProperty] public string Id { get; set; } = Guid.NewGuid().ToString();

        // id del cliente (uid/guid)
        [FirestoreProperty] public string ClientId { get; set; } = default!;

        // destino
        [FirestoreProperty] public string CountryCode { get; set; } = default!;
        [FirestoreProperty] public string City { get; set; } = default!;

        // detalles
        [FirestoreProperty] public int StayDays { get; set; }

        // Guardado como number en Firestore; .NET mapea <-> enum
        [FirestoreProperty] public PType PassengerType { get; set; } = PType.Adult;

        [FirestoreProperty] public DateTime? TravelDate { get; set; }

        [FirestoreProperty] public TStatus Status { get; set; } = TStatus.Pending;

        [FirestoreProperty] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
