using System;

namespace TestApp.Models.Common
{


    /// <summary>
    /// Generic element that can be displayed inside a TagContainer
    /// </summary>
    public interface ISimpleTagElement : IEquatable<ISimpleTagElement>
    {

        /// <summary>
        /// The Id of the element
        /// </summary>
        uint Id { get; set; }

        /// <summary>
        /// The Text to be displayed inside the Tag
        /// </summary>
        string Body { get; set; }
    }
}
