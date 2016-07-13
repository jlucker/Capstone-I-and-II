using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoCaptureUI
{
    public class User
    {
        public String name; //CHANGE TO PRIVATE: getName, setName
        public Pattern password;

       public User()
        {
            name = "";
            password = null;
        }
        
        public User(String _name, Pattern pass)
        {
            this.name = _name;
            this.password = pass;
        }

        public override string ToString()
        {
           return name;
        }

        public String getName()
        {
            return name;
        }

        public void setName(String _name)
        {
            this.name = _name;
        }
    }

}
