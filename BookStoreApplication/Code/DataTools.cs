using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Code {

    public class DbDifferences<TDbType, TDtoType> {
        public ICollection<TDtoType> InsertedItems { get; set; }

        public ICollection<TDbType> RemovedItems { get; set; }

        public ICollection<TDbType> StayedItems { get; set; }
    }

    public class DataTools {

        public static DbDifferences<TDbType, TDtoType> FindDifferences<TDbType, TDtoType>(
            ICollection<TDbType> oldCollection,
            ICollection<TDtoType> newCollection,
            Func<TDbType, TDtoType, bool> equivalenceComparer) 
        {
            if (equivalenceComparer == null)
                throw new ArgumentNullException(nameof(equivalenceComparer));
            if (oldCollection == null)
                oldCollection = Array.Empty<TDbType>().ToList();
            if(newCollection == null)
                newCollection = Array.Empty<TDtoType>().ToList();

            var addedItems = newCollection.Where(newItm => !oldCollection.Any(oldItm => equivalenceComparer(oldItm, newItm))).ToList();
            var removedItms = oldCollection.Where(oldItm => !newCollection.Any(newItm => equivalenceComparer(oldItm, newItm))).ToList();
            var stayedItems = oldCollection.Where(oldItm => newCollection.Any(newItm => equivalenceComparer(oldItm, newItm))).ToList();

            return new DbDifferences<TDbType, TDtoType>() {
                InsertedItems = addedItems,
                RemovedItems = removedItms,
                StayedItems = stayedItems
            };

        }

    }
}
