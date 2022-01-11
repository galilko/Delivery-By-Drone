using System.ComponentModel;
using BO;
using BlApi;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace PL
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        static readonly IBL bl = BlFactory.GetBL();

        Model() { }
        public static Model Instance { get; } = new Model();


        ObservableCollection<DroneToList> drones = new(bl.GetDrones());
        public ObservableCollection<DroneToList> Drones {
            get => drones;
            private set
            {
                drones = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Drones)));
            }
        }
        
        ObservableCollection<BaseStationToList> baseStations = new(bl.GetBaseStations());
        public ObservableCollection<BaseStationToList> BaseStations
        {
            get => baseStations;
            private set
            {
                baseStations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BaseStations)));
            }
        }
        
        ObservableCollection<CustomerToList> customers = new(bl.GetCustomers());
        public ObservableCollection<CustomerToList> Customers
        {
            get => customers;
            private set
            {
                customers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Customers)));
            }
        }

        ObservableCollection<ParcelToList> parcels = new(bl.GetParcels());
        public ObservableCollection<ParcelToList> Parcels
        {
            get => parcels;
            private set
            {
                parcels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parcels)));
            }
        }

        DroneStatusCategories? statusSelector = null;
        public DroneStatusCategories? StatusSelector {
            get => statusSelector;
            set 
            {
                statusSelector = value;
                DronesRefresh();
            }
        }

        WeightCategories? weightSelector = null;
        public WeightCategories? WeightSelector {
            get => weightSelector;
            set
            {
                weightSelector = value;
                DronesRefresh();
            }
        }
        
        ParcelStatus? parcelStatusSelector = null;
        public ParcelStatus? ParcelStatusSelector
        {
            get => parcelStatusSelector;
            set
            {
                parcelStatusSelector = value;
                ParcelsRefresh();
            }
        }

        public void DronesRefresh() =>
            Drones = new(bl.GetDrones(d =>
                (weightSelector == null || d.Weight == weightSelector) &&
                (statusSelector == null || d.Status == statusSelector)
            ));

        public void BaseStationsRefresh() =>
            BaseStations = new(bl.GetBaseStations());

        public void CustomersRefresh() =>
            Customers = new(bl.GetCustomers());

        public void ParcelsRefresh()
        {
            if(parcelStatusSelector != null)
                Parcels = new(bl.GetParcels().Where(p=>p.Status == parcelStatusSelector));
            else
                Parcels = new(bl.GetParcels());
        }



        /* internal void RemoveItem<T>(ObservableCollection<T> collection, T instance)
         {
             collection.Remove(collection.Where(i => i.Equals(instance)).Single());
         }*/
        internal void RemoveBS(BaseStationToList instance)
        {
            baseStations.Remove(baseStations.Where(i => i.Id == instance.Id).Single());
        }

        internal void RemoveDrone(Drone drone)
        {
            drones.Remove(drones.Where(d => d.Id == drone.Id).Single());
        }

        internal void RemoveCustomer(CustomerToList customer)
        {
            customers.Remove(customers.Where(c => c.Id == customer.Id).Single());
        }

        internal void RemoveParcel(ParcelToList parcel)
        {
            parcels.Remove(parcels.Where(p => p.Id == parcel.Id).Single());
        }
    }
}
