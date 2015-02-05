using System;

namespace RecipesSearch.Data.Models.Base.Errors
{
    public enum ErrorCode
    {
        GeneralException = 1,
        MembershipCredentialsInvalid = 2,
        EntityNotFound = 3
    }
}
