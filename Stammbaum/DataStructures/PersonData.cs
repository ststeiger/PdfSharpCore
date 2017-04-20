
namespace Stammbaum.DataStructures
{


    public class PersonData
    {
        public long Id;
        public string gender;
        public long Mother;
        public long Father;

        public long? Child;
        public string ChildGender;
        public string ChildName;

        public string FirstName;
        public string LastName;
        public string FamilyName;

        public System.DateTime? dtBirthDate;
        public string PlaceOfBirth;
        public string composite_name;
        public int generation;

        public bool IsMale
        {
            get
            {
                return System.StringComparer.OrdinalIgnoreCase.Equals(gender, "M");
            }
        }

        public bool IsFemale
        {
            get
            {
                return System.StringComparer.OrdinalIgnoreCase.Equals(gender, "F");
            }
        }


    }


}
