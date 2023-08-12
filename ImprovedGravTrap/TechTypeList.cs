using System.Collections.Generic;
using System.Linq;

namespace ImprovedGravTrap
{
    public class TechTypeList
    {
        public string name;
        public List<TechType> techTypes = new List<TechType>();

        public void Add(TechType techType) => techTypes.Add(techType);
        public bool Remove(TechType techType) => techTypes.Remove(techType);

        public void Add(TechTypeList list) => Add(list.techTypes);
        public void Add(IEnumerable<TechType> list) => techTypes.AddRange(list);

        public bool Contains(TechType techType) => techTypes.Contains(techType);

        public TechTypeList(string name, params TechType[] techTypes)
        {
            this.name = name;
            this.techTypes = techTypes.ToList();
        }

        //public TechTypeList(TechTypeList list) : this(list.name, list.techTypes.ToArray()) { }
    }
}
