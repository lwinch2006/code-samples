using System.Collections.Generic;
using System.Security.Claims;

namespace Application.Models.Authentication;

public interface ISavedClaimsDictionary
{
}

public class SavedClaimsDictionary : Dictionary<string, IEnumerable<Claim>>, ISavedClaimsDictionary
{
}