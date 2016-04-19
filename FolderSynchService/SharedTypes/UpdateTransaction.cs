using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesProject
{
    [DataContract]
    public class UpdateTransaction : Transaction
    {

        /* ------------------------------------------------------------------------------ */
        /* ------------------------ CONSTRUCTOR ----------------------------------------- */
        /* ------------------------------------------------------------------------------ */

        public UpdateTransaction(User user, string folderName, DateTime timestamp) : base(user, folderName, timestamp)
        {
        }
    }
}
