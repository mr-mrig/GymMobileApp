using System;

namespace TestApp.Models.Base
{
    public class BaseEnumeration : IEquatable<BaseEnumeration>
    {

        public uint Id {get; set;}

        public bool Equals(BaseEnumeration other)
        {
            if(other == null)
                return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType()) 
                return false;

            return Equals(obj as BaseEnumeration);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
