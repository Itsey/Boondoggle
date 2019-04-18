using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plisky.Boondoggle2 {
    public interface IProvideEquipmentDetail {

        EquipmentItem GetEquipmentById(int id);

        EquipmentDescription GetEquipmentDescriptionById(int id);
    }
}
