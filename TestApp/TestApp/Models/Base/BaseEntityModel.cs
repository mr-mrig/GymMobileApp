using System;

namespace TestApp.Models.Base
{
    public class BaseEntityModel : BaseModel, IEquatable<BaseEntityModel>
    {

        /// <summary>
        /// The ID identifying the entity. This will be used for comparisons
        /// </summary>
        public uint? Id { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is BaseEntityModel other)
                return Equals(other);

            return false;
        }

        public bool Equals(BaseEntityModel other)
        {
            if(Id == null)
                return other.Id == null;

            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();


        public static bool operator ==(BaseEntityModel left, BaseEntityModel right)
        {
            if (left is null)
                return right is null;

            if (right is null)
                return false;

            return left.Equals(right);
        }
        public static bool operator !=(BaseEntityModel left, BaseEntityModel right) => !(left == right);
    }
}
