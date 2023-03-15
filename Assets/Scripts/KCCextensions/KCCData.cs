using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC {

    public partial class KCCData {

        // PUBLIC MEMBERS

        public bool Sprint;

        // PARTIAL METHODS

        partial void ClearUserData() {
            // Full cleanup here (lists, pools, cached data, ...)
        }

        partial void CopyUserDataFromOther(KCCData other) {
            // Make a deep copy of your properties for correct rollback.
            // This method is executed when you get a new state from server and rollback is triggered.
            // This method is also executed after fixed updates to copy fixed data to render data.

            Sprint = other.Sprint;
        }
    }
}
