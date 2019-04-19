namespace Plisky.Boondoggle2 {

    public abstract class EquipmentRepository : IProvideEquipmentDetail {

        protected abstract EquipmentItem ActualLoadEquipmentItemById(int identity);

        protected abstract BotFrame ActualLoadBotFrame(int idToLoad);

        internal EquipmentItem LoadEquipmentById(int idToLoad) {
            return ActualLoadEquipmentItemById(idToLoad);
        }

        public EquipmentItem GetEquipmentById(int id) {
            throw new System.NotImplementedException();
        }

        public EquipmentDescription GetEquipmentDescriptionById(int id) {
            throw new System.NotImplementedException();
        }

        public BotFrame LoadBotFrame(int idToLoad) {
            return ActualLoadBotFrame(idToLoad);
        }
    }
}