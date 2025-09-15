namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class ProfilesMessageV1 : Message
{
    public List<ProfileMessageV1>? Profiles { get; set; }
}
