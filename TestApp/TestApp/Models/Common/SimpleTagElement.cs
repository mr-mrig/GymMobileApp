using TestApp.Models.Base;

namespace TestApp.Models.Common
{
    public class SimpleTagElement : BaseModel, ISimpleTagElement
    {

        private uint _id;

        public uint Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private string _body;

        public string Body
        {
            get => _body;
            set => Set(ref _body, value);
        }

        public bool Equals(ISimpleTagElement other)

            => other != null && other.Id == Id;

        public override bool Equals(object obj)

            => Equals(obj as ISimpleTagElement);

        public override int GetHashCode()

            => (int)Id;

    }
}
