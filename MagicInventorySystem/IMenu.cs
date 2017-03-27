using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicInventorySystem
{
    interface IMenu
    {
        void HandleMenu();
        int DisplayMenu();
        int GetIntOptionSelected();
    }
}
