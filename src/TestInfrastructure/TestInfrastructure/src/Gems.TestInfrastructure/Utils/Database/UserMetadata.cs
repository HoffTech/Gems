// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database
{
    public class UserMetadata
    {
        public string UserName { get; set; }

        public uint? UserSysid { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is string userName)
            {
                return this.UserName == userName;
            }
            else if (obj is UserMetadata user)
            {
                return this.UserName == user.UserName;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.UserName?.GetHashCode() ?? 0;
        }
    }
}
