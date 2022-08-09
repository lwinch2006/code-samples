using System.Security.Claims;

namespace ApplicationBuildingBlocks.Builder.Authentication;

public interface ISavedClaimsDictionary
{
}

public class SavedClaimsDictionary : Dictionary<string, List<Claim>>, ISavedClaimsDictionary
{
}