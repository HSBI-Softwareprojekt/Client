
//-----------------------------------------------------------------------------
// <auto-generated>
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;
using Unity.Services.Authentication.Shared;

namespace Unity.Services.Authentication.Generated
{
    /// <summary>
    /// UpdateNameRequest
    /// </summary>
    [DataContract(Name = "UpdateNameRequest")]
    [Preserve]
    internal partial class UpdateNameRequest
    {
        /// <summary>
        /// The player&#39;s updated username.
        /// </summary>
        /// <value>The player&#39;s updated username.</value>
        [DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
        [Preserve]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateNameRequest" /> class.
        /// </summary>
        /// <param name="name">The player&#39;s updated username. (required).</param>
        [Preserve]
        public UpdateNameRequest(string name = default(string))
        {
            // to ensure "name" is required (not null)
            if (name == null)
            {
                throw new ArgumentNullException("name is a required property for UpdateNameRequest and cannot be null");
            }
            this.Name = name;
        }
    }

}
