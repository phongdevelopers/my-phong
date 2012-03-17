namespace CommerceBuilder.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommerceBuilder.Data;
    using CommerceBuilder.Common;
    using CommerceBuilder.Users;
    using CommerceBuilder.Utility;

    public partial class BasketShipmentCollection
    {
        //STORES A RECORD OF ITEMS REMOVED BY THE COMBINE METHOD WHEN SAVE PARAMETER IS FALSE
        private List<BasketShipment> removedItems = new List<BasketShipment>();

        /// <summary>
        /// Saves the collection.
        /// </summary>
        /// <returns>True if the save is successful, false otherwise.</returns>
        public override bool Save()
        {
            //IF ITEMS HAVE BEEN REMOVED FROM THE COLLECTION, DELETE THEM NOW
            if (removedItems.Count > 0)
            {
                foreach (BasketShipment removedItem in removedItems)
                {
                    removedItem.Delete();
                }
                removedItems.Clear();
            }
            //INVOKE THE BASE SAVE
            return base.Save();
        }

        /// <summary>
        /// Combines any shipments in the collection that have equivalent data.
        /// </summary>
        /// <param name="save">Flag indicating whether or not to persist changes.</param>
        /// <remarks>When shipments are combined any items they contain are merged into a single shipment.</remarks>
        public void Combine(bool save)
        {
            WebTrace.Write("Combine Basket Shipments");
            for (int i = this.Count - 1; i >= 0; i--)
            {
                BasketShipment shipment = this[i];
                List<BasketItem> shipmentItems = shipment.Items.FindAll(delegate(BasketItem item) { return (item.BasketShipmentId == shipment.BasketShipmentId); });
                if (shipmentItems.Count > 0)
                {
                    WebTrace.Write("i: " + i.ToString() + ", BasketShipmentId: " + shipment.BasketShipmentId.ToString());
                    BasketShipment match = this.Find(new System.Predicate<BasketShipment>(shipment.CanCombine));
                    if ((match != null) && (match.BasketShipmentId != shipment.BasketShipmentId))
                    {
                        WebTrace.Write("match.BasketShipmentId: " + match.BasketShipmentId.ToString());
                        //NEED TO MOVE ITEMS FROM THIS SHIPMENT TO THE MATCHED SHIPMENT
                        foreach (BasketItem item in shipmentItems)
                        {
                            item.BasketShipmentId = match.BasketShipmentId;
                            if (save) item.Save();
                        }
                        if (save) this.DeleteAt(i);
                        else
                        {
                            removedItems.Add(this[i]);
                            this.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (save) this.DeleteAt(i);
                    else
                    {
                        removedItems.Add(this[i]);
                        this.RemoveAt(i);
                    }
                }
            }
            this.Save();
        }


    }
}
