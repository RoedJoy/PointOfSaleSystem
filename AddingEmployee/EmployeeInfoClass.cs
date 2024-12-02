using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPos.AddingEmployee
{
    public class EmployeeInfoClass
    {
        private string fName, lName, mdName, address, mobileNum, username, password, gender;
        private int age, idNum, userID;
        private DateTime birthDate;

        // Public properties for other fields
        public string FirstName
        {
            get { return fName; }
            set { fName = value; }
        }

        public string LastName
        {
            get { return lName; }
            set { lName = value; }
        }

        public string MiddleName
        {
            get { return mdName; }
            set { mdName = value; }
        }
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string MobileNum
        {
            get { return mobileNum; }
            set { mobileNum = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int IdNum
        {
            get { return idNum; }
            set { idNum = value; }
        }

        public DateTime BirthDate
        {
            get { return birthDate; }
            set { birthDate = value; }
        }
        public int UserID
        {
            get { return userID; }
            set {  userID = value; }
        }
    }
}
