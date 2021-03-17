using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.WASM.Data.DTOs {
    public class ActionsAvalaibility {
        public bool CanBeViewed { get; set; }

        public bool CanBeEdited { get; set; }

        public bool CanBeDeleted { get; set; }

    }
}
