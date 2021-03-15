using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApi.Data.DTOs {

    /// <summary>
    /// Indicates avalaible actions to current user
    /// </summary>
    public class ActionsAvalaibility {
        public bool CanBeViewed { get; set; }

        public bool CanBeEdited { get; set; }

        public bool CanBeDeleted { get; set; }

    }
}
