namespace TestAppUniversal
{
    using System.Collections.Generic;
    using System.Dynamic;

    public delegate string ToStringFunc();

    class ToStringExpandoObject : DynamicObject
    {
        public IDictionary<string, object> Members { get; private set; }

        public ToStringExpandoObject()
        {
            Members = new Dictionary<string, object>();
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return Members.Remove(binder.Name);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Members.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Members[binder.Name] = value;
            return true;
        }

        public override string ToString()
        {
            object methodObj;

            Members.TryGetValue("ToString", out methodObj);

            ToStringFunc method = methodObj as ToStringFunc;

            if (method == null)
            {
                return base.ToString();
            }

            return method();
        }
    }
}
