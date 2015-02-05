using System;

namespace RecipesSearch.Data.Models.Base.Errors
{
    public static class StandardErrors
    {
        public static Error GeneralError
        {
            get
            {
                return new Error
                {
                    ErrorCode = (int)ErrorCode.GeneralException,
                    Name = ErrorCode.GeneralException.ToString()
                };
            }
        }
        public static Error UserInvalidCredentialsError
        {
            get
            {
                return new Error
                {
                    ErrorCode = (int)ErrorCode.MembershipCredentialsInvalid,
                    Name = ErrorCode.MembershipCredentialsInvalid.ToString()
                };
            }
        }
        public static Error EntityNotFoundError
        {
            get
            {
                return new Error
                {
                    ErrorCode = (int)ErrorCode.EntityNotFound,
                    Name = ErrorCode.EntityNotFound.ToString()
                };
            }
        }
    }
}
