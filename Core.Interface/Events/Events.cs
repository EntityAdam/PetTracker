using Core.Interface.Models;

namespace Core.Interface.Events;

public record FosterPersonEvent(PersonIdentity PersonIdentity, FosterPersonEventKind FosterPersonEventKind, DateTimeOffset Timestamp);
public record AdopterPersonEvent(PersonIdentity PersonIdentity, AdopterPersonEventKind AdopterPersonEventKind, DateTimeOffset Timestamp);

public record RescuedPetEvent();
public record RescueGroupEvent();

public record ShelterEvent(ShelterIdentity ShelterIdentity, ShelterEventKind ShelterEventKind, DateTimeOffset Timestamp);
public record ShelteredPetEvent(PetIdentity PetIdentity, PetEventKind PetEventKind, DateTimeOffset Timestamp);