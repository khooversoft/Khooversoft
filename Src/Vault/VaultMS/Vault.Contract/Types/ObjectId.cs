using Khooversoft.Toolbox;
using System;

namespace Vault.Contract
{
    public class ObjectId : ICustomType<string>
    {
        private readonly StringVectorBind _bind;
        private readonly Deferred<string> _getValue;

        private ObjectId()
        {
            _bind = new StringVectorBind(2, 3)
                .Add(() => GroupName, (x) => GroupName = new GroupName(x))
                .Add(() => Name, (x) => Name = new SecretName(x))
                .Add(() => Version, (x) => Version = new SecretVersion(x));

            _getValue = new Deferred<string>(() => BuildValue());
        }

        public ObjectId(string value)
            : this()
        {
            Verify.IsNotEmpty(nameof(value), value);

            var sv = new StringVector(value);
            _bind.Set(sv);
        }

        public ObjectId(string groupName, string name, string version = null)
            : this()
        {
            Verify.IsNotEmpty(nameof(groupName), groupName);
            Verify.IsNotEmpty(nameof(name), name);

            GroupName = new GroupName(groupName);
            Name = new SecretName(name);
            Version = version != null ? new SecretVersion(version) : null;
        }

        public ObjectId(GroupName groupName, SecretName name, SecretVersion version = null)
            : this()
        {
            Verify.IsValueValid(nameof(groupName), groupName);
            Verify.IsValueValid(nameof(name), name);
            Verify.IsValueValid(nameof(version), version, allowNull: true);

            GroupName = groupName;
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Group name
        /// </summary>
        public GroupName GroupName { get; private set; }

        /// <summary>
        /// Secret name
        /// </summary>
        public SecretName Name { get; private set; }

        /// <summary>
        /// Secret version
        /// </summary>
        public SecretVersion Version { get; private set; }

        /// <summary>
        /// Get vector value
        /// </summary>
        public string Value { get { return _getValue.Value; } }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator string(ObjectId source)
        {
            return source?.Value;
        }

        /// <summary>
        /// Is valid, group, name, and version being valid
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsValueValid()
        {
            return GroupName.IsValueValid() &&
                Name.IsValueValid() &&
                ((Version == null) || Version.IsValueValid());
        }

        /// <summary>
        /// Get ID (value) without version
        /// </summary>
        /// <returns>group name + name</returns>
        public string GetBaseId()
        {
            return _bind.Get(2).Value;
        }

        /// <summary>
        /// Is ObjectId equal
        /// </summary>
        /// <param name="objectId">object id to compare</param>
        /// <param name="matchVersion">true match version</param>
        /// <returns>true if match, false if not</returns>
        public bool IsEqual(ObjectId objectId, bool matchVersion)
        {
            if (objectId == null)
            {
                return false;
            }

            bool match = GroupName.Value.Equals(objectId.GroupName.Value, StringComparison.OrdinalIgnoreCase) &&
                Name.Value.Equals(objectId.Name.Value, StringComparison.OrdinalIgnoreCase);

            if (!matchVersion || !match)
            {
                return match;
            }

            if (Version != null && objectId.Version != null)
            {
                return Version.Value.Equals(objectId.Version.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        /// <summary>
        /// Return in object form
        /// </summary>
        /// <returns></returns>
        public object GetObjectValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Build vector for group, security name, and version (will skip null values)
        /// </summary>
        /// <returns></returns>
        private string BuildValue()
        {
            Verify.Assert(IsValueValid(), $"{nameof(ObjectId)} is not valid");

            return _bind.Get().Value;
        }
    }
}
